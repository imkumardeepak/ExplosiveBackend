namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class DispatchDetailsDto
	{
		public string CustomerName { get; set; }
		public string IndentNo { get; set; }
		public string BrandName { get; set; }
		public string ProductSize { get; set; }
		public double RequestedWeight { get; set; }
		public int Status { get; set; }
	}
}
