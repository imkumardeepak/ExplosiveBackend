using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IL2barcodegenerationRepository : IGenericRepository<L2barcodegeneration>
	{
		Task<L2barcodegeneration> GetL2BarcodesAsync(string l2barcode);
		Task<int> GetMaxSrNoAsync(string pcode, string mcode);
		Task<List<L2barcodegeneration>> GetReprintBarcodesAsync(DateTime mgfdate, string plant, string pcode, string mcode, int fromsrno, int tosrno, string l2barcode);
	}
}
