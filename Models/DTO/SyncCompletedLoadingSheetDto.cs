namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class SyncCompletedLoadingSheetDto
	{
		public LoadingSheetDto? LoadingSheet { get; set; }
		public List<string>? L1Barcodes { get; set; }
	}
	public class LoadingSheetDto
	{
		public int Id { get; set; }
		public string? LoadingNo { get; set; }
		public string? IndentNo { get; set; }
		public string? TruckNo { get; set; }
		public string? TName { get; set; }
		public string? BName { get; set; }
		public string? Bid { get; set; }
		public string? Product { get; set; }
		public string? PCode { get; set; }
		public string? TypeOofDispatc { get; set; } // typo kept as-is from original JSON
		public string? Magzine { get; set; }
		public double LoadWt { get; set; }
		public int LaodCases { get; set; } // typo kept as-is from original JSON
		public int Complete_Flag { get; set; }
	}

}
