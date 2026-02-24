using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IBatchMasterRepository : IGenericRepository<BatchMasters>
	{
		Task<BatchMasters> GetBatchMasterByPlantCodeAsync(string plantCode);

		Task<BatchMasters> GetBatchMasterByBatchCodeAsync(string batchCode);

		Task<BatchMasters> GetBatchMasterByPlantCodeAndBatchType(string plantcode, string batchtype);
	}
}
