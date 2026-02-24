using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class Re11IndentInfoViewModel
	{
		[Required]
		public string? IndentNo { get; set; }



		public DateTime IndentDt { get; set; } = DateTime.Now;


		public string? CustName { get; set; }

		public List<CustomerMaster>? CustomerslIST { get; set; }

		public List<string>? AllExistingIndentno { get; set; }

		public List<ProductMaster>? ProductList { get; set; }


		public string? ConName { get; set; }



		public string? ConNo { get; set; }


		public string? Clic { get; set; }


		public int? CompFlag { get; set; }


		public string? Month { get; set; }


		public string? Year { get; set; }


		public DateTime PesoDt { get; set; } = DateTime.Now;

		public List<Re11IndentPrdInfoViewModel> PrdInfoList { get; set; } = new List<Re11IndentPrdInfoViewModel>();

	}

	public class Re11IndentPrdInfoViewModel
	{

		[Required(ErrorMessage = "Ptype is required.")]
		public string? Ptype { get; set; }

		[NotMapped]
		public IEnumerable<SelectListItem>? ptypelist { get; set; }

		public string? PtypeCode { get; set; }




		public string? Bname { get; set; }


		public string? Bid { get; set; }


		public string? Psize { get; set; }



		public string? SizeCode { get; set; }

		[Required(ErrorMessage = "Class is required.")]
		public long Class { get; set; }

		[Required(ErrorMessage = "Division is required.")]
		public long Div { get; set; }

		[Required(ErrorMessage = "L1NetWt is required.")]
		[Range(0.1, double.MaxValue, ErrorMessage = "L1NetWt must be greater than 0.")]
		public double L1NetWt { get; set; }

		[Required(ErrorMessage = "ReqWt is required.")]
		public double ReqWt { get; set; }

		[Required(ErrorMessage = "ReqUnit is required.")]
		public string? ReqUnit { get; set; }
		public int reqCase { get; set; }

		public double? RemWt { get; set; }
		public string? RemUnit { get; set; }
		public int remCase { get; set; }
		public int? CompFlag { get; set; }
		public double? LoadWt { get; set; }
		public string? LoadUnit { get; set; }
		public int LoadCase { get; set; }

		public string? TypeOfDispatch { get; set; }

		public string? mag { get; set; }
	}
}
