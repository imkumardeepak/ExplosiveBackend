using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IL3barcodegenerationRepository : IGenericRepository<L3barcodegeneration>
	{
		Task<L3barcodegeneration> GetL3BarcodeAsync(string l3Barcode);
		Task<int> GetMaxSrNoAsync(string pcode, string mcode);
		Task<List<L3barcodegeneration>> GetL3BarcodesAsync(int? from, int? to, string? plant, string? plantcode, string? mcode);
	}
}
