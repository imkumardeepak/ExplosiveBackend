using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    public class RE2StatusReport
    {
        [Key]
        [Required]
        public DateTime? from { get; set; }

        [Required]
        public DateTime? to { get; set; }

        [Required]
        public string typeofreport { get; set; }

        [Required]
        public int re2status { get; set; }

        [Required]
        public string brandname { get; set; }

        [Required]
        public string productsize { get; set; }

        [Required]
        public string magname { get; set; }

        [Required]
        public DateTime? unloadDt { get; set; }

        [Required]
        public string l1barcode { get; set; }


        [Required]
        public string currentdt { get; set; }

        [Required]
        public string month { get; set; }

        [Required]
        public string year { get; set; }
    }
}
