using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface ICustMagazineDetailsRepository : IGenericRepository<CustMagazineDetail>
    {
        Task<List<CustMagazineDetail>> GetMagazinesByCustomerIdAsync(int customerId);
    }
}
