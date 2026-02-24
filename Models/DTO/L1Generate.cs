using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	[NotMapped]
	[Keyless]
	public class L1Generate
	{
		[Required]
		[DisplayName("Country")]
		public string Country { get; set; }

		[Required]
		[StringLength(2)]
		[DisplayName("Code")]
		public string CountryCode { get; set; }

		[Required]
		[DisplayName("MFG Name")]
		public string MfgName { get; set; }

		[Required]
		[DisplayName("MFG Loc")]
		public string MfgLoc { get; set; }

		[Required]
		[StringLength(3)]
		[DisplayName("MFG Code")]
		public string MfgCode { get; set; }

		[Required]
		[DisplayName("Plant Name")]
		public string PlantName { get; set; }

		[NotMapped]
		public List<SelectListItem> plist { get; set; } = new List<SelectListItem>();


		[Required]
		[StringLength(2)]
		[DisplayName("Plant Code")]
		public string PCode { get; set; }

		[Required]
		[StringLength(1)]
		[DisplayName("Machine Code")]
		public string MCode { get; set; }


		[Required]
		[StringLength(1)]
		[DisplayName("Shift")]
		public string Shift { get; set; }

		[NotMapped]
		public List<SelectListItem> shiftlist { get; set; } = new List<SelectListItem>();


		[Required]
		[DisplayName("Brand Name")]
		public string BrandName { get; set; }

		[Required]
		[StringLength(4)]
		[DisplayName("Brand Id")]
		public string BrandId { get; set; }

		[Required]
		[DisplayName("Product Size")]
		public string ProductSize { get; set; }

		[Required]
		[StringLength(3)]
		[DisplayName("Product Code")]
		public string PSizeCode { get; set; }

		[Required]
		[DisplayName("Class")]
		public string Class { get; set; }

		[Required]
		[DisplayName("Division")]
		public string Division { get; set; }

		[Required]
		[DisplayName("SDCAT")]
		public string SdCat { get; set; }

		[Required]
		[DisplayName("UnNoClass")]
		public string UnNoClass { get; set; }

		[Required]
		[DisplayName("MFG Date")]
		public DateTime MfgDt { get; set; }

		[Required]
		[DisplayName("L1NetWt")]
		public double L1NetWt { get; set; }

		[Required]
		[DisplayName("L1NetUnit")]
		public string L1NetUnit { get; set; }

		[Required]
		[DisplayName("No. of L2-Intermediates")]
		public int NoOfL2 { get; set; }

		[Required]
		[DisplayName("No. of L3-units per L2-Intermediates")]
		public int NoOfL3perL2 { get; set; }

		[Required]
		[DisplayName("No. of L3-units per L1-Box")]
		public int NoOfL3 { get; set; }

		[Required]
		[DisplayName("No. of Boxes")]
		public int NoOfbox { get; set; }

		[Required]
		[DisplayName("No. of Stickers")]
		public int NoOfstickers { get; set; }
	}
}
