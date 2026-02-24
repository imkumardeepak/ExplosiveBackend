using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class l1barcodereprint
	{
		[Key]
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string l1barcode { get; set; }

		[Required]
		public int srno { get; set; }

		[Required]
		[MaxLength(50)]
		public string country { get; set; }

		[Required]
		[StringLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string countrycode { get; set; }

		[Required]
		[MaxLength(100)]
		public string mfgname { get; set; }

		[Required]
		[MaxLength(100)]
		public string mfgloc { get; set; }

		[Required]
		[StringLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string mfgcode { get; set; }

		[Required]
		[MaxLength(100)]
		public string plantname { get; set; }

		[Required]
		[StringLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string pcode { get; set; }

		[Required]
		[StringLength(1)]
		[Column(TypeName = "varchar(1)")]
		public string mcode { get; set; }

		[Required]
		[StringLength(1)]
		[Column(TypeName = "varchar(1)")]
		public string shift { get; set; }

		[Required]
		[MaxLength(255)]
		public string brandname { get; set; }

		[Required]
		[StringLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string brandid { get; set; }

		[Required]
		[MaxLength(100)]
		public string productsize { get; set; }

		[Required]
		[StringLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string psizecode { get; set; }

		[Required]
		[MaxLength(50)]
		public string sdcat { get; set; }

		[Required]
		[MaxLength(50)]
		public string unnoclass { get; set; }

		[Required]
		[Column(TypeName = "date")]
		public DateTime mfgdt { get; set; }

		[Required]
		[Column(TypeName = "timestamp without time zone")]
		public DateTime rp_dt_time { get; set; }

		[Required]
		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string month { get; set; }

		[Required]
		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string genyear { get; set; }

		[Required]
		[MaxLength(500)]
		public string reason { get; set; }
	}
}
