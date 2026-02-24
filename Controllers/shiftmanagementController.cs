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
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class shiftmanagementController : ControllerBase
	{
		private readonly IshiftmanagementRepository _shiftmanagementRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "ShiftMaster";

		public shiftmanagementController(IshiftmanagementRepository shiftmanagementRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_shiftmanagementRepository = shiftmanagementRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllShifts()
		{
			try
			{
				string serializedList = string.Empty;
				var encodedList = await _distributedCache.GetAsync(KeyName);

				if (encodedList != null)
				{
					_APIResponse.Data = JsonConvert.DeserializeObject<List<shiftmanagement>>(Encoding.UTF8.GetString(encodedList));
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					var shifts = await _shiftmanagementRepository.GetAllAsync();
					if (shifts != null)
					{
						serializedList = JsonConvert.SerializeObject(shifts);
						encodedList = Encoding.UTF8.GetBytes(serializedList);

						//var options = new DistributedCacheEntryOptions()
						//    .SetSlidingExpiration(TimeSpan.FromDays(30))
						//    .SetAbsoluteExpiration(TimeSpan.FromDays(30));
						//await _distributedCache.SetAsync(KeyName, encodedList, options);

						_APIResponse.Data = shifts;
						_APIResponse.Status = true;
						_APIResponse.StatusCode = HttpStatusCode.OK;
					}
				}

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetShiftById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var shift = await _shiftmanagementRepository.GetByIdAsync(id);
				if (shift == null)
					return NotFound();

				_APIResponse.Data = shift;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateShift(shiftmanagement shift)
		{
			var response = new APIResponse();

			try
			{
				if (shift == null)
				{
					response.Status = false;
					response.Errors.Add("Invalid shift data.");
					response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(response);
				}

				// Normalize inputs
				var pname = shift.pname?.Trim().ToUpper();
				var pcode = shift.pcode?.Trim().ToUpper();
				var shiftName = shift.shift?.Trim().ToUpper();
				var isActive = shift.activef;

				// ✅ Check for existing active shift with same plant, code and shift
				var alreadyExists = await _shiftmanagementRepository.ExistsAsync(s =>
					s.pname == pname && s.pcode == pcode && s.shift == shiftName && s.activef == true);

				if (alreadyExists)
				{
					response.Status = false;
					response.Message = "Shift activation already exists for this plant and shift.";
					response.StatusCode = HttpStatusCode.OK;
					return response;
				}

				// ✅ Optional: Check if any other shift is already active for this plant/code
				var anotherActiveShift = await _shiftmanagementRepository.ExistsAsync(s =>
					s.pname == pname && s.pcode == pcode && s.activef == true);

				if (anotherActiveShift)
				{
					response.Status = false;
					response.Message = "Another shift is already active for this plant and code.";
					response.StatusCode = HttpStatusCode.OK;
					return response;
				}

				// ✅ Save the new shift
				var newShift = new shiftmanagement
				{
					pname = pname,
					pcode = pcode,
					shift = shiftName,
					activef = isActive
				};

				await _shiftmanagementRepository.AddAsync(newShift);
				//await _distributedCache.RemoveAsync(KeyName); // Optional cache clear

				// ✅ Response
				response.Status = true;
				response.StatusCode = HttpStatusCode.Created;
				response.Data = newShift;

				return CreatedAtAction(nameof(GetShiftById), new { id = newShift.Id }, response);
			}
			catch (Exception ex)
			{
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;
				response.Errors.Add(ex.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, response);
			}
		}



		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateShift(int id, shiftmanagement shift)
		{
			var response = new APIResponse();

			try
			{
				if (id != shift.Id)
				{
					response.Status = false;
					response.StatusCode = HttpStatusCode.BadRequest;
					response.Errors.Add("Mismatched shift ID.");
					return BadRequest(response);
				}

				var existingShift = await _shiftmanagementRepository.GetByIdAsync(id);
				if (existingShift == null)
				{
					response.Status = false;
					response.StatusCode = HttpStatusCode.NotFound;
					response.Errors.Add("Shift not found.");
					return NotFound(response);
				}

				// Normalize and update fields
				existingShift.shift = shift.shift?.Trim().ToUpper();
				existingShift.activef = shift.activef; // assumes activef is int

				// Optional: clear cache
				//await _distributedCache.RemoveAsync(KeyName);

				// Update DB
				await _shiftmanagementRepository.UpdateAsync(existingShift);

				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;
				response.Data = null;

				return Ok(response);
			}
			catch (Exception ex)
			{
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;
				response.Errors.Add(ex.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, response);
			}
		}


		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteShift(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var shift = await _shiftmanagementRepository.GetByIdAsync(id);
				if (shift == null)
					return NotFound();

				await _shiftmanagementRepository.DeleteAsync(id);
				await _distributedCache.RemoveAsync(KeyName);

				_APIResponse.Data = true;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}
	}
}

