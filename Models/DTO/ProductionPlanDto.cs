using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
	public class ProductionPlanDto
	{
		public int Id { get; set; }

		[StringLength(20, ErrorMessage = "Production plan number cannot exceed 20 characters.")]
		public string ProductionPlanNo { get; set; }

		[StringLength(50, ErrorMessage = "Production type cannot exceed 50 characters.")]
		public string ProductionType { get; set; }

		[Required(ErrorMessage = "Manufacturing date is required.")]
		public DateTime MfgDt { get; set; }


		[Required(ErrorMessage = "Plant code is required.")]
		[StringLength(2, ErrorMessage = "Plant code cannot exceed 2 characters.")]
		public string PlantCode { get; set; }


		[Required(ErrorMessage = "Brand ID is required.")]
		[StringLength(4, ErrorMessage = "Brand ID cannot exceed 4 characters.")]
		public string BrandId { get; set; }

		[Required(ErrorMessage = "Product size code is required.")]
		[StringLength(3, ErrorMessage = "Product size code cannot exceed 3 characters.")]
		public string PSizeCode { get; set; }

		[Required(ErrorMessage = "Total weight is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Total weight must be greater than 0.")]
		public decimal TotalWeight { get; set; }

		[Required(ErrorMessage = "Number of boxes is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Number of boxes must be at least 1.")]
		public int NoOfbox { get; set; }

		[Required(ErrorMessage = "Number of stickers is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Number of stickers must be at least 1.")]
		public int NoOfstickers { get; set; }

		[Required(ErrorMessage = "Machine code is required.")]
		[StringLength(1, ErrorMessage = "Machine code must be 1 character.")]
		public string MachineCode { get; set; }

		[Required(ErrorMessage = "Shift is required.")]
		[StringLength(1, ErrorMessage = "Shift must be 1 character.")]
		public string Shift { get; set; }

		public DateTime CratedDate { get; set; }
	}

	public class CreateProductionPlanRequestDto
	{
		[StringLength(20, ErrorMessage = "Production plan number cannot exceed 20 characters.")]
		public string ProductionPlanNo { get; set; }

		[StringLength(50, ErrorMessage = "Production type cannot exceed 50 characters.")]
		public string ProductionType { get; set; }

		[Required(ErrorMessage = "Manufacturing date is required.")]
		public DateTime MfgDt { get; set; }


		[Required(ErrorMessage = "Plant code is required.")]
		[StringLength(2, ErrorMessage = "Plant code cannot exceed 2 characters.")]
		public string PlantCode { get; set; }


		[Required(ErrorMessage = "Brand ID is required.")]
		[StringLength(4, ErrorMessage = "Brand ID cannot exceed 4 characters.")]
		public string BrandId { get; set; }

		[Required(ErrorMessage = "Product size code is required.")]
		[StringLength(3, ErrorMessage = "Product size code cannot exceed 3 characters.")]
		public string PSizeCode { get; set; }

		[Required(ErrorMessage = "Total weight is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Total weight must be greater than 0.")]
		public decimal TotalWeight { get; set; }

		[Required(ErrorMessage = "Number of boxes is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Number of boxes must be at least 1.")]
		public int NoOfbox { get; set; }

		[Required(ErrorMessage = "Number of stickers is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Number of stickers must be at least 1.")]
		public int NoOfstickers { get; set; }

		[Required(ErrorMessage = "Machine code is required.")]
		[StringLength(1, ErrorMessage = "Machine code must be 1 character.")]
		public string MachineCode { get; set; }

		[Required(ErrorMessage = "Shift is required.")]
		[StringLength(1, ErrorMessage = "Shift must be 1 character.")]
		public string Shift { get; set; }
	}

	public class UpdateProductionPlanRequestDto
	{
		public int Id { get; set; }

		[StringLength(20, ErrorMessage = "Production plan number cannot exceed 20 characters.")]
		public string ProductionPlanNo { get; set; }

		[StringLength(50, ErrorMessage = "Production type cannot exceed 50 characters.")]
		public string ProductionType { get; set; }

		[Required(ErrorMessage = "Manufacturing date is required.")]
		public DateTime MfgDt { get; set; }


		[Required(ErrorMessage = "Plant code is required.")]
		[StringLength(2, ErrorMessage = "Plant code cannot exceed 2 characters.")]
		public string PlantCode { get; set; }


		[Required(ErrorMessage = "Brand ID is required.")]
		[StringLength(4, ErrorMessage = "Brand ID cannot exceed 4 characters.")]
		public string BrandId { get; set; }

		[Required(ErrorMessage = "Product size code is required.")]
		[StringLength(3, ErrorMessage = "Product size code cannot exceed 3 characters.")]
		public string PSizeCode { get; set; }

		[Required(ErrorMessage = "Total weight is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Total weight must be greater than 0.")]
		public decimal TotalWeight { get; set; }

		[Required(ErrorMessage = "Number of boxes is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Number of boxes must be at least 1.")]
		public int NoOfbox { get; set; }

		[Required(ErrorMessage = "Number of stickers is required.")]
		[Range(1, int.MaxValue, ErrorMessage = "Number of stickers must be at least 1.")]
		public int NoOfstickers { get; set; }

		[Required(ErrorMessage = "Machine code is required.")]
		[StringLength(1, ErrorMessage = "Machine code must be 1 character.")]
		public string MachineCode { get; set; }

		[Required(ErrorMessage = "Shift is required.")]
		[StringLength(1, ErrorMessage = "Shift must be 1 character.")]
		public string Shift { get; set; }
	}

	public class SearchProductionPlanRequestDto
	{
		public string? ProductionPlanNo { get; set; }
		
		public string? ProductionType { get; set; }
		
		public DateTime? MfgDt { get; set; }
		
		public string? PlantCode { get; set; }
		
		public string? BrandId { get; set; }
		
		public string? PSizeCode { get; set; }
		
		public decimal? MinTotalWeight { get; set; }
		
		public decimal? MaxTotalWeight { get; set; }
		
		public int? MinNoOfBox { get; set; }
		
		public int? MaxNoOfBox { get; set; }
		
		public int? MinNoOfStickers { get; set; }
		
		public int? MaxNoOfStickers { get; set; }
		
		public DateTime? CreatedDateFrom { get; set; }
		
		public DateTime? CreatedDateTo { get; set; }
		
		public string? MachineCode { get; set; }
		
		public string? Shift { get; set; }
	}
}