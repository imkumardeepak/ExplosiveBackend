using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class TransportMaster
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string TName { get; set; }

		[StringLength(255)]
		public string Addr { get; set; }

		[MaxLength(50)]
		public string Gstno { get; set; }

		[StringLength(100)]
		public string State { get; set; }

		[StringLength(100)]
		public string City { get; set; }

		[StringLength(100)]
		public string District { get; set; }

		[StringLength(100)]
		public string Tahsil { get; set; }

		// Navigation properties
		public List<TransVehicleDetail> Vehicles { get; set; }
		public List<TransMemberDetail> Members { get; set; }
	}
}
