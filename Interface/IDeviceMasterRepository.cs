using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IDeviceMasterRepository : IGenericRepository<DeviceMaster>
    {
        Task<List<string>> GetDevicesByTypeAsync(string type);
    }
}
