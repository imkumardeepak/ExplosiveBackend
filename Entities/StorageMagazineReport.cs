using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class StorageMagazineReport
	{
		[Key]
		[Required]
		public string L1Barcode { get; set; }

		[Required]
		public string license { get; set; }

		[Required]
		public string magname { get; set; }

		[Required]
		public string typeofreport { get; set; }

		[Required]

		public DateTime? stockfrom { get; set; }

		[Required]

		public DateTime? stockto { get; set; }

		[Required]
		public string brandname { get; set; }

		[Required]
		public string productsize { get; set; }

		[Required]
		public string unit { get; set; }

		[Required]
		public string netqty { get; set; }

		[Required]
		public DateTime? currentstkdt { get; set; }

		[Required]
		public string Month { get; set; }

		[Required]
		public string Year { get; set; }
	}
}
