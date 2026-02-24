using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Entities
{
    public class FormRE4
    {
        public string? despdate { get; set; }
        public string? re11indent { get; set; }
        public string? truckNo { get; set; }
        public string? trucklic { get; set; }
        public string? brandName { get; set; }
        public string? brandCode { get; set; }
        public string? magname { get; set; }
        public string? maglic { get; set; }
        public string? re12 { get; set; }

        public List<re4dispatch> re4dispatch { get; set; }

    }

    [Keyless]
    public class re4dispatch
    {
        public string? dispdt { get; set; }
        public string? l1barcode { get; set; }

        public string? bname { get; set; }

        public string? brandid { get; set; }

        public string? productsize { get; set; }

        public string? psizecode { get; set; }

        public string? @class { get; set; }

        public string? div { get; set; }

        public string? l1netwt { get; set; }

        public string? cname { get; set; }

        public string? clic { get; set; }

        public string? truckno { get; set; }

        public string? trucklic { get; set; }

        public string? magname { get; set; }

        public string? maglic { get; set; }

        public string? re12 { get; set; }

    }
}
