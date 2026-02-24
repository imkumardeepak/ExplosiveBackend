using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	/// <summary>
	/// L1 barcode (outer box) - parent of L2/L3 barcode hierarchy.
	/// This is the SINGLE SOURCE OF TRUTH for all manufacturing metadata.
	/// L2/L3 tables reference this via L1Barcode FK — no data duplication.
	/// </summary>
	public class L1barcodegeneration
	{
		[Key]
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string? L1Barcode { get; set; }

		[Required]
		public long SrNo { get; set; }

		// =====================================================
		// MANUFACTURING IDENTITY (names kept for query performance)
		// =====================================================

		[Required]
		[MaxLength(50)]
		public string? Country { get; set; }

		[Required]
		[StringLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string? CountryCode { get; set; }

		[Required]
		[MaxLength(100)]
		public string? MfgName { get; set; }

		[Required]
		[MaxLength(100)]
		public string? MfgLoc { get; set; }

		[Required]
		[StringLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string? MfgCode { get; set; }

		[Required]
		[MaxLength(100)]
		public string? PlantName { get; set; }

		[Required]
		[StringLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string? PCode { get; set; }

		[Required]
		[StringLength(1)]
		[Column(TypeName = "varchar(1)")]
		public string? MCode { get; set; }

		[Required]
		[StringLength(1)]
		[Column(TypeName = "varchar(1)")]
		public string? Shift { get; set; }

		[Required]
		[MaxLength(255)]
		public string? BrandName { get; set; }

		[Required]
		[StringLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string? BrandId { get; set; }

		[Required]
		[MaxLength(100)]
		public string? ProductSize { get; set; }

		[Required]
		[StringLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string? PSizeCode { get; set; }

		[Required]
		[MaxLength(50)]
		public string? SdCat { get; set; }

		[Required]
		[MaxLength(50)]
		public string? UnNoClass { get; set; }

		// =====================================================
		// DATES & VALUES
		// =====================================================

		[Required]
		[Column(TypeName = "date")]
		public DateTime MfgDt { get; set; }

		[Required]
		[Column(TypeName = "timestamp without time zone")]
		[DefaultValue("now()")]
		public DateTime MfgTime { get; set; } = DateTime.Now;

		[Required]
		[Column(TypeName = "decimal(12,4)")]
		public decimal L1NetWt { get; set; }

		[Required]
		[MaxLength(20)]
		public string? L1NetUnit { get; set; }

		[Required]
		public int NoOfL2 { get; set; }

		[Required]
		public int NoOfL3 { get; set; }

		// =====================================================
		// FLAGS (new typed versions only)
		// =====================================================

		/// <summary>
		/// Magazine assignment flag. True = assigned, False = not assigned
		/// </summary>
		[Required]
		public bool MFlag { get; set; } = false;

		/// <summary>
		/// Check flag status. True = checked, False = unchecked
		/// </summary>
		[Required]
		public bool CheckFlag { get; set; } = false;

		// =====================================================
		// NAVIGATION PROPERTIES (Optional - for eager loading)
		// =====================================================

		/// <summary>
		/// Collection of child L2 barcodes
		/// </summary>
		public virtual ICollection<L2barcodegeneration>? L2Barcodes { get; set; }

		/// <summary>
		/// Collection of child L3 barcodes (direct relationship)
		/// </summary>
		public virtual ICollection<L3barcodegeneration>? L3Barcodes { get; set; }

		/// <summary>
		/// Collection of dispatch transactions for this L1 barcode
		/// </summary>
		public virtual ICollection<DispatchTransaction>? DispatchTransactions { get; set; }

		/// <summary>
		/// Magazine stock record (if assigned to magazine)
		/// </summary>
		public virtual Magzine_Stock? MagazineStock { get; set; }
	}
}

