using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
    [NotMapped]
    public class SmallModel
    {
        public string? L1Barcode { get; set; }
        public double? L1NetWt { get; set; }
        public string? L1NetUnit { get; set; }

    }
}
