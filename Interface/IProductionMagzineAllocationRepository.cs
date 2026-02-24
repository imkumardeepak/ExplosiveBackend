using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IProductionMagzineAllocationRepository : IGenericRepository<ProductionMagzineAllocation>
	{
		Task<List<ProductionMagzineAllocation>> GetAllProductionMagzineAllocationsAsync(int flag, DateTime? fromDate = null, DateTime? toDate = null);
		Task<int> MaxCountAsync(DateTime now);
		Task<List<ProductionMagzineAllocation>> GetScannedDataByTransferIdAsync(string transferId);
		Task UpdateReadFlagAsync(int id, int readFlag);
	}
}
