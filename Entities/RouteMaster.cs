using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class RouteMaster
	{
		public int Id { get; set; }

		[Required]
		[DisplayName("Customer Name")]
		[MaxLength(100)]
		public string cname { get; set; }

		[Required]
		[DisplayName("Start Name")]
		[MaxLength(255)]
		public string startpoint { get; set; }

		[Required]
		[DisplayName("Destination Name")]
		[MaxLength(255)]
		public string destpoint { get; set; }

		[Required]
		[DisplayName("Route Locations")]
		[MaxLength(1000)]
		public string Locations { get; set; }
	}
}
