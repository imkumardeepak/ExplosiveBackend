using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class CustMagazineDetailsRepository : GenericRepository<CustMagazineDetail>, ICustMagazineDetailsRepository
    {
        private readonly ApplicationDbContext _context;
        public CustMagazineDetailsRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<CustMagazineDetail>> GetMagazinesByCustomerIdAsync(int customerId)
        {
            // Assuming you have a Magazine entity with a relationship to CustMemberDetail
            return await _context.Set<CustMagazineDetail>()
                                 .Where(magazine => magazine.Cid == customerId)
                                 .ToListAsync();
        }
    }
}


