using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class PlantMasterController : ControllerBase
	{
		private readonly IPlantMasterRepository _plantMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private string KeyName = "PlantMaster";

		public PlantMasterController(IPlantMasterRepository plantMasterRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_plantMasterRepository = plantMasterRepository;
			_APIResponse = new APIResponse();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		// GET: api/PlantMaster
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllPlants()
		{
			try
			{
				var plantMasters = await _plantMasterRepository.GetAllAsync();

				_APIResponse.Data = plantMasters;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// GET: api/PlantMaster/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPlantById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var plantMaster = await _plantMasterRepository.GetByIdAsync(id);


				if (plantMaster == null)
				{
					return NotFound();
				}
				_APIResponse.Data = plantMaster;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;


				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// POST: api/PlantMaster
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreatePlant(PlantMaster plantMaster)
		{
			try
			{
				if (plantMaster == null)
					return BadRequest();

				if (((await _plantMasterRepository.FindByNameAsync(x => x.plant_type.ToLower() == plantMaster.plant_type.ToLower() && x.PName.ToLower() == plantMaster.PName.ToLower())).Count() != 0))
                {
                    _APIResponse.Message = ("Plant already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

				await _plantMasterRepository.AddAsync(plantMaster);

				_APIResponse.Data = plantMaster;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return CreatedAtAction("GetPlantById", new { id = plantMaster.Id }, _APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// PUT: api/PlantMaster/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdatePlant(int id, PlantMaster plantMaster)
		{
			try
			{
				if (id != plantMaster.Id)
				{
					return BadRequest();
				}

				var existingPlantMaster = await _plantMasterRepository.GetByIdAsync(id);

				if (existingPlantMaster == null)
					return NotFound();


                /*if (((await _plantMasterRepository.FindByNameAsync(x => x.plant_type.ToLower() == plantMaster.plant_type.ToLower() && x.PName.ToLower() == plantMaster.PName.ToLower())).Count() != 0))
                {
                    _APIResponse.Message = ("Plant already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                existingPlantMaster = _mapper.Map(plantMaster, existingPlantMaster);

				await _plantMasterRepository.UpdateAsync(existingPlantMaster);

				_APIResponse.Data = null;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// DELETE: api/PlantMaster/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeletePlant(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var plantMaster = await _plantMasterRepository.GetByIdAsync(id);
				if (plantMaster == null)
					return NotFound();

				await _plantMasterRepository.DeleteAsync(id);


				_APIResponse.Data = true;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPlantNames()
		{
			var response = new APIResponse();

			try
			{
				var plantNames = await _plantMasterRepository.GetPlantNamesAsync();
				response.Data = plantNames;
				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;

				return Ok(response);
			}
			catch (Exception ex)
			{
				response.Errors.Add(ex.Message);
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode((int)HttpStatusCode.InternalServerError, response);
			}
		}


		[HttpGet("{plantName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPlantCodeByName(string plantName)
		{
			var response = new APIResponse();

			try
			{
				var plantCode = await _plantMasterRepository.GetPlantCodeByNameAsync(plantName);

				if (plantCode == null)
				{
					response.Status = false;
					response.StatusCode = HttpStatusCode.NotFound;
					response.Errors.Add("Plant not found");
					return NotFound(response);
				}

				response.Data = plantCode;
				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;

				return Ok(response);
			}
			catch (Exception ex)
			{
				response.Errors.Add(ex.Message);
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode((int)HttpStatusCode.InternalServerError, response);
			}
		}

		[HttpGet("{pcode}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CheckPlantCodeExists(string pcode)
		{

			try
			{
				// Check if the plant code exists
				bool plantExists = await _plantMasterRepository.CheckPlantCodeExistsAsync(pcode);

				if (!plantExists)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Plant not found");
					return NotFound(_APIResponse);
				}

				_APIResponse.Data = new { PlantExists = true };
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}

		[HttpGet("{brandName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPlantNameByBName(string brandName)
		{
			var response = new APIResponse();

			try
			{
				var plantNames = await _plantMasterRepository.GetPlantNameByBNameAsync(brandName);

				if (plantNames == null || !plantNames.Any())
				{
					response.Status = false;
					response.StatusCode = HttpStatusCode.NotFound;
					response.Errors.Add("No plant names found for the given brand.");
					return NotFound(response);
				}

				response.Data = plantNames;
				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;

				return Ok(response);
			}
			catch (Exception ex)
			{
				response.Errors.Add(ex.Message);
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode((int)HttpStatusCode.InternalServerError, response);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPlantCodeByNamemm([FromQuery] List<string> plantName)
		{
			var response = new APIResponse();

			try
			{
				var plantCode = await _plantMasterRepository.GetPlantCodeByNamemmAsync(plantName);

				if (plantCode == null)
				{
					response.Status = false;
					response.StatusCode = HttpStatusCode.NotFound;
					response.Errors.Add("Plant not found");
					return NotFound(response);
				}

				response.Data = plantCode;
				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;

				return Ok(response);
			}
			catch (Exception ex)
			{
				response.Errors.Add(ex.Message);
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode((int)HttpStatusCode.InternalServerError, response);
			}
		}
	}

}

