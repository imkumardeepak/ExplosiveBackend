using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class CustMemberDetailsRepository : GenericRepository<CustMemberDetail>, ICustMemberDetailsRepository
    {
        private readonly ApplicationDbContext _context;
        public CustMemberDetailsRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<CustMemberDetail>> GetMembersByCustomerIdAsync(int customerId)
        {
            return await _context.Set<CustMemberDetail>()
                                 .Where(member => member.Cid == customerId)
                                 .ToListAsync();
        }

        public async Task<CustMemberDetail> GetMemberContactByIdAsync(int memberId)
        {
            // Assuming CustMemberDetail has a 'Contact' property or similar field
            return await _context.Set<CustMemberDetail>()
                                 .Where(member => member.Id == memberId)
                                 .Select(member => new CustMemberDetail
                                 {
                                     // Select only the relevant fields (e.g., contact details)
                                     Id = member.Id,
                                     Name = member.Name,
                                     ContactNo = member.ContactNo  // Adjust to your property name
                                 })
                                 .FirstOrDefaultAsync();
        }
    }
}


