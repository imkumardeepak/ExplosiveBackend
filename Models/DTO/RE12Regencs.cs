using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Based_Barcode_Printing_System_APIBased.Models.DTO
{
    public class RE12Regencs
    {
        [Required]
        public DateTime? MfgDt { get; set; }

        [Required]
        public string IndentNo { get; set; }

        [NotMapped]
        public List<SelectListItem> Indentlist { get; set; } = new List<SelectListItem>();

        [Required]
        public string TruckNo { get; set; }

        [Required]
        public string BrandName { get; set; }

        [NotMapped]
        public List<SelectListItem>? Bnamelist { get; set; } = new List<SelectListItem>();

        [Required]
        public string BrandId { get; set; }

        [Required]
        public string ProductSize { get; set; }

        [NotMapped]
        public List<SelectListItem>? Psizelist { get; set; } = new List<SelectListItem>();

        [Required]
        public string PSizeCode { get; set; }

        [Required]
        public string Magname { get; set; }

     
        public string? fromcase { get; set; }

       
        public string? tocase { get; set; }
        [NotMapped]
        public List<SelectListItem>? Mlist { get; set; } = new List<SelectListItem>();

        // List of small models with checkbox values
        [Required]
        public List<L11DispatchData> L11DispatchData { get; set; } = new List<L11DispatchData>();

    }

    public class L11DispatchData
    {

        [Required]
        public string L1barcode { get; set; }

        public string brand { get; set; }

        public string psize { get; set; }

        public string mag { get; set; }

        public string indentno { get; set; }

        public string truckno { get; set; }

        public DateTime dispdt { get; set; }

        public double l1NetWt { get; set; }

        public string unit { get; set; }
    }
}
