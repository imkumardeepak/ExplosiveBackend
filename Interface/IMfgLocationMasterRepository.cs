using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IMfgLocationMasterRepository :IGenericRepository<MfgLocationMaster>
    {
        Task<List<string>> GetMfgLocationNamesAsync();

        Task<string> GetMfgLocCodeByNameAsync(string name);
    }
}
