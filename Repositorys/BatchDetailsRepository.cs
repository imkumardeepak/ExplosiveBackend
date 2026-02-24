using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class BatchDetailsRepository : GenericRepository<BatchDetails>, IBatchDetailsRepository
	{

		private readonly ApplicationDbContext _context;
		public BatchDetailsRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<BatchDetails> GetBatchDetails(string plantCode, DateTime mfgdt)
		{
			return await _context.BatchDetails.OrderByDescending(b => b.Id)
				.FirstOrDefaultAsync(b => b.PlantCode == plantCode && b.MfgDate.Date == mfgdt.Date);
		}

		public async Task<BatchDetails> GetBatchDetails(string plantCode, string batchNo, DateTime mfgdt)
		{
			return await _context.BatchDetails
				.FirstOrDefaultAsync(b => b.PlantCode == plantCode && b.BatchCode == batchNo && b.MfgDate.Date == mfgdt.Date);
		}

	}
}

