using Peso_Baseed_Barcode_Printing_System_API.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IMagzine_StockRepository : IGenericRepository<Magzine_Stock>
	{
		Task UpdateRE2FlagAsync(List<L1L2> l1l2List, int newRE2FlagValue);
		Task UpdateRE2FlagdataAsync(List<L1L22> l1l2List, int newRE2FlagValue);
		Task<List<string>> GetExistingBarcodesAsync(List<string> barcodes);

		Task<List<Magzine_Stock>> SaveMagzineTransactionBulk(List<Magzine_Stock> magzine_Stocks);

		Task<List<Magzine_Stock>> GetL1DetailsAsync(List<string> barcodes);

		Task<List<MagazineStockSummary>> GetMagstockdetails();
		Task<List<MagazineStockSummary>> GetMagRE2listdetails();
		Task<List<int>> GetMagallotmentdetails();
		Task<List<StorageMagazineReport>> GetStorageStockReportAsync(string? fromDate, string? toDate, string reportType, string? magazine, string? brand, string? productSize);
		Task<List<RE2StatusReport>> GetRE2StatusDataAsync(DateTime fromDate, DateTime toDate, string reportType, int? re2status, string? brand, string? productsize);
		Task<List<MagStock>> GetMagNameStatusAsync(string? magname);

		Task<Magzine_Stock> GetStockByMCodeAsync(string? magname);

		Task<List<MagRE2DetailsDto>> GetMagRE2details();
		Task<List<Magzine_Stock>> GetRecordbyflag(int re2flag, int re12flag, string magname, string brandid, string psizecode);

		Task<List<RE7MonthlyReport>> GetRE7Report(DateTime? fromDate, DateTime? toDate, string? magname);

    }
}
