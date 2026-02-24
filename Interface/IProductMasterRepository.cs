using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IProductMasterRepository : IGenericRepository<ProductMaster>
	{
		Task<List<ProductMaster>> GetAllAsync();

		Task<bool> ProductExistsAsync(string ptypeCode, string bname, string psize);


		Task<List<string>> GetPsizeNamesByBrandAndPCodeAsync(string brandid, string pcode);

		Task<ProductMaster> GetProductDetailsAsync(string pcode, string brandid, string productsize);

		Task<ProductMaster> GetProductByBidAndPsize(string Bname, string psize);

		Task<List<string>> GetPsizeByBrandAsync(string bname);

		Task<List<string>> GetPsizebybrand(string BrandId);
		Task<List<string>> GetPsizebypcode(string productsize);

		string GetSize(string scode);
		Task<string> GetBrandnameAsync(string bid);
		Task<string> GetSizeAsync(string scode);
		string GetBrand(string bid);
	}
}
