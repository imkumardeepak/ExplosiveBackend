using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class AllLoadingSheet
	{
		public int Id { get; set; }

		[MaxLength(50)]
		public string? LoadingSheetNo { get; set; }

		[Column(TypeName = "date")]
		public DateTime Mfgdt { get; set; } = DateTime.Now;

		[MaxLength(100)]
		public string? TName { get; set; }

		[MaxLength(20)]
		public string? TruckNo { get; set; }

		[MaxLength(50)]
		public string? TruckLic { get; set; }

		[Column(TypeName = "date")]
		public DateTime LicVal { get; set; }

		[Column(TypeName = "timestamp without time zone")]
		public DateTime? CreationDateTime { get; set; }

		public int? Month { get; set; }

		public int? Year { get; set; }

		public int? Quarter { get; set; }

		[DefaultValue(0)]
		public int? Compflag { get; set; } = 0;

		public List<AllLoadingIndentDeatils>? IndentDetails { get; set; }
	}

	public class AllLoadingIndentDeatils
	{
		public int Id { get; set; }

		public int LoadingSheetId { get; set; }

		[MaxLength(50)]
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

		[Column(TypeName = "decimal(12,4)")]
		public decimal? LoadWt { get; set; }

		public int? Loadcase { get; set; }

		[MaxLength(50)]
		public string? TypeOfDispatch { get; set; }

		[MaxLength(50)]
		public string? mag { get; set; }

		public int? iscompleted { get; set; }

		[MaxLength(50)]
		public string? Batch { get; set; } = "";
	}
}
