using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class ProductionPlanRepository : GenericRepository<ProductionPlan>, IProductionPlanRepository
	{
		private readonly ApplicationDbContext _context;
		public ProductionPlanRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<string> GetLastProductionPlanNoForMonthAsync(int year, int month)
		{
			// Get the start and end of the month
			var startDate = new DateTime(year, month, 1);
			var endDate = startDate.AddMonths(1).AddDays(-1);

			// Create the prefix for the month (e.g., PP-2601)
			var yearPrefix = year.ToString().Substring(2, 2);
			var monthPrefix = month.ToString("D2");
			var prefix = $"PP-{yearPrefix}{monthPrefix}";

			// Get the last production plan number for the current month
			var lastPlanNo = await _context.ProductionPlan
				.Where(p => p.ProductionPlanNo != null && p.ProductionPlanNo.StartsWith(prefix))
				.OrderByDescending(p => p.ProductionPlanNo)
				.Select(p => p.ProductionPlanNo)
				.FirstOrDefaultAsync();

			return lastPlanNo;
		}
	}
}

