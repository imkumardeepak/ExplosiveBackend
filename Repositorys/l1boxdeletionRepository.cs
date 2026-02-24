using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using static Peso_Baseed_Barcode_Printing_System_API.Repositorys.l1boxdeletionRepository;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class l1boxdeletionRepository : GenericRepository<l1boxdeletion>, Il1boxdeletionRepository
    {
        private readonly ApplicationDbContext _context;
        public l1boxdeletionRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }
        public async Task<List<L1boxDeletionReport>> GetL1BoxDeletionDataAsync(DateTime fromDateTime, DateTime toDateTime, string reportType)
        {
            List<L1boxDeletionReport> plantData = new List<L1boxDeletionReport>();

            if (reportType == "Detailed")
            {
                plantData = _context.L1boxDeletion
                     .AsEnumerable() // switches to in-memory LINQ
                     .Where(x => x.mfgdt >= fromDateTime && x.mfgdt <= toDateTime)
                     .Select(x => new L1boxDeletionReport
                     {
                         plant = x.plant,
                         brand = x.brand,
                         productsize = x.psize,
                         mfgDt = x.mfgdt,
                         l1barcode = x.l1barcode,
                         deletionDt = x.deldt,
                         reason = x.reason,
                     })
                     .OrderBy(x => x.brand)
                     .ThenBy(x => x.l1barcode)
                     .ThenBy(x => x.plant)
                     .ToList();

                return plantData;
            }
            else
            {
                plantData = _context.L1boxDeletion
                     .AsEnumerable()
                      .Where(x => x.mfgdt >= fromDateTime && x.mfgdt <= toDateTime)
                     .GroupBy(x => new { x.plant, x.brand, x.psize })
                     .Select(g => new L1boxDeletionReport
                     {
                         plant = g.Key.plant,
                         brand = g.Key.brand,
                         productsize = g.Key.psize,
                         l1barcode = g.Select(x => x.l1barcode).Distinct().Count().ToString()
                     })
                     .OrderBy(x => x.brand)
                     .ThenBy(x => x.l1barcode)
                     .ThenBy(x => x.plant)
                     .ToList();

                return plantData;
            }
        }



    }
}


