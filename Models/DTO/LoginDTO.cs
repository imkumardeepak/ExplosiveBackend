using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class LoginDTO
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
	}

}
