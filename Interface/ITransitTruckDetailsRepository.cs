using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    
    public interface ITransitTruckDetailsRepository : IGenericRepository<TransitTruckDetails>
    {
        Task<TransitTruckDetails> GetVehicleByTruckNoAsync(int truckNo);

        Task<string> GetTrucknoById(int id);

        Task<string> GetlicBytruckno(string truckno);
    }
}
