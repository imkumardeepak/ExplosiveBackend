using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	/// <summary>
	/// L2 barcode (inner box) - child of L1barcodegeneration.
	/// NORMALIZED: Only stores L2-specific data + FK to parent L1.
	/// All manufacturing metadata (Country, Plant, Brand, etc.) comes from parent L1.
	/// </summary>
	public class L2barcodegeneration
	{
		[Key]
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string L2Barcode { get; set; }

		[Required]
		public long SrNo { get; set; }

		/// <summary>
		/// Foreign key reference to L1Barcodegeneration.L1Barcode
		/// </summary>
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string L1Barcode { get; set; }

		[Required]
		[Column(TypeName = "date")]
		public DateTime MfgDt { get; set; }

		[Required]
		[Column(TypeName = "timestamp without time zone")]
		[DefaultValue("now()")]
		public DateTime MfgTime { get; set; } = DateTime.Now;

		// =====================================================
		// NAVIGATION PROPERTY (Optional - for eager loading)
		// =====================================================

		/// <summary>
		/// Navigation property to parent L1 barcode (NO DB FK CONSTRAINT)
		/// Use with .Include(l2 => l2.ParentL1) for eager loading
		/// </summary>
		public virtual L1barcodegeneration? ParentL1 { get; set; }
	}
}
