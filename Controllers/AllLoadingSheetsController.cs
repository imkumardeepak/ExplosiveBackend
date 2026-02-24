using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class AllLoadingSheetsController : ControllerBase
	{
		private readonly IAllLoadingSheetRepository _allLoadingSheetRepository;
		private readonly IRe11IndentInfoRepository _re11IndentInfoRepository;
		private readonly IRe11IndentPrdInfoRepository _re11IndentPrdInfoRepository;
		private readonly ITransportMasterRepository _transportMasterRepository;
		private readonly ITransVehicleDetailRepository _transVehicleDetailRepository;
		private readonly IBarcodeDataRepository _barcodeDataRepository;
		private readonly IMagzineMasterRepository _magzineMasterRepository;
		private readonly IMagzine_StockRepository _magzine_StockRepository;
		private readonly IMapper _mapper;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly L1DetailsService _L1DetailsService;
		private readonly string KeyName = "AllLoadingSheet";

		public AllLoadingSheetsController(IAllLoadingSheetRepository allLoadingSheetRepository, IDistributedCache distributedCache, IRe11IndentInfoRepository re11IndentInfoRepository, ITransportMasterRepository transportMasterRepository, IMapper mapper, ITransVehicleDetailRepository transVehicleDetailRepository, IRe11IndentPrdInfoRepository re11IndentPrdInfoRepository, IMagzineMasterRepository magzineMasterRepository, IMagzine_StockRepository magzine_StockRepository, L1DetailsService l1DetailsService, IBarcodeDataRepository barcodeDataRepository)
		{
			_allLoadingSheetRepository = allLoadingSheetRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_re11IndentInfoRepository = re11IndentInfoRepository;
			_transportMasterRepository = transportMasterRepository;
			_mapper = mapper;
			_transVehicleDetailRepository = transVehicleDetailRepository;
			_re11IndentPrdInfoRepository = re11IndentPrdInfoRepository;
			_magzineMasterRepository = magzineMasterRepository;
			_magzine_StockRepository = magzine_StockRepository;
			_L1DetailsService = l1DetailsService;
			_barcodeDataRepository = barcodeDataRepository;
		}

		// GET: api/AllLoadingSheets
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllLoadingSheets()
		{
			try
			{
				var allLoadingSheets = await _allLoadingSheetRepository.GetLoadingSheet();
				_APIResponse.Data = allLoadingSheets;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Message = "Success";
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

		// GET: api/AllLoadingSheets
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetLoadingSheetsByCFlag()
		{
			try
			{

				var allLoadingSheets = await _allLoadingSheetRepository.GetLoadingByCflag(0);
				if (allLoadingSheets != null)
				{
					_APIResponse.Data = allLoadingSheets;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}

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

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetBatchInfo(string? bid,string? sizeCode,int reqCase)
		{
			try
			{
				var allLoadingSheets = await _barcodeDataRepository.GetBatchInfo(bid, sizeCode, reqCase);
				_APIResponse.Data = allLoadingSheets;
				_APIResponse.Status = true;
				_APIResponse.Message = "Success";
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

		// GET: api/AllLoadingSheets
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCreateLoadingData()
		{
			try
			{
				var indnetdata = await _re11IndentInfoRepository.GetAllIndentInfoByCompflag();
				var Transportdata = await _transportMasterRepository.GetAllTransportMasterWithDetails();
				var magstock = await _magzine_StockRepository.GetMagRE2details();
				var magzines = await _magzineMasterRepository.GetMagDetails();

				var combined = (from mz in magzines
								join ms in magstock on mz.mcode equals ms.MagName into msGroup
								from ms in msGroup.DefaultIfEmpty()
								group new { ms, mz } by mz.mcode into g
								select new
								{
									MagName = g.Key,
									TotalNetWeight = g.Sum(x => x.ms?.TotalNetWeight ?? 0),
									MagzineWt = Convert.ToDouble(g.Select(x => x.mz.Totalwt).FirstOrDefault()),
									Blankspace = Convert.ToDouble(g.Select(x => x.mz.Totalwt).FirstOrDefault()) - g.Sum(x => x.ms?.TotalNetWeight ?? 0)
								}).ToList();

				var currentDateTime = DateTime.Now;
				var currentDate = currentDateTime.Date;
				var currentTime = currentDateTime.TimeOfDay;
				int currentMonth = currentDateTime.Month;
				int currentYear = currentDateTime.Year;
				int currentQuarter = Convert.ToInt32(_L1DetailsService.GetQuarter(currentDate.ToString()));
				var getcount = _allLoadingSheetRepository.GetLastCOUNT(currentMonth, currentYear).Result + 1;
				string paddedCount = getcount.ToString("D4"); // Pads with leading zeros up to 4 digits  

				var allLoadingSheets = new AllLoadingSheetViewModel()
				{
					LoadingSheetNo = "SHEET" + currentYear + currentMonth.ToString("D2") + paddedCount,
					Trasporter = Transportdata,
					magzine = magzines,
					re11IndentInfos = indnetdata,
					Magzinestock = combined.Cast<object>().ToList()
				};

				_APIResponse.Data = allLoadingSheets;
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

		// GET: api/AllLoadingSheets/5
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllLoadingSheetById(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				string serializedList = string.Empty;
				var encodedList = await _distributedCache.GetAsync(KeyName);

				if (encodedList != null)
				{
					_APIResponse.Data = JsonConvert.DeserializeObject<List<AllLoadingSheet>>(Encoding.UTF8.GetString(encodedList))
						.FirstOrDefault(e => e.Id == id);
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					var loadingSheet = await _allLoadingSheetRepository.GetByIdAsync(id);
					if (loadingSheet == null)
						return NotFound();

					_APIResponse.Data = loadingSheet;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}

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

		// POST: api/AllLoadingSheets
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateAllLoadingSheet(AllLoadingSheetViewModel allLoadingSheetViewModel)
		{
			try
			{
				if (allLoadingSheetViewModel == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Message = "Invalid data submitted.";
					_APIResponse.Errors.Add("Invalid data submitted.");
					return BadRequest(_APIResponse);
				}

				var currentDateTime = DateTime.Now;
				var currentDate = currentDateTime.Date;
				var currentTime = currentDateTime.TimeOfDay;
				int currentMonth = currentDateTime.Month;
				int currentYear = currentDateTime.Year;
				int currentQuarter = Convert.ToInt32(_L1DetailsService.GetQuarter(currentDate.ToString()));
				var getcount = _allLoadingSheetRepository.GetLastCOUNT(currentMonth, currentYear).Result + 1;
				string paddedCount = getcount.ToString("D4"); // Pads with leading zeros up to 4 digits
				allLoadingSheetViewModel.LoadingSheetNo = "SHEET" + currentYear + currentMonth.ToString("D2") + paddedCount;

				foreach(var item in allLoadingSheetViewModel.IndentInfoViewModels.DistinctBy(x => (x.Bid, x.SizeCode)).ToList())
				{
					var re11IndentInfos = await _re11IndentInfoRepository.GetByIndentNO(item.IndentNo);
					if (re11IndentInfos == null)
					{
						_APIResponse.Status = false;
						_APIResponse.StatusCode = HttpStatusCode.NotFound;
						_APIResponse.Message = "Indent not found.";
						_APIResponse.Errors.Add("Indent not found.");
						return NotFound(_APIResponse);
					}

					var totalcase= allLoadingSheetViewModel.IndentInfoViewModels.Where(x => x.Bid == item.Bid && x.SizeCode == item.SizeCode).Sum(x => x.Loadcase);
					var totalwt= allLoadingSheetViewModel.IndentInfoViewModels.Where(x => x.Bid == item.Bid && x.SizeCode == item.SizeCode).Sum(x => x.LoadWt);

					var itemdetails= re11IndentInfos.IndentItems.FirstOrDefault(a=>a.Bid==item.Bid && a.SizeCode==item.SizeCode);

					if (itemdetails != null)
					{
						itemdetails.Loadcase = totalcase ?? 0;
						itemdetails.LoadWt = (decimal?)(totalwt ?? 0);
						itemdetails.Remcase = itemdetails.Remcase - totalcase;
						itemdetails.RemWt = itemdetails.RemWt - (decimal?)(totalwt ?? 0);
					}
					re11IndentInfos.CompletedIndent = re11IndentInfos.IndentItems.All(x => x.Remcase == 0) ? 1 : 0;
				}

				var newsheet = new AllLoadingSheet
				{
					Id = 0,
					LoadingSheetNo = allLoadingSheetViewModel.LoadingSheetNo,
					Mfgdt = currentDate,
					TName = allLoadingSheetViewModel.TransName,
					TruckNo = allLoadingSheetViewModel.TruckNo,
					TruckLic = allLoadingSheetViewModel.LicenseNo,
					LicVal = allLoadingSheetViewModel.Validity,
					CreationDateTime = currentDateTime,
					Month = currentMonth,
					Year = currentYear,
					Quarter = currentQuarter,
					Compflag = 0,
					IndentDetails = allLoadingSheetViewModel.IndentInfoViewModels.Select(x => new AllLoadingIndentDeatils
					{
						Id = 0,
						LoadingSheetId = 0,
						IndentNo = x.IndentNo,
						IndentDt = x.IndentDt,
						Bname = x.Bname,
						Bid = x.Bid,
						Ptype = x.Ptype,
						PtypeCode = x.PtypeCode,
						Psize = x.Psize,
						SizeCode = x.SizeCode,
						Class = (int)x.Class,
						Div = (int)x.Div,
						L1NetWt = (decimal)x.L1NetWt,
						Unit = x.Unit,
						Loadcase = x.Loadcase ?? 0,
						LoadWt = (decimal?)(x.LoadWt ?? 0),
						mag = x.mag,
						iscompleted = x.iscompleted ?? 0,
						TypeOfDispatch = x.TypeOfDispatch,
                        //Batch = string.Join(",", x.batches.Select(a => a.batch))
                        Batch = string.Join(",", x.batches?.Select(a => a.batch) ?? Enumerable.Empty<string>())

                    }).ToList()
				};

				
				await _allLoadingSheetRepository.AddAsync(newsheet);
				_APIResponse.Data = newsheet;
				_APIResponse.Message = "Success";
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction("GetAllLoadingSheetById", new { id = 1 }, _APIResponse);
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
		public async Task<ActionResult<APIResponse>> CreateMultipleTruckLoadingSheet([FromBody] List<MultipleVehicleWithIndetDTO> multipleVehicleWithIndetDTO)
		{


			var response = new APIResponse();

			if (multipleVehicleWithIndetDTO == null || multipleVehicleWithIndetDTO.Count == 0)
			{
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.BadRequest;
				_APIResponse.Message = "Invalid data submitted.";
				_APIResponse.Errors.Add("Invalid data submitted.");
				return BadRequest(_APIResponse);
			}

			try
			{
				foreach (var item in multipleVehicleWithIndetDTO)
				{
					var currentDateTime = DateTime.Now;
					var currentDate = currentDateTime.Date;
					var currentTime = currentDateTime.TimeOfDay;
					int currentMonth = currentDateTime.Month;
					int currentYear = currentDateTime.Year;
					int currentQuarter = Convert.ToInt32(_L1DetailsService.GetQuarter(currentDate.ToString()));
					var getcount = _allLoadingSheetRepository.GetLastCOUNT(currentMonth, currentYear).Result + 1;
					string paddedCount = getcount.ToString("D4"); // Pads with leading zeros up to 4 digits
					item.LoadingSheetNo = "SHEET" + currentYear + currentMonth.ToString("D2") + paddedCount;

					foreach (var newitem in item.Items.DistinctBy(x => (x.Bid, x.SizeCode)).ToList())
					{
						var re11IndentInfos = await _re11IndentInfoRepository.GetByIndentNO(newitem.IndentNo);
						if (re11IndentInfos == null)
						{
							_APIResponse.Status = false;
							_APIResponse.StatusCode = HttpStatusCode.NotFound;
							_APIResponse.Message = "Indent not found.";
							_APIResponse.Errors.Add("Indent not found.");
							return NotFound(_APIResponse);
						}

						var totalcase = item.Items.Where(x => x.Bid == newitem.Bid && x.SizeCode == newitem.SizeCode).Sum(x => x.LoadCase);
						var totalwt = totalcase * newitem.L1NetWt;

						var itemdetails = re11IndentInfos.IndentItems.FirstOrDefault(a => a.Bid == newitem.Bid && a.SizeCode == newitem.SizeCode);

						if (itemdetails != null)
						{
							itemdetails.Loadcase = totalcase;
							itemdetails.LoadWt = (decimal?)totalwt; 
							itemdetails.Remcase = itemdetails.Remcase - totalcase;
							itemdetails.RemWt = itemdetails.RemWt - (decimal?)totalwt;
						}
						re11IndentInfos.CompletedIndent = re11IndentInfos.IndentItems.All(x => x.Remcase == 0) ? 1 : 0;
					}

					var newsheet = new AllLoadingSheet
					{
						Id = 0,
						LoadingSheetNo = item.LoadingSheetNo,
						Mfgdt = currentDate,
						TName = item.TransporterName,
						TruckNo = item.TruckNo,
						TruckLic = item.LicenseNo,
						LicVal = item.Validity,
						CreationDateTime = currentDateTime,
						Month = currentMonth,
						Year = currentYear,
						Quarter = currentQuarter,
						Compflag = 0,
						IndentDetails = item.Items.Select(x => new AllLoadingIndentDeatils
						{
							Id = 0,
							LoadingSheetId = 0,
							IndentNo = x.IndentNo,
							IndentDt = x.IndentDt,
							Bname = x.Bname,
							Bid = x.Bid,
							Ptype = x.Ptype,
							PtypeCode = x.PtypeCode,
							Psize = x.Psize,
							SizeCode = x.SizeCode,
							Class = (int)x.Class,
							Div = (int)x.Div,
							L1NetWt = (decimal)x.L1NetWt,
							Unit = x.Unit,
							Loadcase = x.LoadCase,
							LoadWt = (decimal?)x.LoadWt,
							mag = x.Magazine,
							iscompleted =  0,
							TypeOfDispatch = x.DispatchType,

						}).ToList()
					};

					await _allLoadingSheetRepository.AddAsync(newsheet);

				}
				


				response.Data = multipleVehicleWithIndetDTO; // You can set this to the created resource if needed
				response.Message = "Vehicle assignment created successfully.";
				response.Status = true;
				response.StatusCode = HttpStatusCode.Created;
				return StatusCode(StatusCodes.Status201Created, response);
			}
			catch (Exception ex)
			{

				response.Status = false;
				response.Errors.Add(ex.Message);
				response.Message = "An error occurred while creating the vehicle assignment.";
				response.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, response);
			}
		}

		// PUT: api/AllLoadingSheets/5
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UpdateAllLoadingSheet(int id, AllLoadingSheet allLoadingSheet)
		{
			try
			{
				if (id != allLoadingSheet.Id)
					return BadRequest();

				var existingSheet = await _allLoadingSheetRepository.GetByIdAsync(id);
				if (existingSheet == null)
					return NotFound();

				await _distributedCache.RemoveAsync(KeyName);

				//existingSheet = _mapper.Map(allLoadingSheet, existingSheet);

				await _allLoadingSheetRepository.UpdateAsync(existingSheet);

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

		// DELETE: api/AllLoadingSheets/5
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> DeleteAllLoadingSheet(int id)
		{
			try
			{
				if (id == 0)
					return BadRequest();

				var loadingSheet = await _allLoadingSheetRepository.GetByIdAsync(id);
				if (loadingSheet == null)
					return NotFound();

				await _allLoadingSheetRepository.DeleteAsync(id);
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

		[HttpGet("{CustName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetIndentsByCName(string CustName)
		{
			try
			{
				// Fetch from database if cache doesn't contain the data
				var indents = await _re11IndentInfoRepository.GetIndentsByCName(CustName);

				if (indents == null || !indents.Any())
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("No brands found for the provided pcode.");
					return NotFound(_APIResponse);
				}

				_APIResponse.Data = indents;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				// Handle any exceptions that may occur
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}



		[HttpGet("{Indentno}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetPDetailByIndent(string Indentno)
		{
			var _APIResponse = new APIResponse(); // Ensure this is instantiated properly

			try
			{
				if (string.IsNullOrEmpty(Indentno))
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Errors.Add("Indent number cannot be empty.");
					return BadRequest(_APIResponse);
				}

				// Fetch from repository
				var indents = await _re11IndentInfoRepository.GetProductInfoByIndentNo(Indentno.Replace(".", "/"));

				if (indents == null || !indents.Any())
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("No product details found for the provided Indent number.");
					return NotFound(_APIResponse);
				}

				_APIResponse.Data = indents;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				// Log the exception (optional)
				Console.WriteLine($"Error in GetProductDetailByIndent: {ex.Message}");

				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}




	}
}

