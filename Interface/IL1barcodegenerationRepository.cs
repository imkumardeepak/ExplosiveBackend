using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Models;
using ProductionReport = Peso_Baseed_Barcode_Printing_System_API.Entities.ProductionReport;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IL1barcodegenerationRepository : IGenericRepository<L1barcodegeneration>
	{
		Task<int> GetMaxSrNoAsync(string genYear, string quarter, string brandId, string pSizeCode);

		Task<List<SmallModel>> GetL1MagNotAllot(DateTime mfgdt, string pcode, string brandId, string pSizeCode);

		Task BulkUpdateMFlag(List<string> barcodes, int newMFlagValue);

		Task<L1Generatedata> GetAllDataAsync();
		Task<Dictionary<string, object>> Getdashdetailsdata();
		Task<List<dynamic>> GetGroupedProductionData(string plantName, DateTime startDate);
		Task<List<dynamic>> GetSlurryDataAsync(string timeRange);
		Task<List<dynamic>> GetDetonatorDataAsync(string timeRange);
		Task<List<dynamic>> GetEmultionDataAsync(string timeRange);
		Task<List<dynamic>> GetPETNDataAsync(string timeRange);
		Task<List<ProductionReport>> GetProductionReportAsync(DateTime fromDate, DateTime toDate, string reportType, string? shift, string? plant, string? brand, string? productsize);
		Task<BarcodeSearchResultDto> GetSearchDataAsync(string l1barcode);
		Task<List<L1barcodegeneration>> GetProductionStatusAsync(DateTime mfgDate, string? brand, string? brandId, string? plant, string? plantCode, string? productSize, string? pSizeCode);
		Task<List<L1barcodegeneration>> GetL1ReprintBarcodesAsync(DateTime? MfgDt, string? plant, string? plantcode, string? mode, string? Brandname, string? BrandId, string? productsize,
			string? psizecode, string? l1barcode, string? shift, string? mcode);

		Task<List<L1barcodegeneration>> GetL1DataRecordByL1Async(string l1barcode);

		Task<L1barcodegeneration> GetL1DataRecordByL1FristAndDefultAsync(string l1barcode);

		Task<HashSet<string>> GetBulkExistingL1sAsync(List<string> l1List);

	}
}
