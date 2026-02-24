using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	public class MagzineviewmodelRepository : GenericRepository<MagzineMasterDetails>, IMagzineviewmodelRepository
	{
		private readonly ApplicationDbContext _context;
		public MagzineviewmodelRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}
		public async Task<List<MagzineMasterDetails>> GetByMagcodeAsync(string magcode)
		{
			return await _context.MagzineMasterDetails

				.ToListAsync();
		}


	}
}



