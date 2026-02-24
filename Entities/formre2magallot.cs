using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Peso_Baseed_Barcode_Printing_System_API.Models;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class formre2magallot
	{
		[Key]
		[MaxLength(50)]
		[Column(TypeName = "varchar(50)")]
		public string? l1barcode { get; set; }

		public int? srno { get; set; }

		[MaxLength(100)]
		public string? plantname { get; set; }

		[MaxLength(10)]
		[Column(TypeName = "varchar(10)")]
		public string? pcode { get; set; }

		[MaxLength(255)]
		public string? bname { get; set; }

		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string? bid { get; set; }

		[MaxLength(100)]
		public string? productsize { get; set; }

		[MaxLength(3)]
		[Column(TypeName = "varchar(3)")]
		public string? psize { get; set; }

		[MaxLength(1)]
		public string? shift { get; set; }

		[Column(TypeName = "decimal(12,4)")]
		public decimal? l1netwt { get; set; }

		[NotMapped]
		[MaxLength(20)]
		public string? l1netunit { get; set; }

		[Column(TypeName = "date")]
		public DateTime? dateoftest { get; set; }

		[MaxLength(100)]
		public string? magname { get; set; }

		[MaxLength(50)]
		public string? maglic { get; set; }

		[Column(TypeName = "date")]
		public DateTime? mfgdt { get; set; }

		[Column(TypeName = "timestamp without time zone")]
		public DateTime? gendt { get; set; }

		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string? month { get; set; }

		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string? quarter { get; set; }

		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string? year { get; set; }

		public int? re2 { get; set; }

		public int? re12 { get; set; }

		public int? @class { get; set; }

		public int? div { get; set; }

		[NotMapped]
		public List<SelectListItem>? plist { get; set; } = new List<SelectListItem>();

		[NotMapped]
		public List<SelectListItem>? mlist { get; set; } = new List<SelectListItem>();

		[Required]
		public List<SmallModel>? SmallModels { get; set; } = new List<SmallModel> { };

		[NotMapped]
		[MaxLength(100)]
		public string? MagnameTo { get; set; }
	}
}
