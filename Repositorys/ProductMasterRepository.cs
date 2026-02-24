using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class ProductMasterRepository : GenericRepository<ProductMaster>, IProductMasterRepository
	{
		private readonly ApplicationDbContext _context;
		public ProductMasterRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<bool> ProductExistsAsync(string ptypeCode, string bname, string psize)
		{
			return await _context.ProductMaster.AnyAsync(pm =>
				pm.ptype == ptypeCode &&
				pm.bname == bname &&
				pm.psize == psize);
		}

		public async Task<List<string>> GetPsizeNamesByBrandAndPCodeAsync(string brandid, string pcode)
		{
			try
			{
				// Query the ProductMaster table to get distinct psize values by brandid and pcode
				var psizeNames = await _context.ProductMaster.Where(pm => pm.bid == brandid && pm.ptypecode == pcode).Select(pm => pm.psize).Distinct().ToListAsync();

				return psizeNames;
			}
			catch (Exception ex)
			{
				// Handle exception
				throw new Exception($"An error occurred while fetching psize names: {ex.Message}");
			}
		}

		public async Task<ProductMaster> GetProductDetailsAsync(string pcode, string brandid, string productsize)
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

		public async Task<ProductMaster> GetProductByBidAndPsize(string brandid, string productsize)
		{

			try
			{
				// Query the database for the product details
				var product = await _context.ProductMaster
											.FirstOrDefaultAsync(pm =>
												pm.bid == brandid &&
												pm.psizecode == productsize);

				return product;
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				throw new Exception($"An error occurred while fetching product details: {ex.Message}");
			}
		}

		public async Task<List<string>> GetPsizeByBrandAsync(string bname)
		{
			try
			{

				var psizeNames = await _context.ProductMaster
					.Where(pm => pm.bname == bname)
					.Select(pm => pm.psize)
					.Distinct()
					.ToListAsync();
				return psizeNames;
			}
			catch (Exception ex)
			{
				// Handle exception
				throw new Exception($"An error occurred while fetching psize names: {ex.Message}");
			}
		}

		public async Task<List<string>> GetPsizebybrand(string BrandId)
		{
			try
			{

				var psizeNames = await _context.ProductMaster
					.Where(pm => pm.bid == BrandId)
					.Select(pm => pm.psize)
					.Distinct()
					.ToListAsync();
				return psizeNames;
			}
			catch (Exception ex)
			{
				// Handle exception
				throw new Exception($"An error occurred while fetching psize names: {ex.Message}");
			}
		}
		public async Task<List<string>> GetPsizebypcode(string productsize)
		{
			try
			{

				var psizeNames = await _context.ProductMaster
					.Where(pm => pm.psize == productsize)
					.Select(pm => pm.psizecode)
					.Distinct()
					.ToListAsync();
				return psizeNames;
			}
			catch (Exception ex)
			{
				// Handle exception
				throw new Exception($"An error occurred while fetching psize names: {ex.Message}");
			}
		}


		public string GetSize(string scode)
		{
			return _context.ProductMaster
						   .Where(p => p.psizecode == scode)
						   .Select(p => p.psize)
						   .Distinct()
						   .FirstOrDefault() ?? "";
		}

		public string GetBrand(string bid)
		{
			return _context.BrandMaster
						   .Where(b => b.bid == bid)
						   .Select(b => b.bname)
						   .FirstOrDefault() ?? "";
		}

		public async Task<string> GetBrandnameAsync(string brandId)
		{
			return await _context.ProductMaster
								 .Where(p => p.bid == brandId)
								 .Select(p => p.bname)
								 .Distinct()
								 .FirstOrDefaultAsync();
		}

		public async Task<string> GetSizeAsync(string scode)
		{
			return await _context.ProductMaster
								 .Where(p => p.psizecode == scode)
								 .Select(p => p.psize)
								 .Distinct()
								 .FirstOrDefaultAsync();
		}

		public async Task<List<ProductMaster>> GetAllAsync()
		{
			return await _context.ProductMaster.ToListAsync();
		}
	}
}

