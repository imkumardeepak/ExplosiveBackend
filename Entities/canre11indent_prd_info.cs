using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class canre11indent_prd_info
	{
		[Key]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string indentno { get; set; }

		[Column(TypeName = "date")]
		public DateTime indentdt { get; set; }

		public int pid { get; set; }

		[MaxLength(100)]
		public string ptype { get; set; }

		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string ptypecode { get; set; }

		[MaxLength(255)]
		public string bname { get; set; }

		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string bid { get; set; }

		[MaxLength(100)]
		public string psize { get; set; }

		[MaxLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string sizecode { get; set; }

		public int Class { get; set; }

		public int div { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal l1netwt { get; set; }

		[MaxLength(20)]
		public string unit { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal reqwt { get; set; }

		[MaxLength(20)]
		public string requnit { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal remwt { get; set; }

		[MaxLength(20)]
		public string remunit { get; set; }

		public int compflag { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal loadwt { get; set; }

		[MaxLength(20)]
		public string loadunit { get; set; }
	}
}
