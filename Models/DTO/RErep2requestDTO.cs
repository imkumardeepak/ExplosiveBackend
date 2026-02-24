using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Based_Barcode_Printing_System_APIBased.Models.DTO
{
    [NotMapped]
    public class RErep2requestDTO
    {
        public string mfgdt { get; set; }
        public string plantcode { get; set; }
        public string brandid { get; set; }
        public string psizecode { get; set; }
        
        public string magname { get; set; }
        public string? fromcase { get; set; }
        public string? tocase { get; set; }
    }
}
