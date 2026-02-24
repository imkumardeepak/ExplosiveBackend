using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{

	public class MagzineEditViewModel
	{
		public MagzineMaster Magzine { get; set; }
		public List<MagzineMasterDetails> CapacityDetails { get; set; }
		public List<SelectListItem> MfgLocations { get; set; }
		public List<SelectListItem> UomCodes { get; set; }
	}
}
