
using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class MultipleVehicleWithIndetDTO
	{

		public string? IndentNo { get; set; }
		public string? LoadingSheetNo { get; set; }
		public string? TransporterName { get; set; }

		public string? TruckNo { get; set; }
		public string? LicenseNo { get; set; }
		public DateTime Validity { get; set; }
	
		public List<ItemDto>? Items { get; set; }
	}

	public class ItemDto
	{
		[Required(ErrorMessage = "Bid is required.")]
		public string Bid { get; set; }
		[Required(ErrorMessage = "Bname is required.")]
		public string Bname { get; set; }
		public int Class { get; set; }
		[Required(ErrorMessage = "DispatchType is required.")]
		public string DispatchType { get; set; }
		public int Div { get; set; }
		public int Id { get; set; }
		public DateTime IndentDt { get; set; }
		[Required(ErrorMessage = "IndentNo is required.")]
		public string? IndentNo { get; set; }
		public decimal L1NetWt { get; set; }
		public decimal LoadWt { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "LoadCase must be greater than 0.")]
		public int LoadCase { get; set; }
		[Required(ErrorMessage = "Magazine is required.")]
		public string Magazine { get; set; }
		[Required(ErrorMessage = "Psize is required.")]
		public string Psize { get; set; }
		[Required(ErrorMessage = "Ptype is required.")]
		public string Ptype { get; set; }
		[Required(ErrorMessage = "PtypeCode is required.")]
		public string PtypeCode { get; set; }
		public decimal RemWt { get; set; }
		public int RemCase { get; set; }
		public int ReqCase { get; set; }
		public decimal ReqWt { get; set; }
		[Required(ErrorMessage = "SizeCode is required.")]
		public string SizeCode { get; set; }
		[Required(ErrorMessage = "Unit is required.")]
		public string Unit { get; set; }
	}
}
