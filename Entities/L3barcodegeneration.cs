using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	/// <summary>
	/// L3 barcode (individual item) - child of L2barcodegeneration.
	/// NORMALIZED: Only stores L3-specific data + FKs to parent L1 and L2.
	/// All manufacturing metadata comes from parent L1.
	/// </summary>
	public class L3barcodegeneration
	{
		[Key]
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string L3Barcode { get; set; }

		[Required]
		public long SrNo { get; set; }

		/// <summary>
		/// Foreign key reference to L1Barcodegeneration.L1Barcode
		/// </summary>
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string L1Barcode { get; set; }

		/// <summary>
		/// Foreign key reference to L2Barcodegeneration.L2Barcode
		/// </summary>
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string L2Barcode { get; set; }

		[Required]
		[Column(TypeName = "date")]
		public DateTime MfgDt { get; set; }

		[Required]
		[Column(TypeName = "timestamp without time zone")]
		[DefaultValue("now()")]
		public DateTime MfgTime { get; set; } = DateTime.Now;

		// =====================================================
		// NAVIGATION PROPERTIES (Optional - for eager loading)
		// =====================================================

		/// <summary>
		/// Navigation property to parent L1 barcode
		/// </summary>
		public virtual L1barcodegeneration? ParentL1 { get; set; }

		/// <summary>
		/// Navigation property to parent L2 barcode
		/// </summary>
		public virtual L2barcodegeneration? ParentL2 { get; set; }
	}
}
