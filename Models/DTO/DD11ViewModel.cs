using System.Collections.Generic;

namespace Peso_Baseed_Barcode_Printing_System_API.Models.DTO
{
    public class DD11ViewModel
    {
        public string? TruckNo { get; set; }
        public string? IndentNo { get; set; }
        public string? CustName { get; set; }
        public string? Brand { get; set; }
        public string? Bid { get; set; }
        public string? PSize { get; set; }
        public string? SizeCode { get; set; }
        public int Qty { get; set; }
        public string? ScanQty { get; set; }
        public string? Comp { get; set; }

        public List<DD11DispatchViewModel>? DDTransList { get; set; }
    }
}
