using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class PlantMasterRepository : GenericRepository<PlantMaster>, IPlantMasterRepository
	{
		private readonly ApplicationDbContext _context;
		public PlantMasterRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<List<string>> GetPlantNamesAsync()
		{
			return await _context.PlantMasters
				.Select(p => p.PName)
				.ToListAsync();
		}

		public async Task<string> GetPlantCodeByNameAsync(string plantName)
		{
			return await _context.PlantMasters
				.Where(p => p.PName == plantName)
				.Select(p => p.PCode)
				.FirstOrDefaultAsync();
		}

		public async Task<bool> CheckPlantCodeExistsAsync(string pcode)
		{
			return await _context.PlantMasters
				.AnyAsync(e => e.PCode == pcode);
		}


		public async Task<List<string>> GetPlantNameByBNameAsync(string brandName)
		{
			return await _context.BrandMaster
				.Where(p => p.bname == brandName)
				.Select(p => p.plant_type)
				.Distinct()
				.ToListAsync();
		}

		public async Task<string> GetPlantCodeByNamemmAsync(List<string> plantName)
		{
			// Example logic: fetch codes for the given plant names
			var codes = _context.PlantMasters
								.Where(p => plantName.Contains(p.PName))
								.Select(p => p.PCode)
								.ToList();

			var commaSeparatedCodes = string.Join(",", codes);

			return commaSeparatedCodes;
		}
	}
}


