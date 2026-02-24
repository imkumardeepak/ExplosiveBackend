using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class canre11indent_prd_infoRepository : GenericRepository<canre11indent_prd_info>, Icanre11indent_prd_infoRepository
    {
        private readonly ApplicationDbContext _context;
        public canre11indent_prd_infoRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        //public async Task InsertRangeAsync(IEnumerable<canre11indent_prd_info> products)
        //{
        //    await _context.canre11indent_prd_info.AddRangeAsync(products);
        //    await _context.SaveChangesAsync();
        //}
    }
}


