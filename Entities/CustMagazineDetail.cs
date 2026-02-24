using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class CustMagazineDetail
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey(nameof(Customer))] // Mark Cid as a foreign key
		public int Cid { get; set; }  // Customer ID

		[Required]
		[StringLength(100)] // Adjust length as needed
		public string Magazine { get; set; }

		[StringLength(100)] // Adjust length as needed
		public string License { get; set; }

		[StringLength(100)] // Adjust length as needed
		public string Validity { get; set; }

		[StringLength(50)] // Adjust length as needed
		public string Wt { get; set; }

		[StringLength(50)] // Adjust length as needed
		public string Unit { get; set; }

		// Navigation property
		public CustomerMaster Customer { get; set; } // Relationship to CustomerMaster
	}
}
