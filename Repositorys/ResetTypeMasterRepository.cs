using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
   
    public class ResetTypeMasterRepository : GenericRepository<ResetTypeMaster>, IResetTypeMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public ResetTypeMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        //public async Task<string> GetCodeByMfgNameAsync(string resettype)
        //{
        //    // Ensure case-insensitive search or apply relevant logic based on your database settings
        //    var mfgMaster = await _context.Set<ResetTypeMaster>()
        //        .FirstOrDefaultAsync(m => m.resettype == resettype);

        //    return mfgMaster?.resettype; // Assuming "Code" is the property name for the code
        //}
    }

}


