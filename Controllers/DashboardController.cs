using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{


	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class DashboardController : ControllerBase
	{
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IRe11IndentPrdInfoRepository _re11IndentPrdInfoRepository;
		private readonly IMagzine_StockRepository _magzineStockRepository;
		private readonly IRe11IndentInfoRepository _indentRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "BrandMaster";

		public DashboardController(IL1barcodegenerationRepository L1barcodegenerationRepository, IDistributedCache distributedCache, IMapper mapper, IMagzine_StockRepository magzineStockRepository, IRe11IndentPrdInfoRepository re11IndentPrdInfoRepository, IRe11IndentInfoRepository indentRepository)
		{
			_l1BarcodegenerationRepository = L1barcodegenerationRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_mapper = mapper;
			_magzineStockRepository = magzineStockRepository;
			_re11IndentPrdInfoRepository = re11IndentPrdInfoRepository;
			_indentRepository = indentRepository;
		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getdashboardcard()
		{
			try
			{
				var dashboardData = await _l1BarcodegenerationRepository.Getdashdetailsdata(); // Correct async call

				_APIResponse.Data = dashboardData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetMagazinesstock()
		{
			try
			{
				var groupedMagazines = await _magzineStockRepository.GetMagstockdetails();

				var response = new APIResponse
				{
					Status = true,
					Message = "Success",
					StatusCode = HttpStatusCode.OK,
					Data = groupedMagazines
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRE2()
		{
			try
			{
				var magazineList = await _magzineStockRepository.GetMagRE2listdetails();

				var response = new APIResponse
				{
					Status = true,
					StatusCode = HttpStatusCode.OK,
					Data = magazineList
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getallot()
		{
			try
			{
				var magazineList = await _magzineStockRepository.GetMagallotmentdetails();

				var response = new APIResponse
				{
					Status = true,
					StatusCode = HttpStatusCode.OK,
					Data = magazineList
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetdispatchStatus()
		{
			try
			{
				var PETNList = await _re11IndentPrdInfoRepository.GetRE11Listdetails();


				_APIResponse.Data = PETNList;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetDispatch()
		{
			try
			{
				// Fetch data from both repositories
				var dispatchList = await _re11IndentPrdInfoRepository.GetDispatchdeails();
				//var customerList = await _indentRepository.GetAllAsync(); // Fetch customer data

				// Prepare API response
				var response = new APIResponse
				{
					Status = true,
					StatusCode = HttpStatusCode.OK,
					Data = dispatchList // Return dispatch details list
				};

				return Ok(response);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetProductionDispatchData(string plantName, string timeRange)
		{
			try
			{
				DateTime startDate = DateTime.Now;

				// Determine the time range
				switch (timeRange.ToLower())
				{
					case "day":
						startDate = DateTime.Now.Date; // Set to today's date (midnight)
						break;
					case "week":
						startDate = DateTime.Now.AddDays(-7);
						break;
					case "month":
						startDate = DateTime.Now.AddMonths(-1);
						break;
					case "year":
						startDate = DateTime.Now.AddYears(-1);
						break;
					default:
						startDate = DateTime.Now.AddDays(-7); // Default to weekly
						break;
				}

				// Fetch grouped production data from repository
				var groupedProductionData = await _l1BarcodegenerationRepository.GetGroupedProductionData(plantName, startDate);

				// Fetch grouped dispatch data from repository
				var groupedDispatchData = await _re11IndentPrdInfoRepository.GetGroupedDispatchData(plantName, startDate);

				// Combine both Production and Dispatch data in the response
				var result = new
				{
					Production = groupedProductionData,
					Dispatch = groupedDispatchData
				};

				_APIResponse.Data = result;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetSlurryData(string timeRange)
		{
			try
			{
				// Call the repository method to get the slurry data
				var slurryData = await _l1BarcodegenerationRepository.GetSlurryDataAsync(timeRange);

				// Prepare the API response
				_APIResponse.Data = slurryData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetDetonatorData(string timeRange)
		{
			try
			{
				var detonatorList = await _l1BarcodegenerationRepository.GetDetonatorDataAsync(timeRange);

				_APIResponse.Data = detonatorList;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetEmultionData(string timeRange)
		{
			try
			{
				var EmultionList = await _l1BarcodegenerationRepository.GetEmultionDataAsync(timeRange);


				_APIResponse.Data = EmultionList;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPETNData(string timeRange)
		{
			try
			{
				var PETNList = await _l1BarcodegenerationRepository.GetPETNDataAsync(timeRange);


				_APIResponse.Data = PETNList;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors = new List<string> { ex.Message };
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}
	}
}

