using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using System.Linq.Expressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface ICustomerMastersRepository : IGenericRepository<CustomerMaster>
	{
		Task<CustomerViewModel> GetCustomerViewModelByIdAsync(int customerId);


		Task UpsertAndRemoveMembersAsync(IEnumerable<CustMemberViewModel> memberViewModels, int customerId);
		Task UpsertAndRemoveMagazinesAsync(IEnumerable<CustMagazineViewModel> magazineViewModels, int customerId);

		Task<CustomerMaster> GetCustomerMasterByCname(string Customername);

		Task<List<string>> GetcustoNamesAsync();

		Task<List<CustomerMaster>> GetCustomerDetailsWithAllDetails();

		int GetMaxSrno();

	}
}
