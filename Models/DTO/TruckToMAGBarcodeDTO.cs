namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
    public class TruckToMAGBarcodeDTO
    {
        public string TruckNo { get; set; }
        public string MagName { get; set; }
        public string Barcode { get; set; }
        public string? ScanFlag { get; set; }
    }

    public class TruckToMAGBarcodeViewModel
    {
        public List<TruckToMAGBarcodeDTO> TruckToMAGBarcodes { get; set; } = new List<TruckToMAGBarcodeDTO>();
    }
}
