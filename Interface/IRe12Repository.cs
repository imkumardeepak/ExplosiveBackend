using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IRe12Repository : IGenericRepository<RE12Gen>
	{
		List<string> GetL1DataRE12(string indentno, string truckno, string mag, string bid, string psize, int flag);
		Task UpdateRe12(List<string> barcodes, string IndentNo, string brandid, string psizecode, string mag);
	}
}
