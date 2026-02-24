using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class MfgLocationMasterRepository : GenericRepository<MfgLocationMaster>, IMfgLocationMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public MfgLocationMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<string>> GetMfgLocationNamesAsync()
        {
            return await _context.Set<MfgLocationMaster>()
                                 .Select(location => location.mfgloc)
                                 .ToListAsync();
        }

        public async Task<string> GetMfgLocCodeByNameAsync(string name)
        {
            return await _context.Set<MfgLocationMaster>()
                                 .Where(location => location.mfgloc == name)
                                 .Select(location => location.maincode)
                                 .FirstOrDefaultAsync();
        }
    }
}


