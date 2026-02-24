using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class StateMaster
	{
		public int Id { get; set; }

		[Required]
		public string State { get; set; }

		[Required]
		public string St_code { get; set; }

		[Required]
		public string District { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string Tahsil { get; set; }
	}
}
