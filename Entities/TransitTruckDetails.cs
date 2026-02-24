using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class TransitTruckDetails
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("Cid")]  // Define the foreign key
		public int Cid { get; set; }  // Customer ID

		[Required]
		[StringLength(100)] // Adjust length as needed
		public string VehicleNo { get; set; }

		[StringLength(100)] // Adjust length as needed
		public string License { get; set; }

		[StringLength(100)] // Adjust length as needed
		public string Validity { get; set; }

		[StringLength(50)] // Adjust length as needed
		public string Wt { get; set; }

		[StringLength(50)] // Adjust length as needed
		public string Unit { get; set; }

		// Navigation property
		public TransitTruckMaster TransitTruckMaster { get; set; } // Relationship to CustomerMaster
	}
}
