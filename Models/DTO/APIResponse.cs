using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class APIResponse
	{
		public bool Status { get; set; }

		public HttpStatusCode StatusCode { get; set; }

		public dynamic Data { get; set; }
		public string Message { get; set; }
		public List<string> Errors { get; set; } = new List<string>();
	}
}
