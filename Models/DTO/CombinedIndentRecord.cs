namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
    public class CombinedIndentRecord
    {
        public IndentDetails? IndentDetails { get; set; }
        public List<ExtractedRecord>? ExtractedRecords { get; set; }
    }
    public class ExtractedRecord
    {
        public string SrNo { get; set; }
        public string BName { get; set; }
        public string Bid { get; set; }
        public string PSizeCode { get; set; }
        public string PSizeDesc { get; set; }
        public int L1NetWt { get; set; }
        public string Unit { get; set; }
        public string Class { get; set; }
        public string Div { get; set; }
        public string Quantity { get; set; }
        public string QuantityUnit { get; set; }
    }

    public class IndentDetails
    {
        public string IndentNo { get; set; }
        public string Date { get; set; }
        public string Cname { get; set; }
        public string OccupierName { get; set; }
        public string License { get; set; }
    }
}
