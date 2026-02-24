using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
   
    public class UomMasterRepository : GenericRepository<UomMaster>, IUomMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public UomMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }
    }
}


