using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IShiftMasterRepository : IGenericRepository<ShiftMaster>
    {
        Task<List<ShiftMaster>> GetShiftByNameAsync(string type);
    }
}
