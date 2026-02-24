using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PdfSharpCore.Drawing.BarCodes;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class L1GenerateController : ControllerBase
	{

		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "L1Generate";
		private readonly string plant = "PlantMaster";
		private readonly string shift = "ShiftMaster";
		private readonly ICountryMasterRepository _countryMasterRepository;
		private readonly IMfgLocationMasterRepository _mfgLocationMasterRepository;
		private readonly IPlantMasterRepository _plantMasterRepository;
		private readonly IShiftMasterRepository _shiftMasterRepository;
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IL2barcodegenerationRepository _l2BarcodegenerationRepository;
		private readonly IL3barcodegenerationRepository _l3BarcodegenerationRepository;
		private readonly IBarcodeDataRepository _barcodeDataRepository;
		private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
		private readonly IRe11IndentInfoRepository _Re11IndentInfoRepository;
		private readonly IProductionPlanRepository _ProductionPlanRepository;
		private readonly IMagzine_StockRepository _magzine_StockRepository;
		private readonly IMagzineMasterRepository _magzineMasterRepository;
		private readonly IBatchDetailsRepository _batchDetailsRepository;
		private readonly IBatchMasterRepository _batchMasterRepository;
		private readonly ApplicationDbContext _context;
		private readonly L1DetailsService _L1DetailsService;

		public L1GenerateController(IDistributedCache distributedCache, IMapper mapper, IPlantMasterRepository plantMasterRepository, IShiftMasterRepository shiftMasterRepository, ICountryMasterRepository countryMasterRepository, IMfgLocationMasterRepository mfgLocationMasterRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository, IL2barcodegenerationRepository l2BarcodegenerationRepository, IL3barcodegenerationRepository l3BarcodegenerationRepository, IBarcodeDataRepository barcodeDataRepository, IDispatchTransactionRepository dispatchTransactionRepository, IRe11IndentInfoRepository re11IndentInfoRepository, IProductionPlanRepository ProductionPlanRepository, IMagzine_StockRepository magzine_StockRepository, IMagzineMasterRepository magzineMasterRepository, ApplicationDbContext context, IBatchMasterRepository batchMasterRepository, IBatchDetailsRepository batchDetailsRepository, L1DetailsService l1DetailsService)
		{

			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
			_countryMasterRepository = countryMasterRepository;
			_mfgLocationMasterRepository = mfgLocationMasterRepository;
			_plantMasterRepository = plantMasterRepository;
			_shiftMasterRepository = shiftMasterRepository;
			_l1BarcodegenerationRepository = l1BarcodegenerationRepository;
			_l2BarcodegenerationRepository = l2BarcodegenerationRepository;
			_l3BarcodegenerationRepository = l3BarcodegenerationRepository;
			_barcodeDataRepository = barcodeDataRepository;
			_DispatchTransactionRepository = dispatchTransactionRepository;
			_Re11IndentInfoRepository = re11IndentInfoRepository;
			_ProductionPlanRepository = ProductionPlanRepository;
			_magzine_StockRepository = magzine_StockRepository;
			_magzineMasterRepository = magzineMasterRepository;
			_context = context;
			_batchMasterRepository = batchMasterRepository;
			_batchDetailsRepository = batchDetailsRepository;
			_L1DetailsService = l1DetailsService;
		}

		// GET: api/L1Generate
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetL1Generate()
		{
			var l1barcode = new L1Generate()
			{
				MfgDt = DateTime.Now.Date,
				Country = _countryMasterRepository.GetAllAsync().Result.Select(e => e.cname).FirstOrDefault(),
				CountryCode = _countryMasterRepository.GetAllAsync().Result.Select(e => e.code).FirstOrDefault(),
				MfgName = _mfgLocationMasterRepository.GetAllAsync().Result.Select(e => e.mfgname).FirstOrDefault(),
				MfgCode = _mfgLocationMasterRepository.GetAllAsync().Result.Select(e => e.maincode).FirstOrDefault(),
				MfgLoc = _mfgLocationMasterRepository.GetAllAsync().Result.Select(e => e.mfgloc).FirstOrDefault(),
				NoOfstickers = 1
			};


			try
			{
				string serializedList = string.Empty;
				
				List<string> plants = _plantMasterRepository.GetAllAsync().Result.Select(p => p.PName).ToList();
				plants.Insert(0, "Select Plant");
				l1barcode.plist = plants.Select(p => new SelectListItem { Value = p, Text = p }).ToList();
				
				List<string> shift = _shiftMasterRepository.GetAllAsync().Result.Select(p => p.shift).ToList();
				shift.Insert(0, "Select Plant");
				l1barcode.shiftlist = shift.Select(p => new SelectListItem { Value = p, Text = p }).ToList();

				_APIResponse.Data = l1barcode;
				_APIResponse.Status = true;
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
		public async Task<ActionResult<APIResponse>> GetOnlyL1L1Generate()
		{
			try
			{
				var l1barcode = await _context.L1Barcodegeneration.Where(x => x.MfgDt == DateTime.Now.Date).OrderBy(a => a.L1Barcode).Select(x => x.L1Barcode).ToListAsync();
				List<string> l1Generate = l1barcode;
				_APIResponse.Data = l1Generate;
				_APIResponse.Status = true;
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
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetL1detailsByL1Number([FromBody] List<ProductionTransferCases> l1number)
		{
			try
			{
				List<L1barcodegeneration> l1barcode = new List<L1barcodegeneration>();
				foreach (var item in l1number)
				{
					l1barcode.AddRange(await _l1BarcodegenerationRepository.GetL1DataRecordByL1Async(item.L1Barcode));
				}

				var magstock = await _magzine_StockRepository.GetMagRE2details();
				var magzines = await _magzineMasterRepository.GetMagDetails();

				var combined = (from mz in magzines
								join ms in magstock on mz.mcode equals ms.MagName into msGroup
								from ms in msGroup.DefaultIfEmpty()
								group new { ms, mz } by mz.mcode into g
								select new
								{
									MagName = g.Key,
									TotalNetWeight = g.Sum(x => x.ms?.TotalNetWeight ?? 0),
									MagzineWt = Convert.ToDouble(g.Select(x => x.mz.Totalwt).FirstOrDefault()),
									Blankspace = Convert.ToDouble(g.Select(x => x.mz.Totalwt).FirstOrDefault()) - g.Sum(x => x.ms?.TotalNetWeight ?? 0)
								}).ToList();



				_APIResponse.Data = new { l1barcode, combined, magzines };
				_APIResponse.Status = true;
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


		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateL1Generate(L1Generate l1Generate)
		{
			// Validate input early
			if (l1Generate == null)
			{
				return BadRequest(new APIResponse
				{
					Errors = { "Invalid L1Generate data." },
					StatusCode = HttpStatusCode.BadRequest,
					Status = false
				});
			}

		

			try
			{
				// Pre-calculate common values once
				var mfgDateStr = l1Generate.MfgDt.ToString("dd/MM/yyyy");
				string quarter = _L1DetailsService.GetQuarter(mfgDateStr);
				string mfgTime = DateTime.Now.ToString("HH:mm:ss");
				string month = mfgDateStr.Substring(3, 2);
				string yyyy = mfgDateStr.Substring(6, 4);
				string mfgdate = mfgDateStr.Substring(0, 2) + mfgDateStr.Substring(3, 2) + mfgDateStr.Substring(8, 2);
				
				// Base L1 barcode parts
				string l1BarcodeBase = l1Generate.CountryCode + l1Generate.MfgCode + mfgdate +
									  l1Generate.Shift + l1Generate.BrandId + l1Generate.PSizeCode + l1Generate.PCode;

				var currentBatch = await _L1DetailsService.GetOrCreateBatchAsync(
								l1Generate.PCode,
								l1Generate.MfgDt,
								(decimal)l1Generate.L1NetWt);

				// Process all boxes
				var allL1Data = new List<L1barcodegeneration>();
				var allL2Data = new List<L2barcodegeneration>();
				var allL3Data = new List<L3barcodegeneration>();
				var allBarcodeData = new List<BarcodeData>();

				// Get initial sequence numbers outside the loop
				int initialL1Seq = await _l1BarcodegenerationRepository.GetMaxSrNoAsync(yyyy, quarter, l1Generate.BrandId, l1Generate.PSizeCode);
				int initialL2Seq = await _l2BarcodegenerationRepository.GetMaxSrNoAsync(l1Generate.PCode, l1Generate.MCode);
				int initialL3Seq = await _l3BarcodegenerationRepository.GetMaxSrNoAsync(l1Generate.PCode, l1Generate.MCode);

				for (int i = 0; i < l1Generate.NoOfbox; i++)
				{
					if (currentBatch != null)
					{
						if (currentBatch.RemainingCapacity < (decimal)l1Generate.L1NetWt)
						{
							// Create new batch when capacity is exhausted
							currentBatch = await _L1DetailsService.CreateNewBatchAsync(
								l1Generate.PCode,
								l1Generate.MfgDt);
						}

						// Deduct from remaining capacity
						currentBatch.RemainingCapacity -= (decimal)l1Generate.L1NetWt;
						await _context.SaveChangesAsync();
					}
					

					int L1boxno = initialL1Seq + i;
					string l1Barcode = l1BarcodeBase + L1boxno.ToString("D6");

					// Create L1 data
					var l1data = new L1barcodegeneration
					{
						L1Barcode = l1Barcode,
						SrNo = L1boxno,
						Country = l1Generate.Country,
						CountryCode = l1Generate.CountryCode,
						MfgName = l1Generate.MfgName,
						MfgLoc = l1Generate.MfgLoc,
						MfgCode = l1Generate.MfgCode,
						PlantName = l1Generate.PlantName,
						PCode = l1Generate.PCode,
						MCode = l1Generate.MCode,
						Shift = l1Generate.Shift,
						BrandName = l1Generate.BrandName,
						BrandId = l1Generate.BrandId,
						ProductSize = l1Generate.ProductSize,
						PSizeCode = l1Generate.PSizeCode,
						SdCat = l1Generate.SdCat,
						UnNoClass = l1Generate.UnNoClass,
						MfgDt = l1Generate.MfgDt,
						MfgTime = DateTime.Now,
						L1NetWt = (decimal)l1Generate.L1NetWt,
						L1NetUnit = l1Generate.L1NetUnit,
						NoOfL2 = l1Generate.NoOfL2,
						NoOfL3 = l1Generate.NoOfL3,
						MFlag = false,
						CheckFlag = true
					};
					allL1Data.Add(l1data);

					// Process L2 barcodes for this L1
					for (int j = 0; j < l1Generate.NoOfL2; j++)
					{
						int L2boxno = initialL2Seq + (i * l1Generate.NoOfL2) + j;
						string l2barcode = $"{l1Generate.CountryCode}{l1Generate.MfgCode}{l1Generate.PCode}{l1Generate.MCode}{L2boxno:D12}";

						var l2data = new L2barcodegeneration
						{
							L2Barcode = l2barcode,
							SrNo = L2boxno,
							L1Barcode = l1Barcode
						};
						allL2Data.Add(l2data);

						// Process L3 barcodes
						int numL3 = Convert.ToInt32(l1Generate.NoOfL3perL2);
						bool isG4LargeBatch = l1Generate.PCode == "G4" && numL3 > 199;

						if (isG4LargeBatch)
						{
							// For G4 with large batches, we only generate start and end barcodes
							var l3Start = CreateL3Data(l1Generate, l1Barcode, l2barcode, initialL3Seq, quarter, month, yyyy);
							var l3End = CreateL3Data(l1Generate, l1Barcode, l2barcode, initialL3Seq + numL3 - 1, quarter, month, yyyy);
							initialL3Seq = initialL3Seq + numL3;
							allL3Data.Add(l3Start);
							allL3Data.Add(l3End);

							allBarcodeData.Add(CreateBarcodeData(l1Generate, l1Barcode, l2barcode, l3Start.L3Barcode, quarter, month, yyyy, currentBatch?.BatchCode ?? ""));
							allBarcodeData.Add(CreateBarcodeData(l1Generate, l1Barcode, l2barcode, l3End.L3Barcode, quarter, month, yyyy, currentBatch?.BatchCode ?? ""));
						}
						else
						{
							// For normal cases, generate all L3 barcodes
							for (int k = 0; k < numL3; k++)
							{
								int L3boxno = initialL3Seq + (i * l1Generate.NoOfL2 * numL3) + (j * numL3) + k;
								var l3data = CreateL3Data(l1Generate, l1Barcode, l2barcode, L3boxno, quarter, month, yyyy);
								allL3Data.Add(l3data);
								allBarcodeData.Add(CreateBarcodeData(l1Generate, l1Barcode, l2barcode, l3data.L3Barcode, quarter, month, yyyy, currentBatch?.BatchCode ?? ""));
							}
						}
					}
				}


				await	_l1BarcodegenerationRepository.BulkInsertAsync(allL1Data);
				await _l2BarcodegenerationRepository.BulkInsertAsync(allL2Data);
				await _l3BarcodegenerationRepository.BulkInsertAsync(allL3Data);
				await _barcodeDataRepository.BulkInsertAsync(allBarcodeData);

				return Ok(new APIResponse
				{
					Data = allL1Data,
					Message = "Success",
					Status = true,
					StatusCode = HttpStatusCode.Created
				});
			}
			catch (Exception ex)
			{

				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		// Helper methods to reduce code duplication
		private L3barcodegeneration CreateL3Data(L1Generate l1Generate, string l1Barcode, string l2Barcode, int sequenceNo,
			string quarter, string month, string year)
		{
			string l3barcode = $"{l1Generate.CountryCode}{l1Generate.MfgCode}{l1Generate.PCode}{l1Generate.MCode}{sequenceNo:D15}";

			return new L3barcodegeneration
			{
				L3Barcode = l3barcode,
				SrNo = sequenceNo,
				L1Barcode = l1Barcode,
				L2Barcode = l2Barcode
			};
		}

		private BarcodeData CreateBarcodeData(L1Generate l1Generate, string l1Barcode, string l2Barcode,
			string l3Barcode, string quarter, string month, string year, string batchCode)
		{
			return new BarcodeData
			{
				L1 = l1Barcode,
				L2 = l2Barcode,
				L3 = l3Barcode,
				Batch = batchCode,
				Re2 = false,
				Re12 = false,
				IsFinal = false
			};
		}

		// PUT: api/L1Generate/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateL1Generate(int id, L1Generate l1Generate)
		{
			try
			{
				//if (id != l1Generate.Id)
				//    return BadRequest();

				//var existingL1Generate = await _l1GenerateRepository.GetByIdAsync(id);
				//if (existingL1Generate == null)
				//    return NotFound();

				//await _distributedCache.RemoveAsync(KeyName);

				//existingL1Generate = _mapper.Map(l1Generate, existingL1Generate);

				//await _l1GenerateRepository.UpdateAsync(existingL1Generate);

				_APIResponse.Data = null;
				_APIResponse.Status = true;
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

		// DELETE: api/L1Generate/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteL1Generate(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				//var l1Generate = await _l1GenerateRepository.GetByIdAsync(id);
				//if (l1Generate == null)
				//    return NotFound();

				//await _l1GenerateRepository.DeleteAsync(id);
				//await _distributedCache.RemoveAsync(KeyName);

				_APIResponse.Data = true;
				_APIResponse.Status = true;
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
		public async Task<ActionResult<APIResponse>> Getdashboardcard()
		{
			try
			{
				var date = DateTime.Now.Date;
				// Fetch all records
				var barcodeList = await _l1BarcodegenerationRepository.GetAllAsync();

				// Group by PlantName and count distinct PCode values for each plant
				// Group by PlantName
				var plantData = barcodeList
					.Where(r => r.MfgDt == date)
					.GroupBy(p => p.PlantName.ToUpper().Trim()) // Normalize PlantName
					.Select(group =>
					{
						var pCodeCount = group.Select(x => x.PCode).Count(); // Count of distinct PCode
						var totalWeight = group.Sum(x => x.L1NetWt);
						var totalOutputTons = pCodeCount > 0 ? (pCodeCount * 25 / 1000.0) : 0;
						var totalpetn = totalWeight > 0 ? (pCodeCount * (double)totalWeight / 1000.0) : 0;
						var totalOutputTonsFormatted = Math.Round(totalOutputTons, 3);
						var totaldeci = Math.Round(totalpetn, 3);

						return new
						{
							PlantName = group.Key,
							PCodeCount = pCodeCount,
							TotalOutputTons = totalOutputTonsFormatted,
							totalwt = totalWeight,
							totaldecimal = totaldeci,
						};
					})
					.ToDictionary(x => x.PlantName, x => new { x.PCodeCount, x.TotalOutputTons, x.totalwt, x.totaldecimal });

				// Prepare API response
				_APIResponse.Data = plantData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);

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
		public async Task<ActionResult<APIResponse>> Getproreport(string fromDate, string toDate, string reportType, string? shift, string? plant, string? brand, string? productsize)
		{
			try
			{
				// Convert string dates to DateTime
				//DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
				//DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);
				// Fetch all records from repository
				var barcodeList = await _l1BarcodegenerationRepository.GetAllAsync();
				List<L1barcodegeneration> plantData = new List<L1barcodegeneration>();

				if (reportType == "Detailed")
				{
					plantData = barcodeList
						 .Where(x => x.MfgDt >= fromDateTime && x.MfgDt <= toDateTime &&
									 (string.IsNullOrEmpty(shift) || x.Shift == shift) &&
									 (string.IsNullOrEmpty(plant) || x.PlantName == plant) &&
									 (string.IsNullOrEmpty(brand) || x.BrandName == brand) &&
									 (string.IsNullOrEmpty(productsize) || x.ProductSize == productsize))
						 .OrderBy(x => x.PlantName)
						  .ThenBy(x => x.L1Barcode)   // Then, order by L1Barcode
						 .Select(x => new L1barcodegeneration
						 {
							 PlantName = x.PlantName,
							 Shift = x.Shift,
							 BrandName = x.BrandName,
							 ProductSize = x.ProductSize,
							 L1Barcode = x.L1Barcode,
							 L1NetWt = x.L1NetWt,
							 L1NetUnit = x.L1NetUnit
						 })
						 .ToList();
				}
				else
				{
					plantData = barcodeList
						.Where(x => x.MfgDt >= fromDateTime && x.MfgDt <= toDateTime &&
									(string.IsNullOrEmpty(shift) || x.Shift == shift) &&
									(string.IsNullOrEmpty(plant) || x.PlantName == plant) &&
									(string.IsNullOrEmpty(brand) || x.BrandName == brand) &&
									(string.IsNullOrEmpty(productsize) || x.ProductSize == productsize))
						.GroupBy(x => new { x.PlantName, x.Shift, x.BrandName, x.ProductSize }) // Group by necessary fields

						.Select(g => new L1barcodegeneration
						{
							PlantName = g.Key.PlantName,
							Shift = g.Key.Shift,
							BrandName = g.Key.BrandName,
							ProductSize = g.Key.ProductSize,
							L1Barcode = g.Select(x => x.L1Barcode).Distinct().Count().ToString(),
							L1NetWt = Math.Round(g.Sum(x => x.L1NetWt), 2),
							L1NetUnit = g.First().L1NetUnit // Get the first unit (assuming same for all)
						})
						  .OrderBy(x => x.PlantName)  // First, order by Plant Name
						 .ThenBy(x => x.L1Barcode)   // Then, order by L1Barcode
						.ToList();
				}

				// Prepare API response
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


		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> ProductionPlanL1Generate(ProductionPlan l1Generate)
		{
			try
			{
				//if (id != l1Generate.Id)
				//    return BadRequest();

				//var existingL1Generate = await _l1GenerateRepository.GetByIdAsync(id);
				//if (existingL1Generate == null)
				//    return NotFound();

				//await _distributedCache.RemoveAsync(KeyName);

				//existingL1Generate = _mapper.Map(l1Generate, existingL1Generate);

				//await _l1GenerateRepository.UpdateAsync(existingL1Generate);
				await _ProductionPlanRepository.AddAsync(l1Generate);

				_APIResponse.Data = l1Generate;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;
				_APIResponse.Data = null;
				_APIResponse.Status = true;
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
			//return Ok(_APIResponse);
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetdataPlan()
		{
			try
			{

				var brands = await _ProductionPlanRepository.GetAllAsync();
				var serializedData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(brands));
				var cacheOptions = new DistributedCacheEntryOptions
				{
					SlidingExpiration = TimeSpan.FromDays(30),
					AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
				};

				_APIResponse.Data = brands;


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

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetplanById(int id)
		{

			if (id == 0)
				return BadRequest();

			try
			{
				var brand = await _ProductionPlanRepository.GetByIdAsync(id);
				if (brand == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Brand not found.");
					return NotFound(_APIResponse);
				}

				_APIResponse.Data = brand;
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


		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdatePlanning(int id, ProductionPlan ProductionPlan)
		{
			try
			{
				if (id != ProductionPlan.Id)
					return BadRequest();

				var existingBrand = await _ProductionPlanRepository.GetByIdAsync(id);
				if (existingBrand == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Brand not found.");
					return NotFound(_APIResponse);
				}

				//await _distributedCache.RemoveAsync(CacheKey);


				existingBrand = _mapper.Map(ProductionPlan, existingBrand);

				await _ProductionPlanRepository.UpdateAsync(existingBrand);

				_APIResponse.Data = existingBrand;
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

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeletePlanning(int id)
		{
			try
			{
				var existingBrand = await _ProductionPlanRepository.GetByIdAsync(id);
				if (existingBrand == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Brand not found.");
					return NotFound(_APIResponse);
				}

				//await _distributedCache.RemoveAsync(CacheKey);
				await _ProductionPlanRepository.DeleteAsync(id);

				_APIResponse.Data = true;
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

		[HttpGet("{pcode}/{brandid}/{productsize}/{shift}/{mcode}/{mfgdt}/{countycode}/{mfglocationcode}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getlastl1l2l3Details(string pcode, string brandid, string productsize, string shift, string mcode, DateTime? mfgdt, string countycode, string mfglocationcode)
		{
			if (string.IsNullOrWhiteSpace(pcode) || string.IsNullOrWhiteSpace(brandid) || string.IsNullOrWhiteSpace(productsize) || string.IsNullOrWhiteSpace(shift) || string.IsNullOrWhiteSpace(mcode) || string.IsNullOrWhiteSpace(countycode) || string.IsNullOrWhiteSpace(mfglocationcode))
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				_APIResponse.Data = "Invalid or missing parameters.";
				return BadRequest(_APIResponse);
			}

			try
			{
				// Retrieve product details
				var product = await _barcodeDataRepository.GetLatestL1L2L3DetailsAsync(pcode, brandid, productsize, shift,mcode, mfgdt, countycode, mfglocationcode);

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
	}
}

