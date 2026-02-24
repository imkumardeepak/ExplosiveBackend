using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class AllLoadingSheetRepository : GenericRepository<AllLoadingSheet>, IAllLoadingSheetRepository
	{
		private readonly ApplicationDbContext _context;
		public AllLoadingSheetRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<AllLoadingSheet> GetExistingLoadingSheetAsync(string indentNo, string brandId, string sizeCode, int compFlag)
		{
			return await _context.AllLoadingSheet
				.FirstOrDefaultAsync(ls => ls.Compflag == compFlag);
		}

		public async Task<int> GetLastCOUNT(int months, int years)
		{
			return await _context.AllLoadingSheet
				.Where(ls => ls.Month == months && ls.Year == years)
				.CountAsync();
		}
		public async Task<List<AllLoadingSheet>> GetLoadingByCflag(int compFlag, DateTime? fromDate = null, DateTime? toDate = null)
		{
			var query = _context.AllLoadingSheet
				.Include(ls => ls.IndentDetails)
				.Where(ls => ls.Compflag == compFlag);

			if (fromDate.HasValue)
				query = query.Where(ls => ls.Mfgdt >= fromDate.Value.Date);

			if (toDate.HasValue)
				query = query.Where(ls => ls.Mfgdt <= toDate.Value.Date);

			return await query.ToListAsync();
		}


		public async Task<List<AllLoadingSheet>> GetLoadingSheet()
		{
			return await _context.AllLoadingSheet.Include(ls => ls.IndentDetails)
				.ToListAsync();
		}

		public async Task<AllLoadingSheet> GetLoadingByLoadingSheet(string loadingSheet)
		{
			return await _context.AllLoadingSheet.Include(ls => ls.IndentDetails)
				.FirstOrDefaultAsync(ls => ls.LoadingSheetNo == loadingSheet);
		}

		public async Task UpdateCompFlagAsync(string loadingSheet,string indentno, string truckno, int compflag)
		{

			try
			{

				var existingLoadingSheet = await _context.AllLoadingSheet.Include(ls => ls.IndentDetails).FirstOrDefaultAsync(ls => ls.LoadingSheetNo == loadingSheet);


				if (existingLoadingSheet.IndentDetails != null)
					existingLoadingSheet.Compflag = existingLoadingSheet.IndentDetails.All(x => x.iscompleted == 2) ? 2 : 1;

                await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				// Handle any exceptions (e.g., log the error)
				throw new Exception("Error updating RE2Flag in BarcodeData.", ex);
			}
		}

	}
}


