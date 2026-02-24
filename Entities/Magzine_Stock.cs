using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	/// <summary>
	/// Magazine stock record - tracks L1 barcodes stored in magazines.
	/// NORMALIZED: Brand/Product info comes from L1 via ParentL1 navigation.
	/// Month/Year derived from StkDt.
	/// </summary>
	public class Magzine_Stock
	{
		/// <summary>
		/// Primary key - also serves as FK to L1Barcodegeneration
		/// </summary>
		[Key]
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string L1Barcode { get; set; }

		/// <summary>
		/// Reference to MagzineMaster.mcode
		/// </summary>
		[Required]
		[MaxLength(50)]
		public string MagName { get; set; }

		[Required]
		[MaxLength(255)]
		public string BrandName { get; set; }

		[Required]
		[StringLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string BrandId { get; set; }

		[Required]
		[MaxLength(100)]
		public string PrdSize { get; set; }

		[Required]
		[StringLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string PSizeCode { get; set; }

		[Required]
		public int Stock { get; set; } = 0;

		[Required]
		[Column(TypeName = "date")]
		public DateTime? StkDt { get; set; }

		/// <summary>
		/// RE-2 form generation flag. True = generated
		/// </summary>
		[Required]
		public bool Re2 { get; set; } = false;

		/// <summary>
		/// RE-12 form generation flag. True = generated
		/// </summary>
		[Required]
		public bool Re12 { get; set; } = false;

		/// <summary>
		/// Computed property: Total net weight from parent L1 barcode
		/// </summary>
		[NotMapped]
		public double? TotalNetWeight { get; set; }

		// =====================================================
		// NAVIGATION PROPERTIES (Optional - for eager loading)
		// =====================================================

		/// <summary>
		/// Navigation property to parent L1 barcode (NO DB FK CONSTRAINT)
		/// </summary>
		public virtual L1barcodegeneration? ParentL1 { get; set; }
	}
}

