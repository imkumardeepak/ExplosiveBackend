using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DeviceMastersController : ControllerBase
    {
        private readonly IDeviceMasterRepository _deviceMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private APIResponse _APIResponse;
        private readonly string CacheKey = "DeviceMaster";

        public DeviceMastersController(IDeviceMasterRepository deviceMasterRepository, IDistributedCache distributedCache, IMapper mapper)
        {
            _deviceMasterRepository = deviceMasterRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
            _APIResponse = new APIResponse { Errors = new List<string>() };
        }

        // GET: api/DeviceMasters/GetAllDevices
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllDevices()
        {
            try
            {
               /* var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    _APIResponse.Data = JsonConvert.DeserializeObject<List<DeviceMaster>>(Encoding.UTF8.GetString(cachedData));
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_APIResponse);
                }*/

                var devices = await _deviceMasterRepository.GetAllAsync();
                if (devices != null)
                {
                    /*var serializedData = JsonConvert.SerializeObject(devices);
                    await _distributedCache.SetAsync(CacheKey, Encoding.UTF8.GetBytes(serializedData),
                        new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30)));*/

                    _APIResponse.Data = devices;
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                }
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _APIResponse;
            }
        }

        // GET: api/DeviceMasters/GetDeviceById/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetDeviceById(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                /*string cacheKey = $"{CacheKey}_{id}";
                var cachedData = await _distributedCache.GetAsync(cacheKey);
                if (cachedData != null)
                {
                    _APIResponse.Data = JsonConvert.DeserializeObject<DeviceMaster>(Encoding.UTF8.GetString(cachedData));
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_APIResponse);
                }*/

                var device = await _deviceMasterRepository.GetByIdAsync(id);
                if (device == null)
                    return NotFound();

                /*var serializedData = JsonConvert.SerializeObject(device);
                await _distributedCache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(serializedData),
                    new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30)));*/

                _APIResponse.Data = device;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _APIResponse;
            }
        }

        // POST: api/DeviceMasters/CreateDevice
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateDevice([FromBody] DeviceMaster device)
        {
            try
            {
                if (device == null)
                    return BadRequest();

                //await _distributedCache.RemoveAsync(CacheKey);
                await _deviceMasterRepository.AddAsync(device);

                _APIResponse.Data = device;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetDeviceById", new { id = device.Id }, _APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _APIResponse;
            }
        }

        // PUT: api/DeviceMasters/UpdateDevice/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateDevice(int id, [FromBody] DeviceMaster device)
        {
            try
            {
                if (id != device.Id)
                    return BadRequest();

                var existingDevice = await _deviceMasterRepository.GetByIdAsync(id);
                if (existingDevice == null)
                    return NotFound();

/*                await _distributedCache.RemoveAsync(CacheKey);
                await _distributedCache.RemoveAsync($"{CacheKey}_{id}");*/

                existingDevice = _mapper.Map(device, existingDevice);
                await _deviceMasterRepository.UpdateAsync(existingDevice);

                _APIResponse.Data = null;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return _APIResponse;
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _APIResponse;
            }
        }

        // DELETE: api/DeviceMasters/DeleteDevice/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteDevice(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var device = await _deviceMasterRepository.GetByIdAsync(id);
                if (device == null)
                    return NotFound();

                await _deviceMasterRepository.DeleteAsync(id);
               /* await _distributedCache.RemoveAsync(CacheKey);
                await _distributedCache.RemoveAsync($"{CacheKey}_{id}");*/

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
                return _APIResponse;
            }
        }


        [HttpGet("{type}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetDevicesByType(string type)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                    return BadRequest("Device type is required.");

                

                var deviceNumbers = await _deviceMasterRepository.GetDevicesByTypeAsync(type);
                if (deviceNumbers == null || !deviceNumbers.Any())
                    return NotFound("No devices found for the given type.");


                _APIResponse.Data = deviceNumbers;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _APIResponse;
            }
        }


    }
}

