using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class reprint_l2barcode
	{
		[Key]
		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string l2barcode { get; set; }

		[Required]
		public int srno { get; set; }

		[Required]
		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string plantcode { get; set; }

		[Required]
		[Column(TypeName = "date")]
		public DateTime rptdt { get; set; }

		[Required]
		[Column(TypeName = "timestamp without time zone")]
		public DateTime rpttime { get; set; }

		[Required]
		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string month { get; set; }

		[Required]
		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string year { get; set; }

		[Required]
		[MaxLength(500)]
		public string reason { get; set; }
	}
}
