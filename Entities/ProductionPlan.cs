using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{

	public class ProductionPlan
	{
		public int Id { get; set; }

		[StringLength(20)]
		public string ProductionPlanNo { get; set; }

		[StringLength(50)]
		public string ProductionType { get; set; }

		[Required]
		[Column(TypeName = "Date")]
		public DateTime MfgDt { get; set; }

		[Required]
		[StringLength(2)]
		public string PlantCode { get; set; }


		[Required]
		[StringLength(4)]
		public string BrandId { get; set; }

		[Required]
		[StringLength(3)]
		public string PSizeCode { get; set; }

		[Required]
		public decimal TotalWeight { get; set; }

		[Required]
		public int NoOfbox { get; set; }

		[Required]
		public int NoOfstickers { get; set; }

		[Required]
		[StringLength(1)]
		public string MachineCode { get; set; }

		[Required]
		[StringLength(1)]
		public string Shift { get; set; }

		[Column(TypeName = "Date")]
		public DateTime CratedDate { get; set; } = DateTime.Now;

	}
}
