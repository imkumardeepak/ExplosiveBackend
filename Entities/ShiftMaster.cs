using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class ShiftMaster
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[DisplayName("Shift")]
		public string shift { get; set; }

		[Required]
		[DisplayName("Start Time")]
		[Column(TypeName = "time without time zone")]
		public TimeOnly fromtime { get; set; }

		[Required]
		[DisplayName("Stop Time")]
		[Column(TypeName = "time without time zone")]
		public TimeOnly totime { get; set; }
	}
}
