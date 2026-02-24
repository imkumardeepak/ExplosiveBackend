using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
    public interface IProductionPlanService
    {
        Task<APIResponse> GetAllProductionPlansAsync();
        Task<APIResponse> GetProductionPlanByIdAsync(int id);
        Task<APIResponse> CreateProductionPlanAsync(CreateProductionPlanRequestDto requestDto);
        Task<APIResponse> UpdateProductionPlanAsync(UpdateProductionPlanRequestDto requestDto);
        Task<APIResponse> DeleteProductionPlanAsync(int id);
        Task<APIResponse> SearchProductionPlansAsync(SearchProductionPlanRequestDto searchDto);
        Task<APIResponse> GenerateProductionPlanNoAsync();
    }
}
