using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{


	[Route("api/[controller]/[action]")]
        [ApiController]
        [Authorize]
        public class DispatchReportController : ControllerBase
        {
            private readonly IStorageMagazineReportRepository _StorageMagazineReportRepository;
            private readonly IProductMasterRepository _ProductMasterRepository;
            private readonly IMagzineMasterRepository _magzineMasterRepository;
            private readonly IMagzine_StockRepository _Magzine_StockRepository;
            private readonly IDistributedCache _distributedCache;
            private readonly IL1barcodegenerationRepository _l1barcodegenerationRepository;
            private APIResponse _APIResponse;
            private readonly IMapper _mapper;
            private readonly string KeyName = "DispatchReport";
            private const string CacheKey = "DispatchReport";


            public DispatchReportController(IDistributedCache distributedCache, IProductReportRepository ProductMasterRepository, IMagzineMasterRepository magzineMasterRepository, IMagzine_StockRepository Magzine_StockRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMapper mapper, IBarcodeDataRepository barcodeDataRepository)
            {
                _mapper = mapper;

                _l1barcodegenerationRepository = l1BarcodegenerationRepository;

                _distributedCache = distributedCache;
                _APIResponse = new APIResponse();
            }

            //[HttpGet]
            //[ProducesResponseType(StatusCodes.Status200OK)]
            //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
            //[ProducesResponseType(StatusCodes.Status403Forbidden)]
            //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
            //public async Task<ActionResult<APIResponse>> GetAllProductionreport()
            //{
            //	try
            //	{
            //		string serializedList = string.Empty;
            //		var encodedList = await _distributedCache.GetAsync(KeyName);

            //		if (encodedList != null)
            //		{
            //			_APIResponse.Data = JsonConvert.DeserializeObject<List<ProductionReport>>(Encoding.UTF8.GetString(encodedList));
            //			_APIResponse.Status = true;
            //			_APIResponse.StatusCode = HttpStatusCode.OK;
            //		}
            //		else
            //		{
            //			var countries = await _StorageMagazineReportRepository.GetAllAsync();
            //			if (countries != null)
            //			{
            //				serializedList = JsonConvert.SerializeObject(countries);
            //				encodedList = Encoding.UTF8.GetBytes(serializedList);

            //				var options = new DistributedCacheEntryOptions()
            //					.SetSlidingExpiration(TimeSpan.FromDays(30))
            //					.SetAbsoluteExpiration(TimeSpan.FromDays(30));
            //				await _distributedCache.SetAsync(KeyName, encodedList, options);

            //				_APIResponse.Data = countries;
            //				_APIResponse.Status = true;
            //				_APIResponse.StatusCode = HttpStatusCode.OK;
            //			}
            //		}

            //		return Ok(_APIResponse);
            //	}
            //	catch (Exception ex)
            //	{
            //		_APIResponse.Errors.Add(ex.Message);
            //		_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
            //		_APIResponse.Status = false;
            //		return _APIResponse;
            //	}
            //}


            [HttpGet("{magname}")]
            public async Task<ActionResult<APIResponse>> Getmagcode(string magname)
            {
                var response = new APIResponse();
                try
                {
                    var psizenames = await _StorageMagazineReportRepository.GetAllmagzine(magname);

                    if (psizenames.Any())
                    {
                        response.Data = psizenames;
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



