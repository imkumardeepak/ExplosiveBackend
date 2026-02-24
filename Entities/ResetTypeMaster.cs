using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class ResetTypeMaster
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(20)]
		public string resettype { get; set; }

		[MaxLength(20)]
		public string yeartype { get; set; }
	}
}
