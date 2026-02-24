using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface ITransportMasterRepository : IGenericRepository<TransportMaster>
	{
		Task<TransportViewModel> GetTransportMasterViewModelByIdAsync(int transportMasterId);

		Task<List<TransportMaster>> GetAllTransportMasterWithDetails();

		Task UpsertAndRemoveMembersAsync(IEnumerable<TransportMemberViewModel> memberViewModels, int transportMasterId);

		Task UpsertAndRemoveVehiclesAsync(IEnumerable<TransportVehicleViewModel> vehicleViewModels, int transportMasterId);

		Task<List<TransVehicleDetail>> GetTransportVehicleDetailsByTransporterNameAsync(string transporterName);
	}
}
