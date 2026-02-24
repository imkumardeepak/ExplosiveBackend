using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IPlantMasterRepository : IGenericRepository<PlantMaster>
    {
        Task<List<string>> GetPlantNamesAsync();
        Task<string> GetPlantCodeByNameAsync(string plantName);
        Task<List<string>> GetPlantNameByBNameAsync(string brandName);

        Task<bool> CheckPlantCodeExistsAsync(string pcode);
        Task<string> GetPlantCodeByNamemmAsync(List<string> plantName);
    }
}
