using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using AutoMapper;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;


namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class MfgMastersController : ControllerBase
	{
		private readonly IMfgMasterRepository _mfgMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "MfgMaster";

		public MfgMastersController(IMfgMasterRepository mfgMasterRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_mfgMasterRepository = mfgMasterRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		// GET: api/MfgMasters
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllMfgMasters()
		{
			try
			{

				var mfgMasters = await _mfgMasterRepository.GetAllAsync();
				_APIResponse.Data = mfgMasters;
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

		// GET: api/MfgMasters/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetMfgMasterById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var mfgMaster = await _mfgMasterRepository.GetByIdAsync(id);
				if (mfgMaster == null)
					return NotFound();

				_APIResponse.Data = mfgMaster;
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


		// GET: api/MfgMasters/5
		[HttpGet("{mfgname}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetMfgMasterByName(string mfgname)
		{
			try
			{
				if (mfgname == null)
					return BadRequest();

				var mfgMaster = await _mfgMasterRepository.GetCodeByMfgNameAsync(mfgname);
				if (mfgMaster == null)
					return NotFound();

				_APIResponse.Data = mfgMaster;
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


		// POST: api/MfgMasters
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateMfgMaster(MfgMaster mfgMaster)
		{
			try
			{
				if (mfgMaster == null)
					return BadRequest();

				if ((await _mfgMasterRepository.FindByNameAsync(x => x.mfgname == mfgMaster.mfgname)).Count() != 0)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Message = $"Mfg Name '{mfgMaster.mfgname}' already exists.";
                    return _APIResponse;
                }

				//await _distributedCache.RemoveAsync(KeyName);
				await _mfgMasterRepository.AddAsync(mfgMaster);

				_APIResponse.Data = mfgMaster;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction("GetMfgMasterById", new { id = mfgMaster.Id }, _APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// PUT: api/MfgMasters/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateMfgMaster(int id, MfgMaster mfgMaster)
		{
			try
			{
				if (id != mfgMaster.Id)
					return BadRequest();

				var existingMfgMaster = await _mfgMasterRepository.GetByIdAsync(id);
				if (existingMfgMaster == null)
					return NotFound();

				//if ((await _mfgMasterRepository.FindByNameAsync(x => x.code == mfgMaster.code)).Count() != 0)
				//{
				//	_APIResponse.Status = false;
				//	_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				//	_APIResponse.Message = $"Mfg Code '{mfgMaster.code}' already exists.";
				//	return _APIResponse;
				//}

				//await _distributedCache.RemoveAsync(KeyName);

				existingMfgMaster = _mapper.Map(mfgMaster, existingMfgMaster);

				await _mfgMasterRepository.UpdateAsync(existingMfgMaster);

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

		// DELETE: api/MfgMasters/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteMfgMaster(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var mfgMaster = await _mfgMasterRepository.GetByIdAsync(id);
				if (mfgMaster == null)
					return NotFound();

				await _mfgMasterRepository.DeleteAsync(id);
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
	}
}

