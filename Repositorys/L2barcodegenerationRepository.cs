using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;


namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	/// <summary>
	/// OPTIMIZED L2 Barcode Repository - Uses single scoped DbContext
	/// </summary>
	public class L2barcodegenerationRepository : GenericRepository<L2barcodegeneration>, IL2barcodegenerationRepository
	{
		private readonly ApplicationDbContext _context;

		public L2barcodegenerationRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}
		public async Task<L2barcodegeneration> GetL2BarcodesAsync(string l2barcode)
		{
			return await _context.L2Barcodegeneration.Where(x => x.L2Barcode.Trim() == l2barcode.Trim())
				.OrderBy(x => x.L2Barcode)
				.ThenBy(x => x.SrNo)
				.FirstOrDefaultAsync() ?? new L2barcodegeneration();
		}

		public async Task<int> GetMaxSrNoAsync(string pcode, string mcode)
		{
			try
			{
				return await _context.Set<L2barcodegeneration>()
					.Where(l => l.ParentL1.PCode == pcode
								&& l.ParentL1.MCode == mcode)
					.MaxAsync(l => (int?)l.SrNo + 1) ?? 1; // Returns 0 if no matching rows   
			}
			catch (Exception ex)
			{
				// Log the exception
				Console.WriteLine($"Error fetching max SrNo: {ex.Message}");
				throw;
			}
		}


		public async Task<List<L2barcodegeneration>> GetReprintBarcodesAsync(DateTime mgfdate, string plant, string pcode, string mcode, int fromsrno, int tosrno, string l2barcode)
		{
			//IQueryable<L2barcodegeneration> query = _context.L2barcodegenerations.AsQueryable();

			if (l2barcode != null)
			{
				var query = _context.L2Barcodegeneration
					 .Where(x => x.L2Barcode.Trim() == l2barcode.Trim())
					 .OrderBy(x => x.L2Barcode)
					 .ThenBy(x => x.SrNo);
				return await query.ToListAsync();
			}
			else
			{
				var query = _context.L2Barcodegeneration
					  .Where(x =>
						x.ParentL1.PlantName.Trim() == plant.Trim() &&
						x.ParentL1.PCode.Trim() == pcode.Trim() &&
						x.ParentL1.MCode.Trim() == mcode.Trim() &&
						x.SrNo >= fromsrno && x.SrNo <= tosrno)
					.OrderBy(x => x.L2Barcode)
					.ThenBy(x => x.SrNo);
				return await query.ToListAsync();
			}


		}
	}
}


