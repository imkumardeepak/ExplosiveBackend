using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class MfgMaster
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[DisplayName("MFG Name")]
		[MaxLength(100)]
		public string mfgname { get; set; }

		[Required]
		[DisplayName("MFG Code")]
		[MaxLength(10)]
		public string code { get; set; }

		[MaxLength(20)]
		public string Company_ID { get; set; }
	}
}
