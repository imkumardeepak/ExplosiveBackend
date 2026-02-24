using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustMemberDetailsController : ControllerBase
    {
        private readonly ICustMemberDetailsRepository _custMemberDetailsRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private readonly APIResponse _APIResponse;
        private readonly string KeyName = "CustMemberDetails";

        public CustMemberDetailsController(
            ICustMemberDetailsRepository custMemberDetailsRepository,
            IDistributedCache distributedCache,
            IMapper mapper)
        {
            _custMemberDetailsRepository = custMemberDetailsRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
            _APIResponse = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllCustMemberDetails()
        {
            try
            {
                string cachedData = await _distributedCache.GetStringAsync(KeyName);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    _APIResponse.Data = JsonConvert.DeserializeObject<List<CustMemberDetail>>(cachedData);
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_APIResponse);
                }

                var memberDetails = await _custMemberDetailsRepository.GetAllAsync();
                if (memberDetails != null)
                {
                    var serializedData = JsonConvert.SerializeObject(memberDetails);
                    await _distributedCache.SetStringAsync(KeyName, serializedData, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    });

                    _APIResponse.Data = memberDetails;
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_APIResponse);
                }

                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_APIResponse);
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
        public async Task<ActionResult<APIResponse>> GetCustMemberDetailById(int id)
        {
            try
            {
                var memberDetail = await _custMemberDetailsRepository.GetByIdAsync(id);
                if (memberDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = memberDetail;
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
        public async Task<ActionResult<APIResponse>> CreateCustMemberDetail(CustMemberDetail custMemberDetail)
        {
            try
            {
                if (custMemberDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }

                await _custMemberDetailsRepository.AddAsync(custMemberDetail);
                await _distributedCache.RemoveAsync(KeyName);

                _APIResponse.Data = custMemberDetail;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction(nameof(GetCustMemberDetailById), new { id = custMemberDetail.Id }, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateCustMemberDetail(int id, CustMemberDetail custMemberDetail)
        {
            try
            {
                if (id != custMemberDetail.Id)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }

                var existingDetail = await _custMemberDetailsRepository.GetByIdAsync(id);
                if (existingDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_APIResponse);
                }

                existingDetail = _mapper.Map(custMemberDetail, existingDetail);
                await _custMemberDetailsRepository.UpdateAsync(existingDetail);
                await _distributedCache.RemoveAsync(KeyName);

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
        public async Task<ActionResult<APIResponse>> DeleteCustMemberDetail(int id)
        {
            try
            {
                var existingDetail = await _custMemberDetailsRepository.GetByIdAsync(id);
                if (existingDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_APIResponse);
                }

                await _custMemberDetailsRepository.DeleteAsync(id);
                await _distributedCache.RemoveAsync(KeyName);

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


        // GET: api/CustMemberDetails/{customerId}
        [HttpGet("{customerId}")]
        public async Task<ActionResult<APIResponse>> GetMembersByCustomerId(int customerId)
        {
            var members = await _custMemberDetailsRepository.GetMembersByCustomerIdAsync(customerId);

            if (members == null || !members.Any())
            {
                return NotFound(new { Message = "No members found for the provided customer ID." });
            }

            _APIResponse.Data = members;
            _APIResponse.Status = true;
            _APIResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_APIResponse);
        }

        [HttpGet("{memberId}")]
        public async Task<ActionResult<APIResponse>> GetMemberContactById(int memberId)
        {
            var member = await _custMemberDetailsRepository.GetMemberContactByIdAsync(memberId);

            if (member == null)
            {
                _APIResponse.Data = null ;
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return NotFound(_APIResponse);
            }

            _APIResponse.Data = member;
            _APIResponse.Status = true;
            _APIResponse.StatusCode = HttpStatusCode.OK;


            return Ok(_APIResponse);
        }

    }

}

