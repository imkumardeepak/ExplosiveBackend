using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class L1boxDeletionController : ControllerBase
	{


		private readonly Il1boxdeletionRepository _l1boxdeletionRepository;

		private readonly IDistributedCache _distributedCache;
		private readonly IL1barcodegenerationRepository _l1barcodegenerationRepository;
		private readonly Il1barcodereprintRepository _11barcodereprintRepository;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "L1boxDeletionReport";
		private const string CacheKey = "L1boxDeletionReport";
		private readonly IL2barcodegenerationRepository _l2BarcodegenerationRepository;
		private readonly IL3barcodegenerationRepository _l3BarcodegenerationRepository;
		private readonly IBarcodeDataRepository _barcodeDataRepository;

		public L1boxDeletionController(IDistributedCache distributedCache, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMapper mapper, IBarcodeDataRepository barcodeDataRepository, Il1boxdeletionRepository l1boxdeletionRepository, IL2barcodegenerationRepository l2BarcodegenerationRepository, IL3barcodegenerationRepository l3BarcodegenerationRepository, Il1barcodereprintRepository l1barcodereprintRepository)
		{
			_mapper = mapper;

			_l1barcodegenerationRepository = l1BarcodegenerationRepository;
			_l2BarcodegenerationRepository = l2BarcodegenerationRepository;
			_l3BarcodegenerationRepository = l3BarcodegenerationRepository;
			_barcodeDataRepository = barcodeDataRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_l1boxdeletionRepository = l1boxdeletionRepository;
			_11barcodereprintRepository = l1barcodereprintRepository;


		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteBarcodeData(string barcode, string reason, string deldt)
		{
			try
			{
				var _APIResponse = new APIResponse();

				if (string.IsNullOrWhiteSpace(barcode) || string.IsNullOrWhiteSpace(reason))
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Message = "Invalid or missing parameters.";
					_APIResponse.Errors.Add("Barcode and reason are required.");
					return BadRequest(_APIResponse);
				}

				var upperBarcode = barcode.ToUpper();
				var box = await _l1barcodegenerationRepository.GetFirstOrDefaultAsync(x => x.L1Barcode == upperBarcode);

				if (box == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Message = "L1 box not found.";
					_APIResponse.Errors.Add("Barcode not found.");
					return NotFound(_APIResponse);
				}

				DateTime deleteDate = DateTime.Parse(deldt);

				var deletion = new l1boxdeletion
				{
					plant = box.PlantName,
					brand = box.BrandName,
					psize = box.ProductSize,
					mfgdt = box.MfgDt,
					l1barcode = upperBarcode,
					reason = reason.ToUpper(),
					deldt = deleteDate,
					month = deleteDate.Month.ToString(),
					year = deleteDate.Year.ToString()
				};

				await _l1boxdeletionRepository.AddAsync(deletion);

				await _l1barcodegenerationRepository.Deletel1Async(upperBarcode);
				await _barcodeDataRepository.Deletel1Async(upperBarcode);
				await _l2BarcodegenerationRepository.Deletel1Async(upperBarcode);
				await _l3BarcodegenerationRepository.Deletel1Async(upperBarcode);
				await _11barcodereprintRepository.Deletel1Async(upperBarcode);



				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Data = "Box deleted successfully.";

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				var _APIResponse = new APIResponse();
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

	}
}

