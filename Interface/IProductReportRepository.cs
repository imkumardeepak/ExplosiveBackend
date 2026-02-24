using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
   
        public interface IProductReportRepository : IGenericRepository<ProductionReport>
        {
        Task<bool> ProductreportExistsAsync(string ptypeCode, string bname, string psize);

        //Task<List<string>> GetPsizeByBrandAsync(string bname);

        Task<ProductMaster> GetProductDetasAsync(string pcode, string brandid, string productsize);
      //  Task<L1barcodegeneration> GetL1BarcodeDataAsync(string fromDate, string toDate, string shift, string reportType);
        //Task<ProductMaster> GetProductBidAndPsize(string Bname, string psize);
    }
    
}
