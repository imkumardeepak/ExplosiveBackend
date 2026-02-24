using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class RouteMastersController : ControllerBase
	{
		private readonly IRouteMasterRepository _routeMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "CountryMaster";

		public RouteMastersController(ICountryMasterRepository inntimationMasterRepository, IDistributedCache distributedCache, IMapper mapper, IRouteMasterRepository RouteMasterRepository)
		{
			_routeMasterRepository = RouteMasterRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		// GET: api/Countries
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllRoutes()
		{
			try
			{
				var countries = await _routeMasterRepository.GetAllAsync();
				_APIResponse.Data = countries;
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

		// GET: api/Countries/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRouteById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();
				var country = await _routeMasterRepository.GetByIdAsync(id);
				if (country == null)
					return NotFound();
				_APIResponse.Data = country;
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

		// POST: api/Countries
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateRoute(RouteMaster Route)
		{
			try
			{
				if (Route == null)
					return BadRequest();

				var cName = await _routeMasterRepository.FindByNameAsync(x => x.cname.ToLower() == Route.cname.ToLower());

                if (cName.Count() != 0)
                {
                    _APIResponse.Message = "Route name already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }


				await _routeMasterRepository.AddAsync(Route);

				_APIResponse.Data = Route;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction("GetRouteById", new { id = Route.Id }, _APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return _APIResponse;
			}
		}

		// PUT: api/Countries/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateRoute(int id, RouteMaster Route)
		{
			try
			{
				if (id != Route.Id)
					return BadRequest();

				var existingRoute = await _routeMasterRepository.GetByIdAsync(id);
				if (existingRoute == null)
					return NotFound();

                /*var cName = await _routeMasterRepository.FindByNameAsync(x => x.cname.ToLower() == Route.cname.ToLower());

                if (cName.Count() != 0)
                {
                    _APIResponse.Message = "Route name already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                existingRoute = _mapper.Map(Route, existingRoute);

				await _routeMasterRepository.UpdateAsync(existingRoute);

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

		// DELETE: api/Countries/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteRoute(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var Route = await _routeMasterRepository.GetByIdAsync(id);
				if (Route == null)
					return NotFound();

				await _routeMasterRepository.DeleteAsync(id);
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

