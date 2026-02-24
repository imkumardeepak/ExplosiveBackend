using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
    [NotMapped]
    public class RE7MonthlyReport
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public string MagName { get; set; }
        public string BrandName { get; set; }
        public string BrandId { get; set; }
        public string ProductSize { get; set; }
        public string ProductCode { get; set; }
        public int Inward { get; set; }
        public int Outward { get; set; }
        public int RemainingStock { get; set; }
    }

}
