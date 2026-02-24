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
    public class ResetTypeMasterController : ControllerBase
    {
        private readonly IResetTypeMasterRepository _ResetTypeMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private readonly string KeyName = "MfgMaster";

        public ResetTypeMasterController(IResetTypeMasterRepository ResetTypeMasterRepository, IDistributedCache distributedCache, IMapper mapper)
        {
            _ResetTypeMasterRepository = ResetTypeMasterRepository;
            _APIResponse = new();
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        // GET: api/MfgMasters
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllresets()
        {
            try
            {
                /*  string serializedList = string.Empty;
                  var encodedList = await _distributedCache.GetAsync(KeyName);

                  if (encodedList != null)
                  {
                      _APIResponse.Data = JsonConvert.DeserializeObject<List<MfgMaster>>(Encoding.UTF8.GetString(encodedList));
                      _APIResponse.Status = true;
                      _APIResponse.StatusCode = HttpStatusCode.OK;
                  }
                  else
                  {*/
                var mfgMasters = await _ResetTypeMasterRepository.GetAllAsync();
                if (mfgMasters != null)
                {
                    /*  serializedList = JsonConvert.SerializeObject(mfgMasters);
                      encodedList = Encoding.UTF8.GetBytes(serializedList);*/

                    var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(30))
                        .SetAbsoluteExpiration(TimeSpan.FromDays(30));
                    //await _distributedCache.SetAsync(KeyName, encodedList, options);

                    _APIResponse.Data = mfgMasters;
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

        // GET: api/MfgMasters/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetResetById(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var mfgMaster = await _ResetTypeMasterRepository.GetByIdAsync(id);
                if (mfgMaster == null)
                    return NotFound();

                _APIResponse.Data = mfgMaster;
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


        // GET: api/MfgMasters/5
        //[HttpGet("{mfgname}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<APIResponse>> GetMfgMasterByName(string mfgname)
        //{
        //    try
        //    {
        //        if (mfgname == null)
        //            return BadRequest();

        //        var mfgMaster = await _ResetTypeMasterRepository.GetCodeByMfgNameAsync(mfgname);
        //        if (mfgMaster == null)
        //            return NotFound();

        //        _APIResponse.Data = mfgMaster;
        //        _APIResponse.Status = true;
        //        _APIResponse.StatusCode = HttpStatusCode.OK;

        //        return Ok(_APIResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _APIResponse.Errors.Add(ex.Message);
        //        _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        _APIResponse.Status = false;
        //        return _APIResponse;
        //    }
        //}


        // POST: api/MfgMasters
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateReset(ResetTypeMaster mfgMaster)
        {
            try
            {
                if (mfgMaster == null)
                    return BadRequest();

                // CHECK: Duplicate ResetType and YearType
                var existing = await _ResetTypeMasterRepository
                    .GetFirstOrDefaultAsync(x => x.resettype == mfgMaster.resettype && x.yeartype == mfgMaster.yeartype);

                if (existing != null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    _APIResponse.Message =  $"Reset Type '{mfgMaster.resettype}' with Year Type '{mfgMaster.yeartype}' already exists.";
                    return _APIResponse;
                }

                // If not duplicate, then insert
                await _ResetTypeMasterRepository.AddAsync(mfgMaster);

                _APIResponse.Data = mfgMaster;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return  _APIResponse;
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }


        // PUT: api/MfgMasters/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateRest(int id, ResetTypeMaster mfgMaster)
        {
            try
            {
                if (id != mfgMaster.Id)
                    return BadRequest();

                var existingMfgMaster = await _ResetTypeMasterRepository.GetByIdAsync(id);
              /*  if (existingMfgMaster == null)
                {
                    _APIResponse.Data = null;
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.Found;
                    
                    return _APIResponse;
                }*/

                //// CHECK: Duplicate ResetType and YearType
                //var existing = await _ResetTypeMasterRepository
                //    .GetFirstOrDefaultAsync(x => x.resettype == mfgMaster.resettype && x.yeartype == mfgMaster.yeartype);

                //if (existing != null)
                //{
                //    _APIResponse.Status = false;
                //    _APIResponse.StatusCode = HttpStatusCode.OK;
                //    _APIResponse.Message = $"Reset Type '{mfgMaster.resettype}' with Year Type '{mfgMaster.yeartype}' already exists.";
                //    return _APIResponse;
                //}

                //await _distributedCache.RemoveAsync(KeyName);

                existingMfgMaster = _mapper.Map(mfgMaster, existingMfgMaster);

                await _ResetTypeMasterRepository.UpdateAsync(existingMfgMaster);

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

        // DELETE: api/MfgMasters/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteReset(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var mfgMaster = await _ResetTypeMasterRepository.GetByIdAsync(id);
                if (mfgMaster == null)
                    return NotFound();

                await _ResetTypeMasterRepository.DeleteAsync(id);
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

