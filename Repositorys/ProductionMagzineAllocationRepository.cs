using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class ProductionMagzineAllocationRepository : GenericRepository<ProductionMagzineAllocation>, IProductionMagzineAllocationRepository
	{

		private readonly ApplicationDbContext _context;
		public ProductionMagzineAllocationRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}
		public async Task<List<ProductionMagzineAllocation>> GetAllProductionMagzineAllocationsAsync(int flag, DateTime? fromDate = null, DateTime? toDate = null)
		{
			var query = _context.ProductionMagzineAllocations
				.Include(p => p.TransferToMazgnieScanneddata)
				.Where(p => p.ReadFlag == flag)
				.AsQueryable();

			if (fromDate.HasValue)
			{
				query = query.Where(p => p.TransferDate >= fromDate.Value);
			}

			if (toDate.HasValue)
			{
				query = query.Where(p => p.TransferDate <= toDate.Value);
			}

			return await query.ToListAsync();
		}


		public async Task<int> MaxCountAsync(DateTime now)
		{
			var maxCount = await _context.ProductionTransfers
				.Where(p => p.months == now.Month && p.years == now.Year)
				.CountAsync() + 1;

			if (maxCount >= 1000)
			{
				throw new InvalidOperationException("Maximum count for the day reached.");
			}

			return maxCount;
		}


		public async Task<List<ProductionMagzineAllocation>> GetScannedDataByTransferIdAsync(string transferId)
		{
			return await _context.ProductionMagzineAllocations
				.Where(p => p.TransferId == transferId)
				.Include(p => p.TransferToMazgnieScanneddata)
				.ToListAsync();
		}
		public async Task UpdateReadFlagAsync(int id, int readFlag)
		{
			var allocation = await _context.ProductionMagzineAllocations.FindAsync(id);
			if (allocation != null)
			{
				allocation.ReadFlag = readFlag;
				await _context.SaveChangesAsync();
			}
		}
	}
}


