using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class ProductionReportController : ControllerBase
	{
		private readonly IProductReportRepository _ProductReportRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly IL1barcodegenerationRepository _l1barcodegenerationRepository;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "ProductionReport";
		private const string CacheKey = "ProductionReport";


		public ProductionReportController(IDistributedCache distributedCache, IMagzineMasterRepository magzineMasterRepository, IPlantMasterRepository plantMasterRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMapper mapper, IBarcodeDataRepository barcodeDataRepository)
		{
			_mapper = mapper;

			_l1barcodegenerationRepository = l1BarcodegenerationRepository;

			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllProductionreport()
		{
			try
			{
				string serializedList = string.Empty;
				var encodedList = await _distributedCache.GetAsync(KeyName);

				if (encodedList != null)
				{
					_APIResponse.Data = JsonConvert.DeserializeObject<List<Models.ProductionReport>>(Encoding.UTF8.GetString(encodedList));
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					var countries = await _ProductReportRepository.GetAllAsync();
					if (countries != null)
					{
						serializedList = JsonConvert.SerializeObject(countries);
						encodedList = Encoding.UTF8.GetBytes(serializedList);

						var options = new DistributedCacheEntryOptions()
							.SetSlidingExpiration(TimeSpan.FromDays(30))
							.SetAbsoluteExpiration(TimeSpan.FromDays(30));
						await _distributedCache.SetAsync(KeyName, encodedList, options);

						_APIResponse.Data = countries;
						_APIResponse.Status = true;
						_APIResponse.StatusCode = HttpStatusCode.OK;
					}
				}

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

		[HttpGet("{ptypecode},{bname},{psize}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CheckProductreportExists(string ptypeCode, string bname, string psize)
		{
			if (string.IsNullOrWhiteSpace(ptypeCode) || string.IsNullOrWhiteSpace(bname) || string.IsNullOrWhiteSpace(psize))
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				_APIResponse.Data = "Invalid or missing parameters.";
				return BadRequest(_APIResponse);
			}

			try
			{
				// Check cache
				var cachedData = await _distributedCache.GetAsync(CacheKey);
				if (cachedData != null)
				{
					var Products = JsonConvert.DeserializeObject<List<Models.ProductionReport>>(Encoding.UTF8.GetString(cachedData));

					bool existsdb = Products.Any(e => e.plantname == ptypeCode && e.brandname == bname && e.productsize == psize);

					if (existsdb)
					{
						_APIResponse.Data = new { Exists = existsdb };
						_APIResponse.Status = true;
						_APIResponse.StatusCode = HttpStatusCode.OK;

						return Ok(_APIResponse);
					}
					else
					{
						// Fetch from database
						var exists = await _ProductReportRepository.ProductreportExistsAsync(ptypeCode, bname, psize);

						// Return response
						if (!exists)
						{
							_APIResponse.Status = false;
							_APIResponse.StatusCode = HttpStatusCode.NotFound;
							_APIResponse.Data = "Product not found.";
							return Ok(_APIResponse);
						}

					}


				}

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

		//[httpget("{bname}")]
		//public async task<actionresult<apiresponse>> getpsizenamescode(string bname)
		//{
		//	var response = new apiresponse();
		//	try
		//	{
		//		var psizenames = await _productreportrepository.getpsizebybrandasync(bname);

		//		if (psizenames.any())
		//		{
		//			response.data = psizenames;
		//			response.status = true;
		//			response.statuscode = httpstatuscode.ok;
		//			return ok(response);
		//		}
		//		else
		//		{
		//			response.status = false;
		//			response.statuscode = httpstatuscode.notfound;
		//			return notfound(response);
		//		}
		//	}
		//	catch (exception ex)
		//	{
		//		response.errors.add(ex.message);
		//		response.status = false;
		//		response.statuscode = httpstatuscode.internalservererror;
		//		return statuscode((int)httpstatuscode.internalservererror, response);
		//	}
		//}


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
				var product = await _ProductReportRepository.GetProductDetasAsync(pcode, brandid, productsize);

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



		//[HttpGet("{fromDate}/{toDate}/{shift}/{reportType}")]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status404NotFound)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> GetL1BarcodeData(string pcode, string brandid, string productsize)
		//{
		//    var product = await _l1barcodegenerationRepository.GetL1BarcodeDataAsync(fromDate, toDate, shift, reportType);
		//}
	}
}

