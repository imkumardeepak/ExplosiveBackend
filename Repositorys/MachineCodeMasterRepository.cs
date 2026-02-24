using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{
    public class MachineCodeMasterRepository : GenericRepository<MachineCodeMaster>, IMachineCodeMasterRepository
    {
        private readonly ApplicationDbContext _context;
        public MachineCodeMasterRepository(ApplicationDbContext context) : base(context){
            _context = context;
            }

        public async Task<bool> MachineCodeExistsAsync(string plantCode, string machineCode)
        {
            return await _context.Set<MachineCodeMaster>()
                                 .AnyAsync(m => m.pcode == plantCode && m.mcode == machineCode);
        }

        public async Task<List<MachineCodeMaster>> GetMachineCodesByPlantNameAsync(string plantName)
        {
            return await _context.Set<MachineCodeMaster>()
                .Where(m => m.pname == plantName)
                .ToListAsync();
        }

		public async Task<List<string>> GetByMcodeAsync(string pcode)
		{
			return await _context.MachineCodeMaster
				.Where(p => p.pcode == pcode)
				.Select(p => p.mcode)
				.ToListAsync();
		}

	}

}


