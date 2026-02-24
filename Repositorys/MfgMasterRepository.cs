using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

    public class MfgMasterRepository : GenericRepository<MfgMaster>, IMfgMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public MfgMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<string> GetCodeByMfgNameAsync(string mfgName)
        {
            // Ensure case-insensitive search or apply relevant logic based on your database settings
            var mfgMaster = await _context.Set<MfgMaster>()
                .FirstOrDefaultAsync(m => m.mfgname == mfgName);

            return mfgMaster?.code; // Assuming "Code" is the property name for the code
        }
    }

}


