using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class l1boxdeletion
	{
		[Key]
		public int tid { get; set; }

		[Required]
		[MaxLength(100)]
		public string plant { get; set; }

		[Required]
		[MaxLength(255)]
		public string brand { get; set; }

		[Required]
		[MaxLength(100)]
		public string psize { get; set; }

		[Column(TypeName = "date")]
		public DateTime mfgdt { get; set; }

		[Required]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string l1barcode { get; set; }

		[Required]
		[MaxLength(500)]
		public string reason { get; set; }

		[Column(TypeName = "timestamp without time zone")]
		public DateTime deldt { get; set; }

		[Required]
		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string month { get; set; }

		[Required]
		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string year { get; set; }
	}
}
