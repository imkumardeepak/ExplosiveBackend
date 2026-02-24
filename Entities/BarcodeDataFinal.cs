using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	[Keyless]
	public class BarcodeDataFinal
	{
		[Required]
		[MaxLength(50)]
		public string L1 { get; set; }

		[Required]
		[MaxLength(50)]
		public string L2 { get; set; }

		[Required]
		[MaxLength(50)]
		public string L3 { get; set; }

		[Required]
		[MaxLength(255)]
		public string? BrandName { get; set; }

		[Required]
		[StringLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string? BrandId { get; set; }

		[Required]
		[MaxLength(100)]
		public string? PlantName { get; set; }

		[Required]
		[StringLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string? PlantCode { get; set; }

		[Required]
		[MaxLength(100)]
		public string? ProductSize { get; set; }

		[Required]
		[StringLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string? SizeCode { get; set; }

		[Required]
		[Column(TypeName = "date")]
		public DateTime? MfgDt { get; set; }

		[Required]
		[StringLength(1)]
		[Column(TypeName = "varchar(1)")]
		public string? Shift { get; set; }

		[MaxLength(50)]
		public string? Batch { get; set; }

		[Required]
		[DefaultValue(0)]
		public int Re2 { get; set; } = 0;

		[Required]
		[DefaultValue(0)]
		public int Re12 { get; set; } = 0;
	}
}
