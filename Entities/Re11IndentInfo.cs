using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	/// <summary>
	/// RE-11 Indent Information - represents a customer order/indent
	/// Index: IX_Re11IndentInfo_IndentDt - Date-based queries
	/// Index: IX_Re11IndentInfo_IndentNo - Indent lookup
	/// </summary>
	public class Re11IndentInfo
	{
		[Key]
		[Required(ErrorMessage = "Indent Number is required.")]
		[StringLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string? IndentNo { get; set; }

		[Column(TypeName = "date")]
		public DateTime IndentDt { get; set; }

		[Column(TypeName = "date")]
		public DateTime PesoDt { get; set; }

		[MaxLength(255)]
		public string? CustName { get; set; }

		[MaxLength(255)]
		public string? ConName { get; set; }

		[MaxLength(50)]
		public string? ConNo { get; set; }

		[MaxLength(50)]
		public string? Clic { get; set; }

		[Required(ErrorMessage = "Month is required.")]
		[StringLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string? Month { get; set; }

		[Required(ErrorMessage = "Year is required.")]
		[StringLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string? Year { get; set; }

		/// <summary>
		/// Indent completion flag. 0 = pending, 1 = completed
		/// </summary>
		[DefaultValue(0)]
		public int? CompletedIndent { get; set; } = 0;

		// =====================================================
		// NAVIGATION PROPERTIES
		// =====================================================

		/// <summary>
		/// Collection of product line items for this indent
		/// Configured in DbContext with cascade delete
		/// </summary>
		public List<Re11IndentPrdInfo> IndentItems { get; set; } = new List<Re11IndentPrdInfo>();

		/// <summary>
		/// Collection of dispatch transactions for this indent
		/// Use with .Include(i => i.DispatchTransactions) for eager loading
		/// </summary>
		public virtual ICollection<DispatchTransaction>? DispatchTransactions { get; set; }
	}
}
