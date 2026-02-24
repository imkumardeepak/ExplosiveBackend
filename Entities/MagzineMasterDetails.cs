using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class MagzineMasterDetails
	{
		public int Id { get; set; }

		public int magzineid { get; set; }

		public int Class { get; set; }

		public int division { get; set; }

		[MaxLength(100)]
		public string? Product { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal wt { get; set; }

		public int margin { get; set; }

		[MaxLength(20)]
		public string? units { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal? free_space { get; set; }
	}
}
