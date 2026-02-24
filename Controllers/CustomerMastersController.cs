using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class CustomerMastersController : ControllerBase
	{
		private readonly ICustomerMastersRepository _customerMastersRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly IMapper _mapper;
		private readonly APIResponse _APIResponse;
		private readonly string KeyName = "CustomerMaster";

		public CustomerMastersController(ICustomerMastersRepository customerMastersRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_customerMastersRepository = customerMastersRepository;
			_distributedCache = distributedCache;
			_mapper = mapper;
			_APIResponse = new APIResponse();
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllCustomers()
		{
			try
			{
				var customers = await _customerMastersRepository.GetCustomerDetailsWithAllDetails();
				_APIResponse.Data = customers;
				_APIResponse.Status = true;
				_APIResponse.Message = "Customer List";
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);

			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetCustomerById(int id)
		{
			try
			{
				var customer = await _customerMastersRepository.GetCustomerViewModelByIdAsync(id);
				if (customer == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_APIResponse);
				}

				_APIResponse.Data = customer;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateCustomer(CustomerViewModel customerMaster)
		{
			try
			{
				if (customerMaster == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_APIResponse);
				}

                if ((await _customerMastersRepository.FindByNameAsync(x => x.CName.ToLower() == customerMaster.Cname.ToLower() && x.Gstno.ToLower() == customerMaster.Gstno.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = $"Customer {customerMaster.Cname} already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                CustomerMaster Customer = new CustomerMaster();
				Customer = _mapper.Map(customerMaster, Customer);

				await _customerMastersRepository.AddAsync(Customer);
				//await _distributedCache.RemoveAsync(KeyName);

				_APIResponse.Data = customerMaster;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction(nameof(GetCustomerById), new { id = customerMaster.Id }, _APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateCustomer(int id, CustomerViewModel customerMaster)
		{
			try
			{
				if (id != customerMaster.Id)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_APIResponse);
				}

				var existingCustomer = await _customerMastersRepository.GetByIdAsync(id);
				if (existingCustomer == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_APIResponse);
				}

               /* if ((await _customerMastersRepository.FindByNameAsync(x => x.CName.ToLower() == customerMaster.Cname.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = $"Customer {customerMaster.Cname} already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                existingCustomer = _mapper.Map(customerMaster, existingCustomer);
				await _customerMastersRepository.UpdateAsync(existingCustomer);


				// Upsert members (add/update/remove)
				await _customerMastersRepository.UpsertAndRemoveMembersAsync(customerMaster.Members, id);

				// Upsert magazines (add/update/remove)
				await _customerMastersRepository.UpsertAndRemoveMagazinesAsync(customerMaster.Magazines, id);

				//// Save changes to the database
				//await _customerMastersRepository.UpdateAsync(existingCustomer);

				//await _distributedCache.RemoveAsync(KeyName);

				_APIResponse.Data = null;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteCustomer(int id)
		{
			try
			{
				var existingCustomer = await _customerMastersRepository.GetByIdAsync(id);
				if (existingCustomer == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_APIResponse);
				}

				await _customerMastersRepository.DeleteAsync(id);
				//await _distributedCache.RemoveAsync(KeyName);

				_APIResponse.Data = null;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetcustomerNames()
		{
			var response = new APIResponse();

			try
			{
				var magazine = await _customerMastersRepository.GetcustoNamesAsync();
				response.Data = magazine;
				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;

				return Ok(response);
			}
			catch (Exception ex)
			{
				response.Errors.Add(ex.Message);
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode((int)HttpStatusCode.InternalServerError, response);
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetCustomersid()
		{
			try
			{
				var maxSrno = _customerMastersRepository.GetMaxSrno();
				int nextSrno = maxSrno + 1;
				string nextCid = "C" + nextSrno.ToString("D4");

				return Ok(new APIResponse
				{
					Status = true,
					Data = nextCid,
					Message = "Next Customer ID generated successfully."
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new APIResponse
				{
					Status = false,
					Message = "Server error: " + ex.Message
				});
			}
		}

	}
}

