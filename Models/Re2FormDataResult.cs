using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models
{
    [NotMapped]
    public class Re2FormDataResult
    {
        public DateTime gendt { get; set; }
        public string shift { get; set; }
        public string Class { get; set; }
        public string div { get; set; }
        public string bname { get; set; }
        public string productsize { get; set; }
        public string maglic { get; set; }
        public double netwt { get; set; }
        public DateTime? dateoftest { get; set; }
        public int srno_count { get; set; }
        public string srno_ranges { get; set; }
    }
}
