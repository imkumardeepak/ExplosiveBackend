using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	
		public interface IStorageMagazineReportRepository : IGenericRepository<StorageMagazineReport>
		{

			//  Task<L1barcodegeneration> GetL1BarcodeDataAsync(string fromDate, string toDate, string shift, string reportType);
			Task<List<string>> GetAllmagzine(string magname);
		}
	
}
