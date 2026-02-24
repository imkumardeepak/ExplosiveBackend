using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class CountryMaster
	{
		public int Id { get; set; }

		[Required]
		[DisplayName("Country Name")]
		[MaxLength(100)]
		public string cname { get; set; }

		[Required]
		[MaxLength(2)]
		[DisplayName("Country Code")]
		public string code { get; set; }
	}
}
