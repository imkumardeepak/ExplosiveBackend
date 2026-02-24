using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class PlantTypeMaster
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string plant_type { get; set; }

		[MaxLength(20)]
		public string Company_ID { get; set; }
	}
}