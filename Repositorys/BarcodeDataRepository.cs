using Peso_Baseed_Barcode_Printing_System_API.Interface;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;

using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class BarcodeDataRepository : GenericRepository<BarcodeData>, IBarcodeDataRepository
	{
		private readonly ApplicationDbContext _context;
		public BarcodeDataRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<List<BarcodeData>> GetRecordbyflag(int re2flag, int re12flag, DateTime mfgdt, string plantcode, string brandid, string psizecode)
		{
			try
			{
				var result = await (from bd in _context.BarcodeData
									join l1 in _context.L1Barcodegeneration on bd.L1 equals l1.L1Barcode
									where bd.Re2 == (re2flag != 0) && bd.Re12 == (re12flag != 0) &&
										  l1.MfgDt == mfgdt && l1.PCode == plantcode &&
										  l1.BrandId == brandid && l1.PSizeCode == psizecode
									select bd).ToListAsync();
				return result;
			}
			catch (Exception ex)
			{
				throw new Exception("Error fetching records by flag.", ex);
			}
		}

		public async Task<List<L1L2>> GetBarcodeDataWithMagAsync(DateTime mfgdt, string plantcode, string brandid, string psizecode, string magname)
		{
			try
			{
				var result = await (from bd in _context.BarcodeData
									join ms in _context.Magzine_Stock on bd.L1 equals ms.L1Barcode
									join l1 in _context.L1Barcodegeneration on bd.L1 equals l1.L1Barcode
									join pm in _context.ProductMaster
										on new { BrandId = ms.BrandId, PSizeCode = (string)ms.PSizeCode }
										equals new { BrandId = pm.bid, PSizeCode = (string)pm.psizecode }
									where ms.MagName == magname
										  && l1.MfgDt == mfgdt
										  && ms.BrandId == brandid
										  && ms.PSizeCode == psizecode
										  && bd.Re2 == false
										  && ms.Re2 == false
									orderby bd.L1
									select new L1L2
									{
										L1barcode = bd.L1,
										L1NetWt = (double)pm.l1netwt,
										Unit = pm.unit,
										MagName = ms.MagName
									})
									.Distinct()
									.ToListAsync();

				return result;
			}
			catch (Exception ex)
			{
				// Handle exception, log if necessary
				throw new Exception("Error fetching barcode data for RE2 generation.", ex);
			}



		}

		public async Task<List<L1L2>> GetBarcodeDataWithoutMagAsync(DateTime mfgdt, string plantcode, string brandid, string psizecode)
		{
			try
			{
				var result = await (from bd in _context.BarcodeData
									join ms in _context.Magzine_Stock on bd.L1 equals ms.L1Barcode
									join l1 in _context.L1Barcodegeneration on bd.L1 equals l1.L1Barcode
									join pm in _context.ProductMaster
										on new { BrandId = ms.BrandId, PSizeCode = (string)ms.PSizeCode }
										equals new { BrandId = pm.bid, PSizeCode = (string)pm.psizecode }
									where l1.MfgDt == mfgdt
										  && ms.BrandId == brandid
										  && ms.PSizeCode == psizecode
										  && bd.Re2 == false
										  && ms.Re2 == false
									orderby bd.L1
									select new L1L2
									{
										L1barcode = bd.L1,
										L1NetWt = (double)pm.l1netwt,
										Unit = pm.unit,
										MagName = ms.MagName
									})
									.Distinct()
									.ToListAsync();

				return result;
			}
			catch (Exception ex)
			{
				// Handle exception, log if necessary
				throw new Exception("Error fetching barcode data for RE2 generation.", ex);
			}

		}

		public async Task<List<L1L22>> GetBarcodeDataWithMagdataAsync(DateTime mfgdt, string plantcode, string brandid, string psizecode, string fromcase, string tocase)
		{
			try
			{
				var result = await (from bd in _context.BarcodeData
									join ms in _context.Magzine_Stock on bd.L1 equals ms.L1Barcode
									join l1 in _context.L1Barcodegeneration on bd.L1 equals l1.L1Barcode
									join pm in _context.ProductMaster
										on new { BrandId = ms.BrandId, PSizeCode = (string)ms.PSizeCode }
										equals new { BrandId = pm.bid, PSizeCode = (string)pm.psizecode }
									where l1.MfgDt == mfgdt
										  && ms.BrandId == brandid
										  && ms.PSizeCode == psizecode
										  && bd.Re2 == true
										  && ms.Re2 == true
										   && int.Parse(ms.L1Barcode.Substring(21, 7)) >= int.Parse(fromcase) &&
										  int.Parse(ms.L1Barcode.Substring(21, 7)) <= int.Parse(tocase)
									orderby bd.L1
									select new L1L22
									{
										l1barcode = bd.L1,
										l1netwt = (double)pm.l1netwt,
										unit = pm.unit,
										magname = ms.MagName
									})
									.Distinct()
									.ToListAsync();

				return result;
			}
			catch (Exception ex)
			{
				throw new Exception("Error fetching barcode data for RE2 generation.", ex);
			}
		}


		public async Task<List<L1L22>> GetBarcodeDataWithoutMagdataAsync(DateTime mfgdt, string? plantcode, string? brandid, string? psizecode, string? fromcase, string? tocase)
		{
			try
			{
				int start = string.IsNullOrEmpty(fromcase) ? 0 : Convert.ToInt32(fromcase);
				int end = string.IsNullOrEmpty(tocase) ? int.MaxValue : Convert.ToInt32(tocase);

				var rawData = (from bd in _context.BarcodeData
							   join ms in _context.Magzine_Stock on bd.L1 equals ms.L1Barcode
							   join l1 in _context.L1Barcodegeneration on bd.L1 equals l1.L1Barcode
							   join pm in _context.ProductMaster on new { ms.BrandId, ms.PSizeCode } equals new { BrandId = pm.bid, PSizeCode = pm.psizecode }
							   where l1.MfgDt == mfgdt
									 && ms.BrandId == brandid
									 && ms.PSizeCode == psizecode
									 && bd.Re2 == true
									 && ms.Re2 == true
									 && !string.IsNullOrEmpty(ms.L1Barcode) && ms.L1Barcode.Length >= 7
							   orderby bd.L1
							   select new
							   {
								   l1barcode = bd.L1,
								   l1netwt = pm.l1netwt,
								   unit = pm.unit,
								   magname = ms.MagName,
								   last7str = ms.L1Barcode.Substring(ms.L1Barcode.Length - 7)
							   }).AsEnumerable()
								 .Where(x => int.TryParse(x.last7str, out var last7)
											 && last7 >= start && last7 <= end)
								 .Select(x => new L1L22
								 {
									 l1barcode = x.l1barcode,
									 l1netwt = (double)x.l1netwt,
									 unit = x.unit,
									 magname = x.magname
								 })
								 .DistinctBy(x => x.l1barcode)
								 .ToList();

				return rawData;
			}
			catch (Exception ex)
			{
				throw new Exception("Error fetching barcode data for RE2 generation.", ex);
			}
		}

		public async Task<List<L1L2L3>> GetL1L2L3DataAsync(List<L1L2> l1l2List)
		{
			try
			{
				// Split l1l2List into smaller chunks if needed (e.g., batches of 1000)
				var batchSize = 1000;
				var batches = l1l2List
					.Select((item, index) => new { item, index })
					.GroupBy(x => x.index / batchSize)
					.Select(g => g.Select(x => x.item).ToList())
					.ToList();


				var result = (from batch in batches
							  from barcodeData in _context.BarcodeData
							  where batch.Any(x => x.L1barcode == barcodeData.L1)
							  select new L1L2L3
							  {
								  L1barcode = barcodeData.L1,
								  L2barcode = barcodeData.L2,
								  L3barcode = barcodeData.L3
							  })
			  .OrderBy(e => e.L1barcode)
			  .ThenBy(e => e.L2barcode)
			  .ThenBy(e => e.L3barcode)
			  .ToList();


				return result;
			}
			catch (Exception ex)
			{
				// Log the error
				// _logger.LogError(ex, "An error occurred while retrieving L1L2L3 data.");
				return new List<L1L2L3>();  // Or you can rethrow the exception: throw;
			}
		}

		public async Task<List<L1L2L3>> GetL1L2L3detaAsync(List<L1L22> l1l22List)
		{
			try
			{
				// Split l1l2List into smaller chunks if needed (e.g., batches of 1000)
				var batchSize = 1000;
				var batches = l1l22List
					.Select((item, index) => new { item, index })
					.GroupBy(x => x.index / batchSize)
					.Select(g => g.Select(x => x.item).ToList())
					.ToList();

				//var result = new List<L1L2L3>();

				// Process each batch
				var result = (from batch in batches
							  from barcodeData in _context.BarcodeData
							  where batch.Any(x => x.l1barcode == barcodeData.L1)
							  select new L1L2L3
							  {
								  L1barcode = barcodeData.L1,
								  L2barcode = barcodeData.L2,
								  L3barcode = barcodeData.L3
							  })
			  .OrderBy(e => e.L1barcode)
			  .ThenBy(e => e.L2barcode)
			  .ThenBy(e => e.L3barcode)
			  .ToList();


				return result;
			}
			catch (Exception ex)
			{
				// Log the error
				// _logger.LogError(ex, "An error occurred while retrieving L1L2L3 data.");
				return new List<L1L2L3>();  // Or you can rethrow the exception: throw;
			}
		}

		public async Task<List<BarcodeData>> GetBarcodedataByL1lIST(List<string> l1l22List)
		{
			try
			{


				var result = await _context.BarcodeData
					.Where(b => l1l22List.Contains(b.L1)).AsNoTracking().OrderBy(a => a.L1).ThenBy(a => a.L2).ThenBy(a => a.L3)
					.ToListAsync();
				return result;
			}
			catch (Exception ex)
			{
				// Log the error
				// _logger.LogError(ex, "An error occurred while retrieving L1L2L3 data.");
				return new List<BarcodeData>();  // Or you can rethrow the exception: throw;
			}
		}

		public async Task UpdateRE2FlagAsync(List<L1L2> l1l2List, int newRE2FlagValue)
		{
			try
			{
				// Extract L1Barcode from the provided L1L2 list
				var l1Barcodes = l1l2List.Select(x => x.L1barcode).ToList();

				var output = _context.BarcodeData
					.Where(b => l1Barcodes.Contains(b.L1)).ToList();
				// Execute update statement on BarcodeData table based on L1Barcode
				await _context.BarcodeData
					.Where(b => l1Barcodes.Contains(b.L1)) // Filter only by L1Barcode
					.ExecuteUpdateAsync(b => b.SetProperty(p => p.Re2, newRE2FlagValue != 0)); // Update RE2Flag
			}
			catch (Exception ex)
			{
				// Handle any exceptions (e.g., log the error)
				throw new Exception("Error updating RE2Flag in BarcodeData.", ex);
			}
		}

		public async Task UpdateRE2FlagdataAsync(List<L1L22> l1l22List, int newRE2FlagValue)
		{
			try
			{
				// Extract L1Barcode from the provided L1L2 list
				var l1Barcodes = l1l22List.Select(x => x.l1barcode).ToList();

				// Execute update statement on BarcodeData table based on L1Barcode
				await _context.BarcodeData
					.Where(b => l1Barcodes.Contains(b.L1)) // Filter only by L1Barcode
					.ExecuteUpdateAsync(b => b.SetProperty(p => p.Re2, newRE2FlagValue != 0)); // Update RE2Flag
			}
			catch (Exception ex)
			{
				// Handle any exceptions (e.g., log the error)
				throw new Exception("Error updating RE2Flag in BarcodeData.", ex);
			}
		}

		public async Task<L1L2L3> GetLatestL1L2L3DetailsAsync(string pcode, string brandid, string productsizeCode, string shift, string mcode, DateTime? date, string countryCode, string mfgcode)
		{
			try
			{
				
				string year = date.Value.Year.ToString();
				string month = date.Value.ToString("MM");    

				var nextSrNo = await _context.L1Barcodegeneration
					.Where(x => x.MfgDt.Year == date.Value.Year &&
								x.MfgDt.Month == date.Value.Month &&
								x.BrandId == brandid &&
								x.PSizeCode == productsizeCode)
					.MaxAsync(x => (long?)x.SrNo) ?? 0;

				nextSrNo += 1;

				string paddedSrNo = nextSrNo.ToString().PadLeft(6, '0');

				long nextL2 = await _context.L2Barcodegeneration
					.Where(x => x.ParentL1 != null && x.ParentL1.PCode == pcode && x.ParentL1.MCode == mcode)
					.MaxAsync(x => (long?)x.SrNo) ?? 0;
				nextL2 += 1;
				string l2Serial = nextL2.ToString().PadLeft(12, '0');

				long nextL3 = await _context.L3Barcodegeneration
					.Where(x => x.ParentL1 != null && x.ParentL1.PCode == pcode && x.ParentL1.MCode == mcode)
					.MaxAsync(x => (long?)x.SrNo) ?? 0;
				nextL3 += 1;
				string l3Serial = nextL3.ToString().PadLeft(15, '0');

				return new L1L2L3
				{
					L1barcode = $"{countryCode}{mfgcode}{date.Value.ToString("ddMMyy")}{brandid}{productsizeCode}{pcode}{paddedSrNo}",
					L2barcode = $"{countryCode}{mfgcode}{pcode}{mcode}{l2Serial}",
					L3barcode = $"{countryCode}{mfgcode}{pcode}{mcode}{l3Serial}"	
				};
			}
			catch (Exception ex)
			{
				// Log error here if needed
				return new L1L2L3
				{
					L1barcode = "-",
					L2barcode = "-",
					L3barcode = "-"
				};
			}
		}

		public async Task<DataTable> GetBatchInfo(string? brandid, string? psizecode, int reqCase)
		{
			try
			{
				var groupedResult = await (from bd in _context.BarcodeData
										   join l1 in _context.L1Barcodegeneration on bd.L1 equals l1.L1Barcode
										   where l1.BrandId == brandid && l1.PSizeCode == psizecode && bd.Re2 == true && bd.Re12 == false
										   select bd).Take(reqCase)
				.GroupBy(b => b.Batch)
				.OrderBy(g => g.Key)
				.Select(g => new
				{
					Batch = g.Key,
					DistinctCount = g.Select(x => x.L1).Distinct().Count()
				})
				.ToListAsync();

				var dataTable = new DataTable();
				dataTable.Columns.Add("Batch", typeof(string));
				dataTable.Columns.Add("DistinctCount", typeof(int));

				foreach (var item in groupedResult)
				{
					dataTable.Rows.Add(item.Batch, item.DistinctCount);
				}

				return dataTable;
			}
			catch (Exception ex)
			{
				throw new Exception("Error fetching batch info.", ex);
			}
		}

	}
}




