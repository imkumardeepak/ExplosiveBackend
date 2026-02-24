using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IPlantTypeMasterRepository : IGenericRepository<PlantTypeMaster>
    {

        Task<List<PlantTypeMaster>> GetByPlantTypeCode(string type);
    }
}
