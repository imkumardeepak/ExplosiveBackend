using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UomMasterController : ControllerBase
    {
        private readonly IUomMasterRepository _UomMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private readonly string KeyName = "UomMaster";

        public UomMasterController(IUomMasterRepository UomMasterRepository, IDistributedCache distributedCache, IMapper mapper)
        {
            _UomMasterRepository = UomMasterRepository;
            _APIResponse = new();
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllUOM()
        {
            try
            {
                /* string serializedList = string.Empty;
                 var encodedList = await _distributedCache.GetAsync(KeyName);

                 if (encodedList != null)
                 {
                     _APIResponse.Data = JsonConvert.DeserializeObject<List<ShiftMaster>>(Encoding.UTF8.GetString(encodedList));
                     _APIResponse.Status = true;
                     _APIResponse.StatusCode = HttpStatusCode.OK;
                 }
                 else
                 {*/
                var shifts = await _UomMasterRepository.GetAllAsync();
                if (shifts != null)
                {
                    /* serializedList = JsonConvert.SerializeObject(shifts);
                     encodedList = Encoding.UTF8.GetBytes(serializedList);*/

                    var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(30))
                        .SetAbsoluteExpiration(TimeSpan.FromDays(30));
                    //await _distributedCache.SetAsync(KeyName, encodedList, options);

                    _APIResponse.Data = shifts;
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                }
                //}

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
        public async Task<ActionResult<APIResponse>> GetUOMById(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var shift = await _UomMasterRepository.GetByIdAsync(id);
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
        public async Task<ActionResult<APIResponse>> CreateUOM(UomMaster shift)
        {
            try
            {
                if (shift == null)
                    return BadRequest();

                if ((await _UomMasterRepository.FindByNameAsync(x => x.uom == shift.uom)).Count() != 0)
                {
                    _APIResponse.Message = $"UOM {shift.uom} already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                //await _distributedCache.RemoveAsync(KeyName);
                await _UomMasterRepository.AddAsync(shift);

                _APIResponse.Data = shift;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetUOMById", new { id = shift.Id }, _APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateUOM(int id, UomMaster shift)
        {
            try
            {
                if (id != shift.Id)
                    return BadRequest();

                var existingShift = await _UomMasterRepository.GetByIdAsync(id);
                if (existingShift == null)
                    return NotFound();

                //if ((await _UomMasterRepository.FindByNameAsync(x => x.uomcode == shift.uomcode)).Count() != 0)
                //{
                //    _APIResponse.Message = $"UOM Code {shift.uomcode} already exists!";
                //    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                //    _APIResponse.Status = false;
                //    return _APIResponse;
                //}

                //await _distributedCache.RemoveAsync(KeyName);

                existingShift = _mapper.Map(shift, existingShift);
                await _UomMasterRepository.UpdateAsync(existingShift);

                _APIResponse.Data = null;
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteUOM(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var shift = await _UomMasterRepository.GetByIdAsync(id);
                if (shift == null)
                    return NotFound();

                await _UomMasterRepository.DeleteAsync(id);
                //await _distributedCache.RemoveAsync(KeyName);

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

