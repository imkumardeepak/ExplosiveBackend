using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]

	public class DownloadUploadDataController : ControllerBase
	{
		private readonly ILogger<DownloadUploadDataController> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IDispatchDownloadRepository _dispatchDownloadRepository;
		private readonly IL1barcodegenerationRepository _l1barcodegenerationRepository;
		private readonly ITruckToMAGBarcodeRepository _truckToMAGBarcodeRepository;
		private readonly IAllLoadingSheetRepository _allLoadingSheetRepository;
		private readonly IRe11IndentInfoRepository _re11IndentInfoRepository;
		private readonly IDispatchTransactionRepository _dispatchTransactionRepository;
		private readonly IProductionMagzineAllocationRepository _productionMagzineAllocationRepository;
		private readonly IMagzine_StockRepository _StockRepository;
		private readonly WebSocketService _webSocketService;
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private APIResponse _APIResponse;
		private readonly IServiceScopeFactory _scopeFactory;

		public DownloadUploadDataController(ILogger<DownloadUploadDataController> logger, IMapper mapper, IDispatchDownloadRepository dispatchDownloadRepository, ITruckToMAGBarcodeRepository truckToMAGBarcodeRepository, IAllLoadingSheetRepository allLoadingSheetRepository, IRe11IndentInfoRepository re11IndentInfoRepository, IDispatchTransactionRepository dispatchTransactionRepository, IServiceScopeFactory scopeFactory, IL1barcodegenerationRepository l1barcodegenerationRepository, IMagzine_StockRepository stockRepository, WebSocketService webSocketService, IProductionMagzineAllocationRepository productionMagzineAllocationRepository, IUserRepository userRepository, ApplicationDbContext context)
		{
			_dispatchDownloadRepository = dispatchDownloadRepository;
			_allLoadingSheetRepository = allLoadingSheetRepository;
			_logger = logger;
			_mapper = mapper;
			_APIResponse = new APIResponse();
			_truckToMAGBarcodeRepository = truckToMAGBarcodeRepository;
			_re11IndentInfoRepository = re11IndentInfoRepository;
			_dispatchTransactionRepository = dispatchTransactionRepository;
			_scopeFactory = scopeFactory;
			_l1barcodegenerationRepository = l1barcodegenerationRepository;
			_StockRepository = stockRepository;
			_webSocketService = webSocketService;
			_productionMagzineAllocationRepository = productionMagzineAllocationRepository;
			_userRepository = userRepository;
			_context = context;
		}

		//GET LOGIN DETAILS IN ANDROID DEVICE
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetLoginDetails()
		{
			try
			{
				var users = await _userRepository.GetAllAsync();

				var userDTO = users.Select(x => new LoginDTO()
				{
					Username = x.Username,
					Password = "admin",
				}).ToList();

				_APIResponse.Data = userDTO;
				_APIResponse.Status = true;
				_APIResponse.Message = "Users retrieved successfully.";
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.Message = ex.Message;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}

		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> SyncLoadBoxInTruckData([FromBody] List<ProductionTransfer> transfers)
		{
			if (transfers == null || !transfers.Any())
				return BadRequest("Transfer list is empty.");

			// Collect all L1Barcodes from the request for batch validation
			var allRequestedBarcodes = transfers
				.SelectMany(t => t.Barcodes)
				.Select(b => b.L1Barcode)
				.ToList();

			// Check for duplicates in the database in a single query
			var existingBarcodes = await _context.ProductionTransferCases
				.Where(b => allRequestedBarcodes.Contains(b.L1Barcode))
				.Select(b => b.L1Barcode)
				.ToListAsync();

			if (existingBarcodes.Any())
			{
				var errorMessage = $"The following L1 barcodes already exist in the system: {string.Join(", ", existingBarcodes)}. Please use unique barcodes.";

				// Send error notification
				await _webSocketService.SendNotificationAsync("1",
					$"Failed to create transfers. Duplicate barcodes detected: {string.Join(", ", existingBarcodes.Take(5))}" +
					(existingBarcodes.Count > 5 ? " and more..." : ""));

				return BadRequest(errorMessage);
			}

			var successfulTransfers = new List<ProductionTransfer>();
			var getcount = await _context.ProductionTransfers
				.Where(a => a.months == DateTime.Now.Month && a.years == DateTime.Now.Year)
				.CountAsync() + 1;

			foreach (var transfer in transfers)
			{
				string paddedCount = getcount.ToString("D4");
				var data = new ProductionTransfer
				{
					Id = 0,
					TransId = "MAG-TRAN" + DateTime.Now.ToString("yyyyMM") + paddedCount,
					Barcodes = transfer.Barcodes.Select(b => new ProductionTransferCases
					{
						Id = 0,
						L1Barcode = b.L1Barcode,
						ProductionTransferId = 0
					}).ToList(),
					TruckNo = transfer.TruckNo,
					months = DateTime.Now.Month,
					years = DateTime.Now.Year,
					Allotflag = 0
				};

				_context.ProductionTransfers.Add(data);
				successfulTransfers.Add(data);
				getcount++;
			}

			try
			{
				await _context.SaveChangesAsync();

				// Send success notification
				await _webSocketService.SendNotificationAsync("1",
					$"Successfully created {transfers.Count} production transfers with {allRequestedBarcodes.Count} total cases. " +
					$"Please assign magazines with quantities to the transfers.");

				return Ok(successfulTransfers);
			}
			catch (DbUpdateException ex)
			{
				// Handle potential database constraint violations
				var innerMessage = ex.InnerException?.Message ?? ex.Message;



				return StatusCode(StatusCodes.Status500InternalServerError,
					"An error occurred while saving the transfers. Please try again.");
			}
			catch (Exception ex)
			{

				return StatusCode(StatusCodes.Status500InternalServerError,
					"An unexpected error occurred. Please contact support.");
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<ProductionMagzineAllocation>>> GetMagzineAllottedData()
		{
			try
			{
				var transferToMazgnies = await _context.ProductionMagzineAllocations.Where(a => a.ReadFlag == 0).ToListAsync();
				var transferIds = transferToMazgnies.Select(x => x.TransferId).Distinct().ToList();
				var list1 = await _context.ProductionTransfers.Where(x => transferIds.Contains(x.TransId)).Select(x => x.Id).ToListAsync();
				List<ProductionTransferCases> productionTransferCases = await _context.ProductionTransferCases.Where(x => list1.Contains(x.ProductionTransferId))
					.ToListAsync();

				_APIResponse.Data = new
				{
					transferToMazgnies = transferToMazgnies,
					productionTransferCases = productionTransferCases
				};
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Message = "ProductionMagzineAllocations fetched successfully";
				return Ok(_APIResponse);
			}
			catch (Exception ex)

			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error occurred while fetching ProductionMagzineAllocations";
				_APIResponse.Data = null;
				return Ok(_APIResponse);
			}
		}

		// sync magzine unload data from android device to server
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ProductionMagzineAllocation>> SyncMagzineUnloadData(List<ProductionMagzineAllocation> transferToMazgnie)
		{
			try
			{
				foreach (var item in transferToMazgnie)
				{
					var productionTransfers = await _context.ProductionMagzineAllocations.Where(x => x.PlantCode == item.PlantCode && x.BrandId == item.BrandId && x.ProductSize == item.ProductSize && x.MagazineName == item.MagazineName && x.TransferId == item.TransferId && x.ReadFlag == 0).Include(x => x.TransferToMazgnieScanneddata).FirstOrDefaultAsync();

					if (productionTransfers != null)
					{
						List<Magzine_Stock> magzineStocks = new List<Magzine_Stock>();
						productionTransfers.ReadFlag = 1;
						productionTransfers.TransferToMazgnieScanneddata.AddRange(item.TransferToMazgnieScanneddata);

						foreach (var data in item.TransferToMazgnieScanneddata)
						{

							var l1barcodedetails = await _l1barcodegenerationRepository.GetL1DataRecordByL1FristAndDefultAsync(data.L1Scanned);
							if (l1barcodedetails == null)
								continue;

							var magzineStock = new Magzine_Stock
							{
								L1Barcode = data.L1Scanned,
								MagName = item.MagazineName,
								BrandName = l1barcodedetails.BrandName,
								BrandId = l1barcodedetails.BrandId,
								PrdSize = l1barcodedetails.ProductSize,
								PSizeCode = l1barcodedetails.PSizeCode,
								Stock = 1,
								StkDt = DateTime.Now,
								Re2 = false,
								Re12 = false
							};

							magzineStocks.Add(magzineStock);
						}
						await _webSocketService.SendNotificationAsync("1", $"Successfully unloaded the {item.CaseQuantity} Cases to Magzine  {item.MagazineName}. Please make the RE-2 file of the cases.");
						_context.ProductionMagzineAllocations.Update(productionTransfers);
						await _StockRepository.SaveMagzineTransactionBulk(magzineStocks);
						await _context.SaveChangesAsync();
					}
				}

				_APIResponse.Data = transferToMazgnie;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				_APIResponse.Message = "ProductionMagzineAllocation Created Successfully";
				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Message = "Error occurred while creating ProductionMagzineAllocation";
				_APIResponse.Data = null;
				return Ok(_APIResponse);
			}
		}


		//GET LOADING SHEET DATA IN DEVICE 

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetDispatchData()
		{
			var response = new APIResponse();

			try
			{
				var dispatchData = await _allLoadingSheetRepository.GetLoadingByCflag(0);

				if (dispatchData == null || dispatchData.Count == 0)
				{
					response.Status = true;
					response.StatusCode = HttpStatusCode.OK;
					response.Data = dispatchData;
					response.Message = "No loading sheets found.";
					return Ok(response);
				}

				response.Data = dispatchData;
				response.Status = true;
				response.Message = "Loading sheets fetched successfully.";
				response.StatusCode = HttpStatusCode.OK;
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.Errors.Add(ex.Message);
				response.Status = false;
				response.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, response);
			}
		}

		//sync completed loading sheet
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> SyncCompletedLoadingSheets([FromBody] List<SyncCompletedLoadingSheetDto> request)
		{
			if (request == null || request.Count == 0)
			{
				return BadRequest(new
				{
					status = false,
					statusCode = 400,
					message = "No data provided"
				});
			}

			try
			{
				foreach (var item in request)
				{
					using var scope = _scopeFactory.CreateScope();

					var loadingRepo = scope.ServiceProvider.GetRequiredService<IAllLoadingSheetRepository>();
					var indentRepo = scope.ServiceProvider.GetRequiredService<IRe11IndentInfoRepository>();
					var dispatchRepo = scope.ServiceProvider.GetRequiredService<IDispatchTransactionRepository>();
					var Magzine_StockRepository = scope.ServiceProvider.GetRequiredService<IMagzine_StockRepository>();
					var productionMagzineAllocationRepo = scope.ServiceProvider.GetRequiredService<IProductionMagzineAllocationRepository>();

					var sheet = item.LoadingSheet;
					var barcodes = item.L1Barcodes;


					if (sheet == null || barcodes == null || barcodes.Count == 0)
						throw new ArgumentException("Invalid sheet or barcode data.");

					var loadingSheet = await loadingRepo.GetLoadingByLoadingSheet(sheet.LoadingNo);

					if (loadingSheet == null)
					{
						_APIResponse.Status = false;
						_APIResponse.StatusCode = HttpStatusCode.NotFound;
						_APIResponse.Message = "Loading sheet not found.";
						_APIResponse.Errors.Add($"Loading sheet with number {sheet.LoadingNo} not found.");
					}

					foreach (var barcode in loadingSheet.IndentDetails.Where(x => x.Bid == sheet.Bid && x.SizeCode == sheet.PCode && x.mag == sheet.Magzine && x.TypeOfDispatch == sheet.TypeOofDispatc && x.Loadcase == sheet.LaodCases))
					{
						barcode.iscompleted = 1;
					}
					var branddetails = loadingSheet.IndentDetails?.FirstOrDefault(x => x.Bid == sheet.Bid && x.SizeCode == sheet.PCode && x.mag == sheet.Magzine && x.TypeOfDispatch == sheet.TypeOofDispatc && x.Loadcase == sheet.LaodCases);

					if (branddetails == null)
					{
						_APIResponse.Status = false;
						_APIResponse.StatusCode = HttpStatusCode.NotFound;
						_APIResponse.Message = "Loading sheet not found.";
						_APIResponse.Errors.Add($"Loading sheet with number {sheet.LoadingNo} not found.");
					}


					if (loadingSheet.IndentDetails != null)
						loadingSheet.Compflag = loadingSheet.IndentDetails.All(x => x.iscompleted == 1) ? 1 : 0;

					var now = DateTime.Now;
					var l1NetUnit = branddetails.Unit;
					var l1NetWt = branddetails.L1NetWt;
					List<ProductionMagzineAllocation> productionMagzineAllocation = new List<ProductionMagzineAllocation>();
					if (sheet.TypeOofDispatc == "DD")
					{
						int getcount = await productionMagzineAllocationRepo.MaxCountAsync(DateTime.Now);

						string paddedCount = getcount.ToString("D4"); // Pads with leading zeros up to 4 digits
						productionMagzineAllocation.Add(new ProductionMagzineAllocation
						{
							Plant = branddetails.Ptype,
							PlantCode = branddetails.PtypeCode,
							BrandId = sheet.Bid,
							BrandName = sheet.BName,
							TruckNo = sheet.TruckNo,
							ProductSize = sheet.Product,
							ProductSizecCode = sheet.PCode,
							MagazineName = sheet.Magzine,
							TransferId = "DD-TRAN" + DateTime.Now.ToString("yyyyMM") + paddedCount,
							CaseQuantity = sheet.LaodCases,
							Totalwt = sheet.LaodCases * l1NetWt,
							TransferToMazgnieScanneddata = barcodes.Select(barcode => new ProductionMagzineAllocationCases
							{
								L1Scanned = barcode
							}).ToList(),
							ReadFlag = 1
						});
					}

					var dispatchTransactions = barcodes.Select(barcode => new DispatchTransaction
					{
						Tid = 0,
						Bid = sheet.Bid,
						L1Barcode = barcode,
						DispDt = now,
						IndentNo = sheet.IndentNo,
						TruckNo = sheet.TruckNo,
						Brand = sheet.BName,
						PSize = sheet.Product,
						PSizeCode = sheet.PCode,
						MagName = sheet.Magzine,
						Re12 = false,
						L1NetUnit = l1NetUnit,
						L1NetWt = l1NetWt
					}).ToList();


					List<Magzine_Stock> magzineStocks = new List<Magzine_Stock>();
					foreach (var data in barcodes)
					{

						var magzineStock = new Magzine_Stock
						{
							L1Barcode = data,
							MagName = sheet.Magzine,
							BrandName = sheet.BName,
							BrandId = sheet.Bid,
							PrdSize = sheet.Product,
							PSizeCode = sheet.PCode,
							Stock = 1,
							StkDt = DateTime.Now,
							Re2 = false,
							Re12 = false
						};
						magzineStocks.Add(magzineStock);
					}

					await _webSocketService.SendNotificationAsync("1", $"Successfully loaded {barcodes.Count()} cases from Magazine {sheet.Magzine} to Truck No {sheet.TruckNo}. Please generate the RE2 file for these cases.");

					await Magzine_StockRepository.SaveMagzineTransactionBulk(magzineStocks);
					await productionMagzineAllocationRepo.AddRangeAsync(productionMagzineAllocation);
					await dispatchRepo.SaveDispatchTransactionBulk(dispatchTransactions);
					_context.Update(loadingSheet);
					await _context.SaveChangesAsync();
				}
				return Ok(new
				{
					status = true,
					statusCode = 200,
					message = "Data synced successfully"
				});
			}
			catch (ArgumentException argEx)
			{
				return BadRequest(new
				{
					status = false,
					statusCode = 400,
					message = argEx.Message
				});
			}
			catch (KeyNotFoundException notFoundEx)
			{
				return NotFound(new
				{
					status = false,
					statusCode = 404,
					message = notFoundEx.Message
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while syncing completed loading sheets.");
				return StatusCode(500, new
				{
					status = false,
					statusCode = 500,
					message = $"Internal server error: {ex.Message}"
				});
			}
		}

	}
}





