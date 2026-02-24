using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using System.Threading.Channels;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class BarcodeDataFinalRepository : GenericRepository<BarcodeDataFinal>, IBarcodeDataFinalRepository
    {
        private readonly ApplicationDbContext _context;
        public BarcodeDataFinalRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }
    }
}



