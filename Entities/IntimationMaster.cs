using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class IntimationMaster
	{
		public int Id { get; set; }

		[Required]
		[DisplayName("Intimation Name")]
		[MaxLength(100)]
		public string name { get; set; }

		[Required]
		[DisplayName("Intimation Address")]
		[MaxLength(500)]
		public string address { get; set; }
	}
}
