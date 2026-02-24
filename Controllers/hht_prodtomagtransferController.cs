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
    public class hht_prodtomagtransferController : ControllerBase
    {


        private readonly Ihht_prodtomagtransferRepository _hht_prodtomagtransferRepository;

        private readonly IDistributedCache _distributedCache;

        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private readonly string KeyName = "L1boxDeletionReport";
        private const string CacheKey = "L1boxDeletionReport";


        public hht_prodtomagtransferController(IDistributedCache distributedCache, IMapper mapper, Il1barcodereprintRepository l1barcodereprintRepository, Ihht_prodtomagtransferRepository hht_prodtomagtransferRepository)
        {
            _mapper = mapper;

            _hht_prodtomagtransferRepository = hht_prodtomagtransferRepository;

            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();


        }

       
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<APIResponse>> Getprotransfetchdata(string fromDate, string toDate, string reportType, string? pcode)
        //{
        //    try
        //    {
        //        // Convert string dates to DateTime
        //        //DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        //DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        DateTime fromDateTime = DateTime.Parse(fromDate);
        //        DateTime toDateTime = DateTime.Parse(toDate);
        //        // Fetch all records from repository
        //        var barcodeList = await _hht_prodtomagtransferRepository.GetAllAsync();
        //        List<hht_prodtomagtransfer> plantData = new List<hht_prodtomagtransfer>();

        //        if (reportType == "Detailed")
        //        {
        //            plantData = barcodeList
        //                 .Where(x => DateTime.Parse(x.transferdt) >= fromDateTime && DateTime.Parse(x.transferdt) <= toDateTime &&
        //                             (string.IsNullOrEmpty(pcode) || x.pcode == pcode))
        //                 .Select(x => new hht_prodtomagtransfer
        //                 {
        //                     tid = x.tid,
        //                     pcode = x.pcode,
        //                     mfgdt = x.mfgdt,
        //                     truckno = x.truckno,
        //                     l1barcode = x.l1barcode,
        //                     lul = x.lul,
        //                     transferdt = x.transferdt,
        //                     month = x.month,
        //                     year = x.year,
        //                     td = x.td,
        //                     brand = x.brand,
        //                     psize = x.psize,
                            
        //                 }).OrderBy(x => x.tid)
        //                  .ThenBy(x => x.l1barcode)   // Order by barcode count
        //                  .ThenBy(x => x.brand)   // Order by barcode count
        //                 .ToList();
        //        }
        //        else
        //        {
        //            plantData = barcodeList
        //                       .Where(x =>
        //                           DateTime.TryParse(x.transferdt, out DateTime transferDate) &&
        //                           transferDate >= fromDateTime && transferDate <= toDateTime &&
        //                           (string.IsNullOrEmpty(pcode) || x.pcode == pcode))
        //                       .GroupBy(x => new { x.pcode, x.brand, x.psize, x.tid, x.truckno, x.l1barcode, x.lul, x.transferdt, x.month, x.year, x.td })
        //                       .Select(g => new hht_prodtomagtransfer
        //                       {
        //                           tid = g.Key.tid,
        //                           pcode = g.Key.pcode,
        //                           mfgdt = g.Select(x => x.mfgdt).FirstOrDefault(),
        //                           lul = g.Key.lul,
        //                           transferdt = g.Key.transferdt,
        //                           month = g.Key.month,
        //                           year = g.Key.year,
        //                           td = g.Key.td,
        //                           brand = g.Key.brand,
        //                           psize = g.Key.psize,
        //                           truckno = g.Key.truckno,
        //                           l1barcode = g.Select(x => x.l1barcode).Distinct().Count().ToString(),
        //                       })
        //                       .OrderBy(x => x.tid)
        //                       .ThenBy(x => x.l1barcode)
        //                       .ThenBy(x => x.brand)
        //                       .ToList();

        //            //plantData = barcodeList
        //            //   .Where(x => DateTime.Parse(x.transferDt) >= fromDateTime && DateTime.Parse(x.transferDt) <= toDateTime &&
        //            //                 (string.IsNullOrEmpty(plant) || x.plantname == plant))
        //            //      .GroupBy(x => new { x.plantname, x.brandname, x.productsize }) // Corrected Grouping
        //            //       .Select(g => new hht_prodtomagtransfer
        //            //       {
        //            //           plant = g.Key.plantname,
        //            //           brandname = g.Key.brandname,
        //            //           productsize = g.Key.productsize, // Get first available MagName

        //            //           L1Barcode = g.Select(x => x.l1barcode).Distinct().Count().ToString(),

        //            //       })
        //            //    .OrderBy(x => x.brandname)  // Order by brand instead of PlantName
        //            //    .ThenBy(x => x.L1Barcode)   // Order by barcode count
        //            //    .ThenBy(x => x.plant)   // Order by barcode count
        //            //    .ToList();
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



