using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class ProductReportRepository : GenericRepository<ProductionReport>, IProductReportRepository
	{
		private readonly ApplicationDbContext _context;
		public ProductReportRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<bool> ProductreportExistsAsync(string ptypeCode, string bname, string psize)
		{
			return await _context.ProductMaster.AnyAsync(pm =>
				pm.ptype == ptypeCode &&
				pm.bname == bname &&
				pm.psize == psize);
		}

		//public async Task<List<string>> GetPsizeByBrandAsync(string bname)
		//{
		//	try
		//	{

		//		var psizeNames = await _context.ProductMaster
		//			.Where(pm => pm.bname.Equals(bname, StringComparison.OrdinalIgnoreCase))
		//			.Select(pm => pm.psize)
		//			.Distinct()
		//			.ToListAsync();
		//		return psizeNames;
		//	}
		//	catch (Exception ex)
		//	{
		//		// Handle exception
		//		throw new Exception($"An error occurred while fetching psize names: {ex.Message}");
		//	}
		//}

		public async Task<ProductMaster> GetProductDetasAsync(string pcode, string brandid, string productsize)
		{
			try
			{
				// Query the database for the product details
				var product = await _context.ProductMaster
											.FirstOrDefaultAsync(pm =>
												pm.ptypecode == pcode &&
												pm.bid == brandid &&
												pm.psize == productsize);

				return product;
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				throw new Exception($"An error occurred while fetching product details: {ex.Message}");
			}
		}
		//public async Task<List<L1barcodegeneration>> GetL1BarcodeDataAsync(string fromDate, string toDate, string shift, string reportType)
		//{
		//    DateTime fromDateTime = DateTime.Parse(fromDate);
		//    DateTime toDateTime = DateTime.Parse(toDate);

		//    return await _context.L1Barcodegeneration
		//        .Where(x => DateTime.Parse(x.MfgDt.ToString()) >= fromDateTime && DateTime.Parse(x.MfgDt.ToString()) <= toDateTime && x.Shift == shift)
		//        .Select(x => new L1barcodegeneration
		//        {
		//            PlantName = x.PlantName,
		//            Shift = x.Shift,
		//            BrandName = x.BrandName,
		//            ProductSize = x.ProductSize,
		//            L1Barcode = x.L1Barcode,
		//            L1NetWt = x.L1NetWt,
		//            L1NetUnit = x.L1NetUnit,
		//            // BoxCounts = x.BoxCounts,
		//            // Qty = x.Qty,
		//            // Unit = x.Unit
		//        })
		//        .ToListAsync();
		//}


	}
}


