using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class RE6Generation
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(50)]
		public string re12no { get; set; }

		[MaxLength(255)]
		public string consigneename { get; set; }

		public int consigneeid { get; set; }

		[MaxLength(500)]
		public string consigneeaddress { get; set; }

		[MaxLength(50)]
		public string licno { get; set; }

		[MaxLength(255)]
		public string transname { get; set; }

		public int transid { get; set; }

		[MaxLength(20)]
		public string vehicleno { get; set; }

		[MaxLength(50)]
		public string vehiclelic { get; set; }

		[MaxLength(50)]
		public string vehiclevalue { get; set; }

		[MaxLength(20)]
		public string re6date { get; set; }

		[MaxLength(10)]
		public string re6time { get; set; }

		[MaxLength(2)]
		[Column(TypeName = "varchar(2)")]
		public string re6month { get; set; }

		[MaxLength(4)]
		[Column(TypeName = "varchar(4)")]
		public string re6year { get; set; }
	}
}
