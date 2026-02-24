using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	public class MagzineMasterRepository : GenericRepository<MagzineMaster>, IMagzineMasterRepository
	{
		private readonly ApplicationDbContext _context;
		public MagzineMasterRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<bool> MagazineExistsAsync(string magazineName)
		{
			return await _context.Set<MagzineMaster>()
								 .AnyAsync(m => m.magname == magazineName);
		}

		public async Task<MagzineMaster> GetMagazineByMCodeAsync(string mcode)
		{
			// Fetch the magazine by its mcode
			return await _context.MagzineMasters
								 .FirstOrDefaultAsync(m => m.mcode == mcode);
		}

		public async Task<List<MagzineMaster>> GetMagDetails()
		{
			return await _context.MagzineMasters.Include(a => a.MagzineMasterDetails).AsNoTracking().ToListAsync();
		}
        public async Task<MagzineMaster> GetMagDetailsbyID(int id)
        {
            return await _context.MagzineMasters.Where(a => a.Id == id).Include(a => a.MagzineMasterDetails).FirstOrDefaultAsync();
        }


        public async Task<List<string>> GetmagzineNamesAsync()
		{
			return await _context.MagzineMasters
				.Select(p => p.mcode)
				.ToListAsync();
		}


		public async Task<string> GetLicByMCodeAsync(string mcode)
		{
			var loadingSheet = await _context.MagzineMasters
				.Where(b => b.mcode == mcode)
				.Select(b => b.licno)
				.FirstOrDefaultAsync();

			return loadingSheet ?? "Not Found";
		}

		public async Task AddMagzinesAsync(List<MagzineMaster> magzines)
		{
			await _context.MagzineMasters.AddRangeAsync(magzines);
			await _context.SaveChangesAsync();
		}

	}

}


