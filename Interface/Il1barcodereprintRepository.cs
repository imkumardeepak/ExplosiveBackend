using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface Il1barcodereprintRepository : IGenericRepository<l1barcodereprint>
    {
        Task<List<L1ReprintReport>> GetL1ReprintDataAsync(DateTime fromDateTime, DateTime toDateTime, string reportType, string? plant);
    }
}
