using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
    public class RE2DDAllot
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
        public List<SelectListItem> Bnamelist { get; set; } = new List<SelectListItem>();

        [Required]
        public string BrandId { get; set; }

        [Required]
        public string ProductSize { get; set; }

        [NotMapped]
        public List<SelectListItem> Psizelist { get; set; } = new List<SelectListItem>();

        [Required]
        public string PSizeCode { get; set; }

        [Required]
        public string Magname { get; set; }

        [NotMapped]
        public List<SelectListItem> Mlist { get; set; } = new List<SelectListItem>();

        // List of small models with checkbox values
        [Required]
        public List<L1Data> L1Data { get; set; } = new List<L1Data>();

        public List<L1MapData> L1MapData { get; set; } = new List<L1MapData>();
    }

    public class L1Data
    {
        public bool IsChecked { get; set; } // Checkbox value

        [Required]
        public string L1barcode { get; set; }

        public double L1NetWt { get; set; }

        public string Unit { get; set; }
    }

    public class L1MapData
    {
        [Required]
        public string L1barcode { get; set; }

        [Required]
        public string L1NetWt { get; set; }
    }

}
