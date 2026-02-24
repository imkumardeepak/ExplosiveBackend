using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

    public class PlantTypeMasterRepository : GenericRepository<PlantTypeMaster>, IPlantTypeMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public PlantTypeMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<PlantTypeMaster>> GetByPlantTypeCode(string type)
        {
            return await _context.PlantTypeMaster  // Ensure the DbSet name matches your context
                                 .Where(d => d.plant_type == type)                           
                                 .ToListAsync();
        }

    }

}


