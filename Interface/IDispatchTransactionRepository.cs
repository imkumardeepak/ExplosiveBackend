using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using System.Linq.Expressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IDispatchTransactionRepository : IGenericRepository<DispatchTransaction>
	{
		Task<List<SelectListItem>> GetIndentNoForMagAllotAsync();


		Task<DispatchTransaction> DispatchTransactionByL1Barcode(string L1Barcode);

		Task<string> GetTruckNoByIndentNo(string Indentno);

		Task<List<DispatchTransaction>> SaveDispatchTransactionBulk(List<DispatchTransaction> dispatchTransactions);


		Task<string> GetTruckNoByIndentNo(string Indentno, DateTime mfgdt);

		Task<List<string>> GetBrandsByIndentNo(string Indentno, string truckno);

		Task<List<string>> GetBrandsByIndentNo(string Indentno, string truckno, DateTime mfgdt);

		Task<string> GetBrandIdByIndentNoAndBrandName(string indentNo, string brandName);

		Task<string> GetBrandIdByIndentNoAndBrandName(string indentNo, string brandName, DateTime mfgdt);

		Task<List<string>> GetPSizesByIndentNoBrandNameAndBrandId(string indentNo, string brandName, string brandId);

		Task<List<string>> GetPSizesByIndentNoBrandNameAndBrandId(string indentNo, string brandName, string brandId, DateTime mfgdt);

		Task<string> GetSizeCodesByIndentNo(string indentNo, string brandName, string brandId, string psize);

		Task<string> GetSizeCodesByIndentNo(string indentNo, string brandName, string brandId, string psize, DateTime mfgdt);

		Task<List<L1Data>> GetL1DataWithoutMagName(string indentNo, string truckno, string brandId, string psize);


		Task BulkUpdateMag(List<string> barcodes, string NewMag);

		Task<List<string>> GetIndentsRE12();

		Task<List<string>> GetMagazinesforDispatch(string indentNo, string brandName, string brandId, string pSize, string pSizeCode, DateTime date);

		Task<List<DispatchTransaction>> Getindentnodetails(DateTime dispatchdate);
		Task<List<string>> Gettruckdetails(string IndentNo);
		Task<bool> AnyAsync(Expression<Func<DispatchTransaction, bool>> predicate);

		Task<List<string>> GetTruckNo(string dispatchdate, string truckno);

		Task<List<string>> GetBrandName(string dispatchdate, string truckno, string trucknos);

		Task<List<string>> GetMagName(string dispatchdate, string IndentNo, string trucknos);

		Task<List<L11DispatchData>> GetDataRE12Details(string indentNo, string truckno, string brandId, string psize, string magname, DateTime mfgdt, string fromcase, string tocase);

		Task<string> GetSizemagIndentNo(string indentNo, string brandName, string brandId, string psize, DateTime mfgdt);
		Task<List<string>> Getdispatchdata(string indentNo, string brandName, string brandId, string pSize, string pSizeCode, DateTime date);
		Task<string> GetBrandIdnpsize(string indentNo, string brandName, DateTime mfgdt);
		Task<List<string>> GetProductsizedata(string indentNo, string brandName, string brandId, DateTime mfgdt);
		Task<string> GetTruckNIndent(string indentNo, DateTime mfgdt);
		Task<List<string>> GetBrandsBIndNo(string indentNo, string truckno, DateTime mfgdt);

		Task<List<DispatchReport>> GetDispatchDataAsync(string fromDate, string toDate, string reportType, string? magzine, string? indentno, string? customer);
		Task<List<DispatchTransaction>> GetDispatchStatusAsync(DateTime dispatchDate, string? indentNo, string? truckNo);
		Task RemoveByPidsAsync(string indentNo);
	}

}
