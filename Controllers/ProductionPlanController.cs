using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class ProductionPlanController : ControllerBase
	{
		private readonly IProductionPlanService _productionPlanService;
		private readonly IMapper _mapper;
		private APIResponse _APIResponse;

		public ProductionPlanController(IProductionPlanService productionPlanService, IMapper mapper)
		{
			_productionPlanService = productionPlanService;
			_mapper = mapper;
			_APIResponse = new APIResponse();
		}

		/// <summary>
		/// Get all production plans
		/// </summary>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllProductionPlans()
		{
			var response = await _productionPlanService.GetAllProductionPlansAsync();
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Get a production plan by ID
		/// </summary>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetProductionPlan(int id)
		{
			var response = await _productionPlanService.GetProductionPlanByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Search production plans by various criteria
		/// </summary>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> SearchProductionPlans([FromQuery] SearchProductionPlanRequestDto searchDto)
		{
			var response = await _productionPlanService.SearchProductionPlansAsync(searchDto);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Create a new production plan
		/// </summary>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateProductionPlan([FromBody] CreateProductionPlanRequestDto requestDto)
		{
			if (!ModelState.IsValid)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				_APIResponse.Message = "Invalid input data.";
				_APIResponse.Errors.AddRange(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

				return BadRequest(_APIResponse);
			}

			var response = await _productionPlanService.CreateProductionPlanAsync(requestDto);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Update an existing production plan
		/// </summary>
		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateProductionPlan([FromBody] UpdateProductionPlanRequestDto requestDto)
		{
			if (!ModelState.IsValid)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				_APIResponse.Message = "Invalid input data.";
				_APIResponse.Errors.AddRange(ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

				return BadRequest(_APIResponse);
			}

			var response = await _productionPlanService.UpdateProductionPlanAsync(requestDto);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Delete a production plan by ID
		/// </summary>
		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteProductionPlan(int id)
		{
			var response = await _productionPlanService.DeleteProductionPlanAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Generate next production plan number (Format: PP-YYMM001, resets monthly)
		/// </summary>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GenerateProductionPlanNo()
		{
			var response = await _productionPlanService.GenerateProductionPlanNoAsync();
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
