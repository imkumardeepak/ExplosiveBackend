using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
   
    public interface Ihht_prodtomagtransferRepository : IGenericRepository<hht_prodtomagtransfer>
    {
        Task<List<hht_prodtomagtransfer>> GetTransferDataAsync(DateTime fromDate, DateTime toDate, string reportType, string? pcode);
    }
}
