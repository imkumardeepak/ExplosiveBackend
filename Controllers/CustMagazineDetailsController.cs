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

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustMagazineDetailsController : ControllerBase
    {
        private readonly ICustMagazineDetailsRepository _custMagazineDetailsRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private readonly APIResponse _APIResponse;
        private readonly string KeyName = "CustMagazineDetails";

        public CustMagazineDetailsController(
            ICustMagazineDetailsRepository custMagazineDetailsRepository,
            IDistributedCache distributedCache,
            IMapper mapper)
        {
            _custMagazineDetailsRepository = custMagazineDetailsRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
            _APIResponse = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllCustMagazineDetails()
        {
            try
            {
                string cachedData = await _distributedCache.GetStringAsync(KeyName);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    _APIResponse.Data = JsonConvert.DeserializeObject<List<CustMagazineDetail>>(cachedData);
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_APIResponse);
                }

                var magazineDetails = await _custMagazineDetailsRepository.GetAllAsync();
                if (magazineDetails != null)
                {
                    var serializedData = JsonConvert.SerializeObject(magazineDetails);
                    await _distributedCache.SetStringAsync(KeyName, serializedData, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    });

                    _APIResponse.Data = magazineDetails;
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
        public async Task<ActionResult<APIResponse>> GetCustMagazineDetailById(int id)
        {
            try
            {
                var magazineDetail = await _custMagazineDetailsRepository.GetByIdAsync(id);
                if (magazineDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = magazineDetail;
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
        public async Task<ActionResult<APIResponse>> CreateCustMagazineDetail(CustMagazineDetail custMagazineDetail)
        {
            try
            {
                if (custMagazineDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }

                await _custMagazineDetailsRepository.AddAsync(custMagazineDetail);
                await _distributedCache.RemoveAsync(KeyName);

                _APIResponse.Data = custMagazineDetail;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction(nameof(GetCustMagazineDetailById), new { id = custMagazineDetail.Id }, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateCustMagazineDetail(int id, CustMagazineDetail custMagazineDetail)
        {
            try
            {
                if (id != custMagazineDetail.Id)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }

                var existingDetail = await _custMagazineDetailsRepository.GetByIdAsync(id);
                if (existingDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_APIResponse);
                }

                existingDetail = _mapper.Map(custMagazineDetail, existingDetail);
                await _custMagazineDetailsRepository.UpdateAsync(existingDetail);
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
        public async Task<ActionResult<APIResponse>> DeleteCustMagazineDetail(int id)
        {
            try
            {
                var existingDetail = await _custMagazineDetailsRepository.GetByIdAsync(id);
                if (existingDetail == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_APIResponse);
                }

                await _custMagazineDetailsRepository.DeleteAsync(id);
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

        // GET: api/CustMemberDetails/magazines/{customerId}
        [HttpGet("{customerId}")]
        public async Task<ActionResult<APIResponse>> GetMagazinesCustomerId(int customerId)
        {
            var magazines = await _custMagazineDetailsRepository.GetMagazinesByCustomerIdAsync(customerId);

            if (magazines == null || !magazines.Any())
            {
                _APIResponse.Data = null;
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return NotFound(_APIResponse);
            }

            _APIResponse.Data = magazines;
            _APIResponse.Status = true;
            _APIResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_APIResponse);
        }


    }

}

