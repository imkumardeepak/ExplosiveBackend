using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class BatchMastersController : ControllerBase
	{
		private readonly IBatchMasterService _batchMasterService;
		private APIResponse _APIResponse;

		public BatchMastersController(IBatchMasterService batchMasterService)
		{
			_batchMasterService = batchMasterService;
			_APIResponse = new();
		}
		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetBatchMasters()
		{
			try
			{
				var data = await _batchMasterService.GetAllBatchMastersAsync();
				_APIResponse.Data = data;
				_APIResponse.Status = true;
				_APIResponse.Message = "Batch Masters fetched successfully.";
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetBatchMaster(int id)
		{
			try
			{
				var data = await _batchMasterService.GetBatchMasterByIdAsync(id);
				if (data == null)
				{
                     _APIResponse.Status = false;
					_APIResponse.Message = "Batch Master not found.";
					_APIResponse.Errors.Add($"Batch Master with ID {id} does not exist.");
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_APIResponse);

				}
				_APIResponse.Data = data;
				_APIResponse.Status = true;
				_APIResponse.Message = "Batch Master fetched successfully.";
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateBatchMaster([FromBody] BatchMasters batchMasterDTO)
		{
			try
			{
				if (batchMasterDTO == null)
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "Batch Master object is null.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_APIResponse);
				}
				if (!ModelState.IsValid)
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "Invalid Batch Master object.";
					_APIResponse.Errors.AddRange(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_APIResponse);
				}

				var createResp = await _batchMasterService.CreateBatchMasterAsync(batchMasterDTO);
				if (!createResp.Status)
				{
					return BadRequest(createResp);
				}

				return Ok(createResp);
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateBatchMaster(int id, [FromBody] BatchMasters batchMasterDTO)
		{
			try
			{
				if (batchMasterDTO == null || id != batchMasterDTO.Id)
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "Batch Master object is null or ID does not match.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_APIResponse);
				}

				var updateResp = await _batchMasterService.UpdateBatchMasterAsync(batchMasterDTO);
				return Ok(updateResp);
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteBatchMaster(int id)
		{
			try
			{
				var deleteResp = await _batchMasterService.DeleteBatchMasterAsync(id);
				if (!deleteResp.Status)
				{
					return NotFound(deleteResp);
				}
				return Ok(deleteResp);
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpGet("GetBatchMasterByPlantCode/{plantCode}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetBatchMasterByPlantCode(string plantCode)
		{
			try
			{
				var data = await _batchMasterService.GetBatchMasterByPlantCodeAsync(plantCode);
				_APIResponse.Data = data;
				_APIResponse.Status = true;
				_APIResponse.Message = "Batch Master fetched successfully.";
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Status = false;
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
	}
}
