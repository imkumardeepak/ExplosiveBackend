using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.AspNetCore.Mvc;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
   

    public class RE6GenerationRepository : GenericRepository<RE6Generation>, IRE6GenerationRepository
    {
        private readonly ApplicationDbContext _context;
        public RE6GenerationRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

      


    }
}


