using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	[NotMapped]
	public class L1L2L3
	{
		public string L1barcode { get; set; }
		public string L2barcode { get; set; }
		public string L3barcode { get; set; }
	}
}
