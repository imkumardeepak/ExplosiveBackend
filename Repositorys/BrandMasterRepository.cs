using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class BrandMasterRepository : GenericRepository<BrandMaster>, IBrandMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public BrandMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<string>> GetAllBrandNamesAsync()
        {
            return await _context.BrandMaster
                .Select(b => b.bname)
                .ToListAsync();
        }

        public async Task<String?> GetIdByBrandNameAsync(string brandName)
        {
            return await _context.BrandMaster
                .Where(b => b.bname == brandName)
                .Select(b => b.bid)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckBrandExistsAsync(string brandName, string brandId)
        {
            return await _context.BrandMaster.AnyAsync(b =>
                b.bname == brandName && b.bid == brandId);
        }

        public async Task<List<string>> GetBrandNamesByPCodeAsync(string pcode)
        {
            return await _context.BrandMaster
                                 .Where(b => b.plant_type == pcode)
                                 .Select(b => b.bname)
                                 .ToListAsync();
        }

        //modified by deepak 
        public async Task<List<string>> GetBrandNamesByPnameAsync(string pcode)
        {
            return await _context.BrandMaster
                                 .Where(b => b.plant_type == pcode)
                                 .Select(b => b.bname)
                                 .ToListAsync();
        }

        public async Task<List<string>> GetBrandCodebybnameAsync(string bname)
        {
            return await _context.BrandMaster
                                 .Where(b => b.bname == bname)
                                 .Select(b => b.bid)
                                 .ToListAsync();
        }

        public async Task<List<string>> GetbrandNamesAsync()
        {
            return await _context.BrandMaster
                .Select(p => p.bname)
                .ToListAsync();
        }
    }

}


