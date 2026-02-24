using AutoMapper;
using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Services
{
	public class BatchMasterService : IBatchMasterService
	{
		private readonly IBatchMasterRepository _repository;
		private readonly IMapper _mapper;

		public BatchMasterService(IBatchMasterRepository repository, IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<BatchMasters>> GetAllBatchMastersAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<BatchMasters> GetBatchMasterByIdAsync(int id)
		{
			return await _repository.GetByIdAsync(id);
		}

		public async Task<BatchMasters> GetBatchMasterByPlantCodeAsync(string plantCode)
		{
			return await _repository.GetBatchMasterByPlantCodeAsync(plantCode);
		}

		public async Task<APIResponse> CreateBatchMasterAsync(BatchMasters batchMasterDTO)
		{
			var response = new APIResponse();
			var exist = await _repository.GetBatchMasterByPlantCodeAsync(batchMasterDTO.PlantCode);
			
			if (exist != null)
			{
				response.Status = false;
				response.Errors.Add("Batch Master already exists.");
				response.Message = "Batch Master already exists.";
				response.StatusCode = HttpStatusCode.BadRequest;
				return response;
			}

			var batchMaster = _mapper.Map<BatchMasters>(batchMasterDTO);
			await _repository.AddAsync(batchMaster);
			
			response.Data = batchMaster;
			response.Status = true;
			response.Message = "Batch Master created successfully.";
			response.StatusCode = HttpStatusCode.OK;
			return response;
		}

		public async Task<APIResponse> UpdateBatchMasterAsync(BatchMasters batchMasterDTO)
		{
			var response = new APIResponse();
			var batchMaster = _mapper.Map<BatchMasters>(batchMasterDTO);
			await _repository.UpdateAsync(batchMaster);
			
			response.Data = batchMaster;
			response.Status = true;
			response.Message = "Batch Master updated successfully.";
			response.StatusCode = HttpStatusCode.OK;
			return response;
		}

		public async Task<APIResponse> DeleteBatchMasterAsync(int id)
		{
			var response = new APIResponse();
			var batchMaster = await _repository.GetByIdAsync(id);
			if (batchMaster == null)
			{
				response.Status = false;
				response.Message = "Batch Master not found.";
				response.Errors.Add($"Batch Master with ID {id} does not exist.");
				response.StatusCode = HttpStatusCode.NotFound;
				return response;
			}
			
			await _repository.DeleteAsync(id);
			response.Status = true;
			response.Data = true;
			response.Message = "Batch Master deleted successfully.";
			response.StatusCode = HttpStatusCode.OK;
			return response;
		}
	}
}
