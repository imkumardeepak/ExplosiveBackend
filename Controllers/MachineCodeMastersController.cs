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
    public class MachineCodeMastersController : ControllerBase
    {
        private readonly IMachineCodeMasterRepository _machineCodeMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "MachineCodeMaster";

        public MachineCodeMastersController(IMachineCodeMasterRepository machineCodeMasterRepository, IDistributedCache distributedCache ,IMapper mapper)
        {
            _machineCodeMasterRepository = machineCodeMasterRepository;
            _APIResponse = new APIResponse();
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        // GET: api/MachineCodeMasters
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllMachineCodes()
        {
            try
            {
               /* string serializedList = string.Empty;
                var encodedList = await _distributedCache.GetAsync(CacheKey);*/

               /* if (encodedList != null)
                {
                    _APIResponse.Data = JsonConvert.DeserializeObject<List<MachineCodeMaster>>(Encoding.UTF8.GetString(encodedList));
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {*/
                    var machineCodes = await _machineCodeMasterRepository.GetAllAsync();
                    if (machineCodes != null)
                    {
                        /*serializedList = JsonConvert.SerializeObject(machineCodes);
                        encodedList = Encoding.UTF8.GetBytes(serializedList);*/

                        var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(30))
                            .SetAbsoluteExpiration(TimeSpan.FromDays(30));
                        //await _distributedCache.SetAsync(CacheKey, encodedList, options);

                        _APIResponse.Data = machineCodes;
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

        // GET: api/MachineCodeMasters/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetMachineCodeById(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var machineCode = await _machineCodeMasterRepository.GetByIdAsync(id);
                if (machineCode == null)
                    return NotFound();

                _APIResponse.Data = machineCode;
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

        // POST: api/MachineCodeMasters
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateMachineCode(MachineCodeMaster machineCode)
        {
            try
            {
                if (machineCode == null)
                    return BadRequest();

                var temp = await _machineCodeMasterRepository.FindByNameAsync(x => x.pname.ToLower() == machineCode.pname.ToLower() && x.mcode.ToLower() == machineCode.mcode.ToLower());
                if (temp.Count() != 0)
                {
                    _APIResponse.Message = ("Machine Code already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }


                //await _distributedCache.RemoveAsync(CacheKey);
                await _machineCodeMasterRepository.AddAsync(machineCode);

                _APIResponse.Data = machineCode;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction(nameof(GetMachineCodeById), new { id = machineCode.id }, _APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                _APIResponse.Status = false;
                return _APIResponse;
            }
        }

        // PUT: api/MachineCodeMasters/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateMachineCode(int id, MachineCodeMaster machineCode)
        {
            try
            {
                if (id != machineCode.id)
                    return BadRequest();

                var existingMachineCode = await _machineCodeMasterRepository.GetByIdAsync(id);
                if (existingMachineCode == null)
                    return NotFound();

               /* var temp = await _machineCodeMasterRepository.FindByNameAsync(x => x.pname.ToLower() == machineCode.pname.ToLower() && x.mcode.ToLower() == machineCode.mcode.ToLower());
                if (temp.Count() != 0)
                {
                    _APIResponse.Message = ("Machine Code already exists");
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }*/

                //await _distributedCache.RemoveAsync(CacheKey);

                existingMachineCode = _mapper.Map(machineCode, existingMachineCode);

                await _machineCodeMasterRepository.UpdateAsync(existingMachineCode);

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

        // DELETE: api/MachineCodeMasters/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteMachineCode(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest();

                var machineCode = await _machineCodeMasterRepository.GetByIdAsync(id);
                if (machineCode == null)
                    return NotFound();

                await _machineCodeMasterRepository.DeleteAsync(id);
                //await _distributedCache.RemoveAsync(CacheKey);

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

        [HttpGet("{plantCode}/{machineCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CheckMachineCodeExists(string plantCode, string machineCode)
        {

            try
            {
               /* // Check cache
                var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    // Deserialize cached data
                    var machineCodes = JsonConvert.DeserializeObject<List<MachineCodeMaster>>(Encoding.UTF8.GetString(cachedData));

                    // Check if the machine code exists in cached data
                    bool exists = machineCodes.Any(e => e.pcode == plantCode && e.mcode == machineCode);
                    _APIResponse.Data = new { MachineCodeExists = exists };
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;

                    return exists ? Ok(_APIResponse) : NotFound(_APIResponse);
                }*/

                // Fetch from database if not in cache
                bool existsInDb = await _machineCodeMasterRepository.MachineCodeExistsAsync(plantCode, machineCode);

                // Return response
                if (!existsInDb)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Machine code not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = new { MachineCodeExists = true };
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


        [HttpGet("{plantName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetMachineCodes(string plantName)
        {
            try
            {
                
               /* // Check cache
                var cachedData = await _distributedCache.GetAsync(CacheKey);
                if (cachedData != null)
                {
                    // Deserialize cached data
                    var machineCodes = JsonConvert.DeserializeObject<List<MachineCodeMaster>>(Encoding.UTF8.GetString(cachedData));

                    var mcodes = machineCodes.Where(e => e.pname == plantName).Select(e => e.mcode).ToList();

                    if (mcodes != null && mcodes.Any())
                    {
                        _APIResponse.Data = mcodes;
                        _APIResponse.Status = true;
                        _APIResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(_APIResponse);
                    }
                }*/

                // Fetch from database if not in cache
                var machineCodesFromDb = (await _machineCodeMasterRepository
                                        .GetMachineCodesByPlantNameAsync(plantName))
                                        .Select(e => e.mcode)
                                        .ToList();


                if (machineCodesFromDb == null || !machineCodesFromDb.Any())
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    //_APIResponse.Errors.Add($"No machine codes found for plant name: {plantName}");
                    return NotFound(_APIResponse);
                }

                
                // Return data
                _APIResponse.Data = machineCodesFromDb;
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
		[HttpGet("{pcode}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPcodebyMcode(string pcode)
		{
			var response = new APIResponse();

			try
			{
				var plantCode = await _machineCodeMasterRepository.GetByMcodeAsync(pcode);

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

