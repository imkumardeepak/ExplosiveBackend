using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using System.Linq.Expressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
   
    public class shiftmanagementRepository : GenericRepository<shiftmanagement>, IshiftmanagementRepository
    {
        private readonly ApplicationDbContext _context;
        public shiftmanagementRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }
      


    }

}


