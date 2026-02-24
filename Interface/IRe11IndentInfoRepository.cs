using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IRe11IndentInfoRepository : IGenericRepository<Re11IndentInfo>
	{

		Task<List<Re11IndentInfo>> GetAllIndentInfoAlongProduct();

		Task<Re11IndentInfo> GetByIndentNO(string indentNo);

		Task<List<string>> GetOnlyIndentNo();

		Task<List<Re11IndentInfo>> GetAllIndentInfoByCompflag(); // Add this method

		Task<List<string>> GetIndentsByCName(string cName);

		Task<List<Re11IndentPrdInfo>> GetProductInfoByIndentNo(string indentNo); // New method
	}
}
