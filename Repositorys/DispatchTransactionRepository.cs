using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using System.Globalization;
using System.Linq.Expressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    /// <summary>
    /// OPTIMIZED DispatchTransaction Repository - Uses single scoped DbContext
    /// </summary>
    public class DispatchTransactionRepository : GenericRepository<DispatchTransaction>, IDispatchTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public DispatchTransactionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> GetIndentNoForMagAllotAsync()
        {
            var result = await _context.DispatchTransaction
                .Where(e => e.MagName == "-" && e.Re12 == false && !string.IsNullOrEmpty(e.IndentNo))
                .Select(f => new SelectListItem
                {
                    Value = f.IndentNo,
                    Text = f.IndentNo
                })
                .Distinct()
                .ToListAsync();

            // Return an empty list if null
            return result ?? new List<SelectListItem>();
        }

		public async Task<DispatchTransaction> DispatchTransactionByL1Barcode(string L1Barcode)
		{
			return await _context.DispatchTransaction.AsNoTracking()
				.FirstOrDefaultAsync(d => d.L1Barcode == L1Barcode);
		}
       
        public async Task<List<DispatchTransaction>> SaveDispatchTransactionBulk(List<DispatchTransaction> dispatchTransactions)
        {
            // Get barcodes that already exist
            var existingBarcodes = await _context.DispatchTransaction
                .Where(x => dispatchTransactions.Select(d => d.L1Barcode).Contains(x.L1Barcode))
                .Select(x => x.L1Barcode)
                .ToListAsync();

            // Filter out duplicates
            var newDispatches = dispatchTransactions
                .Where(x => !existingBarcodes.Contains(x.L1Barcode))
                .ToList();

            if (newDispatches.Count == 0)
                return new List<DispatchTransaction>(); // nothing to add

            _context.DispatchTransaction.AddRange(newDispatches);
            await _context.SaveChangesAsync();

            return newDispatches;
        }

        // Get Truck Number by Indent Number
        public async Task<string> GetTruckNoByIndentNo(string indentNo)
        {
            try
            {
                var transaction = await _context.DispatchTransaction
                .FirstOrDefaultAsync(d => d.IndentNo == indentNo && d.Re12 == false);

                return transaction?.TruckNo ?? string.Empty;
            }
            catch (Exception ex)
            {

                string s = ex.Message.ToString();
                return string.Empty;
            }

        }




        // Get Truck Number by Indent Number
        public async Task<string> GetTruckNoByIndentNo(string indentNo, DateTime mfgdt)
        {
            try
            {
                var transaction = await _context.DispatchTransaction
                .FirstOrDefaultAsync(d => d.IndentNo == indentNo && d.DispDt == mfgdt && d.Re12 == false);

                return transaction?.TruckNo ?? string.Empty;
            }
            catch (Exception ex)
            {

                string s = ex.Message.ToString();
                return string.Empty;
            }

        }

        // Get distinct Brands by Indent Number
        public async Task<List<string>> GetBrandsByIndentNo(string indentNo, string truckno)
        {
            return await _context.DispatchTransaction
                .Where(d => d.IndentNo == indentNo && d.Re12 == false)
                .Select(d => d.Brand)
                .Distinct()
                .ToListAsync();
        }

        // Get distinct Brands by Indent Number
        public async Task<List<string>> GetBrandsByIndentNo(string indentNo, string truckno, DateTime mfgdt)
        {
            return await _context.DispatchTransaction
                .Where(d => d.IndentNo == indentNo && d.DispDt == mfgdt && d.Re12 == false)
                .Select(d => d.Brand)
                .Distinct()
                .ToListAsync();
        }

        public async Task<string> GetBrandIdByIndentNoAndBrandName(string indentNo, string brandName)
        {
            try
            {
                var brandId = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.Brand == brandName)
                    .Select(d => d.Bid)
                    .FirstOrDefaultAsync();

                return brandId ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Handle error logging
                return string.Empty;
            }
        }


        public async Task<string> GetBrandIdByIndentNoAndBrandName(string indentNo, string brandName, DateTime mfgdt)
        {
            try
            {
                var brandId = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.DispDt == mfgdt && d.Brand == brandName && d.Re12 == false)
                    .Select(d => d.Bid)
                    .FirstOrDefaultAsync();

                return brandId ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Handle error logging
                return string.Empty;
            }
        }


        public async Task<List<string>> GetPSizesByIndentNoBrandNameAndBrandId(string indentNo, string brandName, string brandId)
        {
            try
            {
                var pSizes = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.Brand == brandName && d.Bid == brandId)
                    .Select(d => d.PSize)
                    .Distinct()
                    .ToListAsync();

                return pSizes;
            }
            catch (Exception ex)
            {
                // Handle error logging
                return new List<string>();
            }
        }

        public async Task<List<string>> GetPSizesByIndentNoBrandNameAndBrandId(string indentNo, string brandName, string brandId, DateTime mfgdt)
        {
            try
            {
                var pSizes = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.Brand == brandName && d.Bid == brandId && d.DispDt == mfgdt && d.Re12 == false)
                    .Select(d => d.PSize)
                    .Distinct()
                    .ToListAsync();

                return pSizes;
            }
            catch (Exception ex)
            {
                // Handle error logging
                return new List<string>();
            }
        }


        public async Task<string> GetSizeCodesByIndentNo(string indentNo, string brandName, string brandId, string psize)
        {
            try
            {
                var pSizesWithCodes = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.Brand == brandName && d.Bid == brandId && d.PSize == psize && d.MagName == "-" && d.Re12 == false)
                    .Select(d => d.PSizeCode)
                    .Distinct()
                    .FirstOrDefaultAsync();

                return pSizesWithCodes ?? string.Empty; // Return empty string if null
            }
            catch (Exception ex)
            {
                // Handle error logging
                return string.Empty; // Return an empty string on error
            }
        }

        public async Task<string> GetSizeCodesByIndentNo(string indentNo, string brandName, string brandId, string psize, DateTime mfgdt)
        {
            try
            {
                var pSizesWithCodes = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.Brand == brandName && d.Bid == brandId && d.PSize == psize && d.DispDt == mfgdt && d.Re12 == false)
                    .Select(d => d.PSizeCode)
                    .Distinct()
                    .FirstOrDefaultAsync();

                return pSizesWithCodes ?? string.Empty; // Return empty string if null
            }
            catch (Exception ex)
            {
                // Handle error logging
                return string.Empty; // Return an empty string on error
            }
        }

        public async Task<List<L1Data>> GetL1DataWithoutMagName(string indentNo, string truckno, string brandId, string psize)
        {
            try
            {
                var l1Data = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.TruckNo == truckno && d.Bid == brandId && d.PSizeCode == psize && d.MagName == "-" && d.Re12 == false)
                    .Select(d => new L1Data
                    {
                        L1barcode = d.L1Barcode,
                        L1NetWt = (double)d.L1NetWt,
                        Unit = d.L1NetUnit
                    })
                    .ToListAsync();

                return l1Data ?? new List<L1Data>(); // Return empty list if no result found
            }
            catch (Exception ex)
            {
                // Handle error logging
                return new List<L1Data>();
            }
        }


        public async Task<List<L11DispatchData>> GetDataRE12Details(string indentNo, string truckno, string brandId, string psize, string magname, DateTime mfgdt, string fromcase, string tocase)
        {
            try
            {
                int start = string.IsNullOrEmpty(fromcase) ? 0 : Convert.ToInt32(fromcase);
                int end = string.IsNullOrEmpty(tocase) ? int.MaxValue : Convert.ToInt32(tocase);

                var rawData = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo
                                && d.TruckNo == truckno
                                && d.Bid == brandId
                                && d.PSizeCode == psize
                                && d.MagName == magname
                                && d.DispDt == mfgdt
                                && d.Re12 == true
                                && !string.IsNullOrEmpty(d.L1Barcode)
                                && d.L1Barcode.Length >= 7)
                    .Select(d => new
                    {
                        L1Barcode = d.L1Barcode,
                        brand = d.Brand,
                        psize = d.PSize,
                        MagName = d.MagName,
                        indentNo = d.IndentNo,
                        truckno = d.TruckNo,
                        disptchdt = d.DispDt,
                        L1NetWt = d.L1NetWt,
                        L1NetUnit = d.L1NetUnit,

                        Last7Str = d.L1Barcode.Substring(d.L1Barcode.Length - 7)
                    })
                    .ToListAsync();

                var filteredData = rawData
                    .Where(x => int.TryParse(x.Last7Str, out var num) && num >= start && num <= end)
                    .Select(x => new L11DispatchData
                    {
                        L1barcode = x.L1Barcode,
                        brand = x.brand,
                        psize = x.psize,
                        mag = x.MagName,
                        indentno = x.indentNo,
                        truckno = x.truckno,
                        dispdt = x.disptchdt,
                        l1NetWt = (double)x.L1NetWt,
                        unit = x.L1NetUnit,
                    })
                    .DistinctBy(x => x.L1barcode)
                    .ToList();

                return filteredData;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return new List<L11DispatchData>();
            }
        }


        public async Task BulkUpdateMag(List<string> barcodes, string NewMag)
        {
            try
            {
                // Ensure the list is not empty
                if (barcodes == null || !barcodes.Any())
                    throw new ArgumentException("The list of barcodes cannot be null or empty.");

                // Perform bulk update using ExecuteUpdateAsync
                await _context.DispatchTransaction
                    .Where(x => barcodes.Contains(x.L1Barcode))
                    .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.MagName, NewMag));
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while performing the bulk update.", ex);
            }
        }


        public async Task<List<string>> GetIndentsRE12()
        {
            return await _context.DispatchTransaction
                                 .Where(d => d.Re12 == false) // Filter based on RE12
                                 .Select(d => d.IndentNo)   // Select only the IndentNo field
                                 .Distinct()                // Avoid duplicate values
                                 .ToListAsync();
        }

        public async Task<List<string>> GetMagazinesforDispatch(string indentNo, string brandName, string brandId, string pSize, string pSizeCode, DateTime date)
        {
            return await _context.DispatchTransaction
                                 .Where(d => d.IndentNo == indentNo
                                          && d.Brand == brandName
                                          && d.Bid == brandId
                                          && d.PSize == pSize
                                          && d.PSizeCode == pSizeCode
                                          && d.DispDt.Date == date.Date
                                          && d.Re12 == false)
                                 .Select(d => d.MagName)
                                 .Distinct()
                                 .ToListAsync();
        }

        public async Task<List<DispatchTransaction>> Getindentnodetails(DateTime dispatchdate)
        {

            return await _context.DispatchTransaction
                                 .Where(d => d.DispDt.Date == dispatchdate.Date).ToListAsync();
        }

        public async Task<List<string>> Gettruckdetails(string IndentNo)
        {
            return await _context.DispatchTransaction
                                 .Where(d => d.IndentNo == IndentNo)  // Compare Date without time part
                                 .Select(d => d.TruckNo)   // Select only the IndentNo field
                                 .Distinct()                // Avoid duplicate values
                                 .ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<DispatchTransaction, bool>> predicate)
        {
            return await _context.DispatchTransaction.AnyAsync(predicate);
        }


        public async Task<string> GetSizemagIndentNo(string indentNo, string brandName, string brandId, string psize, DateTime mfgdt)
        {
            try
            {
                var pSizesWithCodes = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.Brand == brandName && d.Bid == brandId && d.PSize == psize && d.DispDt == mfgdt && d.Re12 == true)
                    .Select(d => d.PSizeCode)
                    .Distinct()
                    .FirstOrDefaultAsync();

                return pSizesWithCodes ?? string.Empty; // Return empty string if null
            }
            catch (Exception ex)
            {
                // Handle error logging
                return string.Empty; // Return an empty string on error
            }
        }

        public async Task<List<string>> Getdispatchdata(string indentNo, string brandName, string brandId, string pSize, string pSizeCode, DateTime date)
        {
            return await _context.DispatchTransaction
                                 .Where(d => d.IndentNo == indentNo
                                          && d.Brand == brandName
                                          && d.Bid == brandId
                                          && d.PSize == pSize
                                          && d.PSizeCode == pSizeCode
                                          && d.DispDt.Date == date.Date
                                          && d.Re12 == true)
                                 .Select(d => d.MagName)
                                 .Distinct()
                                 .ToListAsync();
        }
        public async Task<string> GetBrandIdnpsize(string indentNo, string brandName, DateTime mfgdt)
        {
            try
            {
                var brandId = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.DispDt == mfgdt && d.Brand == brandName && d.Re12 == true)
                    .Select(d => d.Bid)
                    .FirstOrDefaultAsync();

                return brandId ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Handle error logging
                return string.Empty;
            }
        }

        public async Task<List<string>> GetProductsizedata(string indentNo, string brandName, string brandId, DateTime mfgdt)
        {
            try
            {
                var pSizes = await _context.DispatchTransaction
                    .Where(d => d.IndentNo == indentNo && d.Brand == brandName && d.Bid == brandId && d.DispDt == mfgdt && d.Re12 == true)
                    .Select(d => d.PSize)
                    .Distinct()
                    .ToListAsync();

                return pSizes;
            }
            catch (Exception ex)
            {
                // Handle error logging
                return new List<string>();
            }
        }

        public async Task<string> GetTruckNIndent(string indentNo, DateTime mfgdt)
        {
            try
            {
                var transaction = await _context.DispatchTransaction
                .FirstOrDefaultAsync(d => d.IndentNo == indentNo && d.DispDt == mfgdt && d.Re12 == true);

                return transaction?.TruckNo ?? string.Empty;
            }
            catch (Exception ex)
            {

                string s = ex.Message.ToString();
                return string.Empty;
            }

        }

        public async Task<List<string>> GetBrandsBIndNo(string indentNo, string truckno, DateTime mfgdt)
        {
            return await _context.DispatchTransaction
                .Where(d => d.IndentNo == indentNo && d.DispDt == mfgdt && d.Re12 == true)
                .Select(d => d.Brand)
                .Distinct()
                .ToListAsync();
        }


        public async Task<List<string>> GetTruckNo(string dispatchdate, string IndentNo)
        {
            // Try to parse dispatchdate string to DateTime
            DateTime parsedDate;

            // If parsing fails, return an empty list or handle as needed
            if (!DateTime.TryParse(dispatchdate, out parsedDate))
            {
                return new List<string>(); // or handle the error as needed
            }

            return await _context.DispatchTransaction
                                 .Where(d => d.DispDt.Date == parsedDate.Date && d.IndentNo == IndentNo)  // Compare Date without time part
                                 .Select(d => d.TruckNo)   // Select only the IndentNo field
                                 .Distinct()                // Avoid duplicate values
                                 .ToListAsync();
        }


        public async Task<List<string>> GetBrandName(string dispatchdate, string IndentNo, string trucknos)
        {
            // Try to parse dispatchdate string to DateTime
            DateTime parsedDate;

            // If parsing fails, return an empty list or handle as needed
            if (!DateTime.TryParse(dispatchdate, out parsedDate))
            {
                return new List<string>(); // or handle the error as needed
            }

            return await _context.DispatchTransaction
                                 .Where(d => d.DispDt.Date == parsedDate.Date && d.IndentNo == IndentNo && d.TruckNo == trucknos)  // Compare Date without time part
                                 .Select(d => d.Brand)   // Select only the IndentNo field
                                 .Distinct()                // Avoid duplicate values
                                 .ToListAsync();
        }

        public async Task<List<string>> GetMagName(string dispatchdate, string IndentNo, string trucknos)
        {
            // Try to parse dispatchdate string to DateTime
            DateTime parsedDate;

            // If parsing fails, return an empty list or handle as needed
            if (!DateTime.TryParse(dispatchdate, out parsedDate))
            {
                return new List<string>(); // or handle the error as needed
            }

            return await _context.DispatchTransaction
                                 .Where(d => d.DispDt.Date == parsedDate.Date && d.IndentNo == IndentNo && d.TruckNo == trucknos)  // Compare Date without time part
                                 .Select(d => d.MagName)   // Select only the IndentNo field
                                 .Distinct()                // Avoid duplicate values
                                 .ToListAsync();
        }

        public async Task<List<DispatchReport>> GetDispatchDataAsync(string fromDate, string toDate, string reportType, string? magzine, string? indentno, string? customer)
        {
            DateTime fromDateTime = DateTime.Parse(fromDate);
            DateTime toDateTime = DateTime.Parse(toDate);

            List<DispatchReport> plantData = new List<DispatchReport>();

            if (reportType == "Detailed")
            {
                plantData = await (from dt in _context.DispatchTransaction
                                   join cm in _context.Re11IndentInfo on dt.IndentNo equals cm.IndentNo
                                   where dt.DispDt >= fromDateTime && dt.DispDt <= toDateTime &&
                                         (string.IsNullOrEmpty(magzine) || dt.MagName == magzine) &&
                                         (string.IsNullOrEmpty(indentno) || dt.IndentNo == indentno) &&
                                         (string.IsNullOrEmpty(customer) || cm.CustName == customer)
                                   orderby dt.MagName, dt.IndentNo
                                   select new DispatchReport
                                   {
                                       re11indentno = dt.IndentNo,
                                       dispatchDt = dt.DispDt.Date,
                                       truck = dt.TruckNo,
                                       L1Barcode = dt.L1Barcode,
                                       brandname = dt.Brand,
                                       productsize = dt.PSize,
                                       magname = dt.MagName,
                                       netqty = dt.L1NetWt.ToString(),
                                       unit = dt.L1NetUnit
                                   }).OrderBy(x => x.L1Barcode).ThenBy(x => x.brandname)
                                     .ToListAsync();
            }
            else
            {
                plantData = await _context.DispatchTransaction
                    .Join(_context.Re11IndentInfo,
                          dt => dt.IndentNo,
                          cm => cm.IndentNo,
                          (dt, cm) => new { dt, cm })
                    .Where(x => x.dt.DispDt >= fromDateTime && x.dt.DispDt <= toDateTime &&
                                (string.IsNullOrEmpty(magzine) || x.dt.MagName == magzine) &&
                                (string.IsNullOrEmpty(indentno) || x.dt.IndentNo == indentno) &&
                                (string.IsNullOrEmpty(customer) || x.cm.CustName == customer))
                    .GroupBy(x => new
                    {
                        x.dt.IndentNo,
                        x.dt.TruckNo,
                        x.dt.Brand,
                        x.dt.PSize,
                        x.dt.MagName,
                        x.dt.L1NetUnit,
                        x.dt.DispDt
                    })
                    .Select(g => new DispatchReport
                    {
                        re11indentno = g.Key.IndentNo,
                        dispatchDt = g.Key.DispDt,
                        truck = g.Key.TruckNo,
                        brandname = g.Key.Brand,
                        productsize = g.Key.PSize,
                        magname = g.Key.MagName,
                        L1Barcode = g.Select(x => x.dt.L1Barcode).Distinct().Count().ToString(),
                        netqty = g.Sum(x => x.dt.L1NetWt).ToString(),
                        unit = g.Key.L1NetUnit
                    })
                    .OrderBy(x => x.magname)
                    .ThenBy(x => x.re11indentno)
                    .ToListAsync();
            }

            return plantData;
        }

        public async Task<List<DispatchTransaction>> GetDispatchStatusAsync(DateTime dispatchDate, string? indentNo, string? truckNo)
        {
            //var query = _context.DispatchTransactions.AsQueryable();

            var query = _context.DispatchTransaction.Where(x =>
                 (dispatchDate == default || x.DispDt == dispatchDate) &&
                 (string.IsNullOrEmpty(indentNo) || x.IndentNo == indentNo) &&
                 (string.IsNullOrEmpty(truckNo) || x.TruckNo == truckNo));

            return await query
                .Select(x => new DispatchTransaction
                {
                    Tid = x.Tid,
                    IndentNo = x.IndentNo,
                    L1Barcode = x.L1Barcode,
                    Brand = x.Brand,
                    PSize = x.PSize,
                    TruckNo = x.TruckNo,
                    MagName = x.MagName,
                    DispDt = x.DispDt,
                    Re12 = x.Re12,
                    L1NetWt = x.L1NetWt,
                    L1NetUnit = x.L1NetUnit
                })
                .OrderBy(x => x.Tid)
                .ThenBy(x => x.L1Barcode)
                .ThenBy(x => x.Brand)
                .ToListAsync();
        }

        public async Task RemoveByPidsAsync(string indentNo)
        {
            var toDelete = _context.DispatchTransaction
                .Where(x => x.IndentNo == indentNo);

            _context.DispatchTransaction.RemoveRange(toDelete);
            await _context.SaveChangesAsync();
        }
    }
}



