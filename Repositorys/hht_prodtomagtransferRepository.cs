using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	public class hht_prodtomagtransferRepository : GenericRepository<hht_prodtomagtransfer>, Ihht_prodtomagtransferRepository
	{
		private readonly ApplicationDbContext _context;
		public hht_prodtomagtransferRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<List<hht_prodtomagtransfer>> GetTransferDataAsync(DateTime fromDate, DateTime toDate, string reportType, string? pcode)
		{
			//var barcodeList = await GetAllAsync(); // Reuse existing fetch

			if (reportType == "Detailed")
			{
				return _context.HHT_prodtomagtransfer
					 .AsEnumerable() // Now we're working in memory, not in SQL
					.Where(x => DateTime.Parse(x.transferdt) >= fromDate && DateTime.Parse(x.transferdt) <= toDate &&
								(string.IsNullOrEmpty(pcode) || x.pcode == pcode))
					.Select(x => new hht_prodtomagtransfer
					{
						tid = x.tid,
						pcode = x.pcode,
						mfgdt = x.mfgdt,
						truckno = x.truckno,
						l1barcode = x.l1barcode,
						lul = x.lul,
						transferdt = x.transferdt,
						month = x.month,
						year = x.year,
						td = x.td,
						brand = x.brand,
						psize = x.psize,
					})
					.OrderBy(x => x.tid)
					.ThenBy(x => x.l1barcode)
					.ThenBy(x => x.brand)
					.ToList();
			}
			else
			{
				return _context.HHT_prodtomagtransfer
					 .AsEnumerable() // Now we're working in memory, not in SQL
					.Where(x => DateTime.Parse(x.transferdt) >= fromDate && DateTime.Parse(x.transferdt) <= toDate &&
								(string.IsNullOrEmpty(pcode) || x.pcode == pcode))
					.GroupBy(x => new { x.pcode, x.brand, x.psize, x.tid, x.truckno, x.l1barcode, x.lul, x.transferdt, x.month, x.year, x.td })
					.Select(g => new hht_prodtomagtransfer
					{
						tid = g.Key.tid,
						pcode = g.Key.pcode,
						mfgdt = g.Select(x => x.mfgdt).FirstOrDefault(),
						lul = g.Key.lul,
						transferdt = g.Key.transferdt,
						month = g.Key.month,
						year = g.Key.year,
						td = g.Key.td,
						brand = g.Key.brand,
						psize = g.Key.psize,
						truckno = g.Key.truckno,
						l1barcode = g.Select(x => x.l1barcode).Distinct().Count().ToString(),
					})
					.OrderBy(x => x.tid)
					.ThenBy(x => x.l1barcode)
					.ThenBy(x => x.brand)
					.ToList();
			}
		}

	}
}


