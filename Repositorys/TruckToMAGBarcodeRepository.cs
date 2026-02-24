using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    
    public class TruckToMAGBarcodeRepository : GenericRepository<TruckToMAGBarcode>, ITruckToMAGBarcodeRepository
    {
        private readonly ApplicationDbContext _context;
        public TruckToMAGBarcodeRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task RemoveByL1BarcodesAsync(List<string> l1Barcodes)
        {
            var recordsToRemove = await _context.TruckToMAGBarcode
                .Where(t => l1Barcodes.Contains(t.Barcode)) // Filter matching barcodes
                .ToListAsync();

            if (recordsToRemove.Any())
            {
                _context.TruckToMAGBarcode.RemoveRange(recordsToRemove);
                await _context.SaveChangesAsync();
            }
        }
    }
}


