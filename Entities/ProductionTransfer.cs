using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class ProductionTransfer
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Required]
		[Column("trans_id")]
		[MaxLength(50)]
		public string TransId { get; set; }

		public int? months { get; set; } = DateTime.Now.Month;

		public int? years { get; set; } = DateTime.Now.Year;

		[Required]
		[Column("truck_no")]
		[MaxLength(20)]
		public string TruckNo { get; set; }

		public int Allotflag { get; set; } = 0;

		// Navigation property
		public List<ProductionTransferCases> Barcodes { get; set; }
	}

	public class ProductionTransferCases
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Required]
		[Column("production_transfer_id")]
		public int ProductionTransferId { get; set; }

		[Required]
		[Column("l1barcode")]
		[MaxLength(50)]
		public string L1Barcode { get; set; }
	}
}
