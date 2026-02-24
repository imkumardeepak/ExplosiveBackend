using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class canre11indentinfo
	{
		[Key]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string indentno { get; set; }

		[Column(TypeName = "date")]
		public DateTime indentdt { get; set; }

		[MaxLength(255)]
		public string custname { get; set; }

		[MaxLength(255)]
		public string conname { get; set; }

		[MaxLength(50)]
		public string conno { get; set; }

		[MaxLength(50)]
		public string clic { get; set; }

		public int compflag { get; set; }

		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string month { get; set; }

		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string year { get; set; }

		[Column(TypeName = "date")]
		public DateTime pesodt { get; set; }

		[Column(TypeName = "timestamp without time zone")]
		public DateTime canceldttime { get; set; }

		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string cmonth { get; set; }

		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string cyear { get; set; }

		[MaxLength(500)]
		public string reason { get; set; }
	}
}
