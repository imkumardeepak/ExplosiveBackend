using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

    public class DeviceMasterRepository : GenericRepository<DeviceMaster>, IDeviceMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public DeviceMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<List<string>> GetDevicesByTypeAsync(string type)
        {
            return await _context.DeviceMaster  // Ensure the DbSet name matches your context
                                 .Where(d => d.TypeOfDevice == type)
                                 .Select(e => e.DeviceNo)
                                 .ToListAsync();
        }
    }
}


