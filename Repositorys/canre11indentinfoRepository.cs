using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class canre11indentinfoRepository : GenericRepository<canre11indentinfo>, Icanre11indentinfoRepository
    {
        private readonly ApplicationDbContext _context;
        public canre11indentinfoRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }
        //public async Task<canre11indentinfo?> GetcancelByIndentNo(string indentNo)
        //{
        //    return await _context.canre11indentinfo
        //                         .Where(p => p.indentno == indentNo && p.compflag == "0")
        //                         .FirstOrDefaultAsync();
        //}


    }
}


