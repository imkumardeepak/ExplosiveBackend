using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IBrandMasterRepository :IGenericRepository<BrandMaster>
    {
        Task<List<string>> GetAllBrandNamesAsync(); // Fetch all brand names
        Task<string?> GetIdByBrandNameAsync(string brandName); // Get ID by brand name
		Task<bool> CheckBrandExistsAsync(string brandName, string brandId); //check brand Exists

        Task<List<string>> GetBrandNamesByPCodeAsync(string pcode);

        Task<List<string>> GetBrandNamesByPnameAsync(string pcode);

        Task<List<string>> GetBrandCodebybnameAsync(string bname);

        Task<List<string>> GetbrandNamesAsync();
    }
}
