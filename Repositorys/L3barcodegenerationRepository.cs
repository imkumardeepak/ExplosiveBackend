using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	/// <summary>
	/// OPTIMIZED L3 Barcode Repository - Uses single scoped DbContext
	/// </summary>
	public class L3barcodegenerationRepository : GenericRepository<L3barcodegeneration>, IL3barcodegenerationRepository
	{
		private readonly ApplicationDbContext _context;

		public L3barcodegenerationRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<L3barcodegeneration> GetL3BarcodeAsync(string l3barcode)
		{
			return await _context.L3Barcodegeneration
							.FirstOrDefaultAsync(x => x.L3Barcode == l3barcode) ?? new L3barcodegeneration();
		}

		public async Task<int> GetMaxSrNoAsync(string pcode, string mcode)
		{
			try
			{
				return await _context.Set<L3barcodegeneration>()
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

		public async Task<List<L3barcodegeneration>> GetL3BarcodesAsync(int? from, int? to, string? plant, string? plantcode, string? mcode)
		{
			return await _context.L3Barcodegeneration
				.Where(x =>
					(!from.HasValue || x.SrNo >= from) &&
					(!to.HasValue || x.SrNo <= to) &&
					(string.IsNullOrEmpty(plant) || x.ParentL1.PlantName == plant) &&
					(string.IsNullOrEmpty(plantcode) || x.ParentL1.PCode == plantcode) &&
					(string.IsNullOrEmpty(mcode) || x.ParentL1.MCode == mcode)
				)
				.OrderBy(x => x.L1Barcode)
				.ThenBy(x => x.L2Barcode)
				.ThenBy(x => x.L3Barcode)
				.ToListAsync();
		}

	}
}


