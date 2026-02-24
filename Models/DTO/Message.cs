namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class Message
	{
		public string SenderId { get; set; }
		public string ReceiverId { get; set; } // Empty for notifications
		public string Content { get; set; }
	}
}
