using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{


	[Route("api/[controller]/[action]")]
	[ApiController]
	//[Authorize]
	public class Search_FormController : ControllerBase
	{
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IL2barcodegenerationRepository _l2BarcodegenerationRepository;
		private readonly IL3barcodegenerationRepository _l3BarcodegenerationRepository;
		private readonly IBrandMasterRepository _brandMasterRepository;
		private readonly IMagzine_StockRepository _Magzine_StockRepository;
		private readonly IRe11IndentInfoRepository _Re11IndentInfoRepository;
		private readonly IRe11IndentPrdInfoRepository _Re11IndentPrdInfoRepository;
		private readonly Il1boxdeletionRepository _l1boxdeletionRepository;
		private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
		private readonly IBarcodeDataRepository _barcodeDataRepository;
		private readonly Il1barcodereprintRepository _l1barcodereprintRepository;
		private readonly Ihht_prodtomagtransferRepository _hht_prodtomagtransferRepository;
		private readonly Ireprint_l2barcodeRepository _reprint_l2barcodeRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "BrandMaster";


		public Search_FormController(IBrandMasterRepository brandMasterRepository, IDistributedCache distributedCache, IMapper mapper, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMagzine_StockRepository magzine_StockRepository, IDispatchTransactionRepository dispatchTransactionRepository, IRe11IndentPrdInfoRepository re11IndentPrdInfoRepository, Il1boxdeletionRepository l1boxdeletionRepository, Il1barcodereprintRepository l1barcodereprintRepository, Ireprint_l2barcodeRepository reprint_l2barcodeRepository, Ihht_prodtomagtransferRepository hht_prodtomagtransferRepository, IBarcodeDataRepository barcodeDataRepository, IL2barcodegenerationRepository l2BarcodegenerationRepository, IL3barcodegenerationRepository l3BarcodegenerationRepository, IRe11IndentInfoRepository re11IndentInfoRepository)
		{
			_brandMasterRepository = brandMasterRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_mapper = mapper;
			_l1BarcodegenerationRepository = l1BarcodegenerationRepository;
			_Magzine_StockRepository = magzine_StockRepository;
			_DispatchTransactionRepository = dispatchTransactionRepository;
			_Re11IndentPrdInfoRepository = re11IndentPrdInfoRepository;
			_l1boxdeletionRepository = l1boxdeletionRepository;
			_l1barcodereprintRepository = l1barcodereprintRepository;
			_reprint_l2barcodeRepository = reprint_l2barcodeRepository;
			_hht_prodtomagtransferRepository = hht_prodtomagtransferRepository;
			_barcodeDataRepository = barcodeDataRepository;
			_l2BarcodegenerationRepository = l2BarcodegenerationRepository;
			_l3BarcodegenerationRepository = l3BarcodegenerationRepository;
			_Re11IndentInfoRepository = re11IndentInfoRepository;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getsearchdata(string l1barcode)
		{
			try
			{
				var listl1barcode = new List<string> { l1barcode };

				// Await every async call

				var l1barcodedata = await _l1BarcodegenerationRepository.GetL1DataRecordByL1Async(l1barcode);
				var barcodedata = await _barcodeDataRepository.GetBarcodedataByL1lIST(listl1barcode);
				var magzineStock = await _Magzine_StockRepository.GetL1DetailsAsync(listl1barcode);
				var dispatchIndentDetails = await _DispatchTransactionRepository.DispatchTransactionByL1Barcode(l1barcode);

				var indentList = new List<Re11IndentInfo>();
				if (dispatchIndentDetails != null)
				{
					indentList.Add(await _Re11IndentInfoRepository.GetByIndentNO(dispatchIndentDetails.IndentNo));
				}

				_APIResponse.Data = new
				{
					manufacturingDetails = l1barcodedata,
					L1L2L3Details = barcodedata,
					magzineStock= magzineStock,
					DispatchIndentDetails = indentList,
				};
				_APIResponse.Status = true;
				_APIResponse.Message = "Success";
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetL2searchdata(string l2barcode)
		{
			try
			{


				// Await every async call
				var l2barcodedata = await _l2BarcodegenerationRepository.GetL2BarcodesAsync(l2barcode);
				if (l2barcodedata == null || string.IsNullOrEmpty(l2barcodedata.L1Barcode))
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "L2 Barcode not found";
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_APIResponse);
				}
				var listl1barcode = new List<string> { l2barcodedata.L1Barcode };
				var l1barcodedata = await _l1BarcodegenerationRepository.GetL1DataRecordByL1Async(l2barcodedata.L1Barcode);
				var barcodedata = await _barcodeDataRepository.GetBarcodedataByL1lIST(listl1barcode);
				var dispatchIndentDetails = await _DispatchTransactionRepository.DispatchTransactionByL1Barcode(l2barcodedata.L1Barcode);

				var indentList = new List<Re11IndentInfo>();
				if (dispatchIndentDetails != null)
				{
					indentList.Add(await _Re11IndentInfoRepository.GetByIndentNO(dispatchIndentDetails.IndentNo));
				}

				_APIResponse.Data = new
				{
					manufacturingDetails = l1barcodedata,
					L1L2L3Details = barcodedata,
					DispatchIndentDetails = indentList,
				};
				_APIResponse.Status = true;
				_APIResponse.Message = "Success";
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetL3searchdata(string l3barcode)
		{
			try
			{
				var l3barcodedata = await _l3BarcodegenerationRepository.GetL3BarcodeAsync(l3barcode);
				if (l3barcodedata == null || string.IsNullOrEmpty(l3barcodedata.L1Barcode))
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "L3 Barcode not found";
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_APIResponse);
				}
				var listl1barcode = new List<string> { l3barcodedata.L1Barcode };

				// Await every async call
				var l1barcodedata = await _l1BarcodegenerationRepository.GetL1DataRecordByL1Async(l3barcodedata.L1Barcode);
				var barcodedata = await _barcodeDataRepository.GetBarcodedataByL1lIST(listl1barcode);
				var dispatchIndentDetails = await _DispatchTransactionRepository.DispatchTransactionByL1Barcode(l3barcodedata.L1Barcode);

				var indentList = new List<Re11IndentInfo>();
				if (dispatchIndentDetails != null)
				{
					indentList.Add(await _Re11IndentInfoRepository.GetByIndentNO(dispatchIndentDetails.IndentNo));
				}

				_APIResponse.Data = new
				{
					manufacturingDetails = l1barcodedata,
					L1L2L3Details = barcodedata,
					DispatchIndentDetails = indentList,
				};
				_APIResponse.Status = true;
				_APIResponse.Message = "Success";
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetProstatus(string mfgdate, string? brand, string? BrandId, string? plant, string? plantcode, string? productsize, string? psizecode)
		{
			try
			{
				DateTime mfgDate = DateTime.Parse(mfgdate);

				var plantData = await _l1BarcodegenerationRepository.GetProductionStatusAsync(mfgDate, brand, BrandId, plant, plantcode, productsize, psizecode);

				_APIResponse.Data = plantData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getdispatchstatus(string dispatchdate, string? IndentNo, string? truckno)
		{
			try
			{
				DateTime disptdt = DateTime.Parse(dispatchdate);

				var plantData = await _DispatchTransactionRepository.GetDispatchStatusAsync(disptdt, IndentNo, truckno);

				_APIResponse.Data = plantData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getmagnamestatus(string? magname)
		{
			try
			{
				var plantData = await _Magzine_StockRepository.GetMagNameStatusAsync(magname);

				_APIResponse.Data = plantData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

	}
}

