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
	public class TruckToMAGBarcodeController : ControllerBase
	{
		private readonly ITruckToMAGBarcodeRepository _barcodeRepository;
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly IMagzine_StockRepository _magzine_StockRepository;
		private readonly IProductMasterRepository _productMasterRepository;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "TruckToMAGBarcode";

		public TruckToMAGBarcodeController(ITruckToMAGBarcodeRepository barcodeRepository, IDistributedCache distributedCache, IMapper mapper, IMagzine_StockRepository magzine_StockRepository, IProductMasterRepository productMasterRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository)
		{
			_barcodeRepository = barcodeRepository;
			_distributedCache = distributedCache;
			_productMasterRepository = productMasterRepository;
			_APIResponse = new APIResponse();
			_mapper = mapper;
			_magzine_StockRepository = magzine_StockRepository;
			_l1BarcodegenerationRepository = l1BarcodegenerationRepository;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllBarcodes()
		{
			try
			{

				var barcodes = await _barcodeRepository.GetAllAsync();
				if (barcodes != null)
				{
					_APIResponse.Data = barcodes;
				}

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
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DownloadMagUnloadingData()
		{
			try
			{
				var Productmaster = await _productMasterRepository.GetAllAsync();
				var TruckTransferData = await _barcodeRepository.GetAllAsync();

				var barcodesToCheck = TruckTransferData.Select(t => t.Barcode).ToList();
				var existingBarcodes = await _magzine_StockRepository.GetExistingBarcodesAsync(barcodesToCheck);
				var existingBarcodeSet = new HashSet<string>(existingBarcodes);

				var notAvailableBarcodes = TruckTransferData
					.Where(t => !existingBarcodeSet.Contains(t.Barcode))
					.ToList();

				var bidLookup = Productmaster
	.GroupBy(e => e.bid) // Ensures uniqueness
	.Select(g => g.First()) // Takes the first occurrence
	.ToDictionary(e => e.bid, e => e.bname); // Creates dictionary

				var psizeLookup = Productmaster
					.GroupBy(e => e.bid + e.psizecode) // Ensures uniqueness
					.Select(g => g.First()) // Takes the first occurrence
					.ToDictionary(e => e.bid + e.psizecode, e => e.psize); // Creates dictionary


				var newMagzineStockEntries = notAvailableBarcodes
					.Where(t => t.Barcode.Length >= 19) // Ensure Barcode has enough length
					.Select(t => new Magzine_Stock
					{
						L1Barcode = t.Barcode,
						MagName = t.MagName,
						BrandId = t.Barcode.Substring(12, 4), // Extract BrandId from Barcode
						BrandName = bidLookup.TryGetValue(t.Barcode.Substring(12, 4), out var brand) ? brand : "Unknown", // Lookup BrandName
						PSizeCode = t.Barcode.Substring(16, 3), // Extract Size Code
						PrdSize = psizeLookup.TryGetValue(t.Barcode.Substring(12, 4) + t.Barcode.Substring(16, 3), out var size) ? size : "Unknown", // Lookup Product Size
						Stock = 1,
						StkDt = DateTime.Now.Date,
						Re2 = false,
						Re12 = false
					})
					.ToList();


				if (newMagzineStockEntries.Any())
				{
					await _magzine_StockRepository.BulkInsertAsync(newMagzineStockEntries);
					await _barcodeRepository.RemoveByL1BarcodesAsync(notAvailableBarcodes.Select(e => e.Barcode).ToList());
					await _l1BarcodegenerationRepository.BulkUpdateMFlag(notAvailableBarcodes.Select(e => e.Barcode).ToList(), 1);
				}

				_APIResponse.Data = newMagzineStockEntries;
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


