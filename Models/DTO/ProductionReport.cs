using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
    public class ProductionReport
    {
        [Key]
        public int id { get; set; }

        [Required]
        [DisplayName("From")]
        public DateTime? from { get; set; }

        [Required]
        [DisplayName("To")]
        public DateTime? to { get; set; }


        [Required]
        [DisplayName("Shift")]
        public string shift { get; set; }

        [Required]
        [DisplayName("Plant Name")]
        public string plantname { get; set; }

        [Required]
        [DisplayName("Brand Name")]
        public string brandname { get; set; }

        [Required]
        [DisplayName("Product Size")]
        public string productsize { get; set; }

        [Required]
        [DisplayName("RE2")]
        public string re2 { get; set; }

        [Required]
        [DisplayName("Type of Report")]
        public string typeofreport { get; set; }

        [Required]
        [DisplayName("L1 Barcode")]
        public string l1barcode { get; set; }
        [Required]
        [DisplayName("L1 Net Qty.")]
        public string l1netqty { get; set; }

        [Required]
        [DisplayName("L1 Net Unit")]
        public string l1netunit { get; set; }

        [Required]
        [DisplayName("Box Counts")]
        public string boxcount { get; set; }

        [Required]
        [DisplayName("Qty.")]
        public string Qty { get; set; }

        [Required]
        [DisplayName("Unit")]
        public string unit { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime? currentdate { get; set; }
        [Required]
        public string time { get; set; }

        [Required]
        public string month { get; set; }
        [Required]
        public string year { get; set; }
    }
}
