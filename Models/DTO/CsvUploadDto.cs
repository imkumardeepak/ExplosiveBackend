using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class CsvUploadDto
	{
		[Required]
		public string L1 { get; set; }

		[Required]
		public string L2 { get; set; }

		[Required]
		public string L3 { get; set; }
	}

	public class CsvUploadResponseDto
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public int TotalRecords { get; set; }
		public int SuccessfulRecords { get; set; }
		public int TotalCases { get; set; } //TotalCases
		public int FailedRecords { get; set; }
		public List<CsvUploadDto> Data { get; set; }
		public List<string> Errors { get; set; }
	}
}
