using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
	public class User
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string? Username { get; set; }

		[MaxLength(255)]
		public string? PasswordHash { get; set; }

		[MaxLength(20)]
		public string? Company_ID { get; set; }

		[MaxLength(50)]
		public string? Role { get; set; }
	}
}
