using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{

    public interface ITransVehicleDetailRepository : IGenericRepository<TransVehicleDetail>
    {
        Task<TransVehicleDetail> GetVehicleByTruckNoAsync(int truckNo);

        Task<string> GetTrucknoById(int id);

        Task<string> GetlicBytruckno(string truckno);
    }

    

}
