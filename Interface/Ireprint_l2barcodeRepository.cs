using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface Ireprint_l2barcodeRepository : IGenericRepository<reprint_l2barcode>
    {
        Task<List<L2ReprintReport>> GetL2ReprintDataAsync(DateTime fromDateTime, DateTime toDateTime, string reportType, string? plant);
    }
   
}
