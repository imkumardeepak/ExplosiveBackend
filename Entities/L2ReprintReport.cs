using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    public class L2ReprintReport
    {
        [Key]
   
        [Required]
        public DateTime? from { get; set; }

        [Required]
        public DateTime? to { get; set; }

        [Required]
        public string typeofreport { get; set; }

        [Required]
        public string plant { get; set; }

        [Required]
        public string plantcode { get; set; }

        [Required]
        public string L2Barcode { get; set; }

        [Required]
        public DateTime? reprintDt { get; set; }

        [Required]
        public string reason { get; set; }

        [Required]
        public string time { get; set; }

        [Required]
        public DateTime? currentstkdt { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        public string Year { get; set; }
    }
}
