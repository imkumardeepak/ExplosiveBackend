using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
    public class Re6BarcodeModel
    {
        [NotMapped]
        public string VehicleNumber { get; set; }
        public string VehicleLicense { get; set; }
        public string RE12 { get; set; }
        public string VehicleValue { get; set; }
        public string ConsigneeName { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string LicenseNumber { get; set; }
        public string ConsignorLicense { get; set; }
        public List<Re6ProductModel> Products { get; set; }
    }
    public class Re6ProductModel
    {
        public string ProductName { get; set; }
        public string CD { get; set; }
        public string Qty { get; set; }
        public string UOM { get; set; }
        public string Cases { get; set; }
    }
}
