using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class MfgLocationMaster
	{
		[Key]
		public int id { get; set; }

		[Required]
		[DisplayName("MFG Name")]
		[MaxLength(100)]
		public string mfgname { get; set; }

		[NotMapped]
		public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> mfglist { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

		[Required]
		[DisplayName("MFG Code")]
		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string mfgcode { get; set; }

		[Required]
		[DisplayName("MFG Location")]
		[MaxLength(100)]
		public string mfgloc { get; set; }

		[Required]
		[DisplayName("MFG Location Code")]
		[MaxLength(1)]
		[Column(TypeName = "varchar(1)")]
		public string mfgloccode { get; set; }

		[Required]
		[DisplayName("Main code")]
		[MaxLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string maincode { get; set; }

		[MaxLength(20)]
		public string Company_ID { get; set; }
	}
}
