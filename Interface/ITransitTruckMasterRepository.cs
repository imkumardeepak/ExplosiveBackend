using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{

	public interface ITransitTruckMasterRepository : IGenericRepository<TransitTruckMaster>
    {
        //Task<TransitTruckViewModel> GetTransportMasterViewModelByIdAsync(int transportMasterId);
        Task<TransitTruckViewModel> GetTransportMasterViewModelByIdAsync(int transportMasterId);
        Task InsertAndRemoveMembersAsync(IEnumerable<TransitMemberViewModel> memberViewModels, int transportMasterId);

        Task InsertAndRemoveVehiclesAsync(IEnumerable<TransitTruckDetailViewModel> vehicleViewModels, int transportMasterId);

        Task<List<TransitTruckDetails>> GetTransittruckDetailsAsync(string transporterName);
    }
}

