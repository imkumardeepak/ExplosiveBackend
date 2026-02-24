using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class PlantTypeMasterController : ControllerBase
    {
        private readonly IPlantTypeMasterRepository _plantTypeRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private readonly string KeyName = "PlantTypeMaster";

        public PlantTypeMasterController(IPlantTypeMasterRepository plantTypeRepository, IDistributedCache distributedCache, IMapper mapper)
        {
            _plantTypeRepository = plantTypeRepository;
            _APIResponse = new();
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        // GET: api/planttypes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllPlantTypes()
        {
            try
            {
                var countries = await _plantTypeRepository.GetAllAsync();
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
        public async Task<ActionResult<APIResponse>> GetPlantTypeById(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();
                var country = await _plantTypeRepository.GetByIdAsync(id);
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
        public async Task<ActionResult<APIResponse>> CreatePlantType(PlantTypeMaster plantsType)
        {
            try
            {
                if (plantsType == null)
                    return BadRequest();

                var existingPlantType = await _plantTypeRepository.GetByPlantTypeCode(plantsType.plant_type);

                if (existingPlantType != null && existingPlantType.Count != 0)
                {

                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    _APIResponse.Message = "Plant Type already exists";
                    return _APIResponse;
                }



                await _plantTypeRepository.AddAsync(plantsType);

                _APIResponse.Data = plantsType;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction("GetPlantTypeById", new { id = plantsType.Id }, _APIResponse);
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
        public async Task<ActionResult<APIResponse>> UpdatePlantType(int id, PlantTypeMaster plantsType)
        {
            try
            {
                if (id != plantsType.Id)
                    return BadRequest();

                var existingCountry = await _plantTypeRepository.GetByIdAsync(id);
                if (existingCountry == null)
                    return NotFound();



                existingCountry = _mapper.Map(plantsType, existingCountry);

                await _plantTypeRepository.UpdateAsync(existingCountry);

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
        public async Task<ActionResult<APIResponse>> DeletePlantType(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var country = await _plantTypeRepository.GetByIdAsync(id);
                if (country == null)
                    return NotFound();

                await _plantTypeRepository.DeleteAsync(id);
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

