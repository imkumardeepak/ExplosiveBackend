using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class shiftmanagement
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(100)]
		public string pname { get; set; }

		[MaxLength(10)]
		public string pcode { get; set; }

		[MaxLength(1)]
		public string shift { get; set; }

		public bool activef { get; set; }
	}
}
