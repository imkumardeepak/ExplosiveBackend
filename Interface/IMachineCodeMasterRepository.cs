using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IMachineCodeMasterRepository :IGenericRepository<MachineCodeMaster>
    {
        Task<bool> MachineCodeExistsAsync(string plantCode, string machineCode);

        Task<List<MachineCodeMaster>> GetMachineCodesByPlantNameAsync(string plantName);

		Task<List<string>> GetByMcodeAsync(string pcode);
	}
}
