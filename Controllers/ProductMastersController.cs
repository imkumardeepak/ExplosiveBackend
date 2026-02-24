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
    public class ProductMastersController : ControllerBase
    {
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "ProductMaster";

        public ProductMastersController(IProductMasterRepository productMasterRepository, IDistributedCache distributedCache,IMapper mapper)
        {
            _productMasterRepository = productMasterRepository;
            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllProducts()
        {
            try
            {
               /* var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    var products = JsonConvert.DeserializeObject<List<ProductMaster>>(Encoding.UTF8.GetString(cachedData));
                    _APIResponse.Data = products;
                }
                else
                {*/
                    var products = await _productMasterRepository.GetAllAsync();
                    /*var serializedData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(products));
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromDays(30),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    };
                    await _distributedCache.SetAsync(CacheKey, serializedData, cacheOptions);*/
                _APIResponse.Data = products;
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
        public async Task<ActionResult<APIResponse>> GetProductById(int id)
        {
            if (id == 0)
                return BadRequest();

            try
            {
                var product = await _productMasterRepository.GetByIdAsync(id);
                if (product == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Product not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = product;
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
        public async Task<ActionResult<APIResponse>> CreateProduct(ProductMaster productMaster)
        {
            try
            {
                if (productMaster == null)
                    return BadRequest();

                if ((await _productMasterRepository.FindByNameAsync(x => x.bname.ToLower() == productMaster.bname.ToLower() && x.ptype.ToLower() == productMaster.ptype.ToLower() && x.psize.ToLower() == productMaster.psize.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = ("Product already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                //For Next ID
                var products = await _productMasterRepository.GetAllAsync();
                int nextId = (products?.Max(b => (int?)b.Id) ?? 0) + 1;

                productMaster.Id = nextId;

                //await _distributedCache.RemoveAsync(CacheKey);
                await _productMasterRepository.AddAsync(productMaster);

                _APIResponse.Data = productMaster;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetProductById", new { id = productMaster.Id }, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdateProduct(int id, ProductMaster productMaster)
        {
            try
            {
                if (id != productMaster.Id)
                    return BadRequest();

               /* if ((await _productMasterRepository.FindByNameAsync(x => x.bname.ToLower() == productMaster.bname.ToLower() && x.ptype.ToLower() == productMaster.ptype.ToLower() && x.psize.ToLower() == productMaster.psize.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = ("Product already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                var existingProduct = await _productMasterRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Product not found.");
                    return NotFound(_APIResponse);
                }

                //await _distributedCache.RemoveAsync(CacheKey);

                //existingProduct.bname = productMaster.bname;
                //existingProduct.bid = productMaster.bid;
                //existingProduct.ptype = productMaster.ptype;
                //existingProduct.ptypecode = productMaster.ptypecode;
                // Map other fields as needed

                existingProduct= _mapper.Map(productMaster, existingProduct);


                await _productMasterRepository.UpdateAsync(existingProduct);

                _APIResponse.Data = existingProduct;
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
        public async Task<ActionResult<APIResponse>> DeleteProduct(int id)
        {
            try
            {
                var existingProduct = await _productMasterRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Product not found.");
                    return NotFound(_APIResponse);
                }

                //await _distributedCache.RemoveAsync(CacheKey);
                await _productMasterRepository.DeleteAsync(id);

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

        [HttpGet("{ptypecode},{bname},{psize}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CheckProductExists(string ptypeCode, string bname,string psize)
        {
            if (string.IsNullOrWhiteSpace(ptypeCode) || string.IsNullOrWhiteSpace(bname) || string.IsNullOrWhiteSpace(psize))
            {
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                _APIResponse.Data ="Invalid or missing parameters.";
                return BadRequest(_APIResponse);
            }

            try
            {
                /*// Check cache
                var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    var Products = JsonConvert.DeserializeObject<List<ProductMaster>>(Encoding.UTF8.GetString(cachedData));

                    bool existsdb = Products.Any(e => e.ptype == ptypeCode && e.bname == bname && e.psize == psize);

                    if (existsdb)
                    {
                        _APIResponse.Data = new { Exists = existsdb };
                        _APIResponse.Status = true;
                        _APIResponse.StatusCode = HttpStatusCode.OK;
                
                        return Ok(_APIResponse);
                    }
                    else
                    {*/
                        // Fetch from database
                        var exists = await _productMasterRepository.ProductExistsAsync(ptypeCode, bname, psize);

                        // Return response
                        if (!exists)
                        {
                            _APIResponse.Status = false;
                            _APIResponse.StatusCode = HttpStatusCode.NotFound;
                            _APIResponse.Data="Product not found.";
                            return Ok(_APIResponse);
                        }

                    //}                    
                //}

                _APIResponse.Data = new { Exists = true };
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

        [HttpGet("{brandid}/{pcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetPsizeNames(string brandid, string pcode)
        {
            try
            {
                // Call the repository method
                var psizeNames = await _productMasterRepository.GetPsizeNamesByBrandAndPCodeAsync(brandid, pcode);

                if (psizeNames.Any())
                {
                    _APIResponse.Data = psizeNames;
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_APIResponse);
                }
                else
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    //_APIResponse.Errors.Add("No psize names found for the provided brandid and pcode.");
                    return NotFound(_APIResponse);
                }
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }


        [HttpGet("{pcode}/{brandid}/{productsize}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProductDetails(string pcode, string brandid, string productsize)
        {
            if (string.IsNullOrWhiteSpace(pcode) || string.IsNullOrWhiteSpace(brandid) || string.IsNullOrWhiteSpace(productsize))
            {
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                _APIResponse.Data = "Invalid or missing parameters.";
                return BadRequest(_APIResponse);
            }

            try
            {
                // Retrieve product details
                var product = await _productMasterRepository.GetProductDetailsAsync(pcode, brandid, productsize);

                if (product == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Data = "Product not found.";
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = product;
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


        [HttpGet("{bname}")]
        public async Task<ActionResult<APIResponse>> GetPsizeNamescode(string bname)
        {
            var response = new APIResponse();
            try
            {
                var psizeNames = await _productMasterRepository.GetPsizeByBrandAsync(bname);

                if (psizeNames.Any())
                {
                    response.Data = psizeNames;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                response.Status = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        [HttpGet("{BrandId}")]
        public async Task<ActionResult<APIResponse>> GetPsizbybid(string BrandId)
        {

            var response = new APIResponse();
            try
            {
                var psizeNames = await _productMasterRepository.GetPsizebybrand(BrandId);

                if (psizeNames.Any())
                {
                    response.Data = psizeNames;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                response.Status = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }
        [HttpGet("{productsize}")]
        public async Task<ActionResult<APIResponse>> GetPsizecodedata(string productsize)
        {
            var response = new APIResponse();
            try
            {
                var psizeNames = await _productMasterRepository.GetPsizebypcode(productsize);

                if (psizeNames.Any())
                {
                    response.Data = psizeNames;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }
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

