using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using PdfSharpCore.Drawing.BarCodes;
using Peso_Baseed_Barcode_Printing_System_API.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class RE2FileGenerationsController : ControllerBase
	{

		private readonly IMagzine_StockRepository _magzine_StockRepository;
		private readonly IPlantMasterRepository _plantMasterRepository;
		private readonly IProductMasterRepository _productMasterRepository;
		private readonly IMagzineMasterRepository _magzineMasterRepository;
		private readonly IL1barcodegenerationRepository _l1barcodegenerationRepository;
		private readonly IBarcodeDataRepository _barcodeDataRepository;
		private readonly IMagzine_StockRepository magzine_StockRepository;
		private readonly IRe11IndentPrdInfoRepository _re11IndentPrdInfoRepository;
		private readonly IProductionMagzineAllocationRepository _productionMagzineAllocationRepository;
		private readonly IMapper _mapper;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly string KeyName = "RE2Genrate";


		public RE2FileGenerationsController(IMagzine_StockRepository magzine_StockRepository, IDistributedCache distributedCache, IMagzineMasterRepository magzineMasterRepository, IPlantMasterRepository plantMasterRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMapper mapper, IBarcodeDataRepository barcodeDataRepository, IRe11IndentPrdInfoRepository re11IndentPrdInfoRepository, IProductMasterRepository productMasterRepository, IProductionMagzineAllocationRepository productionMagzineAllocationRepository)
		{
			_mapper = mapper;
			_magzine_StockRepository = magzine_StockRepository;
			_plantMasterRepository = plantMasterRepository;
			_magzineMasterRepository = magzineMasterRepository;
			_l1barcodegenerationRepository = l1BarcodegenerationRepository;
			_barcodeDataRepository = barcodeDataRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_re11IndentPrdInfoRepository = re11IndentPrdInfoRepository;
			_productMasterRepository = productMasterRepository;
			_productionMagzineAllocationRepository = productionMagzineAllocationRepository;
		}


		// GET: api/MagzineAllotment
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRE2GenData()
		{
			try
			{
				List<ProductionMagzineAllocation> result = _productionMagzineAllocationRepository.GetAllProductionMagzineAllocationsAsync(1).Result.ToList(); 
				_APIResponse.Data = result;
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
				return _APIResponse;
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getregenratere2(DateTime? fromDate, DateTime? toDate)
		{
			try
			{
				var result = await _productionMagzineAllocationRepository.GetAllProductionMagzineAllocationsAsync(2, fromDate, toDate);

				_APIResponse.Data = result;
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
				return _APIResponse;
			}
		}


		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GenerateRE2File(RE2GenData rE2GenData)
		{
			try
			{
				// Validate input
				if (rE2GenData == null || string.IsNullOrEmpty(rE2GenData.L1L2.Count.ToString()))
				{
                     _APIResponse.Status = false;
					_APIResponse.Message = "Invalid input data.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Errors.Add("Invalid input data.");
					return BadRequest(_APIResponse);
				}

				// 1. Fetch data from repository
				var RE2Data = await _barcodeDataRepository.GetL1L2L3DataAsync(rE2GenData.L1L2);

				if (RE2Data == null || !RE2Data.Any())
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "No data found for the given L1L2 combination.";
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("No data found for the given L1L2 combination.");
					return BadRequest(_APIResponse);
				}

				if (rE2GenData.ProductionMagzineAllocation.PlantCode == "G4")
				{
					var barcodeRanges = ExtractBarcodeRanges(RE2Data);
					RE2Data = GenerateAllL3Barcodes(barcodeRanges);
				}
			
				await _magzine_StockRepository.UpdateRE2FlagAsync(rE2GenData.L1L2, 1);
				await _barcodeDataRepository.UpdateRE2FlagAsync(rE2GenData.L1L2, 1);
				await _productionMagzineAllocationRepository.UpdateReadFlagAsync(rE2GenData.ProductionMagzineAllocation.Id, 2);


				_APIResponse.Data = RE2Data;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Status = false,
					StatusCode = HttpStatusCode.InternalServerError,
					Errors = new List<string> { ex.Message }
				});
			}
		}

		private List<BarcodeRange> ExtractBarcodeRanges(List<L1L2L3> data)
		{
			var ranges = new List<BarcodeRange>();

			// Group by L1 and L2 to find start/end L3 pairs
			var groupedData = data
				.GroupBy(x => new { x.L1barcode, x.L2barcode })
				.Where(g => g.Count() == 2) // Ensure we have exactly 2 L3 barcodes per L2
				.ToList();

			foreach (var group in groupedData)
			{
				var l3Barcodes = group.OrderBy(x => x.L3barcode).ToList();

				if (!long.TryParse(l3Barcodes[0].L3barcode.Substring(8), out long startNum) ||
					!long.TryParse(l3Barcodes[1].L3barcode.Substring(8), out long endNum))
				{
					continue; // Skip invalid barcodes
				}

				ranges.Add(new BarcodeRange
				{
					L1Barcode = group.Key.L1barcode,
					L2Barcode = group.Key.L2barcode,
					StartL3Number = startNum,
					EndL3Number = endNum
				});
			}

			return ranges;
		}

		private List<L1L2L3> GenerateAllL3Barcodes(List<BarcodeRange> ranges)
		{
			var results = new List<L1L2L3>();
			const string prefix = "INKEGG4B"; // Common prefix for L3 barcodes

			foreach (var range in ranges)
			{
				// Validate range
				if (range.StartL3Number > range.EndL3Number)
				{
					continue; // Skip invalid ranges
				}

				// Limit the maximum range to prevent memory issues
				const long maxRange = 10000;
				if (range.EndL3Number - range.StartL3Number > maxRange)
				{
					throw new InvalidOperationException($"Barcode range too large (max {maxRange} allowed)");
				}

				for (long i = range.StartL3Number; i <= range.EndL3Number; i++)
				{
					string l3Barcode = $"{prefix}{i:D15}";
					// Create L1L2L3 object
					results.Add(new L1L2L3
					{
						L1barcode = range.L1Barcode,
						L2barcode = range.L2Barcode,
						L3barcode = l3Barcode
					});
				}
			}

			return results;
		}


		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> ReGenerateRE2File(RE2GenData rE2GenData)
		{
			try
			{

				var RE2Data = await _barcodeDataRepository.GetL1L2L3DataAsync(rE2GenData.L1L2);

				if (rE2GenData.ProductionMagzineAllocation.PlantCode == "G4")
				{
					var barcodeRanges = ExtractBarcodeRanges(RE2Data);
					RE2Data = GenerateAllL3Barcodes(barcodeRanges);
				}

				_APIResponse.Data = RE2Data;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{

				return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
			}
		}



		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetdispatchStatus()
		{
			try
			{
				var PETNList = await _re11IndentPrdInfoRepository.GetAllAsync();

				var result = PETNList
					.GroupBy(x => x.Id)
					.Select(group => new
					{
						Status = group.Key == 1 ? "Completed" : "Pending",
						Count = group.Count()
					})
					.ToList();

				_APIResponse.Data = result;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetProductionDispatchData(string plantName, string timeRange)
		{
			try
			{
				DateTime startDate = DateTime.Now;

				// Determine the time range
				switch (timeRange.ToLower())
				{
					case "day":
						startDate = DateTime.Now.Date; // Set to today's date (midnight)
						break;
					case "week":
						startDate = DateTime.Now.AddDays(-7);
						break;
					case "month":
						startDate = DateTime.Now.AddMonths(-1);
						break;
					case "year":
						startDate = DateTime.Now.AddYears(-1);
						break;
					default:
						startDate = DateTime.Now.AddDays(-7); // Default to weekly
						break;
				}

				// Fetch data from repositories
				var productionData = await _l1barcodegenerationRepository.GetAllAsync();
				var dispatchData = await _re11IndentPrdInfoRepository.GetAllAsync();

				// Filter and group Production Data
				var groupedProductionData = productionData
					.Where(a => a.PlantName.ToUpper() == plantName.ToUpper() && a.MfgDt >= startDate)
					.GroupBy(a => new { a.MfgDt.Date, a.BrandName, a.PlantName }) // Group by Date & Brand
					.Select(g => new
					{
						Date = g.Key.Date,
						BrandName = g.Key.BrandName,  // Brand Wise Grouping
													  //Plantname = g.Key.PlantName,  // Brand Wise Grouping
						Count = g.Sum(x => x.L1NetWt)
					})
					.ToList();

				// Filter and group Dispatch Data
				var groupedDispatchData = dispatchData
					.Where(a => a.Ptype.ToUpper() == plantName.ToUpper() && a.IndentDt >= startDate)
					.GroupBy(a => new { a.IndentDt.Date, a.Bname, a.Ptype }) // Group by Date & Brand
					.Select(g => new
					{
						Date = g.Key.Date,
						BrandName = g.Key.Bname,
						// Plantname = g.Key.Ptype,
						Count = g.Sum(x => x.L1NetWt)
					})
					.ToList();
				// Combine both Production and Dispatch data in the response
				var result = new
				{
					Production = groupedProductionData,
					Dispatch = groupedDispatchData
				};

				_APIResponse.Data = result;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetSlurryData(string timeRange)
		{
			try
			{
				var slurryList = await _l1barcodegenerationRepository.GetAllAsync();
				DateTime startDate = DateTime.Now;
				//DateTime endDate = DateTime.Now;
				// Determine the time range
				switch (timeRange.ToLower())
				{
					case "week":
						startDate = DateTime.Now.AddDays(-7);
						break;
					case "month":
						startDate = DateTime.Now.AddMonths(-1);
						break;
					case "yesterday":
						startDate = DateTime.Now.Date.AddDays(-1);
						//endDate = DateTime.Now.Date.AddDays(-1).AddDays(1).AddTicks(-1); // End of yesterday

						break;
					case "day":
					default:
						startDate = DateTime.Now.Date; // Today's data
						break;
				}

				var result = slurryList
					.Where(x => x.PlantName.ToUpper() == "SLURRY" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
					.GroupBy(b => b.BrandName)
					.Select(group => new
					{
						BrandName = group.Key,
						TotalL1NetWt = (group.Sum(x => x.L1NetWt) / 1000).ToString("0.00")

					})
					.ToList();

				_APIResponse.Data = result;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetDetonatorData(string timeRange)
		{
			try
			{
				var detonatorList = await _l1barcodegenerationRepository.GetAllAsync();
				DateTime startDate = DateTime.Now;
				//DateTime endDate = DateTime.Now;
				// Determine the time range
				switch (timeRange.ToLower())
				{
					case "week":
						startDate = DateTime.Now.AddDays(-7);
						break;
					case "month":
						startDate = DateTime.Now.AddMonths(-1);
						break;
					case "yesterday":
						startDate = DateTime.Now.Date.AddDays(-1);
						//endDate = DateTime.Now.Date.AddDays(-1).AddDays(1).AddTicks(-1); // End of yesterday

						break;
					case "day":
					default:
						startDate = DateTime.Now.Date; // Today's data
						break;
				}

				var result = detonatorList
					.Where(x => x.PlantName.ToUpper() == "DETONATING FUSE" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
					.GroupBy(b => b.BrandName)
					.Select(group => new
					{
						BrandName = group.Key,
						TotalL1NetWt = group.Sum(x => x.L1NetWt)
					})
					.ToList();

				_APIResponse.Data = result;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetEmultionData(string timeRange)
		{
			try
			{
				var EmultionList = await _l1barcodegenerationRepository.GetAllAsync();
				DateTime startDate = DateTime.Now;
				//DateTime endDate = DateTime.Now;
				// Determine the time range
				switch (timeRange.ToLower())
				{
					case "week":
						startDate = DateTime.Now.AddDays(-7);
						break;
					case "month":
						startDate = DateTime.Now.AddMonths(-1);
						break;
					case "yesterday":
						startDate = DateTime.Now.Date.AddDays(-1);
						//endDate = DateTime.Now.Date.AddDays(-1).AddDays(1).AddTicks(-1); // End of yesterday

						break;
					case "day":
					default:
						startDate = DateTime.Now.Date; // Today's data
						break;
				}

				var result = EmultionList
					.Where(x => x.PlantName.ToUpper() == "EMULSION" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
					.GroupBy(b => b.BrandName)
					.Select(group => new
					{
						BrandName = group.Key,
						TotalL1NetWt = (group.Sum(x => x.L1NetWt) / 1000).ToString("0.00")
					})
					.ToList();

				_APIResponse.Data = result;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPETNData(string timeRange)
		{
			try
			{
				var PETNList = await _l1barcodegenerationRepository.GetAllAsync();
				DateTime startDate = DateTime.Now;
				//DateTime endDate = DateTime.Now;
				// Determine the time range
				switch (timeRange.ToLower())
				{
					case "week":
						startDate = DateTime.Now.AddDays(-7);
						break;
					case "month":
						startDate = DateTime.Now.AddMonths(-1);
						break;
					case "yesterday":
						startDate = DateTime.Now.Date.AddDays(-1);
						//endDate = DateTime.Now.Date.AddDays(-1).AddDays(1).AddTicks(-1); // End of yesterday

						break;
					case "day":
					default:
						startDate = DateTime.Now.Date; // Today's data
						break;
				}

				var result = PETNList
					.Where(x => x.PlantName.ToUpper() == "PETN" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
					.GroupBy(b => b.BrandName)
					.Select(group => new
					{
						BrandName = group.Key,
						TotalL1NetWt = (group.Sum(x => x.L1NetWt) / 1000).ToString("0.00")
					})
					.ToList();

				_APIResponse.Data = result;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

	}
}
public class BarcodeRange
{
	public string L1Barcode { get; set; }
	public string L2Barcode { get; set; }
	public long StartL3Number { get; set; }
	public long EndL3Number { get; set; }
}
