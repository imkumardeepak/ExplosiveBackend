using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
	public class RoleMasterRepository : GenericRepository<RoleMaster>, IRoleMasterRepository
	{
		private readonly IMapper _mapper1;
		private readonly ApplicationDbContext _context;
		public RoleMasterRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<List<RoleMaster>> GetRoleMaster()
		{

			return await _context.RoleMasters.Include(x => x.pageAccesses).ToListAsync();

		}
		public async Task<RoleMaster> Searchbyrole(string role)
		{
			return await _context.RoleMasters
				.Include(x => x.pageAccesses)
				.FirstOrDefaultAsync(x => x.RoleName == role);
		}
	}
}


