using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class MagzineAllotmentController : ControllerBase
	{
		private readonly IMagzine_StockRepository _magzine_StockRepository;
		private readonly IPlantMasterRepository _plantMasterRepository;
		private readonly IMagzineMasterRepository _magzineMasterRepository;
		private readonly IL1barcodegenerationRepository _l1barcodegenerationRepository;
		private readonly IMapper _mapper;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly string KeyName = "MagzineAllotment";


		public MagzineAllotmentController(IMagzine_StockRepository magzine_StockRepository, IDistributedCache distributedCache, IMagzineMasterRepository magzineMasterRepository, IPlantMasterRepository plantMasterRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMapper mapper)
		{
			_mapper = mapper;
			_magzine_StockRepository = magzine_StockRepository;
			_plantMasterRepository = plantMasterRepository;
			_magzineMasterRepository = magzineMasterRepository;
			_l1barcodegenerationRepository = l1BarcodegenerationRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
		}

		// GET: api/MagzineAllotment
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

		// GET: api/MagzineAllotment/5
		[HttpGet("{mfgdt}/{pcode}/{brandid}/{psizecode}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetL1barcodeDetails(DateTime mfgdt, string pcode, string brandid, string psizecode)
		{
			try
			{
				if (pcode == null || brandid == null || psizecode == null)
				{
					return BadRequest();
				}


				var L1data = await _l1barcodegenerationRepository.GetL1MagNotAllot(mfgdt, pcode, brandid, psizecode);

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

		// POST: api/MagzineAllotment
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> SetMagzineAllotment(ManualMagAllot magzineAllotment)
		{
			try
			{
				if (magzineAllotment == null)
					return BadRequest(new APIResponse
					{
						Status = false,
						StatusCode = HttpStatusCode.BadRequest,
						Errors = new List<string> { "Invalid request: magzineAllotment cannot be null" }
					});

				// Map ManualMagAllot to List<Magzine_Stock>
				var magzinestock = _mapper.Map<List<Magzine_Stock>>(magzineAllotment);

				var l1barcodes = magzinestock.Select(e => e.L1Barcode).ToList();

				await _l1barcodegenerationRepository.BulkUpdateMFlag(l1barcodes, 1);

				// Save mapped data to database
				await _magzine_StockRepository.AddRangeAsync(magzinestock);



				// Clear cache
				await _distributedCache.RemoveAsync(KeyName);

				// Set response
				_APIResponse.Data = magzinestock;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction(nameof(SetMagzineAllotment), new { }, _APIResponse);
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


		// PUT: api/MagzineAllotment/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateMagzineAllotment(int id, Magzine_Stock magzineAllotment)
		{
			try
			{
				//if (id != magzineAllotment.Id)
				//    return BadRequest();

				var existingMagzineAllotment = await _magzine_StockRepository.GetByIdAsync(id);
				if (existingMagzineAllotment == null)
					return NotFound();

				await _distributedCache.RemoveAsync(KeyName);

				//existingMagzineAllotment = _mapper.Map(magzineAllotment, existingMagzineAllotment);

				await _magzine_StockRepository.UpdateAsync(existingMagzineAllotment);

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

		// DELETE: api/MagzineAllotment/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteMagzineAllotment(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var magzineAllotment = await _magzine_StockRepository.GetByIdAsync(id);
				if (magzineAllotment == null)
					return NotFound();

				await _magzine_StockRepository.DeleteAsync(id);
				await _distributedCache.RemoveAsync(KeyName);

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

