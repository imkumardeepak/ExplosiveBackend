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
	public class CountryMastersController : ControllerBase
	{
		private readonly ICountryMasterRepository _countryMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "CountryMaster";

		public CountryMastersController(ICountryMasterRepository countryMasterRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_countryMasterRepository = countryMasterRepository;
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
		public async Task<ActionResult<APIResponse>> GetAllCountries()
		{
			try
			{
				var countries = await _countryMasterRepository.GetAllAsync();
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
		public async Task<ActionResult<APIResponse>> GetCountryById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();
				var country = await _countryMasterRepository.GetByIdAsync(id);
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
		public async Task<ActionResult<APIResponse>> CreateCountry(CountryMaster country)
		{
			try
			{
				if (country == null)
					return BadRequest();

				var cName = await _countryMasterRepository.FindByNameAsync(x => x.cname.ToLower() == country.cname.ToLower());

                if (cName.Count() != 0)
                {
                    _APIResponse.Message = "Country name already exists!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }


				await _countryMasterRepository.AddAsync(country);

				_APIResponse.Data = country;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction("GetCountryById", new { id = country.Id }, _APIResponse);
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
		public async Task<ActionResult<APIResponse>> UpdateCountry(int id, CountryMaster country)
		{
			try
			{
				if (id != country.Id)
					return BadRequest();

				var existingCountry = await _countryMasterRepository.GetByIdAsync(id);
				if (existingCountry == null)
					return NotFound();

				//var cName = await _countryMasterRepository.FindByNameAsync(x => x.code.ToLower() == country.code.ToLower());

				//if (cName.Count() != 0)
				//{
				//	_APIResponse.Message = $"Country Code {country.code} already exists!";
				//	_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				//	_APIResponse.Status = false;
				//	return _APIResponse;
				//}

				existingCountry = _mapper.Map(country, existingCountry);

				await _countryMasterRepository.UpdateAsync(existingCountry);

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
		public async Task<ActionResult<APIResponse>> DeleteCountry(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var country = await _countryMasterRepository.GetByIdAsync(id);
				if (country == null)
					return NotFound();

				await _countryMasterRepository.DeleteAsync(id);
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

