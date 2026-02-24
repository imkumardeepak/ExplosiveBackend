using Peso_Baseed_Barcode_Printing_System_API.Interface;

using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TransportMastersController : ControllerBase
    {
        private readonly ITransportMasterRepository _transportMasterRepository;
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "TransportMaster";

        public TransportMastersController(ITransportMasterRepository transportMasterRepository, IDistributedCache distributedCache, IMapper mapper, ApplicationDbContext context)
        {
            _transportMasterRepository = transportMasterRepository;
            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllTransports()
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
                    var transports = await _transportMasterRepository.GetAllTransportMasterWithDetails();

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
        public async Task<ActionResult<APIResponse>> GetTransportById(int id)
        {
            if (id == 0)
                return BadRequest();

            try
            {
                var transport = await _transportMasterRepository.GetTransportMasterViewModelByIdAsync(id);
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
        public async Task<ActionResult<APIResponse>> CreateTransport(TransportViewModel transportMaster)
        {
            try
            {
                if (transportMaster == null)
                    return BadRequest();

                if ((await _transportMasterRepository.FindByNameAsync(x => x.TName.ToLower() == transportMaster.TName.ToLower() )).Count() != 0)
                {
                    _APIResponse.Message = $"Transporter {transportMaster.TName} already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                /*if ((await _context.TransVehicleDetail.Where(x => x.VehicleNo.ToLower() == transportMaster.VehicleNo.ToLower()).ToListAsync().ToListAsync()).Count() != 0)
                {
                    _APIResponse.Message = $"Transporter {transportMaster.TName} already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                TransportMaster transport = new TransportMaster();
                transport = _mapper.Map(transportMaster, transport);


                //await _distributedCache.RemoveAsync(CacheKey);
                await _transportMasterRepository.AddAsync(transport);

                _APIResponse.Data = transportMaster;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetTransportById", new { id = transportMaster.Id }, _APIResponse);
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateTransport(int id, TransportViewModel transportMaster)
        {
            try
            {
                if (id != transportMaster.Id)
                    return BadRequest();

               /* if ((await _transportMasterRepository.FindByNameAsync(x => x.TName.ToLower() == transportMaster.TName.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = $"Transporter {transportMaster.TName} already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                var existingTransport = await _transportMasterRepository.GetByIdAsync(id);
                if (existingTransport == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Transport not found.");
                    return NotFound(_APIResponse);
                }


                existingTransport = _mapper.Map(transportMaster, existingTransport);
                await _transportMasterRepository.UpdateAsync(existingTransport);


                // Upsert members (add/update/remove)
                await _transportMasterRepository.UpsertAndRemoveMembersAsync(transportMaster.Members, id);

                // Upsert magazines (add/update/remove)
                await _transportMasterRepository.UpsertAndRemoveVehiclesAsync(transportMaster.Vehicles, id);



                //await _distributedCache.RemoveAsync(CacheKey);

                existingTransport = _mapper.Map(transportMaster, existingTransport);

                await _transportMasterRepository.UpdateAsync(existingTransport);

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
        public async Task<ActionResult<APIResponse>> DeleteTransport(int id)
        {
            try
            {
                var existingTransport = await _transportMasterRepository.GetByIdAsync(id);
                if (existingTransport == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Transport not found.");
                    return NotFound(_APIResponse);
                }

                //await _distributedCache.RemoveAsync(CacheKey);
                await _transportMasterRepository.DeleteAsync(id);

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

                var vehicleDetails = await _transportMasterRepository.GetTransportVehicleDetailsByTransporterNameAsync(transporterName);

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


