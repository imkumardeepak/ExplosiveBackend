using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.Collections.Generic;
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
	public class ShiftMastersController : ControllerBase
	{
		private readonly IShiftMasterRepository _shiftMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "ShiftMaster";

		public ShiftMastersController(IShiftMasterRepository shiftMasterRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_shiftMasterRepository = shiftMasterRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllShifts()
		{
			try
			{
				/* string serializedList = string.Empty;
				 var encodedList = await _distributedCache.GetAsync(KeyName);

				 if (encodedList != null)
				 {
					 _APIResponse.Data = JsonConvert.DeserializeObject<List<ShiftMaster>>(Encoding.UTF8.GetString(encodedList));
					 _APIResponse.Status = true;
					 _APIResponse.StatusCode = HttpStatusCode.OK;
				 }
				 else
				 {*/
				var shifts = await _shiftMasterRepository.GetAllAsync();
				if (shifts != null)
				{
					/* serializedList = JsonConvert.SerializeObject(shifts);
					 encodedList = Encoding.UTF8.GetBytes(serializedList);*/

					var options = new DistributedCacheEntryOptions()
						.SetSlidingExpiration(TimeSpan.FromDays(30))
						.SetAbsoluteExpiration(TimeSpan.FromDays(30));
					//await _distributedCache.SetAsync(KeyName, encodedList, options);

					_APIResponse.Data = shifts;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				//}

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

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetShiftById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var shift = await _shiftMasterRepository.GetByIdAsync(id);
				if (shift == null)
					return NotFound();

				_APIResponse.Data = shift;
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

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateShift(ShiftMaster shift)
		{
			try
			{
				if (shift == null)
					return BadRequest();

                var existingShift = await _shiftMasterRepository.GetShiftByNameAsync(shift.shift);
                if (existingShift.Count() != 0)
				{
                    _APIResponse.Message = $"Shift {shift.shift} already exists.";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
				}                   

				//await _distributedCache.RemoveAsync(KeyName);
				await _shiftMasterRepository.AddAsync(shift);

				_APIResponse.Data = shift;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction("GetShiftById", new { id = shift.Id }, _APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateShift(int id, ShiftMaster shift)
		{
			try
			{
				if (id != shift.Id)
					return BadRequest();

				var existingShift = await _shiftMasterRepository.GetByIdAsync(id);
				if (existingShift == null)
					return NotFound();

               /* var existingShift1 = await _shiftMasterRepository.GetShiftByNameAsync(shift.shift);
                if (existingShift1 != null)
                {
                    _APIResponse.Message = $"Shift {shift.shift} already exists.";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                //await _distributedCache.RemoveAsync(KeyName);

                existingShift = _mapper.Map(shift, existingShift);
				await _shiftMasterRepository.UpdateAsync(existingShift);

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

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteShift(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var shift = await _shiftMasterRepository.GetByIdAsync(id);
				if (shift == null)
					return NotFound();

				await _shiftMasterRepository.DeleteAsync(id);
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

