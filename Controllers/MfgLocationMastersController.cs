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
	public class MfgLocationController : ControllerBase
	{
		private readonly IMfgLocationMasterRepository _mfgLocationMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private string KeyName = "MfgLocationMaster";

		public MfgLocationController(IMfgLocationMasterRepository mfgLocationMasterRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_mfgLocationMasterRepository = mfgLocationMasterRepository;
			_APIResponse = new APIResponse();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		// GET: api/MfgLocation
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllLocations()
		{
			try
			{
				var mfgLocations = await _mfgLocationMasterRepository.GetAllAsync();

				_APIResponse.Data = mfgLocations;
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

		// GET: api/MfgLocation/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetLocationById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();


				var mfgLocation = await _mfgLocationMasterRepository.GetByIdAsync(id);

				if (mfgLocation == null)
				{
					return NotFound();
				}
				_APIResponse.Data = mfgLocation;
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

		// POST: api/MfgLocation
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateLocation(MfgLocationMaster mfgLocationMaster)
		{
			try
			{
				if (mfgLocationMaster == null)
					return BadRequest();

				if ((await _mfgLocationMasterRepository.FindByNameAsync(x => x.maincode.ToLower() == mfgLocationMaster.maincode.ToLower())).Count() != 0)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Message = $"Mfg Code '{mfgLocationMaster.maincode}' already exists.";
                    return _APIResponse;
                }

				await _mfgLocationMasterRepository.AddAsync(mfgLocationMaster);

				_APIResponse.Data = mfgLocationMaster;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return CreatedAtAction("GetLocationById", new { id = mfgLocationMaster.id }, _APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// PUT: api/MfgLocation/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateLocation(int id, MfgLocationMaster mfgLocationMaster)
		{
			try
			{
				if (id != mfgLocationMaster.id)
				{
					return BadRequest();
				}

               /* if ((await _mfgLocationMasterRepository.FindByNameAsync(x => x.maincode.ToLower() == mfgLocationMaster.maincode.ToLower())).Count() != 0)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Message = $"Mfg Code '{mfgLocationMaster.maincode}' already exists.";
                    return _APIResponse;
                }*/

                var existingMfgLocation = await _mfgLocationMasterRepository.GetByIdAsync(id);

				if (existingMfgLocation == null)
					return NotFound();

				//await _distributedCache.RemoveAsync(KeyName);

				existingMfgLocation = _mapper.Map(mfgLocationMaster, existingMfgLocation);

				await _mfgLocationMasterRepository.UpdateAsync(existingMfgLocation);

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

		// DELETE: api/MfgLocation/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteLocation(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var mfgLocation = await _mfgLocationMasterRepository.GetByIdAsync(id);
				if (mfgLocation == null)
					return NotFound();

				await _mfgLocationMasterRepository.DeleteAsync(id);
				//await _distributedCache.RemoveAsync(KeyName);

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
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetMfgLocations()
		{
			try
			{
				// Initialize response
				APIResponse _APIResponse = new APIResponse();


				var locationNames = await _mfgLocationMasterRepository.GetMfgLocationNamesAsync();

				if (locationNames != null)
				{
					// Set response data
					_APIResponse.Data = locationNames;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					// No data found
					_APIResponse.Data = null;
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("No manufacturing locations found.");
				}
				//}

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				// Handle errors
				APIResponse _APIResponse = new APIResponse();
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet("{mfgLocName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetCodeByLocationName(string mfgLocName)
		{
			var response = new APIResponse();

			try
			{
				// Fetch MfgLocCode from the repository
				var mfgLocCode = await _mfgLocationMasterRepository.GetMfgLocCodeByNameAsync(mfgLocName);

				if (mfgLocCode == null)
				{
					// Not found
					response.Status = false;
					response.StatusCode = HttpStatusCode.NotFound;
					response.Errors.Add("Manufacturing location not found");
					return NotFound(response);
				}

				// Found the code
				response.Data = mfgLocCode;
				response.Status = true;
				response.StatusCode = HttpStatusCode.OK;

				return Ok(response);
			}
			catch (Exception ex)
			{
				// Internal server error
				response.Errors.Add(ex.Message);
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode((int)HttpStatusCode.InternalServerError, response);
			}
		}



	}
}

