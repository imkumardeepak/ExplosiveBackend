using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	public class Re11IndentInfoRepository : GenericRepository<Re11IndentInfo>, IRe11IndentInfoRepository
	{
		private readonly ApplicationDbContext _context;
		public Re11IndentInfoRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public async Task<Re11IndentInfo> GetByIndentNO(string indentNo)
		{
			return await _context.Re11IndentInfo.Where(p => p.IndentNo == indentNo).Include(p => p.IndentItems).FirstOrDefaultAsync();
		}

		public async Task<List<string>> GetOnlyIndentNo()
		{
			return await _context.Set<Re11IndentInfo>()
								 .Select(e => e.IndentNo)
								 .ToListAsync();
		}

		public async Task<List<Re11IndentInfo>> GetAllIndentInfoByCompflag()
		{
			return await _context.Set<Re11IndentInfo>()
								 .Where(c => c.CompletedIndent == 0).Include(p => p.IndentItems) // Adjust condition as needed
								 .ToListAsync();
		}

		public async Task<List<Re11IndentInfo>> GetAllIndentInfoAlongProduct()
		{
			return await _context.Set<Re11IndentInfo>()
					   .Include(p => p.IndentItems)
								 .ToListAsync();
		}

		public async Task<List<string>> GetIndentsByCName(string cName)
		{
			return await _context.Set<Re11IndentInfo>()
								 .Where(i => i.CustName == cName).Select(e => e.IndentNo) // Filter by CName
								 .ToListAsync();
		}

		public async Task<List<Re11IndentPrdInfo>> GetProductInfoByIndentNo(string indentNo)
		{
			return await _context.Set<Re11IndentPrdInfo>()
								 .Where(p => p.IndentNo == indentNo) // Filter by IndentNo
								 .ToListAsync();
		}



	}
}


