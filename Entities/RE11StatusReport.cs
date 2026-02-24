using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    public class RE11StatusReport
    {
        [Key]
        [Required]
        public DateTime? from { get; set; }

        [Required]
        public DateTime? to { get; set; }

        [Required]
        public string customer { get; set; }

        [Required]
        public string status { get; set; }

        [Required]
        public string re11indentno { get; set; }

        [Required]
        public DateTime? indentDt { get; set; }

        [Required]
        public DateTime? pesore11Dt { get; set; }

        [Required]
        public string Ptype { get; set; }

        [Required]
        public string brand { get; set; }

        [Required]
        public string productsize { get; set; }

        [Required]
        public string reqqty { get; set; }

        [Required]
        public string requnit { get; set; }

        [Required]
        public string remqty { get; set; }

        [Required]
        public string remunit { get; set; }

        [Required]
        public string currentdt { get; set; }

        [Required]
        public string month { get; set; }

        [Required]
        public string year { get; set; }
    }
}
