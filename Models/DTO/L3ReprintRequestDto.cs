namespace Peso_Based_Barcode_Printing_System_APIBased.Models.DTO
{
    public class L3ReprintRequestDto
    {
        public List<ReprintdData> ReprintData { get; set; }
        public string fromValue { get; set; }
        public string toValue { get; set; }
        public string Plant { get; set; }
    }

    public class ReprintdData
    {
        public string L1Barcode { get; set; }
        public string L2Barcode { get; set; }
        public string L3Barcode { get; set; }
    }
}
