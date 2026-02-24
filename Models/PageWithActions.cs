using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
	public class PageWithActions
    {
        public string? Page { get; set; }
        public bool Selected { get; set; }
        public List<ActionCheckBox> Actions { get; set; }
    }
}
