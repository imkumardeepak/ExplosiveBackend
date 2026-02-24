namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    
    public class StockSummary
    {
        public DateTime? mfgdt { get; set; }
        public string bname { get; set; }
        public string bid { get; set; }
        public string productsize { get; set; }
        public string psizecode { get; set; }

        public string @class { get; set; }
        public string div { get; set; }
        public string magname { get; set; }
        public string maglic { get; set; }
        public double opening { get; set; }
        public string recbname { get; set; }
        public string recproductsize { get; set; }
        public double quantity { get; set; }
        public string batch { get; set; }
        public string licence { get; set; }
        public string transport { get; set; }
        public string passno { get; set; }
        public double production { get; set; }
        public double closing { get; set; }
    }
}
