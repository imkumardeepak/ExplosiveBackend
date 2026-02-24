using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class TransMemberDetail
	{
		[Key]
		public int Id { get; set; }

		public int Cid { get; set; }  // Customer ID

		[Required]
		[StringLength(100)] // Adjust length as needed
		public string Name { get; set; }

		[EmailAddress]
		[StringLength(100)] // Adjust length as needed
		public string Email { get; set; }

		[StringLength(15)] // Adjust length as needed
		public string ContactNo { get; set; }

	}
}
