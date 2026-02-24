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
    public class TransVehicleDetailsController : ControllerBase
    {
        private readonly ITransVehicleDetailRepository _transVehicleDetailRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "TransVehicleDetail";

        public TransVehicleDetailsController(ITransVehicleDetailRepository transVehicleDetailRepository, IDistributedCache distributedCache, IMapper mapper)
        {
            _transVehicleDetailRepository = transVehicleDetailRepository;
            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllVehicles()
        {
            try
            {
                var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    var vehicles = JsonConvert.DeserializeObject<List<TransVehicleDetail>>(Encoding.UTF8.GetString(cachedData));
                    _APIResponse.Data = vehicles;
                }
                else
                {
                    var vehicles = await _transVehicleDetailRepository.GetAllAsync();
                    var serializedData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(vehicles));
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromDays(30),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    };
                    await _distributedCache.SetAsync(CacheKey, serializedData, cacheOptions);
                    _APIResponse.Data = vehicles;
                }

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
        public async Task<ActionResult<APIResponse>> GetVehicleById(int id)
        {
            if (id == 0)
                return BadRequest();

            try
            {
                var vehicle = await _transVehicleDetailRepository.GetByIdAsync(id);
                if (vehicle == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Vehicle not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = vehicle;
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
        public async Task<ActionResult<APIResponse>> CreateVehicle(TransVehicleDetail transVehicleDetail)
        {
            try
            {
                if (transVehicleDetail == null)
                    return BadRequest();

                await _distributedCache.RemoveAsync(CacheKey);
                await _transVehicleDetailRepository.AddAsync(transVehicleDetail);

                _APIResponse.Data = transVehicleDetail;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetVehicleById", new { id = transVehicleDetail.Id }, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateVehicle(int id, TransVehicleDetail transVehicleDetail)
        {
            try
            {
                if (id != transVehicleDetail.Id)
                    return BadRequest();

                var existingVehicle = await _transVehicleDetailRepository.GetByIdAsync(id);
                if (existingVehicle == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Vehicle not found.");
                    return NotFound(_APIResponse);
                }

                await _distributedCache.RemoveAsync(CacheKey);

                existingVehicle = _mapper.Map(transVehicleDetail, existingVehicle);

                await _transVehicleDetailRepository.UpdateAsync(existingVehicle);

                _APIResponse.Data = existingVehicle;
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
        public async Task<ActionResult<APIResponse>> DeleteVehicle(int id)
        {
            try
            {
                var existingVehicle = await _transVehicleDetailRepository.GetByIdAsync(id);
                if (existingVehicle == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Vehicle not found.");
                    return NotFound(_APIResponse);
                }

                await _distributedCache.RemoveAsync(CacheKey);
                await _transVehicleDetailRepository.DeleteAsync(id);

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


        [HttpGet("{Truckno}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetLicenseAndValidity(int Truckno)
        {
           
            try
            {
                var vehicle = await _transVehicleDetailRepository.GetVehicleByTruckNoAsync(Truckno);
                if (vehicle == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Vehicle not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = vehicle;
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

        [HttpGet("{Truckno}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetLicense(string Truckno)
        {

            try
            {
                var vehicle = await _transVehicleDetailRepository.GetlicBytruckno(Truckno);
                if (vehicle == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Vehicle not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = vehicle;
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
    }
}

