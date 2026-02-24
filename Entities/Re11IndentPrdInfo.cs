using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class Re11IndentPrdInfo
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Indent Number is required.")]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string? IndentNo { get; set; }

		[Column(TypeName = "date")]
		public DateTime IndentDt { get; set; }

		[Required(ErrorMessage = "Product Type is required.")]
		[MaxLength(100)]
		public string? Ptype { get; set; }

		[MaxLength(10)]
		public string? PtypeCode { get; set; }

		[Required(ErrorMessage = "Brand Name is required.")]
		[MaxLength(100)]
		public string? Bname { get; set; }

		[Required(ErrorMessage = "Brand ID is required.")]
		[StringLength(4, ErrorMessage = "Brand ID cannot be longer than 4 characters.")]
		[Column(TypeName = "varchar(4)")]
		public string? Bid { get; set; }

		[Required(ErrorMessage = "Product Size is required.")]
		[MaxLength(100)]
		public string? Psize { get; set; }

		[Required(ErrorMessage = "Size Code is required.")]
		[StringLength(3, ErrorMessage = "Size Code cannot be longer than 3 characters.")]
		[Column(TypeName = "varchar(3)")]
		public string? SizeCode { get; set; }

		[Required(ErrorMessage = "Class is required.")]
		public int Class { get; set; }

		[Required(ErrorMessage = "Division is required.")]
		public int Div { get; set; }

		[Required(ErrorMessage = "Net Weight is required.")]
		[Column(TypeName = "decimal(12,4)")]
		public decimal L1NetWt { get; set; }

		[Required(ErrorMessage = "Unit is required.")]
		[MaxLength(20)]
		public string? Unit { get; set; }

		[Required(ErrorMessage = "Required Weight is required.")]
		[Column(TypeName = "decimal(12,4)")]
		public decimal ReqWt { get; set; }

		[Required(ErrorMessage = "Required Unit is required.")]
		public int ReqCase { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal? RemWt { get; set; }

		public int? Remcase { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal? LoadWt { get; set; }

		public int? Loadcase { get; set; }
	}
}
