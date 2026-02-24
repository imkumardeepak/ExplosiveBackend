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
	public class TransitTruckMasterController : ControllerBase
	{
		private readonly ITransitTruckMasterRepository _TransitTruckMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "TransportMaster";

		public TransitTruckMasterController(ITransitTruckMasterRepository TransitTruckMasterRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_TransitTruckMasterRepository = TransitTruckMasterRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllTransit()
		{
			try
			{
				/* var cachedData = await _distributedCache.GetAsync(CacheKey);
                 if (cachedData != null)
                 {
                     var transports = JsonConvert.DeserializeObject<List<TransportMaster>>(Encoding.UTF8.GetString(cachedData));
                     _APIResponse.Data = transports;
                 }
                 else
                 {*/
				var transports = await _TransitTruckMasterRepository.GetAllAsync();
				var serializedData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(transports));
				var cacheOptions = new DistributedCacheEntryOptions
				{
					SlidingExpiration = TimeSpan.FromDays(30),
					AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
				};
				//await _distributedCache.SetAsync(CacheKey, serializedData, cacheOptions);
				_APIResponse.Data = transports;
				//}

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

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetTransitById(int id)
		{
			if (id == 0)
				return BadRequest();

			try
			{
				var transport = await _TransitTruckMasterRepository.GetTransportMasterViewModelByIdAsync(id);
				if (transport == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Transport not found.");
					return NotFound(_APIResponse);
				}

				_APIResponse.Data = transport;
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
		public async Task<ActionResult<APIResponse>> CreateTransit(TransitTruckViewModel TransitTruckViewModel)
		{
			try
			{
				if (TransitTruckViewModel == null)
					return BadRequest();

				// Mapping TransitTruckViewModel to TransitTruckMaster
				//var transport = new List< TransitTruckMaster>();
				//transport=_mapper.Map(TransitTruckViewModel, transport);
				TransitTruckMaster transport = new TransitTruckMaster();
				transport = _mapper.Map(TransitTruckViewModel, transport);


				// var transport = _mapper.Map<TransitTruckMaster>(TransitTruckViewModel);
				await _TransitTruckMasterRepository.AddAsync(transport);

				// TransitTruckMaster transport = new TransitTruckMaster();
				//transport = _mapper.Map(TransitTruckViewModel, transport);
				// Save the transport entity
				//await _TransitTruckMasterRepository.AddAsync(transport);

				_APIResponse.Data = TransitTruckViewModel;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				// Return Created response with data
				return CreatedAtAction("GetTransitById", new { id = TransitTruckViewModel.Id }, _APIResponse);
			}
			catch (Exception ex)
			{
				// Handle exceptions and return Internal Server Error response
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}



		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateTransit(int id, TransitTruckViewModel transportMaster)
		{
			try
			{
				if (id != transportMaster.Id)
					return BadRequest();

				var existingTransport = await _TransitTruckMasterRepository.GetByIdAsync(id);
				if (existingTransport == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Transport not found.");
					return NotFound(_APIResponse);
				}


				existingTransport = _mapper.Map(transportMaster, existingTransport);
				await _TransitTruckMasterRepository.UpdateAsync(existingTransport);


				// Upsert members (add/update/remove)
				await _TransitTruckMasterRepository.InsertAndRemoveMembersAsync(transportMaster.Members, id);

				// Upsert magazines (add/update/remove)
				await _TransitTruckMasterRepository.InsertAndRemoveVehiclesAsync(transportMaster.Vehicles, id);



				//await _distributedCache.RemoveAsync(CacheKey);

				existingTransport = _mapper.Map(transportMaster, existingTransport);

				await _TransitTruckMasterRepository.UpdateAsync(existingTransport);

				_APIResponse.Data = existingTransport;
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
		public async Task<ActionResult<APIResponse>> DeleteTransit(int id)
		{
			try
			{
				var existingTransport = await _TransitTruckMasterRepository.GetByIdAsync(id);
				if (existingTransport == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Transport not found.");
					return NotFound(_APIResponse);
				}

				//await _distributedCache.RemoveAsync(CacheKey);
				await _TransitTruckMasterRepository.DeleteAsync(id);

				_APIResponse.Data = true;
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


		// GET: api/Transport/vehicles/{transporterName}
		[HttpGet("{transporterName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetTruckNumbers(string transporterName)
		{
			try
			{
				if (string.IsNullOrEmpty(transporterName))
				{
					return BadRequest("Transporter name cannot be empty.");
				}

				var vehicleDetails = await _TransitTruckMasterRepository.GetTransittruckDetailsAsync(transporterName);

				if (vehicleDetails == null)
				{
					return NotFound("No transport vehicle details found for the given transporter name.");
				}



				_APIResponse.Data = vehicleDetails;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;


				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
			}
		}

	}
}

