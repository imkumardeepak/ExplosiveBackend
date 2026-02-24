using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using System;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class reprint_l2barcodeRepository : GenericRepository<reprint_l2barcode>, Ireprint_l2barcodeRepository
    {
        private readonly ApplicationDbContext _context;
        public reprint_l2barcodeRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<L2ReprintReport>> GetL2ReprintDataAsync(DateTime fromDateTime, DateTime toDateTime, string reportType, string? plant)
        {
            //var barcodeList = await GetAllAsync(); // This fetches all data; optimize as needed with filters in DB.

            if (reportType == "Detailed")
            {
                return _context.Reprint_L2barcode
                     .AsEnumerable() // Now we're working in memory, not in SQL
                     .Where(x => x.rptdt >= fromDateTime && x.rptdt <= toDateTime &&
                                 (string.IsNullOrEmpty(plant) || x.plantcode == plant))
                     .Select(x => new L2ReprintReport
                     {
                         plantcode = x.plantcode,
                         L2Barcode = x.l2barcode,
                         reprintDt = x.rptdt,
                         reason = x.reason
                     })
                     .OrderBy(x => x.plantcode)
                     .ThenBy(x => x.L2Barcode)
                     .ToList();
            }
            else
            {
                return _context.Reprint_L2barcode
                     .AsEnumerable() // Now we're working in memory, not in SQL
                    .Where(x => x.rptdt >= fromDateTime && x.rptdt <= toDateTime &&
                                (string.IsNullOrEmpty(plant) || x.plantcode == plant))
                    .GroupBy(x => x.plantcode)
                    .Select(g => new L2ReprintReport
                    {
                        plantcode = g.Key,
                        L2Barcode = g.Select(x => x.l2barcode).Distinct().Count().ToString()
                    })
                    .OrderBy(x => x.plantcode)
                    .ToList();
            }
        }

    }

}


