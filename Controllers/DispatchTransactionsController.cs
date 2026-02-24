using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class DispatchTransactionsController : ControllerBase
	{
		private readonly IDispatchTransactionRepository _dispatchTransactionRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly IMapper _mapper;
		private APIResponse _APIResponse;
		private readonly string CacheKey = "DispatchTransaction";

		public DispatchTransactionsController(IDispatchTransactionRepository dispatchTransactionRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_dispatchTransactionRepository = dispatchTransactionRepository;
			_distributedCache = distributedCache;
			_mapper = mapper;
			_APIResponse = new APIResponse { Errors = new List<string>() };
		}

		// GET: api/DispatchTransaction/GetAllTransactions
		[HttpGet("{indentno}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetTruckBrandByIndentNo(string indentno)
		{
			try
			{
				// Replace "." with "/"
				var formattedIndentNo = indentno.Replace(".", "/");

				// Fetch truck number and brands
				var transactions = await _dispatchTransactionRepository.GetTruckNoByIndentNo(formattedIndentNo);
				var brands = await _dispatchTransactionRepository.GetBrandsByIndentNo(formattedIndentNo, transactions);

				if (transactions != null)
				{
					// Create dynamic object without using a model
					var response = new
					{
						TruckNo = transactions, // Assuming this returns a string
						Brands = brands         // Assuming this returns a list or object
					};

					_APIResponse.Data = response;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}



		// GET: api/DispatchTransaction/GetAllTransactions
		[HttpGet("{indentno}/{mfgdt}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetTruckBrandByIndentNoDate(string indentno, string mfgdt)
		{
			try
			{
				// Replace "." with "/"
				var formattedIndentNo = indentno.Replace(".", "/");
				var formattedDate = Convert.ToDateTime(mfgdt.Replace(".", "/"));

				// Fetch truck number and brands
				var transactions = await _dispatchTransactionRepository.GetTruckNoByIndentNo(formattedIndentNo, formattedDate);
				var brands = await _dispatchTransactionRepository.GetBrandsByIndentNo(formattedIndentNo, transactions, formattedDate);

				if (transactions != null)
				{
					// Create dynamic object without using a model
					var response = new
					{
						TruckNo = transactions, // Assuming this returns a string
						Brands = brands         // Assuming this returns a list or object
					};

					_APIResponse.Data = response;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}



		[HttpGet("{indentNo}/{brandName}")]
		public async Task<ActionResult<APIResponse>> GetBrandIdAndPSizes(string indentNo, string brandName)
		{
			try
			{
				var brandId = await _dispatchTransactionRepository.GetBrandIdByIndentNoAndBrandName(indentNo.Replace(".", "/"), brandName);
				var pSizes = await _dispatchTransactionRepository.GetPSizesByIndentNoBrandNameAndBrandId(indentNo.Replace(".", "/"), brandName, brandId);

				var response = new
				{
					BrandId = brandId,
					ProductSizes = pSizes
				};

				_APIResponse.Data = response;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet("{indentNo}/{brandName}/{mfgdt}")]
		public async Task<ActionResult<APIResponse>> GetBrandIdAndPSizesDate(string indentNo, string brandName, string mfgdt)
		{
			// Replace "." with "/"
			var formattedIndentNo = indentNo.Replace(".", "/");
			var formattedDate = Convert.ToDateTime(mfgdt.Replace(".", "/"));

			try
			{
				var brandId = await _dispatchTransactionRepository.GetBrandIdByIndentNoAndBrandName(indentNo.Replace(".", "/"), brandName, formattedDate);
				var pSizes = await _dispatchTransactionRepository.GetPSizesByIndentNoBrandNameAndBrandId(indentNo.Replace(".", "/"), brandName, brandId, formattedDate);

				var response = new
				{
					BrandId = brandId,
					ProductSizes = pSizes
				};

				_APIResponse.Data = response;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}



		[HttpGet("{indentNo}/{brandName}/{brandId}/{psize}")]
		public async Task<ActionResult<APIResponse>> GetPSizesAndCodes(string indentNo, string brandName, string brandId, string psize)
		{
			try
			{
				var pSizesWithCodes = await _dispatchTransactionRepository.GetSizeCodesByIndentNo(indentNo.Replace(".", "/"), brandName, brandId, psize);

				_APIResponse.Data = pSizesWithCodes;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet("{indentNo}/{brandName}/{brandId}/{psize}/{mfgdt}")]
		public async Task<ActionResult<APIResponse>> GetPSizesAndCodesDate(string indentNo, string brandName, string brandId, string psize, string mfgdt)
		{
			// Replace "." with "/"
			var formattedIndentNo = indentNo.Replace(".", "/");
			var formattedDate = Convert.ToDateTime(mfgdt.Replace(".", "/"));

			try
			{
				var pSizesWithCodes = await _dispatchTransactionRepository.GetSizeCodesByIndentNo(indentNo.Replace(".", "/"), brandName, brandId, psize, formattedDate);

				var magazines = await _dispatchTransactionRepository.GetMagazinesforDispatch(formattedIndentNo, brandName, brandId, psize, pSizesWithCodes, formattedDate);

				var response = new
				{
					psizecode = pSizesWithCodes,
					magazines = magazines
				};

				_APIResponse.Data = response;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpGet("{indentNo}/{brandName}/{brandId}/{psize}/{mfgdt}")]
		public async Task<ActionResult<APIResponse>> GetmagnPsizedata(string indentNo, string brandName, string brandId, string psize, string mfgdt)
		{
			// Replace "." with "/"
			var formattedIndentNo = indentNo.Replace(".", "/");
			var formattedDate = Convert.ToDateTime(mfgdt.Replace(".", "/"));

			try
			{
				var pSizesWithCodes = await _dispatchTransactionRepository.GetSizemagIndentNo(indentNo.Replace(".", "/"), brandName, brandId, psize, formattedDate);

				var magazines = await _dispatchTransactionRepository.Getdispatchdata(formattedIndentNo, brandName, brandId, psize, pSizesWithCodes, formattedDate);

				var response = new
				{
					psizecode = pSizesWithCodes,
					magazines = magazines
				};

				_APIResponse.Data = response;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpGet("{indentNo}/{brandName}/{mfgdt}")]
		public async Task<ActionResult<APIResponse>> GetREgenBrandIdAndPSizesDate(string indentNo, string brandName, string mfgdt)
		{
			// Replace "." with "/"
			var formattedIndentNo = indentNo.Replace(".", "/");
			var formattedDate = Convert.ToDateTime(mfgdt.Replace(".", "/"));

			try
			{
				var brandId = await _dispatchTransactionRepository.GetBrandIdnpsize(indentNo.Replace(".", "/"), brandName, formattedDate);
				var pSizes = await _dispatchTransactionRepository.GetProductsizedata(indentNo.Replace(".", "/"), brandName, brandId, formattedDate);

				var response = new
				{
					BrandId = brandId,
					ProductSizes = pSizes
				};

				_APIResponse.Data = response;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet("{indentNo}/{truckno}/{brandId}/{psize}")]
		public async Task<ActionResult<APIResponse>> GetL1DataWithoutMag(string indentNo, string truckno, string brandId, string psize)
		{
			try
			{
				var l1Data = await _dispatchTransactionRepository.GetL1DataWithoutMagName(indentNo.Replace(".", "/"), truckno, brandId, psize);

				_APIResponse.Data = l1Data;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}




		[HttpGet("{indentNo}/{truckNo}/{brandId}/{pSizeCode}/{mfgDate}/{magName}/{fromcase}/{tocase}")]
		public async Task<ActionResult<APIResponse>> GetTransMagDetail(string indentNo, string truckNo, string brandId, string pSizeCode, string mfgDate, string magName, string fromcase, string tocase)
		{
			try
			{
				var formattedIndentNo = indentNo.Replace(".", "/");

				// Convert "null" string back to null for logic
				string? fromCaseVal = fromcase == "null" ? null : fromcase;
				string? toCaseVal = tocase == "null" ? null : tocase;

				if (!DateTime.TryParse(mfgDate, out DateTime formattedDate))
					throw new Exception("Invalid manufacturing date format.");

				var l1Data = await _dispatchTransactionRepository.GetDataRE12Details(
					formattedIndentNo, truckNo, brandId, pSizeCode, magName, formattedDate, fromCaseVal, toCaseVal);

				_APIResponse.Data = l1Data;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet("{dispatchdate}")]
		public async Task<ActionResult<APIResponse>> GetRE11indentedata(DateTime dispatchdate)
		{
			var response = new APIResponse();
			try
			{
				var psizeNames = await _dispatchTransactionRepository.Getindentnodetails(dispatchdate);

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

		[HttpGet("{IndentNo}")]
		public async Task<ActionResult<APIResponse>> Gettruckbyindentedata(string IndentNo)
		{
			var response = new APIResponse();
			try
			{
				IndentNo = Uri.UnescapeDataString(IndentNo);
				var psizeNames = await _dispatchTransactionRepository.Gettruckdetails(IndentNo);

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

		//[HttpGet]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> Getdispatchstatus(string dispatchdate, string? IndentNo, string? truckno)
		//{
		//    try
		//    {
		//        // Convert string dates to DateTime
		//        //DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		//        //DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		//        DateTime disptdt = DateTime.Parse(dispatchdate);

		//        // Fetch all records from repository
		//        var barcodeList = await _dispatchTransactionRepository.GetAllAsync();
		//        List<DispatchTransaction> plantData = new List<DispatchTransaction>();


		//        plantData = barcodeList
		//             .Where(x =>
		//                         (string.IsNullOrEmpty(dispatchdate) || x.DispDt == disptdt) &&
		//                         (string.IsNullOrEmpty(IndentNo) || x.IndentNo == IndentNo) &&
		//                         (string.IsNullOrEmpty(truckno) || x.TruckNo == truckno))
		//             // Then, order by L1Barcode
		//             .Select(x => new DispatchTransaction
		//             {
		//                 Tid = x.Tid,
		//                 IndentNo = x.IndentNo,
		//                 L1Barcode = x.L1Barcode,
		//                 Brand = x.Brand,
		//                 PSize = x.PSize,
		//                 TruckNo = x.TruckNo,
		//                 MagName = x.MagName,
		//                 DispDt = x.DispDt,
		//                 Month = x.Month,
		//                 Year = x.Year,
		//                 Re12 = x.Re12,
		//                 L1NetWt = x.L1NetWt,
		//                 L1NetUnit = x.L1NetUnit


		//             })
		//             .OrderBy(x => x.Tid)
		//              .ThenBy(x => x.L1Barcode)
		//              .ThenBy(x => x.Brand)
		//             .ToList();


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

		[HttpGet("{indentno}/{mfgdt}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetTruckBrandByIndentdata(string indentno, string mfgdt)
		{
			try
			{
				// Replace "." with "/"
				var formattedIndentNo = indentno.Replace(".", "/");
				var formattedDate = Convert.ToDateTime(mfgdt.Replace(".", "/"));

				// Fetch truck number and brands
				var transactions = await _dispatchTransactionRepository.GetTruckNIndent(formattedIndentNo, formattedDate);
				var brands = await _dispatchTransactionRepository.GetBrandsBIndNo(formattedIndentNo, transactions, formattedDate);

				if (transactions != null)
				{
					// Create dynamic object without using a model
					var response = new
					{
						TruckNo = transactions, // Assuming this returns a string
						Brands = brands         // Assuming this returns a list or object
					};

					_APIResponse.Data = response;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpGet]
		public async Task<ActionResult<APIResponse>> GetIndentsREGenForRE12()
		{
			try
			{
				var Indents = await _dispatchTransactionRepository.GetIndentsRE12();

				var RE12Indents = new RE12Regencs
				{
					Indentlist = Indents.Select(n => new SelectListItem { Value = n, Text = n }).ToList(),
					MfgDt = DateTime.Now.Date

				};

				_APIResponse.Data = RE12Indents;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}



		[HttpGet("{dispatchdate}/{indentNo}")]
		public async Task<ActionResult<APIResponse>> GetTruckbyIndent(string dispatchdate, string indentNo)
		{
			try
			{
				var l1Data = await _dispatchTransactionRepository.GetTruckNo(dispatchdate.Replace(".", "/"), indentNo.Replace(".", "/"));

				_APIResponse.Data = l1Data;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet("{dispatchdate}/{indentNo}/{truckno}")]
		public async Task<ActionResult<APIResponse>> GetBrandName(string dispatchdate, string indentNo, string truckno)
		{
			try
			{
				var l1Data = await _dispatchTransactionRepository.GetBrandName(dispatchdate.Replace(".", "/"), indentNo.Replace(".", "/"), truckno);

				_APIResponse.Data = l1Data;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet("{dispatchdate}/{indentNo}/{truckno}")]
		public async Task<ActionResult<APIResponse>> GetMagName(string dispatchdate, string indentNo, string truckno)
		{
			try
			{
				var l1Data = await _dispatchTransactionRepository.GetMagName(dispatchdate.Replace(".", "/"), indentNo.Replace(".", "/"), truckno);

				_APIResponse.Data = l1Data;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
	}
}

