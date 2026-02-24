using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IRoleMasterRepository : IGenericRepository<RoleMaster>
	{

		Task<List<RoleMaster>> GetRoleMaster();

		Task<RoleMaster> Searchbyrole(string role);
	}
}
