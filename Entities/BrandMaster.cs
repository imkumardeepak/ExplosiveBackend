using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class BrandMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[DisplayName("Plant Type Name")]
		[MaxLength(100)]
		public string plant_type { get; set; }

		[Required]
		[DisplayName("Brand Name")]
		[MaxLength(100)]
		public string bname { get; set; }

		[Required]
		[MinLength(4), MaxLength(4)]
		[DisplayName("Brand Id")]
		[Column(TypeName = "varchar(4)")]
		[RegularExpression(@"^\d{1,4}$", ErrorMessage = "Brand Id must be numeric and up to 4 digits.")]
		public string bid { get; set; }

		[Required]
		[DisplayName("Class")]
		public int Class { get; set; }

		[Required]
		[DisplayName("Division")]
		public int Division { get; set; }

		[Required]
		[DisplayName("Unit")]
		[MaxLength(20)]
		public string unit { get; set; }
	}
}
