using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class MachineCodeMaster
	{
		[Key]
		public int id { get; set; }

		[Required]
		[DisplayName("Plant Name")]
		[MaxLength(100)]
		public string pname { get; set; }

		[NotMapped]
		public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> plist { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

		[Required]
		[MaxLength(2)]
		[DisplayName("Plant Code")]
		[Column(TypeName = "varchar(2)")]
		public string pcode { get; set; }

		[Required]
		[MaxLength(1)]
		[DisplayName("Machine Code")]
		[Column(TypeName = "varchar(1)")]
		public string mcode { get; set; }

		[MaxLength(20)]
		public string Company_ID { get; set; }
	}
}
