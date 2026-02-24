using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using EFCore.BulkExtensions;
using Npgsql;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using System.Net;
using DocumentFormat.OpenXml.InkML;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	/// <summary>
	/// OPTIMIZED L1 Barcode Repository - Uses single scoped DbContext
	/// </summary>
	public class L1barcodegenerationRepository : GenericRepository<L1barcodegeneration>, IL1barcodegenerationRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public L1barcodegenerationRepository(IMapper mapper, ApplicationDbContext context) : base(context)
		{
			_mapper = mapper;
			_context = context;
		}

		public async Task<int> GetMaxSrNoAsync(string genYear, string quarter, string brandId, string pSizeCode)
		{
			try
			{
				// Parse year and quarter for date-based filtering
				int year = int.Parse(genYear);
				int qtr = int.Parse(quarter);
				int startMonth = (qtr - 1) * 3 + 1;
				var startDate = new DateTime(year, startMonth, 1);
				var endDate = startDate.AddMonths(3).AddDays(-1);

				// Get the maximum SrNo for the given filters using date range
				var maxSrNo = await _context.Set<L1barcodegeneration>()
					.Where(l => l.MfgDt >= startDate
								&& l.MfgDt <= endDate
								&& l.BrandId == brandId
								&& l.PSizeCode == pSizeCode)
					.MaxAsync(l => (long?)l.SrNo);

				// If maxSrNo is null, it means no matching records, so return 1
				return (int)((maxSrNo ?? 0) + 1);
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error fetching max SrNo: {ex.Message}");
				throw;
			}
		}


		public async Task<List<SmallModel>> GetL1MagNotAllot(DateTime mfgdt, string pcode, string brandId, string pSizeCode)
		{
			return await _context.Set<L1barcodegeneration>()
				.Where(b => b.MfgDt == mfgdt && b.PCode == pcode && b.BrandId == brandId && b.PSizeCode == pSizeCode && b.MFlag == false)
				.Select(b => new SmallModel
				{
					L1Barcode = b.L1Barcode,
					L1NetWt = (double?)b.L1NetWt,
					L1NetUnit = b.L1NetUnit
				})
				.ToListAsync();
		}




		public async Task BulkUpdateMFlag(List<string> barcodes, int newMFlagValue)
		{
			try
			{
				// Ensure the list is not empty
				if (barcodes == null || !barcodes.Any())
					throw new ArgumentException("The list of barcodes cannot be null or empty.");

				bool newFlag = newMFlagValue != 0;
				// Perform bulk update using ExecuteUpdateAsync
				await _context.L1Barcodegeneration
					.Where(x => barcodes.Contains(x.L1Barcode))
					.ExecuteUpdateAsync(setters => setters.SetProperty(x => x.MFlag, newFlag));
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while performing the bulk update.", ex);
			}
		}

		public async Task<Dictionary<string, object>> Getdashdetailsdata()
		{
			try
			{
				var currentDate = DateTime.Today; // Current date: 2025-06-02
				var startDate = currentDate.AddDays(-90); // 90 days ago: 2025-03-04

				// Fetch and group L1Barcodegeneration data for the last 90 days
				var plantData = await _context.L1Barcodegeneration
					.Where(r => r.MfgDt >= startDate && r.MfgDt <= currentDate)
					.GroupBy(p => new { PlantName = p.PlantName != null ? p.PlantName.ToUpper().Trim() : "Unknown", Date = p.MfgDt.Date })
					.Select(g => new
					{
						PCodeCount = g.Count(),
						PlantName = g.Key.PlantName,
						MfgDate = g.Key.Date
					})
					.ToListAsync();

				// Convert to Dictionary<string, object> with a composite string key
				var plantDataDict = plantData.ToDictionary(
					g => $"{g.PlantName}_{g.MfgDate:yyyy-MM-dd}",
					g => (object)new
					{
						PCodeCount = g.PCodeCount,
						PlantName = g.PlantName,
						MfgDate = g.MfgDate.ToString("yyyy-MM-dd") // Format date for client
					});

				return plantDataDict;
			}
			catch (Exception ex)
			{

				throw new Exception("Failed to fetch dashboard details: " + ex.Message);
			}
		}


		//public async Task<Dictionary<string, object>> Getdashdetailsdata()
		//{
		//    try
		//    {
		//        var date = DateTime.Now.Date;

		//        var rawData = await _context.L1Barcodegeneration
		//            .Where(r => r.MfgDt == date)
		//            .ToListAsync(); // Pull data into memory

		//        var plantData = rawData
		//            .GroupBy(p => p.PlantName.ToUpper().Trim())
		//            .ToDictionary(
		//                group => group.Key,
		//                group =>
		//                {
		//                    var pCodeCount = group.Count(); // Total count of PCode entries
		//                    var totalWeight = group.Sum(x => x.L1NetWt);
		//                    var totalOutputTons = pCodeCount > 0 ? (pCodeCount * 25 / 1000.0) : 0;
		//                    var totalpetn = totalWeight > 0 ? (pCodeCount * totalWeight / 1000.0) : 0;

		//                    return (object)new
		//                    {
		//                        PCodeCount = pCodeCount,
		//                        TotalOutputTons = Math.Round(totalOutputTons, 3),
		//                        TotalWeight = totalWeight,
		//                        TotalDecimal = Math.Round(totalpetn, 3)
		//                    };
		//                });

		//        return plantData;
		//    }
		//    catch (Exception ex)
		//    {
		//        return new Dictionary<string, object>
		//        {
		//            { "error", ex.Message }
		//        };
		//    }
		//}
		public async Task<List<dynamic>> GetGroupedProductionData(string plantName, DateTime startDate)
		{
			var data = await _context.L1Barcodegeneration
				.Where(a => a.PlantName.ToUpper() == plantName.ToUpper() && a.MfgDt >= startDate)
				.GroupBy(a => new { a.MfgDt.Date, a.BrandName, a.PlantName })
				.Select(g => new
				{
					Date = g.Key.Date,
					BrandName = g.Key.BrandName,
					Count = g.Sum(x => x.L1NetWt)
				})
				.ToListAsync();

			// Explicitly convert each element to dynamic
			return data.Cast<dynamic>().ToList();
		}
		public async Task<List<dynamic>> GetSlurryDataAsync(string timeRange)
		{
			DateTime startDate = DateTime.Now;

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
					break;
				case "day":
				default:
					startDate = DateTime.Now.Date; // Today's data
					break;
			}

			// Fetch slurry data from the database
			var slurryData = await _context.L1Barcodegeneration
				.Where(x => x.PlantName.ToUpper() == "SLURRY" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
				.GroupBy(b => b.BrandName)
				.Select(group => new
				{
					BrandName = group.Key,
					TotalL1NetWt = (group.Sum(x => x.L1NetWt) / 1000).ToString("0.00") // Convert weight to kg
				})
				.ToListAsync();

			return slurryData.Cast<dynamic>().ToList();
		}

		public async Task<List<dynamic>> GetDetonatorDataAsync(string timeRange)
		{
			DateTime startDate = DateTime.Now;

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
					break;
				case "day":
				default:
					startDate = DateTime.Now.Date; // Today's data
					break;
			}

			// Fetch slurry data from the database
			var detonatorData = await _context.L1Barcodegeneration
			   .Where(x => x.PlantName.ToUpper() == "DETONATING FUSE" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
				.GroupBy(b => b.BrandName)
				.Select(group => new
				{
					BrandName = group.Key,
					TotalL1NetWt = (group.Sum(x => x.L1NetWt)).ToString("0.00") // Convert weight to kg
				})
				.ToListAsync();

			return detonatorData.Cast<dynamic>().ToList();
		}
		public async Task<List<dynamic>> GetEmultionDataAsync(string timeRange)
		{
			DateTime startDate = DateTime.Now;

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
					break;
				case "day":
				default:
					startDate = DateTime.Now.Date; // Today's data
					break;
			}

			// Fetch slurry data from the database
			var emultionData = await _context.L1Barcodegeneration
				.Where(x => x.PlantName.ToUpper() == "EMULSION" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
				.GroupBy(b => b.BrandName)
				.Select(group => new
				{
					BrandName = group.Key,
					TotalL1NetWt = (group.Sum(x => x.L1NetWt) / 1000).ToString("0.00") // Convert weight to kg
				})
				.ToListAsync();

			return emultionData.Cast<dynamic>().ToList();
		}

		public async Task<List<dynamic>> GetPETNDataAsync(string timeRange)
		{
			DateTime startDate = DateTime.Now;

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
					break;
				case "day":
				default:
					startDate = DateTime.Now.Date; // Today's data
					break;
			}

			// Fetch slurry data from the database
			var PETNnData = await _context.L1Barcodegeneration
			   .Where(x => x.PlantName.ToUpper() == "PETN" && x.MfgDt >= startDate && x.MfgDt <= DateTime.Now)
				.GroupBy(b => b.BrandName)
				.Select(group => new
				{
					BrandName = group.Key,
					TotalL1NetWt = (group.Sum(x => x.L1NetWt) / 1000).ToString("0.00") // Convert weight to kg
				})
				.ToListAsync();

			return PETNnData.Cast<dynamic>().ToList();
		}


		public async Task<List<Models.ProductionReport>> GetProductionReportAsync(DateTime fromDate, DateTime toDate, string reportType, string? shift, string? plant, string? brand, string? productsize)
		{
			if (reportType == "Detailed")
			{
				return await _context.L1Barcodegeneration
					.Where(x => x.MfgDt >= fromDate && x.MfgDt <= toDate &&
								(string.IsNullOrEmpty(shift) || x.Shift == shift) &&
								(string.IsNullOrEmpty(plant) || x.PlantName == plant) &&
								(string.IsNullOrEmpty(brand) || x.BrandName == brand) &&
								(string.IsNullOrEmpty(productsize) || x.ProductSize == productsize))
					.OrderBy(x => x.PlantName)
					.ThenBy(x => x.L1Barcode)
					.Select(x => new Models.ProductionReport
					{
						mfgdt = x.MfgDt,
						plantname = x.PlantName,
						shift = x.Shift,
						brandname = x.BrandName,
						productsize = x.ProductSize,
						l1barcode = x.L1Barcode,
						l1netqty = (double)Math.Round(x.L1NetWt),
						l1netunit = x.L1NetUnit
					}).OrderBy(x => x.mfgdt)
					.ThenBy(x => x.plantname)
					.ToListAsync();
			}
			else
			{
				return await _context.L1Barcodegeneration
					.Where(x => x.MfgDt >= fromDate && x.MfgDt <= toDate &&
								(string.IsNullOrEmpty(shift) || x.Shift == shift) &&
								(string.IsNullOrEmpty(plant) || x.PlantName == plant) &&
								(string.IsNullOrEmpty(brand) || x.BrandName == brand) &&
								(string.IsNullOrEmpty(productsize) || x.ProductSize == productsize))
					.GroupBy(x => new { x.MfgDt, x.PlantName, x.Shift, x.BrandName, x.ProductSize, x.L1NetUnit })
					.Select(g => new Models.ProductionReport
					{
						mfgdt = g.Key.MfgDt,
						plantname = g.Key.PlantName,
						shift = g.Key.Shift,
						brandname = g.Key.BrandName,
						productsize = g.Key.ProductSize,
						boxcount = g.Select(x => x.L1Barcode).Distinct().Count().ToString(),
						l1netqty = (double)Math.Round(g.Sum(x => x.L1NetWt)),
						l1netunit = g.Key.L1NetUnit
					})
					.OrderBy(x => x.mfgdt)
					.ThenBy(x => x.plantname)
					.ThenBy(x => x.shift)
					.ThenBy(x => x.brandname)
					.ToListAsync();
			}
		}





		public async Task<L1Generatedata> GetAllDataAsync()
		{
			try
			{
				/*var date = DateTime.Now.Date;*/ // Get the current date, with the time set to midnight

				// Fetching today's records and grouping them by PlantName and PCode
				var groupedData = await _context.Set<L1barcodegeneration>()
					//.Where(a => a.MfgDt.Date == date) // Filter by today's date
					.GroupBy(a => new { a.PlantName, a.PCode }) // Group by both PlantName and PCode
					.ToListAsync();

				// Count distinct PCode entries for each PlantName
				var slurryCount = groupedData.Count(g => g.Key.PlantName.ToUpper() == "SLURRY");
				var emulsionCount = groupedData.Count(g => g.Key.PlantName.ToUpper() == "EMULSION");
				var detonatorCount = groupedData.Count(g => g.Key.PlantName.ToUpper() == "DETONATING FUSE");
				var petnCount = groupedData.Count(g => g.Key.PlantName.ToUpper() == "PETN");

				// Returning an object with counts of the different plants
				return new L1Generatedata
				{
					slurry = slurryCount,
					emultion = emulsionCount,
					detonator = detonatorCount,
					petn = petnCount
				};
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while fetching data.", ex);
			}
		}


		public async Task<BarcodeSearchResultDto> GetSearchDataAsync(string l1barcode)
		{
			//var barcodeList = await _context.L1Barcodegeneration.ToListAsync();
			//var l2barcodeList = await _context.BarcodeData.ToListAsync();
			//var dispatchList = await _context.DispatchTransaction.ToListAsync();
			//var indentList = await _context.Re11IndentInfo.ToListAsync();

			var l1barcodedata = _context.L1Barcodegeneration.FirstOrDefault(x => x.L1Barcode == l1barcode)?.L1Barcode;

			var L1detaillist = _context.L1Barcodegeneration
				.Where(x => x.L1Barcode == l1barcodedata)
				.Select(x => new
				{
					x.L1NetWt,
					x.L1NetUnit,
					x.NoOfL2,
					x.NoOfL3
				}).ToList();

			var manufacturingDetails = _context.L1Barcodegeneration
				.Where(x => x.L1Barcode == l1barcode)
				.Select(x => new
				{
					x.L1Barcode,
					x.SrNo,
					x.Country,
					x.CountryCode,
					x.MfgName,
					x.MfgLoc,
					x.MfgCode,
					x.PlantName,
					x.PCode,
					x.MCode,
					x.Shift,
					x.BrandName,
					x.BrandId,
					x.ProductSize,
					x.PSizeCode,
					x.SdCat,
					x.UnNoClass,
					x.MfgDt,
					x.MfgTime,
					x.L1NetWt,
					x.L1NetUnit,
					x.NoOfL2,
					x.NoOfL3,
					x.MFlag,
					x.CheckFlag
				}).ToList();

			var l1l2l3Details = _context.BarcodeData
				.Where(x => x.L1 == l1barcode)
				.Select(x => new
				{
					x.L1,
					x.L2,
					x.L3,
					x.Batch,
					x.Re2,
					x.Re12
				}).ToList();

			var dispatchIndentDetails = _context.DispatchTransaction
				.Where(dispatch => dispatch.L1Barcode == l1barcode)
				.Join(_context.Re11IndentInfo,
					dispatch => dispatch.IndentNo,
					indent => indent.IndentNo,
					(dispatch, indent) => new
					{
						dispatch.IndentNo,
						indent.IndentDt,
						indent.CustName,
						dispatch.TruckNo,
						dispatch.Brand,
						dispatch.PSize,
						dispatch.MagName,
						dispatch.DispDt
					}).ToList();

			return new BarcodeSearchResultDto
			{
				ManufacturingDetails = manufacturingDetails,
				L1L2L3Details = l1l2l3Details,
				DispatchIndentDetails = dispatchIndentDetails,
				L1DetailList = L1detaillist
			};
		}

		public async Task<List<L1barcodegeneration>> GetProductionStatusAsync(DateTime mfgDate, string? brand, string? brandId, string? plant, string? plantCode, string? productSize, string? pSizeCode)
		{
			// var query = _context.L1barcodegenerations.AsQueryable();

			var query = _context.L1Barcodegeneration.Where(x =>
				 (mfgDate == default || x.MfgDt == mfgDate) &&
				 (string.IsNullOrEmpty(plant) || x.PlantName == plant) &&
				 (string.IsNullOrEmpty(plantCode) || x.PCode == plantCode) &&
				 (string.IsNullOrEmpty(brand) || x.BrandName == brand) &&
				 (string.IsNullOrEmpty(brandId) || x.BrandId == brandId) &&
				 (string.IsNullOrEmpty(productSize) || x.ProductSize == productSize) &&
				 (string.IsNullOrEmpty(pSizeCode) || x.PSizeCode == pSizeCode));

			return await query
				.OrderBy(x => x.PlantName)
				.ThenBy(x => x.L1Barcode)
				.ThenBy(x => x.SrNo)
				.ThenBy(x => x.BrandName)
				.Select(x => new L1barcodegeneration
				{
					L1Barcode = x.L1Barcode,
					SrNo = x.SrNo,
					Country = x.Country,
					CountryCode = x.CountryCode,
					MfgName = x.MfgName,
					MfgLoc = x.MfgLoc,
					MfgCode = x.MfgCode,
					PlantName = x.PlantName,
					PCode = x.PCode,
					MCode = x.MCode,
					Shift = x.Shift,
					BrandName = x.BrandName,
					BrandId = x.BrandId,
					ProductSize = x.ProductSize,
					PSizeCode = x.PSizeCode,
					SdCat = x.SdCat,
					UnNoClass = x.UnNoClass,
					MfgDt = x.MfgDt,
					MfgTime = x.MfgTime,
					L1NetWt = x.L1NetWt,
					L1NetUnit = x.L1NetUnit,
					NoOfL2 = x.NoOfL2,
					NoOfL3 = x.NoOfL3,
					MFlag = x.MFlag,
					CheckFlag = x.CheckFlag
				}).ToListAsync();
		}

		public async Task<List<L1barcodegeneration>> GetL1DataRecordByL1Async(string l1barcode)
		{
			var l1barcodedata = await _context.L1Barcodegeneration.Where(x => x.L1Barcode == l1barcode).ToListAsync();
			return l1barcodedata;
		}

		public async Task<HashSet<string>> GetBulkExistingL1sAsync(List<string> l1List)
		{
			if (l1List == null || l1List.Count == 0)
				return new HashSet<string>();

			var existingL1s = await _context.L1Barcodegeneration
				.Where(x => l1List.Contains(x.L1Barcode))
				.Select(x => x.L1Barcode)
				.ToListAsync();

			return existingL1s.ToHashSet();
		}


		public async Task<L1barcodegeneration> GetL1DataRecordByL1FristAndDefultAsync(string l1barcode)
		{
			var l1barcodedata = await _context.L1Barcodegeneration.Where(x => x.L1Barcode == l1barcode).FirstOrDefaultAsync();
			return l1barcodedata;
		}



		public async Task<List<L1barcodegeneration>> GetL1ReprintBarcodesAsync(DateTime? MfgDt, string? plant, string? plantcode, string? mode, string? Brandname, string? BrandId, string? productsize,
			string? psizecode, string? l1barcode, string? shift, string? mcode)
		{
			// var data = await _context.L1barcodegenerations.ToListAsync();
			List<L1barcodegeneration> result;

			if (!string.IsNullOrWhiteSpace(l1barcode))
			{
				result = _context.L1Barcodegeneration
					.Where(x => x.L1Barcode.Trim() == l1barcode.Trim())
					.OrderBy(x => x.L1Barcode)
					.ThenBy(x => x.SrNo)
					.ToList();
			}
			else
			{
				string newshift = shift == "ALL" ? "" : shift;
				string newmcode = mcode == "ALL" ? "" : mcode;
				var query = _context.L1Barcodegeneration
				.Where(x => x.PCode.Trim() == plantcode
					&& x.MfgDt == MfgDt
					&& x.BrandId == BrandId
					&& x.PSizeCode == psizecode
				);

				if (!string.IsNullOrEmpty(newshift))
				{
					query = query.Where(x => x.Shift == newshift);
				}

				if (!string.IsNullOrEmpty(newmcode))
				{
					query = query.Where(x => x.MCode == newmcode);
				}

				result = query
					.OrderBy(x => x.L1Barcode)
					.ThenBy(x => x.SrNo)
					.ToList();
			}

			return result;
		}

	}
	public class L1Generatedata
	{
		public int slurry { get; set; }
		public int emultion { get; set; }
		public int detonator { get; set; }
		public int petn { get; set; }

	}

}



