using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface ITruckToMAGBarcodeRepository :IGenericRepository<TruckToMAGBarcode>
    {
        Task RemoveByL1BarcodesAsync(List<string> l1Barcodes);
    }
}
