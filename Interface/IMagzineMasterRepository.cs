using Microsoft.AspNetCore.Mvc;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IMagzineMasterRepository : IGenericRepository<MagzineMaster>
	{
		Task<bool> MagazineExistsAsync(string magazineName);
		// Add the new method to fetch a magazine by its mcode
		Task<MagzineMaster> GetMagazineByMCodeAsync(string mcode);

		Task<List<string>> GetmagzineNamesAsync();
		Task<string> GetLicByMCodeAsync(string mcode);

		Task<List<MagzineMaster>> GetMagDetails();

		Task<MagzineMaster> GetMagDetailsbyID(int id);


    }
}
