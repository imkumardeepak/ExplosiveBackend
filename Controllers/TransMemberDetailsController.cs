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
    public class TransMemberDetailsController : ControllerBase
    {
        private readonly ITransMemberDetailRepository _transMemberDetailRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "TransMemberDetail";

        public TransMemberDetailsController(ITransMemberDetailRepository transMemberDetailRepository, IDistributedCache distributedCache, IMapper mapper)
        {
            _transMemberDetailRepository = transMemberDetailRepository;
            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllMembers()
        {
            try
            {
                var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    var members = JsonConvert.DeserializeObject<List<TransMemberDetail>>(Encoding.UTF8.GetString(cachedData));
                    _APIResponse.Data = members;
                }
                else
                {
                    var members = await _transMemberDetailRepository.GetAllAsync();
                    var serializedData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(members));
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromDays(30),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    };
                    await _distributedCache.SetAsync(CacheKey, serializedData, cacheOptions);
                    _APIResponse.Data = members;
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
        public async Task<ActionResult<APIResponse>> GetMemberById(int id)
        {
            if (id == 0)
                return BadRequest();

            try
            {
                var member = await _transMemberDetailRepository.GetByIdAsync(id);
                if (member == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Member not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = member;
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
        public async Task<ActionResult<APIResponse>> CreateMember(TransMemberDetail transMemberDetail)
        {
            try
            {
                if (transMemberDetail == null)
                    return BadRequest();

                await _distributedCache.RemoveAsync(CacheKey);
                await _transMemberDetailRepository.AddAsync(transMemberDetail);

                _APIResponse.Data = transMemberDetail;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetMemberById", new { id = transMemberDetail.Id }, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateMember(int id, TransMemberDetail transMemberDetail)
        {
            try
            {
                if (id != transMemberDetail.Id)
                    return BadRequest();

                var existingMember = await _transMemberDetailRepository.GetByIdAsync(id);
                if (existingMember == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Member not found.");
                    return NotFound(_APIResponse);
                }

                await _distributedCache.RemoveAsync(CacheKey);

                existingMember = _mapper.Map(transMemberDetail, existingMember);

                await _transMemberDetailRepository.UpdateAsync(existingMember);

                _APIResponse.Data = existingMember;
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
        public async Task<ActionResult<APIResponse>> DeleteMember(int id)
        {
            try
            {
                var existingMember = await _transMemberDetailRepository.GetByIdAsync(id);
                if (existingMember == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Member not found.");
                    return NotFound(_APIResponse);
                }

                await _distributedCache.RemoveAsync(CacheKey);
                await _transMemberDetailRepository.DeleteAsync(id);

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
    }
}


