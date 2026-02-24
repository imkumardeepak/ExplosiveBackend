using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{

	[Route("api/[controller]/[action]")]
	[ApiController]
    [Authorize]
	public class RoleMasterController : ControllerBase
	{
		private readonly IRoleMasterRepository _roleMasterRepository;
		private readonly APIResponse _aPIResponse;

		public RoleMasterController(IRoleMasterRepository roleMasterRepository, APIResponse aPIResponse)
		{
			_roleMasterRepository = roleMasterRepository;
			_aPIResponse = aPIResponse;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRoleMasterList()
		{
			try
			{
				var roleMasterList = await _roleMasterRepository.GetRoleMaster();
				_aPIResponse.Data = roleMasterList;
				_aPIResponse.Status = true;
				_aPIResponse.Message = "Role Master List retrieved successfully.";
				_aPIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_aPIResponse);
			}
			catch (Exception ex)
			{
				_aPIResponse.Errors.Add(ex.Message);
				_aPIResponse.Status = false;
				_aPIResponse.Message = "An error occurred while retrieving the Role Master List.";
				_aPIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _aPIResponse);
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetRoleMasterById(int id)
		{
			try
			{
				var roleMaster = await _roleMasterRepository.GetByIdAsync(id);
				if (roleMaster == null)
				{
                    _aPIResponse.Status = false;
					_aPIResponse.Message = "Role Master not found.";
					_aPIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_aPIResponse);
				}
				_aPIResponse.Data = roleMaster;
				_aPIResponse.Status = true;
				_aPIResponse.Message = "Role Master retrieved successfully.";
				_aPIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_aPIResponse);
			}
			catch (Exception ex)
			{
				_aPIResponse.Errors.Add(ex.Message);
				_aPIResponse.Status = false;
				_aPIResponse.Message = "An error occurred while retrieving the Role Master.";
				_aPIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _aPIResponse);
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateRoleMaster(RoleMaster roleMaster)
		{
			try
			{
				if (roleMaster == null)
				{
					_aPIResponse.Status = false;
					_aPIResponse.Message = "Role Master object is null.";
					_aPIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_aPIResponse);
				}

				var existingRole = await _roleMasterRepository.Searchbyrole(roleMaster.RoleName);

				if (existingRole != null)
				{
					_aPIResponse.Status = false;
					_aPIResponse.Message = "Role Master already exists.";
					_aPIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_aPIResponse);
				}

				await _roleMasterRepository.AddAsync(roleMaster);
				_aPIResponse.Data = roleMaster;
				_aPIResponse.Status = true;
				_aPIResponse.Message = "Role Master created successfully.";
				_aPIResponse.StatusCode = HttpStatusCode.Created;
				return CreatedAtAction("GetRoleMasterById", new { id = roleMaster.Id }, _aPIResponse);
			}
			catch (Exception ex)
			{ 
                _aPIResponse.Errors.Add(ex.Message);
				_aPIResponse.Status = false;
				_aPIResponse.Message = "An error occurred while creating the Role Master.";
				_aPIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _aPIResponse);
			}
		}
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateRoleMaster(int id, RoleMaster roleMaster)
		{
			try
			{
				if (id != roleMaster.Id)
				{
					_aPIResponse.Status = false;
					_aPIResponse.Message = "Role Master object is null.";
					_aPIResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_aPIResponse);
				}
			
				
				await _roleMasterRepository.DeleteAsync(id);

				await _roleMasterRepository.AddAsync(roleMaster);
				_aPIResponse.Data = roleMaster;
				_aPIResponse.Status = true;
				_aPIResponse.Message = "Role Master updated successfully.";
				_aPIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_aPIResponse);
			}
			catch (Exception ex)
			{
				_aPIResponse.Errors.Add(ex.Message);
				_aPIResponse.Status = false;
				_aPIResponse.Message = "An error occurred while updating the Role Master.";
				_aPIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _aPIResponse);
			}
		}
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteRoleMaster(int id)
		{
			try
			{
				var roleMaster = await _roleMasterRepository.GetByIdAsync(id);
				if (roleMaster == null)
				{
					_aPIResponse.Status = false;
					_aPIResponse.Message = "Role Master not found.";
					_aPIResponse.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_aPIResponse);
				}
				await _roleMasterRepository.DeleteAsync(roleMaster.Id);
				_aPIResponse.Status = true;
				_aPIResponse.Message = "Role Master deleted successfully.";
				_aPIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_aPIResponse);
			}
			catch (Exception ex)
			{
				_aPIResponse.Errors.Add(ex.Message);
				_aPIResponse.Status = false;
				_aPIResponse.Message = "An error occurred while deleting the Role Master.";
				_aPIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _aPIResponse);
			}
		}
	}
}

