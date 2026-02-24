using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Peso_Based_Barcode_Printing_System_APIBased.Models;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Services
{
	public class ProductionPlanService : IProductionPlanService
	{
		private readonly IProductionPlanRepository _productionPlanRepository;
		private readonly IMapper _mapper;
		private APIResponse _APIResponse;

		public ProductionPlanService(IProductionPlanRepository productionPlanRepository, IMapper mapper)
		{
			_productionPlanRepository = productionPlanRepository;
			_mapper = mapper;
			_APIResponse = new APIResponse();
		}

		public async Task<APIResponse> GetAllProductionPlansAsync()
		{
			try
			{
				var productionPlans = await _productionPlanRepository.GetAllAsync();
				var productionPlanDtos = _mapper.Map<List<ProductionPlanDto>>(productionPlans);

				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Data = productionPlanDtos;
				_APIResponse.Message = "Production plans retrieved successfully.";

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error retrieving production plans.";
				_APIResponse.Errors.Add(ex.Message);

				return _APIResponse;
			}
		}

		public async Task<APIResponse> GetProductionPlanByIdAsync(int id)
		{
			try
			{
				var productionPlan = await _productionPlanRepository.GetFirstOrDefaultAsync(x => x.Id == id);

				if (productionPlan == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Message = "Production plan not found.";
					_APIResponse.Errors.Add("Production plan with the specified ID does not exist.");

					return _APIResponse;
				}

				var productionPlanDto = _mapper.Map<ProductionPlanDto>(productionPlan);

				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Data = productionPlanDto;
				_APIResponse.Message = "Production plan retrieved successfully.";

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error retrieving production plan.";
				_APIResponse.Errors.Add(ex.Message);

				return _APIResponse;
			}
		}

		public async Task<APIResponse> CreateProductionPlanAsync(CreateProductionPlanRequestDto requestDto)
		{
			try
			{
				var existingPlan = await _productionPlanRepository.GetFirstOrDefaultAsync(x =>
					x.MfgDt == requestDto.MfgDt &&
					x.PlantCode == requestDto.PlantCode &&
					x.BrandId == requestDto.BrandId &&
					x.PSizeCode == requestDto.PSizeCode);

				if (existingPlan != null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Message = "A production plan with the same manufacturing date, plant code, brand ID, and product size code already exists.";
					_APIResponse.Errors.Add("Duplicate production plan detected.");

					return _APIResponse;
				}

				var productionPlan = _mapper.Map<ProductionPlan>(requestDto);
				
				// Auto-generate ProductionPlanNo if not provided
				if (string.IsNullOrEmpty(productionPlan.ProductionPlanNo))
				{
					productionPlan.ProductionPlanNo = await GenerateProductionPlanNoInternalAsync();
				}
				
				productionPlan.CratedDate = DateTime.Now;

				await _productionPlanRepository.AddAsync(productionPlan);

				var productionPlanDto = _mapper.Map<ProductionPlanDto>(productionPlan);

				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Data = productionPlanDto;
				_APIResponse.Message = "Production plan created successfully.";

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error creating production plan.";
				_APIResponse.Errors.Add(ex.Message);

				return _APIResponse;
			}
		}

		public async Task<APIResponse> UpdateProductionPlanAsync(UpdateProductionPlanRequestDto requestDto)
		{
			try
			{
				var existingPlan = await _productionPlanRepository.GetFirstOrDefaultAsync(x => x.Id == requestDto.Id);

				if (existingPlan == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Message = "Production plan not found.";
					_APIResponse.Errors.Add("Production plan with the specified ID does not exist.");

					return _APIResponse;
				}

				// Check if there's another plan with the same unique combination
				var duplicatePlan = await _productionPlanRepository.GetFirstOrDefaultAsync(x =>
					x.Id != requestDto.Id &&
					x.MfgDt == requestDto.MfgDt &&
					x.PlantCode == requestDto.PlantCode &&
					x.BrandId == requestDto.BrandId &&
					x.PSizeCode == requestDto.PSizeCode);

				if (duplicatePlan != null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Message = "A production plan with the same manufacturing date, plant code, brand ID, and product size code already exists.";
					_APIResponse.Errors.Add("Duplicate production plan detected.");

					return _APIResponse;
				}

				_mapper.Map(requestDto, existingPlan);
				existingPlan.CratedDate = DateTime.Now; // Update the created date to current time

				await _productionPlanRepository.UpdateAsync(existingPlan);

				var productionPlanDto = _mapper.Map<ProductionPlanDto>(existingPlan);

				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Data = productionPlanDto;
				_APIResponse.Message = "Production plan updated successfully.";

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error updating production plan.";
				_APIResponse.Errors.Add(ex.Message);

				return _APIResponse;
			}
		}

		public async Task<APIResponse> DeleteProductionPlanAsync(int id)
		{
			try
			{
				var existingPlan = await _productionPlanRepository.GetFirstOrDefaultAsync(x => x.Id == id);

				if (existingPlan == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Message = "Production plan not found.";
					_APIResponse.Errors.Add("Production plan with the specified ID does not exist.");

					return _APIResponse;
				}

				await _productionPlanRepository.DeleteAsync(existingPlan.Id);

				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Message = "Production plan deleted successfully.";

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error deleting production plan.";
				_APIResponse.Errors.Add(ex.Message);

				return _APIResponse;
			}
		}

		public async Task<APIResponse> SearchProductionPlansAsync(SearchProductionPlanRequestDto searchDto)
		{
			try
			{
				var productionPlans = await _productionPlanRepository.GetAllAsync();
				var productionPlanDtos = _mapper.Map<List<ProductionPlanDto>>(productionPlans);

				var filteredPlans = productionPlanDtos.AsQueryable();

				if (searchDto.MfgDt.HasValue)
					filteredPlans = filteredPlans.Where(p => p.MfgDt.Date == searchDto.MfgDt.Value.Date);

				if (!string.IsNullOrEmpty(searchDto.PlantCode))
					filteredPlans = filteredPlans.Where(p => p.PlantCode.Equals(searchDto.PlantCode, StringComparison.OrdinalIgnoreCase));

				if (!string.IsNullOrEmpty(searchDto.BrandId))
					filteredPlans = filteredPlans.Where(p => p.BrandId.Equals(searchDto.BrandId, StringComparison.OrdinalIgnoreCase));

				if (!string.IsNullOrEmpty(searchDto.PSizeCode))
					filteredPlans = filteredPlans.Where(p => p.PSizeCode.Equals(searchDto.PSizeCode, StringComparison.OrdinalIgnoreCase));

				if (searchDto.MinTotalWeight.HasValue)
					filteredPlans = filteredPlans.Where(p => p.TotalWeight >= searchDto.MinTotalWeight.Value);

				if (searchDto.MaxTotalWeight.HasValue)
					filteredPlans = filteredPlans.Where(p => p.TotalWeight <= searchDto.MaxTotalWeight.Value);

				if (searchDto.MinNoOfBox.HasValue)
					filteredPlans = filteredPlans.Where(p => p.NoOfbox >= searchDto.MinNoOfBox.Value);

				if (searchDto.MaxNoOfBox.HasValue)
					filteredPlans = filteredPlans.Where(p => p.NoOfbox <= searchDto.MaxNoOfBox.Value);

				if (searchDto.MinNoOfStickers.HasValue)
					filteredPlans = filteredPlans.Where(p => p.NoOfstickers >= searchDto.MinNoOfStickers.Value);

				if (searchDto.MaxNoOfStickers.HasValue)
					filteredPlans = filteredPlans.Where(p => p.NoOfstickers <= searchDto.MaxNoOfStickers.Value);

				if (searchDto.CreatedDateFrom.HasValue)
					filteredPlans = filteredPlans.Where(p => p.CratedDate >= searchDto.CreatedDateFrom.Value);

				if (searchDto.CreatedDateTo.HasValue)
					filteredPlans = filteredPlans.Where(p => p.CratedDate <= searchDto.CreatedDateTo.Value);

				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Data = filteredPlans.ToList();
				_APIResponse.Message = "Production plans retrieved successfully.";

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error searching production plans.";
				_APIResponse.Errors.Add(ex.Message);

				return _APIResponse;
			}
		}

		public async Task<APIResponse> GenerateProductionPlanNoAsync()
		{
			try
			{
				var productionPlanNo = await GenerateProductionPlanNoInternalAsync();

				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Data = new { ProductionPlanNo = productionPlanNo };
				_APIResponse.Message = "Production plan number generated successfully.";

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error generating production plan number.";
				_APIResponse.Errors.Add(ex.Message);

				return _APIResponse;
			}
		}

		private async Task<string> GenerateProductionPlanNoInternalAsync()
		{
			var currentDate = DateTime.Now;
			var year = currentDate.Year;
			var month = currentDate.Month;

			// Get the last production plan number for current month
			var lastPlanNo = await _productionPlanRepository.GetLastProductionPlanNoForMonthAsync(year, month);

			int nextSerial = 1;
			if (!string.IsNullOrEmpty(lastPlanNo))
			{
				// Extract the serial number from the last plan number (e.g., PP-2601001 -> 001)
				var serialPart = lastPlanNo.Substring(lastPlanNo.Length - 3);
				if (int.TryParse(serialPart, out int lastSerial))
				{
					nextSerial = lastSerial + 1;
				}
			}

			// Format: PP-YYMM001
			var yearPrefix = year.ToString().Substring(2, 2);
			var monthPrefix = month.ToString("D2");
			var serialNumber = nextSerial.ToString("D3");

			return $"PP-{yearPrefix}{monthPrefix}{serialNumber}";
		}
	}
}
