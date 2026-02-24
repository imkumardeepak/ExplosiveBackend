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
    public class BrandMastersController : ControllerBase
    {
        private readonly IBrandMasterRepository _brandMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "BrandMaster";

        public BrandMastersController(IBrandMasterRepository brandMasterRepository, IDistributedCache distributedCache,IMapper mapper)
        {
            _brandMasterRepository = brandMasterRepository;
            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllBrands()
        {
            try
            {
               /* var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    var brands = JsonConvert.DeserializeObject<List<BrandMaster>>(Encoding.UTF8.GetString(cachedData));
                    _APIResponse.Data = brands;
                }
                else
                {*/
                    var brands = await _brandMasterRepository.GetAllAsync();
                    var serializedData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(brands));
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromDays(30),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    };
                    await _distributedCache.SetAsync(CacheKey, serializedData, cacheOptions);
                    _APIResponse.Data = brands;
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
        public async Task<ActionResult<APIResponse>> GetBrandById(int id)
        {

            if (id == 0)
                return BadRequest();

            try
            {
                var brand = await _brandMasterRepository.GetByIdAsync(id);
                if (brand == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Brand not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = brand;
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
        public async Task<ActionResult<APIResponse>> CreateBrand(BrandMaster brandMaster)
        {
            try
            {
                if (brandMaster == null)
                    return BadRequest();

                if ((await _brandMasterRepository.FindByNameAsync(x => x.plant_type.ToLower() == brandMaster.plant_type.ToLower() && x.bname.ToLower() == brandMaster.bname.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = ("Brand already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                //For Max ID
                var brands = await _brandMasterRepository.GetAllAsync();
                int nextId = (brands?.Max(b => (int?)b.Id) ?? 0) + 1;

                brandMaster.Id = nextId;

                //await _distributedCache.RemoveAsync(CacheKey);
                await _brandMasterRepository.AddAsync(brandMaster);

                _APIResponse.Data = brandMaster;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetBrandById", new { id = brandMaster.Id }, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateBrand(int id, BrandMaster brandMaster)
        {
            try
            {
                if (id != brandMaster.Id)
                    return BadRequest();

                var existingBrand = await _brandMasterRepository.GetByIdAsync(id);
                if (existingBrand == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Brand not found.");
                    return NotFound(_APIResponse);
                }

                /*if ((await _brandMasterRepository.FindByNameAsync(x => x.plant_type.ToLower() == brandMaster.plant_type.ToLower() && x.bname.ToLower() == brandMaster.bname.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = ("Brand already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                //await _distributedCache.RemoveAsync(CacheKey);


                existingBrand = _mapper.Map(brandMaster, existingBrand);

                await _brandMasterRepository.UpdateAsync(existingBrand);

                _APIResponse.Data = existingBrand;
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
        public async Task<ActionResult<APIResponse>> DeleteBrand(int id)
        {
            try
            {
                var existingBrand = await _brandMasterRepository.GetByIdAsync(id);
                if (existingBrand == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Brand not found.");
                    return NotFound(_APIResponse);
                }

                //await _distributedCache.RemoveAsync(CacheKey);
                await _brandMasterRepository.DeleteAsync(id);

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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllBrandNames()
        {
            try
            {
                var brandNames = await _brandMasterRepository.GetAllBrandNamesAsync();
                _APIResponse.Data = brandNames;
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

        [HttpGet("{brandName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetIdByBrandName(string brandName)
        {
            try
            {
                var brandId = await _brandMasterRepository.GetIdByBrandNameAsync(brandName);
                if (brandId == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Brand not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = brandId;
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

		[HttpGet("{brandName}/{brandId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CheckBrandExists(string brandName, string brandId)
		{
			

			try
			{

				/*// Check cache
				var cachedData = await _distributedCache.GetAsync(CacheKey);
				if (cachedData != null)
				{
					var brands = JsonConvert.DeserializeObject<List<BrandMaster>>(Encoding.UTF8.GetString(cachedData));

					bool exists = brands.Any(e => e.bname == brandName && e.bid == brandId);
					_APIResponse.Data = new { BrandExists = exists };
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;

					return exists ? Ok(_APIResponse) : NotFound(_APIResponse);
				}*/

				// Fetch from database
				var existsInDb = await _brandMasterRepository.CheckBrandExistsAsync(brandName, brandId);

				// Return response
				if (!existsInDb)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Brand not found.");
					return NotFound(_APIResponse);
				}

				_APIResponse.Data = new { BrandExists = true };
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

        [HttpGet("{pcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetBrandNamesByPCode(string pcode)
        {
            try
            {
               /* // Check if the cache contains the brand data
                var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    var brands = JsonConvert.DeserializeObject<List<BrandMaster>>(Encoding.UTF8.GetString(cachedData));

                    // Filter the brands by pcode and return only the brand names
                    var brandNames = brands.Where(b => b.pcode == pcode)
                                            .Select(b => b.bname)
                                            .ToList();

                    if (brandNames.Any())
                    {
                        _APIResponse.Data = brandNames;
                        _APIResponse.Status = true;
                        _APIResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(_APIResponse);
                    }
                    else
                    {
                        _APIResponse.Status = false;
                        _APIResponse.StatusCode = HttpStatusCode.NotFound;
                        _APIResponse.Errors.Add("No brands found for the provided pcode.");
                        return NotFound(_APIResponse);
                    }
                }*/

                // Fetch from database if cache doesn't contain the data
                var brandNamesFromDb = await _brandMasterRepository.GetBrandNamesByPCodeAsync(pcode);

                if (brandNamesFromDb == null || !brandNamesFromDb.Any())
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("No brands found for the provided pcode.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = brandNamesFromDb;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }

        [HttpGet("{pname}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetBrandNamesByPName(string pname)
        {
            try
            {
                /*// Check if the cache contains the brand data
                var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    var brands = JsonConvert.DeserializeObject<List<BrandMaster>>(Encoding.UTF8.GetString(cachedData));

                    // Filter the brands by pcode and return only the brand names
                    var brandNames = brands.Where(b => b.pname == pname)
                                            .Select(b => b.bname)
                                            .ToList();

                    if (brandNames.Any())
                    {
                        _APIResponse.Data = brandNames;
                        _APIResponse.Status = true;
                        _APIResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(_APIResponse);
                    }
                    //else
                    //{
                    //    _APIResponse.Status = false;
                    //    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    //    _APIResponse.Errors.Add("No brands found for the provided pcode.");
                    //    return NotFound(_APIResponse);
                    //}
                }*/

                // Fetch from database if cache doesn't contain the data
                var brandNamesFromDb = await _brandMasterRepository.GetBrandNamesByPnameAsync(pname);

                if (brandNamesFromDb == null || !brandNamesFromDb.Any())
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("No brands found for the provided pcode.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = brandNamesFromDb;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetbrandsNames()
        {
            var response = new APIResponse();

            try
            {
                var magazine = await _brandMasterRepository.GetbrandNamesAsync();
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

    }
}

