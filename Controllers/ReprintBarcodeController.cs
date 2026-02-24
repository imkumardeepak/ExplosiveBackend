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
	public class ReprintBarcodeController : ControllerBase
	{
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IL2barcodegenerationRepository _l2BarcodegenerationRepository;
		private readonly IL3barcodegenerationRepository _l3BarcodegenerationRepository;
		private readonly IBrandMasterRepository _brandMasterRepository;
		private readonly IMagzine_StockRepository _Magzine_StockRepository;
		private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
		private readonly IRe11IndentPrdInfoRepository _Re11IndentPrdInfoRepository;
		private readonly Il1boxdeletionRepository _l1boxdeletionRepository;
		private readonly Il1barcodereprintRepository _l1barcodereprintRepository;
		private readonly Ihht_prodtomagtransferRepository _hht_prodtomagtransferRepository;
		private readonly Ireprint_l2barcodeRepository _reprint_l2barcodeRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "BrandMaster";

		public ReprintBarcodeController(IBrandMasterRepository brandMasterRepository, IDistributedCache distributedCache, IMapper mapper, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMagzine_StockRepository magzine_StockRepository, IDispatchTransactionRepository dispatchTransactionRepository, IRe11IndentPrdInfoRepository re11IndentPrdInfoRepository, Il1boxdeletionRepository l1boxdeletionRepository, Il1barcodereprintRepository l1barcodereprintRepository, Ireprint_l2barcodeRepository reprint_l2barcodeRepository, Ihht_prodtomagtransferRepository hht_prodtomagtransferRepository, IL2barcodegenerationRepository l2BarcodegenerationRepository, IL3barcodegenerationRepository l3BarcodegenerationRepository)
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
			_l2BarcodegenerationRepository = l2BarcodegenerationRepository;
			_l3BarcodegenerationRepository = l3BarcodegenerationRepository;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetL1Reprintbarcd(DateTime? MfgDt, string? plant, string? plantcode, string? mcode, string? shift, string? Brandname, string? BrandId, string? productsize, string? psizecode, string? l1barcode)
		{
			try
			{
				var plantData = await _l1BarcodegenerationRepository.GetL1ReprintBarcodesAsync(
					MfgDt, plant, plantcode, mcode, Brandname, BrandId, productsize, psizecode, l1barcode, shift, mcode
				);

				if (plantData == null || plantData.Count == 0)
				{
					_APIResponse.Data = plantData;
					_APIResponse.Status = false;
					_APIResponse.Message = "No Data Found";
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return Ok(_APIResponse);
				}

				_APIResponse.Data = plantData;
				_APIResponse.Status = true;
				_APIResponse.Message = "Data Found";
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
		public async Task<ActionResult<APIResponse>> GetReprintL3barcode(int? from, int? to, string? plant, string? plantcode, string? mcode)
		{
			try
			{
				var plantData = await _l3BarcodegenerationRepository.GetL3BarcodesAsync(from, to, plant, plantcode, mcode);

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

