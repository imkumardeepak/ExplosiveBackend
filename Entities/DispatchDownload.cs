using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class DispatchDownload
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string Barcode { get; set; }

		[MaxLength(20)]
		public string TruckNo { get; set; }

		[MaxLength(50)]
		public string IndentNo { get; set; }

		[MaxLength(50)]
		public string Mag { get; set; }

		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string Bid { get; set; }

		[MaxLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string SizeCode { get; set; }

		[MaxLength(1)]
		public string ScanFlag { get; set; }
	}
}
