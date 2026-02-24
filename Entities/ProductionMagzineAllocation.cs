using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    [Table("ProductionMagzineAllocations")]
    public class ProductionMagzineAllocation
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string TransferId { get; set; }

        public DateTime TransferDate { get; set; }

        [MaxLength(10)]
        public string PlantCode { get; set; }

        [MaxLength(100)]
        public string Plant { get; set; }

        [MaxLength(4)]
        public string BrandId { get; set; }

        [MaxLength(255)]
        public string BrandName { get; set; }

        [MaxLength(100)]
        public string ProductSize { get; set; }

        [MaxLength(3)]
        public string ProductSizecCode { get; set; }

        [MaxLength(100)]
        public string MagazineName { get; set; }

        public int CaseQuantity { get; set; }

        public decimal? Totalwt { get; set; }

        [MaxLength(20)]
        public string TruckNo { get; set; }

        public int? ReadFlag { get; set; }

        public virtual ICollection<ProductionMagzineAllocationCases> TransferToMazgnieScanneddata { get; set; }
    }
}
