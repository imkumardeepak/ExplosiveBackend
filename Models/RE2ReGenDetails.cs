using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
    [NotMapped]
    public class RE2ReGenDetails
    {
        [Required]
        public DateTime? mfgdt { get; set; }

        [Required]
        public string PlantName { get; set; }

        [NotMapped]
        public List<SelectListItem> plist { get; set; } = new List<SelectListItem>();

        [Required]
        public string plantcode { get; set; }
        [Required]
        public string brandname { get; set; }
        [Required]
        public string brandid { get; set; }
        [Required]
        public string productsize { get; set; }
        [Required]
        public string psizecode { get; set; }

        [Required]
        public string magname { get; set; }

        [Required]
        public string maglicense { get; set; }

        [Required]
        public string? fromcase { get; set; }

        [Required]
        public string? tocase { get; set; }
        [NotMapped]
        public List<SelectListItem> mlist { get; set; } = new List<SelectListItem>();

        // List of small models with checkbox values
        [Required]
        public List<L1L22> L1L22 { get; set; }

        public RE2ReGenDetails()
        {
            L1L22 = new List<L1L22>();

        }
    }

    public class L1L22
    {
        public bool IsChecked { get; set; } // Checkbox value
        public string l1barcode { get; set; }

        public double l1netwt { get; set; }

        public string unit { get; set; }

        public string magname { get; set; }
    }
}
