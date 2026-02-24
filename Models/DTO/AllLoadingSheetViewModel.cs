using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class AllLoadingSheetViewModel
	{
		[Required]
		public string? LoadingSheetNo { get; set; }

		public string? CustName { get; set; }

		public string? TransName { get; set; }

		[NotMapped]
		public List<TransportMaster>? Trasporter { get; set; }

		[NotMapped]
		public List<MagzineMaster>? magzine { get; set; }

		public string? TruckNo { get; set; }


		public string? LicenseNo { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime Validity { get; set; }

		[NotMapped]
		public List<Re11IndentInfo>? re11IndentInfos { get; set; }

		[NotMapped]
		public List<object>? Magzinestock { get; set; }=new List<object>();

		public List<AllLoadingSheetPrdInfoViewModel>? IndentInfoViewModels { get; set; } = new List<AllLoadingSheetPrdInfoViewModel>();

	}

	public class AllLoadingSheetPrdInfoViewModel
	{

		public int Id { get; set; }

		public int LoadingSheetId { get; set; } // FK

		public string? IndentNo { get; set; }


		public DateTime IndentDt { get; set; }

		public string? Ptype { get; set; }

		public string? PtypeCode { get; set; } // Optional

		public string? Bname { get; set; }

		public string? Bid { get; set; }

		public string? Psize { get; set; }


		public string? SizeCode { get; set; }


		public long Class { get; set; }


		public long Div { get; set; }


		public double L1NetWt { get; set; }


		public string? Unit { get; set; }

		public double? LoadWt { get; set; }
		public int? Loadcase { get; set; }

		public string? TypeOfDispatch { get; set; }

		public string? mag { get; set; }

		public int? iscompleted { get; set; }

		public List<Batches>? batches { get; set; }
	}

	public class Batches
	{
		public string? batch { get; set; }

		public int distinctCount { get; set; }
 
	}
}
