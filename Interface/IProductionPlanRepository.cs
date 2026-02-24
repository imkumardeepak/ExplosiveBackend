using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IProductionPlanRepository : IGenericRepository<ProductionPlan>
	{
		Task<string> GetLastProductionPlanNoForMonthAsync(int year, int month);
	}
}
