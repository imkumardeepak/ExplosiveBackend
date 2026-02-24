using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class BatchDetails
	{
		public int Id { get; set; }

		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string? PlantCode { get; set; }

		[Column(TypeName = "date")]
		public DateTime MfgDate { get; set; }

		[MaxLength(10)]
		public string? BatchCode { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal TotalCapacity { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal RemainingCapacity { get; set; }
	}
}
