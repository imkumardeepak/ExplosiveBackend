using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class MagzineMaster
	{
		public int Id { get; set; }

		[Required]
		[DisplayName("MFG Location")]
		[MaxLength(100)]
		public string? mfgloc { get; set; }

		[NotMapped]
		public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? mloclist { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

		[Required]
		[DisplayName("MFG Location Code")]
		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string? mfgloccode { get; set; }

		[Required]
		[DisplayName("MAG Name")]
		[MaxLength(100)]
		public string? magname { get; set; }

		[Required]
		[DisplayName("MAG Code")]
		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string? mcode { get; set; }

		[Required]
		[DisplayName("License No.")]
		[MaxLength(50)]
		public string? licno { get; set; }

		[Required]
		[DisplayName("Issue Date")]
		public DateTime issuedate { get; set; }

		[Required]
		[DisplayName("Validity Date")]
		[Column(TypeName = "date")]
		public DateTime validitydt { get; set; }

		[Required]
		[DisplayName("Capacity")]
		[Column(TypeName = "decimal(12,4)")]
		public decimal Totalwt { get; set; }

		public int margin { get; set; }

		public bool autoallot_flag { get; set; } = false;

		public List<MagzineMasterDetails>? MagzineMasterDetails { get; set; } = new List<MagzineMasterDetails>();
	}
}
