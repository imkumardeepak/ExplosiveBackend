using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class PlantMaster
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string plant_type { get; set; }

		[Required]
		[MaxLength(100)]
		public string PName { get; set; }

		[Required]
		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string PCode { get; set; }

		[Required]
		[MaxLength(50)]
		public string License { get; set; }

		[MaxLength(20)]
		public string Company_ID { get; set; }

		[Column("issue_dt", TypeName = "date")]
		public DateTime issue_dt { get; set; }

		[Column("validity_dt", TypeName = "date")]
		public DateTime validity_dt { get; set; }
	}
}
