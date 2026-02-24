using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class ProductMaster
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[DisplayName("Brand Name")]
		[MaxLength(100)]
		public string bname { get; set; }

		[NotMapped]
		public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> blist { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

		[Required]
		[DisplayName("Brand Id")]
		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string bid { get; set; }

		[Required]
		[DisplayName("Plant Name")]
		[MaxLength(100)]
		public string ptype { get; set; }

		[NotMapped]
		public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> plist { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

		[Required]
		[DisplayName("Plant Code")]
		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string ptypecode { get; set; }

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

		[Required]
		[DisplayName("Product Size")]
		[MaxLength(100)]
		public string psize { get; set; }

		[Required]
		[DisplayName("Product Size Code")]
		[MaxLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string psizecode { get; set; }

		[Required]
		[DisplayName("Dimension (MM)")]
		public int dimnesion { get; set; }

		[MaxLength(20)]
		public string? dimensionunit { get; set; }

		[Required]
		[DisplayName("Unit Weight (GM)")]
		[Column(TypeName = "decimal(12,4)")]
		public decimal dimunitwt { get; set; }

		[MaxLength(20)]
		public string? wtunit { get; set; }

		[Required]
		[DisplayName("L1NetWt")]
		[Column(TypeName = "decimal(12,4)")]
		public decimal l1netwt { get; set; }

		[Required]
		[DisplayName("NoofL2")]
		public int noofl2 { get; set; }

		[Required]
		[DisplayName("NoofL3perL2")]
		public int noofl3perl2 { get; set; }

		[Required]
		[DisplayName("NoofL3perL1")]
		public int noofl3perl1 { get; set; }

		[DisplayName("BPL1")]
		[MaxLength(255)]
		public string? bpl1 { get; set; }

		[DisplayName("BPL2")]
		[MaxLength(255)]
		public string? bpl2 { get; set; }

		[DisplayName("BPL3")]
		[MaxLength(255)]
		public string? bpl3 { get; set; }

		[Required]
		[DisplayName("SDCAT")]
		[MaxLength(50)]
		public string sdcat { get; set; }

		[Required]
		[DisplayName("UN N0 Class")]
		[MaxLength(50)]
		public string unnoclass { get; set; }

		[Required]
		[DisplayName("Active Flag")]
		public bool act { get; set; }
	}
}
