using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class CustMemberDetail
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey(nameof(Customer))] // Mark Cid as a foreign key
		public int Cid { get; set; }  // Customer ID

		[Required]
		[StringLength(100)] // Adjust length as needed
		public string Name { get; set; }

		[EmailAddress]
		[StringLength(100)] // Adjust length as needed
		public string Email { get; set; }

		[StringLength(15)] // Adjust length as needed
		public string ContactNo { get; set; }

		// Navigation property
		public CustomerMaster Customer { get; set; } // Relationship to CustomerMaster
	}
}
