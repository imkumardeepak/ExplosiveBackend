using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class L1boxDeletionReport
	{
		[Key]
		[Required]
		public DateTime? from { get; set; }

		[Required]
		public DateTime? to { get; set; }

		[Required]
		public DateTime? mfgDt { get; set; }

		[Required]
		public string typeofreport { get; set; }

		[Required]
		public string plant { get; set; }

		[Required]
		public string brand { get; set; }
		[Required]
		public string productsize { get; set; }

		[Required]
		public string l1barcode { get; set; }

		[Required]
		public DateTime deletionDt { get; set; }

		[Required]
		public string reason { get; set; }

		[Required]
		public DateTime? currentstkdt { get; set; }

		[Required]
		public string Month { get; set; }

		[Required]
		public string Year { get; set; }
	}
}
