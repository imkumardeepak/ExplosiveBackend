using System.ComponentModel.DataAnnotations;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    public class UomMaster
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string uom { get; set; }
        [Required]
        public string uomcode { get; set; }

    }
}
