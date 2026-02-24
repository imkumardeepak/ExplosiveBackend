using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
    [NotMapped]
    public class RE2RequestDTO
    {
        public DateTime Mfgdt { get; set; }
        public string Plantcode { get; set; }
        public string Brandid { get; set; }
        public string Psizecode { get; set; }
        public string Magname { get; set; }
    }
}
