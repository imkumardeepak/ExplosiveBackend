using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class ShiftMasterRepository : GenericRepository<ShiftMaster>, IShiftMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public ShiftMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<ShiftMaster>> GetShiftByNameAsync(string type)
        {
            return await _context.ShiftMaster  // Ensure the DbSet name matches your context
                                 .Where(d => d.shift == type)
                                 .ToListAsync();
        }
    }
}


