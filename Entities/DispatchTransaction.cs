using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	/// <summary>
	/// Represents a dispatch transaction record linking L1 barcodes to indent orders.
	/// NORMALIZED: removed deprecated Month/Year/Re12 columns. 
	/// Brand/Product info kept for indexed query performance.
	/// </summary>
	public class DispatchTransaction
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Tid { get; set; }

		/// <summary>
		/// Reference to Re11IndentInfo.IndentNo (FK relationship)
		/// </summary>
		[Required]
		[StringLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string IndentNo { get; set; }

		/// <summary>
		/// Reference to L1Barcodegeneration.L1Barcode
		/// </summary>
		[Required]
		[StringLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string L1Barcode { get; set; }

		[Required]
		[StringLength(50)]
		public string Brand { get; set; }

		[Required]
		[StringLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string Bid { get; set; }

		[Required]
		[StringLength(50)]
		public string PSize { get; set; }

		[Required]
		[StringLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string PSizeCode { get; set; }

		[Required]
		[StringLength(20)]
		public string TruckNo { get; set; }

		/// <summary>
		/// Reference to MagzineMaster.mcode
		/// </summary>
		[Required]
		[StringLength(50)]
		public string MagName { get; set; }

		[Required]
		[Column(TypeName = "date")]
		public DateTime DispDt { get; set; }

		/// <summary>
		/// RE-12 generation flag. True = generated
		/// </summary>
		[Required]
		public bool Re12 { get; set; } = false;

		[Required]
		[Column(TypeName = "decimal(12,4)")]
		public decimal L1NetWt { get; set; }

		[Required]
		[StringLength(20)]
		public string L1NetUnit { get; set; }

		// =====================================================
		// NAVIGATION PROPERTY (Optional - for eager loading)
		// =====================================================

		/// <summary>
		/// Navigation property to parent Re11IndentInfo
		/// </summary>
		public virtual Re11IndentInfo? Indent { get; set; }
	}
}

