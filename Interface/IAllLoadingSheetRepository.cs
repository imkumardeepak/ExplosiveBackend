using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IAllLoadingSheetRepository : IGenericRepository<AllLoadingSheet>
	{
		Task<List<AllLoadingSheet>> GetLoadingSheet();
		Task<AllLoadingSheet> GetExistingLoadingSheetAsync(string indentNo, string brandId, string sizeCode, int compFlag);
		Task<List<AllLoadingSheet>> GetLoadingByCflag(int compFlag, DateTime? fromDate = null, DateTime? toDate = null);

		Task<AllLoadingSheet> GetLoadingByLoadingSheet(string loadingSheet);

		Task<int> GetLastCOUNT(int months, int year);

		Task UpdateCompFlagAsync(string loadingSheet,string indentno, string truckno, int compflag);
	}
}
