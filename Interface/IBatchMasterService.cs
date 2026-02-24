using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IBatchMasterService
	{
		Task<IEnumerable<BatchMasters>> GetAllBatchMastersAsync();
		Task<BatchMasters> GetBatchMasterByIdAsync(int id);
		Task<BatchMasters> GetBatchMasterByPlantCodeAsync(string plantCode);
		Task<APIResponse> CreateBatchMasterAsync(BatchMasters batchMasterDTO);
		Task<APIResponse> UpdateBatchMasterAsync(BatchMasters batchMasterDTO);
		Task<APIResponse> DeleteBatchMasterAsync(int id);
	}
}
