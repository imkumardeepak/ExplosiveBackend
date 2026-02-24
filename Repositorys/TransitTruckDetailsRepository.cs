using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

using Microsoft.EntityFrameworkCore;
namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
  
    public class TransitTruckDetailsRepository : GenericRepository<TransitTruckDetails>, ITransitTruckDetailsRepository
    {
        private readonly ApplicationDbContext _context;
        public TransitTruckDetailsRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<TransitTruckDetails> GetVehicleByTruckNoAsync(int truckNo)
        {
            return await _context.Set<TransitTruckDetails>()
                                 .Where(tv => tv.Id == truckNo)
                                 .FirstOrDefaultAsync();
        }
        public async Task<string> GetTrucknoById(int id)
        {
            var loadingSheet = await _context.TransitTruckDetails
                .Where(ls => ls.Id == id)
                .Select(ls => ls.VehicleNo)
                .FirstOrDefaultAsync();

            return loadingSheet ?? "Not Found";
        }

        public async Task<string> GetlicBytruckno(string truckno)
        {
            var loadingSheet = await _context.TransitTruckDetails
                .Where(ls => ls.VehicleNo == truckno)
                .Select(ls => ls.License)
                .FirstOrDefaultAsync();

            return loadingSheet ?? "Not Found";
        }

    }
}



