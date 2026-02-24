using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class MagStock
	{
		[Key]
		[Required]
		public string L1Barcode { get; set; }

		[Required]
		public string MagName { get; set; }

		[Required]
		public string BrandName { get; set; }

		[Required]
		public string BrandId { get; set; }

		[Required]
		public string PrdSize { get; set; }

		[Required]
		public string PSizeCode { get; set; }

		[Required]
		public double l1netwt { get; set; }

		[Required]
		public string l1netunit { get; set; }
	}
}
