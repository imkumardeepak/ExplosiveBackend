using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
	[NotMapped]
	public class RE2GenData
	{

		public DateTime? MfgDt { get; set; }


		public string? PlantName { get; set; }

		[NotMapped]
		public List<ProductMaster>? plist { get; set; } = new List<ProductMaster>();


		public string? PlantCode { get; set; }

		public string? BrandName { get; set; }

		public string? BrandId { get; set; }

		public string? ProductSize { get; set; }

		public string? PSizeCode { get; set; }

		public string? Magname { get; set; }


		public string? MagLicense { get; set; }

		[NotMapped]
		public List<MagzineMaster>? mlist { get; set; } = new List<MagzineMaster>();

		// List of small models with checkbox values
		[Required]
		public List<L1L2>? L1L2 { get; set; }

		public ProductionMagzineAllocation? ProductionMagzineAllocation { get; set; }

	}
	public class L1L2
	{
		public string? L1barcode { get; set; }

		public double L1NetWt { get; set; }

		public string? Unit { get; set; }

		public string? MagName { get; set; }
	}
}


