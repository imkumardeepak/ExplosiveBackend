using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class hht_prodtomagtransfer
	{
		[Key]
		public int tid { get; set; }

		[Required]
		[DisplayName("Plant Code")]
		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string pcode { get; set; }

		[NotMapped]
		public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> plist { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

		[Required]
		[DisplayName("Truck No.")]
		[MaxLength(20)]
		public string truckno { get; set; }

		[Required]
		[DisplayName("L1barcode")]
		[MaxLength(50)]
		public string l1barcode { get; set; }

		[Required]
		[MaxLength(10)]
		public string lul { get; set; }

		[Required]
		[MaxLength(20)]
		public string transferdt { get; set; }

		[Required]
		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string month { get; set; }

		[Required]
		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string year { get; set; }

		[MaxLength(50)]
		public string td { get; set; }

		[MaxLength(255)]
		public string brand { get; set; }

		[MaxLength(100)]
		public string psize { get; set; }

		[MaxLength(20)]
		public string mfgdt { get; set; }
	}
}
