using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class BarcodeDatasController : ControllerBase
	{

		private readonly IBarcodeDataRepository _barcodeDataRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "RE2GEN";

		public BarcodeDatasController(IBarcodeDataRepository barcodeDataRepository, IDistributedCache distributedCache, IMapper mapper)
		{
			_barcodeDataRepository = barcodeDataRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
		}

		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetDataForRe2(RE2RequestDTO rE2RequestDTO)
		{
			try
			{
				if (rE2RequestDTO.Magname == "NoData")
				{
					// Fetch the filtered data using the repository method
					var data = await _barcodeDataRepository.GetBarcodeDataWithoutMagAsync(rE2RequestDTO.Mfgdt, rE2RequestDTO.Plantcode, rE2RequestDTO.Brandid, rE2RequestDTO.Psizecode);

					if (data == null || !data.Any())
					{
						_APIResponse.Status = false;
						_APIResponse.StatusCode = HttpStatusCode.NotFound;
						//_APIResponse.Errors.Add("No data found matching the given criteria.");
						return NotFound(_APIResponse);
					}

					_APIResponse.Data = data;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					// Fetch the filtered data using the repository method
					var data = await _barcodeDataRepository.GetBarcodeDataWithMagAsync(rE2RequestDTO.Mfgdt, rE2RequestDTO.Plantcode, rE2RequestDTO.Brandid, rE2RequestDTO.Psizecode, rE2RequestDTO.Magname);

					if (data == null || !data.Any())
					{
						_APIResponse.Status = false;
						_APIResponse.StatusCode = HttpStatusCode.NotFound;
						//_APIResponse.Errors.Add("No data found matching the given criteria.");
						return NotFound(_APIResponse);
					}

					_APIResponse.Data = data;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;

				}

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

		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]



		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetDataForRe2detail(RErep2requestDTO rErep2requestDTO)
		{
			try
			{
				DateTime mfgDate;

				if (!DateTime.TryParse(rErep2requestDTO.mfgdt, out mfgDate))
				{
					return BadRequest("Invalid manufacturing date format.");
				}
				if (rErep2requestDTO.magname == "NoData")
				{
					// Fetch the filtered data using the repository method
					var data = await _barcodeDataRepository.GetBarcodeDataWithoutMagdataAsync(mfgDate, rErep2requestDTO.plantcode, rErep2requestDTO.brandid, rErep2requestDTO.psizecode, rErep2requestDTO.fromcase, rErep2requestDTO.tocase);

					if (data == null || !data.Any())
					{
						_APIResponse.Status = false;
						_APIResponse.StatusCode = HttpStatusCode.NotFound;
						//_APIResponse.Errors.Add("No data found matching the given criteria.");
						return NotFound(_APIResponse);
					}

					_APIResponse.Data = data;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					// Fetch the filtered data using the repository method
					var data = await _barcodeDataRepository.GetBarcodeDataWithoutMagdataAsync(mfgDate, rErep2requestDTO.plantcode, rErep2requestDTO.brandid, rErep2requestDTO.psizecode, rErep2requestDTO.fromcase, rErep2requestDTO.tocase);

					if (data == null || !data.Any())
					{
						_APIResponse.Status = false;
						_APIResponse.StatusCode = HttpStatusCode.NotFound;
						//_APIResponse.Errors.Add("No data found matching the given criteria.");
						return NotFound(_APIResponse);
					}

					_APIResponse.Data = data;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;

				}

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


	}
}

