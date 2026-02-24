using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Globalization;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IBatchDetailsRepository : IGenericRepository<BatchDetails>
	{

		Task<BatchDetails> GetBatchDetails(string plantCode,DateTime mfgdt);

		Task<BatchDetails> GetBatchDetails(string plantCode, string batchNo, DateTime mfgdt);
	}
}
