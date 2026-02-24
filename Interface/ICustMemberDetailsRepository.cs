using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface ICustMemberDetailsRepository : IGenericRepository<CustMemberDetail>
    {
        Task<List<CustMemberDetail>> GetMembersByCustomerIdAsync(int customerId);
        Task<CustMemberDetail> GetMemberContactByIdAsync(int memberId);
    }
}
