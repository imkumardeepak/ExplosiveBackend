using Peso_Baseed_Barcode_Printing_System_API.Interface;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class l1barcodereprintRepository : GenericRepository<l1barcodereprint>, Il1barcodereprintRepository
	{
		private readonly ApplicationDbContext _context;
		public l1barcodereprintRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<List<L1ReprintReport>> GetL1ReprintDataAsync(DateTime fromDateTime, DateTime toDateTime, string reportType, string? plant)
		{
			// var data = await _context.l1barcodereprint.ToListAsync();

			if (reportType == "Detailed")
			{
				return _context.L1BarcodeReprint
					 .AsEnumerable() // Now we're working in memory, not in SQL
					 .Where(x => x.rp_dt_time >= fromDateTime && x.rp_dt_time <= toDateTime &&
								 (string.IsNullOrEmpty(plant) || x.plantname == plant))
					 .Select(x => new L1ReprintReport
					 {
						 plant = x.plantname,
						 brandname = x.brandname,
						 productsize = x.productsize,
						 L1Barcode = x.l1barcode,
						 reprintDt = x.rp_dt_time,
						 reason = x.reason,
						 time = x.rp_dt_time.Ticks.ToString()
					 })
					 .OrderBy(x => x.plant)
					 .ThenBy(x => x.L1Barcode)
					 .ThenBy(x => x.brandname)
					 .ToList();
			}
			else
			{
				return _context.L1BarcodeReprint
					 .AsEnumerable() // Now we're working in memory, not in SQL
					.Where(x => x.rp_dt_time >= fromDateTime && x.rp_dt_time <= toDateTime &&
								(string.IsNullOrEmpty(plant) || x.plantname == plant))
					.GroupBy(x => new { x.plantname, x.brandname, x.productsize })
					.Select(g => new L1ReprintReport
					{
						plant = g.Key.plantname,
						brandname = g.Key.brandname,
						productsize = g.Key.productsize,
						L1Barcode = g.Select(x => x.l1barcode).Distinct().Count().ToString(),
					})
					.OrderBy(x => x.brandname)
					.ThenBy(x => x.L1Barcode)
					.ThenBy(x => x.plant)
					.ToList();
			}
		}

	}
}


