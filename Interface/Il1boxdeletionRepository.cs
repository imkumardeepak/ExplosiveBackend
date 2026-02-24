using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface Il1boxdeletionRepository : IGenericRepository<l1boxdeletion>
    {
        Task<List<L1boxDeletionReport>> GetL1BoxDeletionDataAsync(DateTime fromDateTime, DateTime toDateTime, string reportType);
    }
    
}
