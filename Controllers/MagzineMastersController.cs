using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Linq;
using System.Net;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    // [Authorize]
    public class MagzineMastersController : ControllerBase
    {

        private readonly IMagzineMasterRepository _magzineMasterRepository;
        private readonly IMagzineviewmodelRepository _MagzineviewmodelRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "MagzineMaster";
        private readonly IProductMasterRepository _productMasterRepository;

        public MagzineMastersController(IMagzineMasterRepository magzineMasterRepository, IDistributedCache distributedCache, IMapper mapper, IMagzineviewmodelRepository magzineviewmodelRepository, IProductMasterRepository productMasterRepository)
        {
            _magzineMasterRepository = magzineMasterRepository;
            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _mapper = mapper;
            _MagzineviewmodelRepository = magzineviewmodelRepository;
            _productMasterRepository = productMasterRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllMagzines()
        {
            try
            {

                var magzines = await _magzineMasterRepository.GetMagDetails();
                _APIResponse.Data = magzines;

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
        public async Task<ActionResult<APIResponse>> GetMagzineById(int id)
        {
            if (id == 0)
                return BadRequest();

            try
            {
                var magzine = await _magzineMasterRepository.GetByIdAsync(id);
                if (magzine == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Magzine not found.");
                    return NotFound(_APIResponse);
                }

                var capacityDetails = await _MagzineviewmodelRepository.GetByMagcodeAsync(magzine.mcode);

                var viewModel = new MagzineEditViewModel
                {
                    Magzine = magzine,
                    CapacityDetails = capacityDetails
                };

                _APIResponse.Data = viewModel;
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

        //[HttpGet("{magname}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<APIResponse>> GetMagzineGetaallById(string magname)
        //{
        //    if (magname == "")
        //        return BadRequest();

        //    try
        //    {
        //        var magzine = await _magzineMasterRepository.GetBymagnameIdAsync(magname);
        //        if (magzine == null)
        //        {
        //            _APIResponse.Status = false;
        //            _APIResponse.StatusCode = HttpStatusCode.NotFound;
        //            _APIResponse.Errors.Add("Magzine not found.");
        //            return NotFound(_APIResponse);
        //        }

        //        _APIResponse.Data = magzine;
        //        _APIResponse.Status = true;
        //        _APIResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(_APIResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _APIResponse.Errors.Add(ex.Message);
        //        _APIResponse.Status = false;
        //        _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
        //    }
        //}
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateMagzine(MagzineMaster magzineMaster)
        {
            try
            {
                if (magzineMaster == null || magzineMaster.MagzineMasterDetails == null || !magzineMaster.MagzineMasterDetails.Any())
                    return BadRequest("No capacity data provided.");

                if ((await _magzineMasterRepository.FindByNameAsync(x => x.mfgloc.ToLower().Trim() == magzineMaster.mfgloc.ToLower().Trim() && x.mcode.ToLower().Trim() == magzineMaster.mcode.ToLower().Trim())).Count() != 0)
                {
                    _APIResponse.Message = ("Magzine already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                //CalculateBoxes(magzineMaster.MagzineMasterDetails);

                List<MagzineMaster> magzineDetails = new List<MagzineMaster>();
                magzineDetails.Add(magzineMaster);
                await _magzineMasterRepository.AddRangeAsync(magzineDetails);



                _APIResponse.Data = magzineMaster;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return Created(string.Empty, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateMagzine(int id, MagzineMaster magzineMaster)
        {
            try
            {
                if (id != magzineMaster.Id)
                    return BadRequest("Mismatched magazine ID.");

                var existing = await _magzineMasterRepository.GetMagDetailsbyID(id);
                if (existing == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Magzine not found.");
                    return NotFound(_APIResponse);
                }

                /* if ((await _magzineMasterRepository.FindByNameAsync(x => x.mfgloc.ToLower() == magzineMaster.mfgloc.ToLower() && x.mcode.ToLower() == magzineMaster.mcode.ToLower())).Count() != 0)
                 {
                     _APIResponse.Message = ("Magzine already exists");
                     _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                     _APIResponse.Status = false;
                     return _APIResponse;
                 }*/

                existing.MagzineMasterDetails.Clear();
                _mapper.Map(magzineMaster, existing);
                await _magzineMasterRepository.UpdateAsync(existing);


                _APIResponse.Data = magzineMaster;
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
        public async Task<ActionResult<APIResponse>> DeleteMagzine(int id)
        {
            try
            {
                var existingMagzine = await _magzineMasterRepository.GetByIdAsync(id);
                if (existingMagzine == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Magzine not found.");
                    return NotFound(_APIResponse);
                }

                //await _distributedCache.RemoveAsync(CacheKey);
                await _magzineMasterRepository.DeleteAsync(id);

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

        [HttpGet("{magazineName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckMagazineExists(string magazineName)
        {
            var response = new APIResponse();

            try
            {
                // Check if the magazine exists using the path parameter
                bool exists = await _magzineMasterRepository.MagazineExistsAsync(magazineName);

                if (exists)
                {
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Data = "Magazine already exists.";
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Data = "Magazine Not exists.";
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Errors.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet("{mcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetMagzineByMCode(string mcode)
        {
            try
            {
                // Fetch the magazine from the repository
                var magazine = await _magzineMasterRepository.GetMagazineByMCodeAsync(mcode);

                if (magazine == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Magazine not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = magazine;
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


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetmagNames()
        {
            var response = new APIResponse();

            try
            {
                var magazine = await _magzineMasterRepository.GetmagzineNamesAsync();

                response.Data = magazine;
                response.Status = true;
                response.StatusCode = HttpStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                response.Status = false;
                response.StatusCode = HttpStatusCode.InternalServerError;

                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }


        [HttpGet("{mcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetLicByMCode(string mcode)
        {
            try
            {
                // Fetch the magazine from the repository
                var magazine = await _magzineMasterRepository.GetLicByMCodeAsync(mcode);

                if (magazine == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Magazine not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = magazine;
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


        /*private async Task<IActionResult> CalculateBoxes(MagzineMasterDetails details)
        {
            try
            {
                var products = await _productMasterRepository.FindByNameAsync(x => x.ptype == details.Product && x.psize == details.psize && x.bname == details.bname);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }*/
    }
}

