using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class BatchMasterRepository : GenericRepository<BatchMasters>, IBatchMasterRepository
	{
		private readonly ApplicationDbContext _context;
		public BatchMasterRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<BatchMasters> GetBatchMasterByPlantCodeAsync(string plantCode)
		{
			return await _context.BatchMasters.AsNoTracking().FirstOrDefaultAsync(b => b.PlantCode == plantCode);
		}

		public async Task<BatchMasters> GetBatchMasterByBatchCodeAsync(string batchCode)
		{
			return await _context.BatchMasters.AsNoTracking().FirstOrDefaultAsync(b => b.BatchCode == batchCode);
		}

		public async Task<BatchMasters> GetBatchMasterByPlantCodeAndBatchType(string plantcode, string batchtype)
		{
			return await _context.BatchMasters.AsNoTracking()
				.FirstOrDefaultAsync(b => b.PlantCode == plantcode && b.BatchType.ToString() == batchtype);
		}
	}
}

