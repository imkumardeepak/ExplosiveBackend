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
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;


namespace YourNamespace.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class StateMastersController : ControllerBase
	{
		private readonly IStateMasterRepository _stateMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "StateMaster";

		public StateMastersController(IStateMasterRepository stateMasterRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_stateMasterRepository = stateMasterRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		// GET: api/States
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllStates()
		{
			try
			{
				var states = await _stateMasterRepository.GetAllAsync();
				_APIResponse.Data = states;
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

		// GET: api/States/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetStateById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();
				var state = await _stateMasterRepository.GetByIdAsync(id);
				if (state == null)
					return NotFound();

				_APIResponse.Data = state;
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

		// POST: api/States
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateState(StateMaster state)
		{
			try
			{
				if (state == null)
					return BadRequest();

				if ((await _stateMasterRepository.FindByNameAsync(x => x.State.ToLower() == state.State.ToLower())).Count() != 0)
                {
                    _APIResponse.Message = $"State {state.State} already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

				await _stateMasterRepository.AddAsync(state);

				_APIResponse.Data = state;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction("GetStateById", new { id = state.Id }, _APIResponse);

			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// PUT: api/States/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateState(int id, StateMaster state)
		{
			try
			{
				if (id != state.Id)
					return BadRequest();

				var existingState = await _stateMasterRepository.GetByIdAsync(id);
				if (existingState == null)
					return NotFound();

				//if ((await _stateMasterRepository.FindByNameAsync(x => x.St_code.ToLower() == state.St_code.ToLower())).Count() != 0)
				//{
				//	_APIResponse.Message = $"State Code {state.St_code} already exists!";
				//	_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				//	_APIResponse.Status = false;
				//	return _APIResponse;
				//}


				//await _distributedCache.RemoveAsync(KeyName);

				existingState = _mapper.Map(state, existingState);

				await _stateMasterRepository.UpdateAsync(existingState);

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

		// DELETE: api/States/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteState(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var state = await _stateMasterRepository.GetByIdAsync(id);
				if (state == null)
					return NotFound();

				await _stateMasterRepository.DeleteAsync(id);
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

