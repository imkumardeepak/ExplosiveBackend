namespace Peso_Based_Barcode_Printing_System_APIBased.Models.DTO
{
	public class ReprintRequestDto
	{

		public List<ReprintRowData>? ReprintData { get; set; }
		public string? Reason { get; set; }

		public int? NoOfCopies { get; set; } = 1;
	}

	public class ReprintRowData
	{
		public string? L1Barcode { get; set; }
		public string? SrNo { get; set; }
		public string? Country { get; set; }
		public string? CountryCode { get; set; }
		public string? MfgName { get; set; }
		public string? MfgCode { get; set; }
		public string? PlantName { get; set; }
		public string? PCode { get; set; }
		public string? MCode { get; set; }
		public string? Shift { get; set; }
		public string? Brand { get; set; }
		public string? BrandId { get; set; }
		public string? PSize { get; set; }
		public string? PSizeCode { get; set; }
		public string? SDCat { get; set; }
		public string? UNClass { get; set; }
		public DateTime? MfgDt { get; set; }
		public string? Month { get; set; }
		public string? Year { get; set; }
		public string? L1NetWt { get; set; }
		public string? L1NetUnit { get; set; }
	}

}
