using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{

    public class IntimationMasterRepository : GenericRepository<IntimationMaster>, IIntimationMasterRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _context1;
        private readonly ApplicationDbContext _context2;
        private readonly ApplicationDbContext _context3;
        private readonly ApplicationDbContext _context4;
        private readonly ApplicationDbContext _context5;
        private readonly ApplicationDbContext _context6;
        private readonly ApplicationDbContext _context7;

        public IntimationMasterRepository(ApplicationDbContext context, ApplicationDbContext context1, ApplicationDbContext context2, ApplicationDbContext context3, ApplicationDbContext context4, ApplicationDbContext context5, ApplicationDbContext context6, ApplicationDbContext context7) : base(context, context1, context2, context3, context4, context5, context6, context7)
        {
            _context = context;
            _context1 = context1;
            _context2 = context2;
            _context3 = context3;
            _context4 = context4;
            _context5 = context5;
            _context6 = context6;
            _context7 = context7;

        }


    }

}
