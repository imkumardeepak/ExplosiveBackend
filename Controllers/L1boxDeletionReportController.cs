using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class L1boxDeletionReportController : ControllerBase
    {
        private readonly IStorageMagazineReportRepository _StorageMagazineReportRepository;
        private readonly IProductMasterRepository _ProductMasterRepository;
        private readonly Il1boxdeletionRepository _l1boxdeletionRepository;
        private readonly IMagzineMasterRepository _magzineMasterRepository;
        private readonly IMagzine_StockRepository _Magzine_StockRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IL1barcodegenerationRepository _l1barcodegenerationRepository;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private readonly string KeyName = "L1boxDeletionReport";
        private const string CacheKey = "L1boxDeletionReport";


        public L1boxDeletionReportController(IDistributedCache distributedCache, IProductReportRepository ProductMasterRepository, IMagzineMasterRepository magzineMasterRepository, IMagzine_StockRepository Magzine_StockRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMapper mapper, IBarcodeDataRepository barcodeDataRepository, Il1boxdeletionRepository l1boxdeletionRepository)
        {
            _mapper = mapper;

            _l1barcodegenerationRepository = l1BarcodegenerationRepository;

            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _l1boxdeletionRepository = l1boxdeletionRepository;

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


        //[HttpGet("{magname}")]
        //public async Task<ActionResult<APIResponse>> Getmagcode(string magname)
        //{
        //    var response = new APIResponse();
        //    try
        //    {
        //        var psizenames = await _StorageMagazineReportRepository.GetAllmagzine(magname);

        //        if (psizenames.Any())
        //        {
        //            response.Data = psizenames;
        //            response.Status = true;
        //            response.StatusCode = HttpStatusCode.OK;
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            response.Status = false;
        //            response.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //        response.Status = false;
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode((int)HttpStatusCode.InternalServerError, response);
        //    }
        //}

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<APIResponse>> Getl1boxdata(string fromDate, string toDate, string reportType)
        //{
        //    try
        //    {
        //        // Convert string dates to DateTime
        //        //DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        //DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        DateTime fromDateTime = DateTime.Parse(fromDate);
        //        DateTime toDateTime = DateTime.Parse(toDate);
        //        // Fetch all records from repository
        //        var barcodeList = await _l1boxdeletionRepository.GetAllAsync();
        //        List<L1boxDeletionReport> plantData = new List<L1boxDeletionReport>();

        //        if (reportType == "Detailed")
        //        {
        //            plantData = barcodeList
        //                 .Where(x => DateTime.Parse(x.mfgdt) >= fromDateTime && DateTime.Parse(x.mfgdt) <= toDateTime)
        //                 .Select(x => new L1boxDeletionReport
        //                 {
        //                     plant = x.plant,
        //                     brand = x.brand,
        //                     productsize = x.psize,
        //                     mfgDt = DateTime.Parse(x.mfgdt),
        //                     l1barcode = x.l1barcode,
        //                     deletionDt = x.deldt,
        //                     reason = x.reason,
        //                 }).OrderBy(x => x.brand)
        //                  .ThenBy(x => x.l1barcode)   // Order by barcode count
        //                  .ThenBy(x => x.plant)   // Order by barcode count
        //                 .ToList();
        //        }
        //        else
        //        {
        //            plantData = barcodeList
        //                 .Where(x => DateTime.Parse(x.mfgdt) >= fromDateTime && DateTime.Parse(x.mfgdt) <= toDateTime)
        //                  .GroupBy(x => new { x.plant, x.brand, x.psize}) // Corrected Grouping
        //                   .Select(g => new L1boxDeletionReport
        //                   {
        //                       plant = g.Key.plant,
        //                       brand = g.Key.brand,
        //                       productsize = g.Key.psize, // Get first available MagName

        //                       l1barcode = g.Select(x => x.l1barcode).Distinct().Count().ToString(),

        //                   })
        //                .OrderBy(x => x.brand)  // Order by brand instead of PlantName
        //                .ThenBy(x => x.l1barcode)   // Order by barcode count
        //                .ThenBy(x => x.plant)   // Order by barcode count
        //                .ToList();
        //        }


        //        // Prepare API response
        //        _APIResponse.Data = plantData;
        //        _APIResponse.Status = true;
        //        _APIResponse.StatusCode = HttpStatusCode.OK;

        //        return Ok(_APIResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _APIResponse.Errors.Add(ex.Message);
        //        _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        _APIResponse.Status = false;

        //        return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
        //    }
        //}
    }
}

