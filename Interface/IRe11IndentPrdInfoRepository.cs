using Microsoft.AspNetCore.Mvc;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IRe11IndentPrdInfoRepository : IGenericRepository<Re11IndentPrdInfo>
	{
		Task<List<DispatchStatusDto>> GetRE11Listdetails();
		Task<List<DispatchDetailsDto>> GetDispatchdeails();
		Task<List<dynamic>> GetGroupedDispatchData(string plantName, DateTime startDate);
		Task<List<RE11StatusReport>> GetRE11StatusDataAsync(DateTime fromDate, DateTime toDate, int? status, string? indentno, string? customer);


        Task<Re11IndentPrdInfo> GetByIndentNO(string indentNo);
	}
}
