using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class formre2magallotController : ControllerBase
    {

        private readonly Iformre2magallotRepository _formre2magallotRepository;
        private readonly IL1barcodegenerationRepository _L1barcodegenerationRepository;
        private readonly IPlantMasterRepository _plantMasterRepository;
        private readonly IMagzineMasterRepository _magzineMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private string KeyName = "formre2magallot";

        public formre2magallotController(Iformre2magallotRepository formre2magallotRepository, IDistributedCache distributedCache, IMapper mapper, IL1barcodegenerationRepository L1barcodegenerationRepository, IMagzineMasterRepository magzineMasterRepository, IPlantMasterRepository plantMasterRepository)
        {
            _formre2magallotRepository = formre2magallotRepository;
            _APIResponse = new APIResponse();
            _distributedCache = distributedCache;
            _mapper = mapper;
            _L1barcodegenerationRepository = L1barcodegenerationRepository;
            _plantMasterRepository = plantMasterRepository;
            _mapper = mapper;
            _magzineMasterRepository = magzineMasterRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetManualMagzineAllot()
        {
            try
            {
                List<string> plants = _plantMasterRepository.GetAllAsync().Result.Select(p => p.PName).ToList();
                plants.Insert(0, "Select Plant");

                List<string> mags = _magzineMasterRepository.GetAllAsync().Result.Select(p => p.mcode).ToList();
                mags.Insert(0, "Select Magzine");

                // Initialize a new ManualMagAllot object with an empty list of SmallModels
                var model = new ManualMagAllot
                {
                    MfgDt = DateTime.Now.Date,
                    //plist = plants.Select(pname => new SelectListItem { Value = pname, Text = pname }).ToList(),
                    //mlist = mags.Select(mname => new SelectListItem { Value = mname, Text = mname }).ToList(),
                    SmallModels = new List<SmallModel>()
                };


                _APIResponse.Data = model;
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
        public async Task<ActionResult<APIResponse>> SetFormMagzineAllotment(formre2magallot formre2magallot)
        {
            try
            {
                if (formre2magallot == null)
                    return BadRequest(new APIResponse
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = new List<string> { "Invalid request: magzineAllotment cannot be null" }
                    });

                formre2magallot.gendt = formre2magallot.mfgdt;
                formre2magallot.month = DateTime.Now.Month.ToString();
                formre2magallot.year = DateTime.Now.Year.ToString();
                formre2magallot.quarter = Convert.ToString((DateTime.Now.Month - 1) / 3 + 1);
                formre2magallot.maglic = "E/HQ/MH/21/270 (E342)";
                formre2magallot.re12 = 0;
                formre2magallot.re2 = 0;
                formre2magallot.srno = 1;
                formre2magallot.@class = 2;
                formre2magallot.div = 1;
                formre2magallot.shift = "A";


                // Use AutoMapper to generate list of entities to save
                var dataToSave = _mapper.Map<List<formre2magallot>>(formre2magallot);

                // Update flags
                var l1barcodes = dataToSave.Select(e => e.l1barcode).ToList();
                await _L1barcodegenerationRepository.BulkUpdateMFlag(l1barcodes, 1);


                // Save to DB
                await _formre2magallotRepository.AddRangeAsync(dataToSave);

                /* // Map ManualMagAllot to List<Magzine_Stock>
				 var datare2 = _mapper.Map<List<FormRE2Manallot>>(formre2magallot);

				 var l1barcodes = datare2.Select(e => e.l1barcode).ToList();

				 await _L1barcodegenerationRepository.BulkUpdateMFlag(l1barcodes, "1");

				 // Save mapped data to database
				 await _formre2magallotRepository.AddRangeAsync((IEnumerable<formre2magallot>)datare2);*/



                // Clear cache
                await _distributedCache.RemoveAsync(KeyName);

                // Set response
                _APIResponse.Data = dataToSave;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction(nameof(SetFormMagzineAllotment), new { }, _APIResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse
                {
                    Status = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Errors = new List<string> { ex.Message }
                });
            }
        }


        // PUT: api/DeviceMasters/UpdateDevice/5
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> SetFormTesting(formre2magallot formre2magallot)
        {
            try
            {
                if (formre2magallot == null)
                    return BadRequest(new APIResponse
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = new List<string> { "Invalid request: magzineAllotment cannot be null" }
                    });

                string l1 = formre2magallot.l1barcode;

                var existingL1 = await _formre2magallotRepository.GetAllAsync();

                var data = existingL1.FirstOrDefault(x => x.l1barcode == l1);
                data.dateoftest = formre2magallot.dateoftest;

                //existingDevice = _mapper.Map(device, existingDevice);
                await _formre2magallotRepository.UpdateAsync(data);

                _APIResponse.Data = null;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _APIResponse;
            }
        }

        [HttpGet("{mfgdt}/{pcode}/{brandid}/{psizecode}/{magname}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetMagtransData(DateTime mfgdt, string pcode, string brandid, string psizecode, string magname)
        {
            try
            {
                if (pcode == null || brandid == null || psizecode == null)
                {
                    return BadRequest();
                }


                var L1data = await _formre2magallotRepository.GetL1MagtransData(mfgdt, pcode, brandid, psizecode, magname);

                if (L1data != null)
                {
                    _APIResponse.Data = L1data;
                    _APIResponse.Status = true;
                    _APIResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(_APIResponse);
                }

                _APIResponse.Data = "No data Found";
                _APIResponse.Status = false;
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


        // PUT: api/DeviceMasters/UpdateDevice/5
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateMag(formre2magallot formre2magallot)
        {
            try
            {
                if (formre2magallot == null || formre2magallot.SmallModels == null || !formre2magallot.SmallModels.Any())
                    return BadRequest(new APIResponse
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = new List<string> { "Invalid request: data is incomplete or missing." }
                    });

                var l1Barcodes = formre2magallot.SmallModels.Select(sm => sm.L1Barcode).ToList();

                var existingL1Records = await _formre2magallotRepository.GetAllAsync();
                var recordsToUpdate = existingL1Records
                    .Where(x => l1Barcodes.Contains(x.l1barcode))
                    .ToList();

                var maglicList = await _magzineMasterRepository.GetAllAsync();
                var maglicToUpdate = maglicList.FirstOrDefault(x => x.mcode == formre2magallot.MagnameTo)?.licno;

                foreach (var record in recordsToUpdate)
                {
                    record.magname = formre2magallot.MagnameTo;
                    record.maglic = maglicToUpdate;
                    await _formre2magallotRepository.UpdateAsync(record);
                }

                _APIResponse.Data = null;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                _APIResponse.Status = false;
                _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _APIResponse;
            }
        }


    }
}


