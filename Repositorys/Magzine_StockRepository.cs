using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;

using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using System.Linq;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

    /// <summary>
    /// OPTIMIZED Magzine_Stock Repository - Uses single scoped DbContext
    /// </summary>
    public class Magzine_StockRepository : GenericRepository<Magzine_Stock>, IMagzine_StockRepository
    {
        private readonly ApplicationDbContext _context;

        public Magzine_StockRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Magzine_Stock>> GetRecordbyflag(int re2flag, int re12flag, string magzine, string brandid, string psizecode)
        {
            var records = await _context.Magzine_Stock
                .Where(x => x.Re2 == (re2flag != 0) && x.Re12 == (re12flag != 0) && x.MagName == magzine && x.BrandId == brandid && x.PSizeCode == psizecode)
                .ToListAsync();
            return records;
        }

        public async Task UpdateRE2FlagAsync(List<L1L2> l1l2List, int newRE2FlagValue)
        {
            try
            {
                // Constructing the list of L1Barcodes (only L1Barcode is considered)
                var l1Barcodes = l1l2List.Select(x => x.L1barcode).ToList();

                // Execute update statement on Magzine_Stock table based on L1Barcode only
                await _context.Magzine_Stock
                    .Where(ms => l1Barcodes.Contains(ms.L1Barcode)) // Only check for L1Barcode
                    .ExecuteUpdateAsync(ms => ms.SetProperty(m => m.Re2, newRE2FlagValue != 0)); // Update RE2Flag
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., log the error)
                throw new Exception("Error updating RE2Flag in Magzine_Stock.", ex);
            }
        }

        public async Task UpdateRE2FlagdataAsync(List<L1L22> l1l22List, int newRE2FlagValue)
        {
            try
            {
                var l1Barcodes = l1l22List.Select(x => x.l1barcode).ToList();

                // Execute update statement on Magzine_Stock table based on L1Barcode only
                await _context.Magzine_Stock
                    .Where(ms => l1Barcodes.Contains(ms.L1Barcode)) // Only check for L1Barcode
                    .ExecuteUpdateAsync(ms => ms.SetProperty(m => m.Re2, newRE2FlagValue != 0)); // Update RE2Flag
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., log the error)
                throw new Exception("Error updating RE2Flag in Magzine_Stock.", ex);
            }
        }
        public async Task<List<string>> GetExistingBarcodesAsync(List<string> barcodes)
        {
            return new List<string>(
                await _context.Magzine_Stock
                    .AsNoTracking()
                    .Where(m => barcodes.Contains(m.L1Barcode))
                    .Select(m => m.L1Barcode)
                    .ToListAsync()
            );
        }
        public async Task<List<Magzine_Stock>> SaveMagzineTransactionBulk(List<Magzine_Stock> magzine_Stocks)
        {
            // Get barcodes that already exist
            var existingBarcodes = await _context.Magzine_Stock.AsNoTracking()
                .Where(x => magzine_Stocks.Select(d => d.L1Barcode).Contains(x.L1Barcode))
                .Select(x => x.L1Barcode)
                .ToListAsync();

            // Filter out duplicates
            var newDispatches = magzine_Stocks
                .Where(x => !existingBarcodes.Contains(x.L1Barcode))
                .ToList();

            if (newDispatches.Count == 0)
                return new List<Magzine_Stock>(); // nothing to add

            await _context.Magzine_Stock.AddRangeAsync(newDispatches);
            await _context.SaveChangesAsync();

            return newDispatches;
        }


        public async Task<List<Magzine_Stock>> GetL1DetailsAsync(List<string> barcodes)
        {
            return await _context.Magzine_Stock
                .AsNoTracking()
                .Where(m => barcodes.Contains(m.L1Barcode))
                .ToListAsync();
        }

        public async Task<List<MagazineStockSummary>> GetMagstockdetails()
        {

			var groupedMagazines = await _context.Magzine_Stock
				.Where(m => m.Stock == 1  && m.Re12 == false)
				.GroupBy(m => m.MagName)
				.Select(g => new MagazineStockSummary
				{
					Name = g.Key,
					Count = g.Count()
				})
				.ToListAsync();


            return groupedMagazines;
        }

        public async Task<List<MagRE2DetailsDto>> GetMagRE2details()
        {
            var result = await (
                from ms in _context.Magzine_Stock.Where(x => x.Stock == 1 && x.Re2 == true && x.Re12 == false)
                join pm in _context.ProductMaster on new { BrandId = ms.BrandId, PSizeCode = ms.PSizeCode }
                                                   equals new { BrandId = pm.bid, PSizeCode = pm.psizecode }
                group pm by ms.MagName into g
                select new MagRE2DetailsDto
                {
                    MagName = g.Key,
                    BarcodeCount = g.Count(),
                    TotalNetWeight = (double)g.Sum(x => x.l1netwt)
                }).ToListAsync();



            return result;
        }

        public async Task<List<MagazineStockSummary>> GetMagRE2listdetails()
        {
            var groupedMagazines = await _context.Magzine_Stock
                .Where(m => m.Stock == 1 && m.Re2 == true)
                .GroupBy(m => m.MagName)
                .Select(g => new MagazineStockSummary
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return groupedMagazines;
        }
        public async Task<List<int>> GetMagallotmentdetails()
        {
            var magdata = await _context.Magzine_Stock
                .Where(m => m.Stock == 1)
                .GroupBy(m => m.MagName)
                .Select(a => a.Count())
                .ToListAsync();

            return magdata;
        }


        public async Task<List<StorageMagazineReport>> GetStorageStockReportAsync(string? fromDate, string? toDate, string reportType, string? magazine, string? brand, string? productSize)
        {
            DateTime? fromDateTime = DateTime.TryParse(fromDate, out var fromDt) ? fromDt : null;
            DateTime? toDateTime = DateTime.TryParse(toDate, out var toDt) ? toDt : null;

            if (reportType == "Storage")
            {
                var query = _context.MagzineMasters
                    .Join(_context.Magzine_Stock, m => m.mcode, s => s.MagName, (m, s) => new { m, s })
                    .Join(_context.ProductMaster, ms => new { ms.s.BrandId, ms.s.PrdSize, ms.s.PSizeCode },
                        p => new { BrandId = p.bid, PrdSize = p.psize, PSizeCode = p.psizecode },
                        (ms, p) => new { ms.m, ms.s, p })
                    .Where(x => x.s.Stock == 1 && x.s.Re12 == false
                        && (!fromDateTime.HasValue || x.s.StkDt >= fromDateTime)
                        && (!toDateTime.HasValue || x.s.StkDt <= toDateTime)
                        && (string.IsNullOrEmpty(magazine) || x.m.mcode == magazine)
                        && (string.IsNullOrEmpty(brand) || x.s.BrandName == brand)
                        && (string.IsNullOrEmpty(productSize) || x.s.PrdSize == productSize))
                    .Select(x => new StorageMagazineReport
                    {
                        magname = x.m.mcode,
                        license = x.m.licno,
                        brandname = x.s.BrandName,
                        productsize = x.s.PrdSize,
                        L1Barcode = x.s.L1Barcode,
                        netqty = x.p.l1netwt.ToString(),
                        unit = x.p.unit
                    })
                    .Distinct()
                    .OrderBy(x => x.magname)
                    .ThenBy(x => x.productsize)
                    .ThenBy(x => x.L1Barcode);

                // Ensure proper awaiting of the result
                var result = await query.ToListAsync();
                return result;
            }
            else
            {
                var query = _context.MagzineMasters
                    .Join(_context.Magzine_Stock, m => m.mcode, s => s.MagName, (m, s) => new { m, s })
                    .Join(_context.ProductMaster, ms => new { ms.s.BrandId, ms.s.PrdSize, ms.s.PSizeCode },
                        p => new { BrandId = p.bid, PrdSize = p.psize, PSizeCode = p.psizecode },
                        (ms, p) => new { ms.m, ms.s, p })
                    .Where(x => x.s.Stock == 1 && x.s.Re12 == false
                        && (!fromDateTime.HasValue || x.s.StkDt >= fromDateTime)
                        && (!toDateTime.HasValue || x.s.StkDt <= toDateTime)
                        && (string.IsNullOrEmpty(magazine) || x.m.mcode == magazine)
                        && (string.IsNullOrEmpty(brand) || x.s.BrandName == brand)
                        && (string.IsNullOrEmpty(productSize) || x.s.PrdSize == productSize))
                    .GroupBy(x => new { x.m.mcode, x.m.licno, x.s.BrandName, x.s.PrdSize, x.p.unit })
                    .Select(g => new StorageMagazineReport
                    {
                        magname = g.Key.mcode,
                        license = g.Key.licno,
                        brandname = g.Key.BrandName,
                        productsize = g.Key.PrdSize,
                        L1Barcode = g.Select(x => x.s.L1Barcode).Distinct().Count().ToString(),
                        netqty = g.Sum(x => Convert.ToInt32(x.p.l1netwt)).ToString(),
                        unit = g.Key.unit
                    })
                    .OrderBy(x => x.magname)
                    .ThenBy(x => x.brandname)
                    .ThenBy(x => x.productsize);

                // Ensure proper awaiting of the result
                var result = await query.ToListAsync();
                return result;
            }
        }

        public async Task<List<RE2StatusReport>> GetRE2StatusDataAsync(DateTime fromDate, DateTime toDate, string reportType, int? re2status, string? brand, string? productsize)
        {
            if (reportType == "Detailed")
            {
                var re2data = _context.Magzine_Stock
                    .Where(x => x.StkDt >= fromDate && x.StkDt <= toDate &&
                                (!re2status.HasValue || x.Re2 == (re2status.Value != 0)) &&
                                (string.IsNullOrEmpty(brand) || x.BrandName == brand) &&
                                (string.IsNullOrEmpty(productsize) || x.PrdSize == productsize))
                    .OrderBy(x => x.BrandName)
                    .ThenBy(x => x.L1Barcode)
                    .Select(x => new RE2StatusReport
                    {
                        brandname = x.BrandName,
                        productsize = x.PrdSize,
                        magname = x.MagName,
                        unloadDt = x.StkDt,
                        l1barcode = x.L1Barcode,
                        re2status = x.Re2 ? 1 : 0
                    })
                    .OrderBy(x => x.magname)
                    .ToList();

                return re2data;
            }
            else // Summary
            {
                var re2data = _context.Magzine_Stock
                    .Where(x => x.StkDt >= fromDate && x.StkDt <= toDate &&
                                (!re2status.HasValue || x.Re2 == (re2status.Value != 0)) &&
                                (string.IsNullOrEmpty(brand) || x.BrandName == brand) &&
                                (string.IsNullOrEmpty(productsize) || x.PrdSize == productsize))
                    .GroupBy(x => new { Re2 = x.Re2, x.BrandName, x.PrdSize, x.MagName, x.StkDt })
                    .Select(g => new RE2StatusReport
                    {
                        brandname = g.Key.BrandName,
                        productsize = g.Key.PrdSize,
                        magname = g.Key.MagName,
                        unloadDt = g.Key.StkDt,
                        l1barcode = g.Select(x => x.L1Barcode).Distinct().Count().ToString(),
                        re2status = g.Key.Re2 ? 1 : 0
                    })
                    .OrderBy(x => x.brandname)
                    .ThenBy(x => x.l1barcode)
                    .ThenBy(x => x.magname)
                    .ToList();

                return re2data;
            }

        }

        public async Task<List<MagStock>> GetMagNameStatusAsync(string? magname)
        {
            // var barcodeList = await _context.MagzineStocks.ToListAsync();
            //var magList = await _context.ProductMasters.ToListAsync();

            var result = (from barcode in _context.Magzine_Stock
                          join mag in _context.ProductMaster
                          on barcode.BrandName equals mag.bname into magJoin
                          from mag in magJoin.DefaultIfEmpty()
                          where string.IsNullOrEmpty(magname) || barcode.MagName == magname
                          group new { barcode, mag } by new
                          {
                              barcode.MagName,
                              barcode.PrdSize,
                              barcode.BrandName
                          } into grouped
                          select new MagStock
                          {
                              MagName = grouped.Key.MagName,
                              BrandName = grouped.Key.BrandName,
                              BrandId = grouped.First().barcode.BrandId,
                              PrdSize = grouped.Key.PrdSize,
                              PSizeCode = grouped.First().barcode.PSizeCode,
                              l1netwt = grouped.First().mag != null ? (double)grouped.First().mag.l1netwt : 0,
                              l1netunit = grouped.First().mag != null ? grouped.First().mag.unit : "",
                              L1Barcode = grouped.Select(x => x.barcode.L1Barcode).Distinct().Count().ToString()
                          })
                         .OrderBy(x => x.MagName)
                         .ThenBy(x => x.BrandName)
                         .ThenBy(x => x.PrdSize)
                         .ToList();

            return result;
        }

        public async Task<Magzine_Stock> GetStockByMCodeAsync(string? mcode)
        {
            var loadingSheet = await _context.Magzine_Stock
                .Where(b => b.MagName == mcode && b.Re12 == false)
                .FirstOrDefaultAsync();

            return loadingSheet ?? new Magzine_Stock();
        }

        public async Task<List<RE7MonthlyReport>> GetRE7Report(DateTime? fromDate, DateTime? toDate, string? magname)
        {
            var query = _context.Magzine_Stock
                .Where(x => x.StkDt != null &&
                            x.StkDt >= fromDate &&
                            x.StkDt <= toDate &&
                            (string.IsNullOrEmpty(magname) || x.MagName == magname))
                .GroupBy(x => new
                {
                    x.MagName,
                    x.BrandName,
                    x.BrandId,
                    x.PrdSize,
                    x.PSizeCode,
                    Month = x.StkDt.Value.Month,
                    Year = x.StkDt.Value.Year
                })
                .Select(g => new
                {
                    g.Key.MagName,
                    g.Key.BrandName,
                    g.Key.BrandId,
                    g.Key.PrdSize,
                    g.Key.PSizeCode,
                    g.Key.Month,
                    g.Key.Year,
                    Inward = g.Count(x => x.Re2 == false) + g.Count(x => x.Re2 == true),
                    Outward = g.Count(x => x.Re12 == true)
                })
                .Where(x => x.Inward > 0 || x.Outward > 0)
                .AsEnumerable() // switch to client-side
                .Select(x => new RE7MonthlyReport
                {
                    Month = x.Month.ToString("D2"),
                    Year = x.Year.ToString(),
                    MagName = x.MagName,
                    BrandName = x.BrandName,
                    BrandId = x.BrandId,
                    ProductSize = x.PrdSize,
                    ProductCode = x.PSizeCode,
                    Inward = x.Inward,
                    Outward = x.Outward,
                    RemainingStock = x.Inward - x.Outward
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.MagName)
                .ThenBy(x => x.BrandName)
                .ToList();

            return query;
        }


    }
}



