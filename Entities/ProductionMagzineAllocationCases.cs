using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    [Table("ProductionMagzineAllocationCases")]
    public class ProductionMagzineAllocationCases
    {
        [Key]
        public int Id { get; set; }

        public int TransferToMazgnieId { get; set; }

        [MaxLength(50)]
        public string L1Scanned { get; set; }

        [ForeignKey("TransferToMazgnieId")]
        public virtual ProductionMagzineAllocation ProductionMagzineAllocation { get; set; }
    }
}
