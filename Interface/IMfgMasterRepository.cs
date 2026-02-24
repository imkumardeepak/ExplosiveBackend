using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IMfgMasterRepository :IGenericRepository<MfgMaster>
    {
        // Method to get code by mfgname
        Task<string> GetCodeByMfgNameAsync(string mfgName);
    }
}
