using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	/// <summary>
	/// BarcodeData - Maps L1, L2, L3 barcode relationships.
	/// NORMALIZED: removed redundant brand/plant/product columns (get from L1 via JOIN).
	/// Added IsFinal flag to replace BarcodeDataFinal table.
	/// </summary>
	public class BarcodeData
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string L1 { get; set; }

		[Required]
		[MaxLength(50)]
		public string L2 { get; set; }

		[Required]
		[MaxLength(50)]
		public string L3 { get; set; }

		[MaxLength(50)]
		public string? Batch { get; set; }

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
		/// Replaces the BarcodeDataFinal table. True = finalized record.
		/// </summary>
		[Required]
		public bool IsFinal { get; set; } = false;
	}
}
