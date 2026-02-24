using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class BatchMasters
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string? PlantName { get; set; }

		[MaxLength(3)]
		public string? PlantCode { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal BatchSize { get; set; }

		[MaxLength(20)]
		public string? unit { get; set; }

		[MaxLength(10)]
		public string? BatchCode { get; set; }

		public char? BatchType { get; set; }

		public char? ResetType { get; set; }

		[MaxLength(50)]
		public string? BatchFormat { get; set; }
	}
}
