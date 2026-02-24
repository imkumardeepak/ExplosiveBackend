using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class TruckToMAGBarcode
	{
		[MaxLength(20)]
		public string TruckNo { get; set; }

		[MaxLength(50)]
		public string MagName { get; set; }

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string Barcode { get; set; }

		[MaxLength(1)]
		public string ScanFlag { get; set; }
	}
}
