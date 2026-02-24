using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
    [NotMapped]
    public class ManualMagAllot    {
        
        
        public DateTime? MfgDt { get; set; }


        public string? PlantName { get; set; }

        //public List<SelectListItem>? plist { get; set; } = new List<SelectListItem>();


        public string? PlantCode { get; set; }
     
        public string? BrandName { get; set; }
      
        public string? BrandId { get; set; }
      
        public string? ProductSize { get; set; }

        public string? PSizeCode { get; set; }

    
        public string? Magname { get; set; }

     
        //public List<SelectListItem>? mlist { get; set; } = new List<SelectListItem>();

        // List of small models with checkbox values
        
        public List<SmallModel>? SmallModels { get; set; } = new List<SmallModel> { };
    }
}
