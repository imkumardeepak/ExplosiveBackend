using Peso_Baseed_Barcode_Printing_System_API.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using System.Data;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IBarcodeDataRepository : IGenericRepository<BarcodeData>
	{
		Task<List<L1L2>> GetBarcodeDataWithMagAsync(DateTime mfgdt, string plantcode, string brandid, string psizecode, string magname);
		Task<List<L1L2>> GetBarcodeDataWithoutMagAsync(DateTime mfgdt, string plantcode, string brandid, string psizecode);


		// New method to get L1L2L3 data based on a list of L1L2
		Task<List<L1L2L3>> GetL1L2L3DataAsync(List<L1L2> l1l2List);
		Task<List<L1L2L3>> GetL1L2L3detaAsync(List<L1L22> l1l22List);

		Task<List<BarcodeData>> GetBarcodedataByL1lIST(List<string> l1barcode);

		// New method to update RE2Flag based on L1L2
		Task UpdateRE2FlagAsync(List<L1L2> l1l2List, int newRE2FlagValue);
		Task UpdateRE2FlagdataAsync(List<L1L22> l1l22List, int newRE2FlagValue);

		Task<List<L1L22>> GetBarcodeDataWithMagdataAsync(DateTime mfgdt, string plantcode, string brandid, string psizecode, string fromcase, string tocase);
		Task<List<L1L22>> GetBarcodeDataWithoutMagdataAsync(DateTime mfgdt, string? plantcode, string? brandid, string? psizecode, string? fromcase, string? tocase);
		Task<L1L2L3> GetLatestL1L2L3DetailsAsync(string pcode, string brandid, string productsizeCode, string shift,string mcode,DateTime? date,string countryCode,string mfgcode);

		Task<List<BarcodeData>> GetRecordbyflag(int re2flag, int re12flag, DateTime mfgdt, string plantcode, string brandid, string psizecode);

		Task<DataTable> GetBatchInfo(string? brandid, string? psizecode,int reqCase);
	}
}
