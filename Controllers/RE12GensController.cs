using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class RE12GensController : ControllerBase
	{

		private readonly IDispatchTransactionRepository _dispatchTransactionRepository;
		private readonly IRe12Repository _re12Repository;
		private readonly IAllLoadingSheetRepository _allLoadingSheetRepository;
		private readonly IRe11IndentInfoRepository _re11IndentInfoRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "Re12gen";


		public RE12GensController(IDistributedCache distributedCache, IMapper mapper, IDispatchTransactionRepository dispatchTransactionRepository, IAllLoadingSheetRepository allLoadingSheetRepository, IRe11IndentInfoRepository re11IndentInfoRepository, IRe12Repository re12Repository)//
		{

			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_mapper = mapper;
			_allLoadingSheetRepository = allLoadingSheetRepository;
			_dispatchTransactionRepository = dispatchTransactionRepository;
			_re11IndentInfoRepository = re11IndentInfoRepository;
			_re12Repository = re12Repository;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRe12s()
		{
			try
			{
				var allLoadingSheets = await _allLoadingSheetRepository.GetLoadingByCflag(1);

				List<RE12Gen> re12s = new List<RE12Gen>();

				foreach (var allLoadingSheet in allLoadingSheets)
				{
					
					foreach (var re11 in allLoadingSheet.IndentDetails.Where(a => a.iscompleted == 1))
					{
						re12s.Add(new RE12Gen
						{
							LoadingSheet=allLoadingSheet.LoadingSheetNo,
							BrandName = re11.Bname,
							IndentNo = re11.IndentNo,
							Magname = re11.mag,
							TruckNo = allLoadingSheet.TruckNo,
							ProductSize = re11.Psize,
							PSizeCode = re11.SizeCode,
							BrandId = re11.Bid,
							loadcase = re11.Loadcase ?? 0

						});
					}

				}


				_APIResponse.Data = re12s;
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateRe12(RE12Gen product)
		{
			try
			{
				if (product == null)
					return BadRequest();

				List<string> l1Barcodes = _re12Repository.GetL1DataRE12(product.IndentNo, product.TruckNo, product.Magname, product.BrandId, product.PSizeCode, 0);

				await _re12Repository.UpdateRe12(l1Barcodes, product.IndentNo, product.BrandId, product.PSizeCode, product.Magname);

				await _allLoadingSheetRepository.UpdateCompFlagAsync(product.LoadingSheet,product.IndentNo, product.TruckNo, 2);

				_APIResponse.Data = l1Barcodes;
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
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRegenerate12([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
		{
			try
			{
				var allLoadingSheets = await _allLoadingSheetRepository.GetLoadingByCflag(2, fromDate, toDate);

				List<RE12Gen> re12s = new List<RE12Gen>();

				foreach (var allLoadingSheet in allLoadingSheets)
				{
					foreach (var re11 in allLoadingSheet.IndentDetails.Where(a => a.iscompleted == 2))
					{
						re12s.Add(new RE12Gen
						{
							LoadingSheet = allLoadingSheet.LoadingSheetNo,
							BrandName = re11.Bname,
							IndentNo = re11.IndentNo,
							Magname = re11.mag,
							TruckNo = allLoadingSheet.TruckNo,
							ProductSize = re11.Psize,
							PSizeCode = re11.SizeCode,
							BrandId = re11.Bid,
							loadcase = re11.Loadcase ?? 0
						});
					}
				}

				_APIResponse.Data = re12s;
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateRegenRe12(RE12Gen product)
		{
			try
			{
				if (product == null)
					return BadRequest();

				List<string> l1Barcodes = _re12Repository.GetL1DataRE12(product.IndentNo, product.TruckNo, product.Magname, product.BrandId, product.PSizeCode, 1);

				_APIResponse.Data = l1Barcodes;
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
	}
}

