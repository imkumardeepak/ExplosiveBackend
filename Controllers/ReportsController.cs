using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using OfficeOpenXml;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class ReportsController : ControllerBase
	{
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IBrandMasterRepository _brandMasterRepository;
		private readonly IMagzine_StockRepository _Magzine_StockRepository;
		private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
		private readonly IRe11IndentPrdInfoRepository _Re11IndentPrdInfoRepository;
		private readonly Il1boxdeletionRepository _l1boxdeletionRepository;
		private readonly Il1barcodereprintRepository _l1barcodereprintRepository;
		private readonly Ihht_prodtomagtransferRepository _hht_prodtomagtransferRepository;
		private readonly Ireprint_l2barcodeRepository _reprint_l2barcodeRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "BrandMaster";

		public ReportsController(IBrandMasterRepository brandMasterRepository, IDistributedCache distributedCache, IMapper mapper, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMagzine_StockRepository magzine_StockRepository, IDispatchTransactionRepository dispatchTransactionRepository, IRe11IndentPrdInfoRepository re11IndentPrdInfoRepository, Il1boxdeletionRepository l1boxdeletionRepository, Il1barcodereprintRepository l1barcodereprintRepository, Ireprint_l2barcodeRepository reprint_l2barcodeRepository, Ihht_prodtomagtransferRepository hht_prodtomagtransferRepository)
		{
			_brandMasterRepository = brandMasterRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_mapper = mapper;
			_l1BarcodegenerationRepository = l1BarcodegenerationRepository;
			_Magzine_StockRepository = magzine_StockRepository;
			_DispatchTransactionRepository = dispatchTransactionRepository;
			_Re11IndentPrdInfoRepository = re11IndentPrdInfoRepository;
			_l1boxdeletionRepository = l1boxdeletionRepository;
			_l1barcodereprintRepository = l1barcodereprintRepository;
			_reprint_l2barcodeRepository = reprint_l2barcodeRepository;
			_hht_prodtomagtransferRepository = hht_prodtomagtransferRepository;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getproreport(string fromDate, string toDate, string reportType, string? shift, string? plant, string? brand, string? productsize)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _l1BarcodegenerationRepository.GetProductionReportAsync(fromDateTime, toDateTime, reportType, shift, plant, brand, productsize);

				_APIResponse.Data = reportData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		//export report in excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportProReportToExcel(string fromDate, string toDate, string reportType, string? shift, string? plant, string? brand, string? productsize)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _l1BarcodegenerationRepository.GetProductionReportAsync(fromDateTime, toDateTime, reportType, shift, plant, brand, productsize);

				// Create a new Excel package

				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("Production Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "Production Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(shift) || !string.IsNullOrEmpty(plant) || !string.IsNullOrEmpty(brand) || !string.IsNullOrEmpty(productsize))
					{
						worksheet.Cells[currentRow, 1].Value = "Filters Applied:";
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow++;

						int filterCol = 1;
						if (!string.IsNullOrEmpty(shift))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Shift:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = shift;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(plant))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Plant:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = plant;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(brand))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Brand:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = brand;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(productsize))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Product Size:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = productsize;
						}
						currentRow += 2;
					}

					// Create column headers based on report type
					var headers = reportType == "Detailed"
						? new[] { "Manufacturing Date", "Plant Name", "Shift", "Brand Name", "Product Size", "L1 Barcode", "L1 Net Qty", "L1 Net Unit" }
						: new[] { "Manufacturing Date", "Plant Name", "Shift", "Brand Name", "Product Size", "Box Count", "L1 Net Qty", "L1 Net Unit" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							if (reportType == "Detailed")
							{
								worksheet.Cells[currentRow, 1].Value = item.mfgdt.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 2].Value = item.plantname;
								worksheet.Cells[currentRow, 3].Value = item.shift;
								worksheet.Cells[currentRow, 4].Value = item.brandname;
								worksheet.Cells[currentRow, 5].Value = item.productsize;
								worksheet.Cells[currentRow, 6].Value = item.l1barcode;
								worksheet.Cells[currentRow, 7].Value = item.l1netqty;
								worksheet.Cells[currentRow, 8].Value = item.l1netunit;
							}
							else
							{
								worksheet.Cells[currentRow, 1].Value = item.mfgdt.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 2].Value = item.plantname;
								worksheet.Cells[currentRow, 3].Value = item.shift;
								worksheet.Cells[currentRow, 4].Value = item.brandname;
								worksheet.Cells[currentRow, 5].Value = item.productsize;
								worksheet.Cells[currentRow, 6].Value = item.boxcount;
								worksheet.Cells[currentRow, 7].Value = item.l1netqty;
								worksheet.Cells[currentRow, 8].Value = item.l1netunit;
							}

							// Apply borders to data cells
							for (int col = 1; col <= 8; col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= 9; col++)
						{
							worksheet.Column(col).AutoFit();
						}

						// Add summary row for summarized report
						if (reportType != "Detailed")
						{
							currentRow++;
							worksheet.Cells[currentRow, 1].Value = "Total";
							worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total box count
							var totalBoxCount = reportData.Sum(x => int.TryParse(x.l1barcode, out int boxCount) ? boxCount : 0);
							worksheet.Cells[currentRow, 7].Value = totalBoxCount;
							worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total quantity
							var totalQuantity = reportData.Sum(x => double.TryParse(x.l1netunit, out double qty) ? qty : 0);
							worksheet.Cells[currentRow, 8].Value = totalQuantity;
							worksheet.Cells[currentRow, 8].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Apply background color to summary row
							for (int col = 1; col <= 9; col++)
							{
								worksheet.Cells[currentRow, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
								worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
							}
						}
					}

					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Generate file name with timestamp
					var fileName = $"ProductionReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getstockreport(string? fromDate, string? toDate, string reportType, string? magzine, string? brand, string? productsize)
		{
			try
			{
				// Fetch report data
				var plantData = await _Magzine_StockRepository.GetStorageStockReportAsync(fromDate, toDate, reportType, magzine, brand, productsize);

				// Return successful response
				return Ok(new APIResponse
				{
					Data = plantData,
					Status = true,
					StatusCode = HttpStatusCode.OK
				});
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		// Export stock report to Excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportStockReportToExcel(string? fromDate, string? toDate, string reportType, string? magzine, string? brand, string? productsize)
		{
			try
			{
				// Fetch report data
				var stockData = await _Magzine_StockRepository.GetStorageStockReportAsync(fromDate, toDate, reportType, magzine, brand, productsize);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("Stock Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "Storage Stock Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = string.IsNullOrEmpty(fromDate) ? "All" : DateTime.Parse(fromDate).ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = string.IsNullOrEmpty(toDate) ? "All" : DateTime.Parse(toDate).ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(magzine) || !string.IsNullOrEmpty(brand) || !string.IsNullOrEmpty(productsize))
					{
						worksheet.Cells[currentRow, 1].Value = "Filters Applied:";
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow++;

						int filterCol = 1;
						if (!string.IsNullOrEmpty(magzine))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Magazine:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = magzine;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(brand))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Brand:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = brand;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(productsize))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Product Size:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = productsize;
						}
						currentRow += 2;
					}

					// Create column headers based on report type
					var headers = reportType == "Storage"
						? new[] { "Magazine Name", "License No", "Brand Name", "Product Size", "L1 Barcode", "Net Quantity", "Unit" }
						: new[] { "Magazine Name", "License No", "Brand Name", "Product Size", "Box Count", "Total Quantity", "Unit" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (stockData != null && stockData.Any())
					{
						foreach (var item in stockData)
						{
							worksheet.Cells[currentRow, 1].Value = item.magname;
							worksheet.Cells[currentRow, 2].Value = item.license;
							worksheet.Cells[currentRow, 3].Value = item.brandname;
							worksheet.Cells[currentRow, 4].Value = item.productsize;
							worksheet.Cells[currentRow, 5].Value = item.L1Barcode;

							// Handle numeric values safely
							if (double.TryParse(item.netqty, out double netQty))
							{
								worksheet.Cells[currentRow, 6].Value = netQty;
							}
							else
							{
								worksheet.Cells[currentRow, 6].Value = item.netqty;
							}

							worksheet.Cells[currentRow, 7].Value = item.unit;

							// Apply borders to data cells
							for (int col = 1; col <= 7; col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= 7; col++)
						{
							worksheet.Column(col).AutoFit();
						}

						// Add summary row for summarized report
						if (reportType != "Storage")
						{
							currentRow++;
							worksheet.Cells[currentRow, 1].Value = "Total";
							worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total box count
							var totalBoxCount = stockData.Sum(x => int.TryParse(x.L1Barcode, out int boxCount) ? boxCount : 0);
							worksheet.Cells[currentRow, 5].Value = totalBoxCount;
							worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total quantity
							var totalQuantity = stockData.Sum(x => double.TryParse(x.netqty, out double qty) ? qty : 0);
							worksheet.Cells[currentRow, 6].Value = totalQuantity;
							worksheet.Cells[currentRow, 6].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Apply background color to summary row
							for (int col = 1; col <= 7; col++)
							{
								worksheet.Cells[currentRow, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
								worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
							}
						}
					}
					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;

					// Generate file name with timestamp
					var fileName = $"StockReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		// Export L2 reprint data to Excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportL2ReprintDataToExcel(string fromDate, string toDate, string reportType, string? plant)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _reprint_l2barcodeRepository.GetL2ReprintDataAsync(fromDateTime, toDateTime, reportType, plant);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("L2 Reprint Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "L2 Reprint Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(plant))
					{
						worksheet.Cells[currentRow, 1].Value = "Plant:";
						worksheet.Cells[currentRow, 2].Value = plant;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow += 2;
					}

					// Create column headers based on report type - matching specified column structure
					var headers = reportType == "Detailed"
						? new[] { "Plant Code", "L2 Barcode", "Reprint Date", "Reason" }
						: new[] { "Plant Code", "No. Of Stickers" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							if (reportType == "Detailed")
							{
								worksheet.Cells[currentRow, 1].Value = item.plantcode;
								worksheet.Cells[currentRow, 2].Value = item.L2Barcode;
								worksheet.Cells[currentRow, 3].Value = item.reprintDt?.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 4].Value = item.reason;
							}
							else
							{
								worksheet.Cells[currentRow, 1].Value = item.plantcode;
								worksheet.Cells[currentRow, 2].Value = item.L2Barcode; // This contains the count in summary view
							}

							// Apply borders to data cells
							for (int col = 1; col <= (reportType == "Detailed" ? 4 : 2); col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= (reportType == "Detailed" ? 4 : 2); col++)
						{
							worksheet.Column(col).AutoFit();
						}

						// Add summary row for summarized report
						if (reportType != "Detailed")
						{
							currentRow++;
							worksheet.Cells[currentRow, 1].Value = "Total";
							worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total stickers
							var totalStickers = reportData.Sum(x => int.TryParse(x.L2Barcode, out int stickerCount) ? stickerCount : 0);
							worksheet.Cells[currentRow, 2].Value = totalStickers;
							worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Apply background color to summary row
							for (int col = 1; col <= 2; col++)
							{
								worksheet.Cells[currentRow, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
								worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
							}
						}
					}
					else
					{
						int columnCount = reportType == "Detailed" ? 4 : 2;
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, columnCount].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;

					// Generate file name with timestamp
					var fileName = $"L2ReprintReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportL1ReprintDataToExcel(string fromDate, string toDate, string reportType, string? plant)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _l1barcodereprintRepository.GetL1ReprintDataAsync(fromDateTime, toDateTime, reportType, plant);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("L1 Reprint Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "L1 Reprint Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(plant))
					{
						worksheet.Cells[currentRow, 1].Value = "Plant:";
						worksheet.Cells[currentRow, 2].Value = plant;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow += 2;
					}

					// Create column headers based on report type - matching specified column structure
					var headers = reportType == "Detailed"
						? new[] { "Plant Name", "Brand Name", "Product Size", "L1 Barcode", "Reprint Date", "Reason", "Reprint Time" }
						: new[] { "Plant Name", "Brand Name", "Product Size", "Box Count" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							if (reportType == "Detailed")
							{
								worksheet.Cells[currentRow, 1].Value = item.plant;
								worksheet.Cells[currentRow, 2].Value = item.brandname;
								worksheet.Cells[currentRow, 3].Value = item.productsize;
								worksheet.Cells[currentRow, 4].Value = item.L1Barcode;
								worksheet.Cells[currentRow, 5].Value = item.reprintDt?.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 6].Value = item.reason;
								worksheet.Cells[currentRow, 7].Value = item.time;
							}
							else
							{
								worksheet.Cells[currentRow, 1].Value = item.plant;
								worksheet.Cells[currentRow, 2].Value = item.brandname;
								worksheet.Cells[currentRow, 3].Value = item.productsize;

								// For summarized report, box count is calculated in the repository
								worksheet.Cells[currentRow, 4].Value = item.L1Barcode; // This contains the box count in summary view
							}

							// Apply borders to data cells
							for (int col = 1; col <= (reportType == "Detailed" ? 7 : 4); col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= (reportType == "Detailed" ? 7 : 4); col++)
						{
							worksheet.Column(col).AutoFit();
						}

						// Add summary row for summarized report
						if (reportType != "Detailed")
						{
							currentRow++;
							worksheet.Cells[currentRow, 1].Value = "Total";
							worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total box count
							var totalBoxCount = reportData.Sum(x => int.TryParse(x.L1Barcode, out int boxCount) ? boxCount : 0);
							worksheet.Cells[currentRow, 4].Value = totalBoxCount;
							worksheet.Cells[currentRow, 4].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Apply background color to summary row
							for (int col = 1; col <= 4; col++)
							{
								worksheet.Cells[currentRow, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
								worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
							}
						}
					}
					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, (reportType == "Detailed" ? 7 : 4)].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;

					// Generate file name with timestamp
					var fileName = $"L1ReprintReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportL1BoxDataToExcel(string fromDate, string toDate, string reportType)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _l1boxdeletionRepository.GetL1BoxDeletionDataAsync(fromDateTime, toDateTime, reportType);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("L1 Box Deletion Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "L1 Box Deletion Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Create column headers based on report type - matching specified column structure
					var headers = reportType == "Detailed"
						? new[] { "Plant Name", "Brand Name", "Product Size", "MFG Date", "L1 Barcode", "Deletion Date", "Reason" }
						: new[] { "Plant Name", "Brand Name", "Product Size", "Box Count" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							if (reportType == "Detailed")
							{
								worksheet.Cells[currentRow, 1].Value = item.plant;
								worksheet.Cells[currentRow, 2].Value = item.brand;
								worksheet.Cells[currentRow, 3].Value = item.productsize;
								worksheet.Cells[currentRow, 4].Value = item.mfgDt?.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 5].Value = item.l1barcode;
								worksheet.Cells[currentRow, 6].Value = item.deletionDt.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 7].Value = item.reason;
							}
							else
							{
								worksheet.Cells[currentRow, 1].Value = item.plant;
								worksheet.Cells[currentRow, 2].Value = item.brand;
								worksheet.Cells[currentRow, 3].Value = item.productsize;

								// For summarized report, box count is calculated in the repository
								worksheet.Cells[currentRow, 4].Value = item.l1barcode; // This contains the box count in summary view
							}

							// Apply borders to data cells
							for (int col = 1; col <= (reportType == "Detailed" ? 7 : 4); col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= (reportType == "Detailed" ? 7 : 4); col++)
						{
							worksheet.Column(col).AutoFit();
						}

						// Add summary row for summarized report
						if (reportType != "Detailed")
						{
							currentRow++;
							worksheet.Cells[currentRow, 1].Value = "Total";
							worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total box count
							var totalBoxCount = reportData.Sum(x => int.TryParse(x.l1barcode, out int boxCount) ? boxCount : 0);
							worksheet.Cells[currentRow, 4].Value = totalBoxCount;
							worksheet.Cells[currentRow, 4].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Apply background color to summary row
							for (int col = 1; col <= 4; col++)
							{
								worksheet.Cells[currentRow, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
								worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
							}
						}
					}
					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, (reportType == "Detailed" ? 7 : 4)].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;

					// Generate file name with timestamp
					var fileName = $"L1BoxDeletionReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		// Export RE7 report to Excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportRE7ReportToExcel(string? fromDate, string? toDate, string? magname)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _Magzine_StockRepository.GetRE7Report(fromDateTime, toDateTime, magname);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("RE7 Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "RE7 Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "From Date:";
					worksheet.Cells[currentRow, 2].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "To Date:";
					worksheet.Cells[currentRow, 5].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(magname))
					{
						worksheet.Cells[currentRow, 1].Value = "Magazine:";
						worksheet.Cells[currentRow, 2].Value = magname;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow += 2;
					}

					// Create column headers - matching specified column structure (detailed view only)
					var headers = new[] { "Brand Name", "Product Size", "Inward Quantity", "Outward Quantity", "Remaining Quantity" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							// Map data to match specified column structure
							worksheet.Cells[currentRow, 1].Value = item.BrandName;
							worksheet.Cells[currentRow, 2].Value = item.ProductSize;
							worksheet.Cells[currentRow, 3].Value = item.Inward;
							worksheet.Cells[currentRow, 4].Value = item.Outward;
							worksheet.Cells[currentRow, 5].Value = item.RemainingStock; // Remaining Quantity

							// Apply borders to data cells
							for (int col = 1; col <= 5; col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= 5; col++)
						{
							worksheet.Column(col).AutoFit();
						}
					}
					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;

					// Generate file name with timestamp
					var fileName = $"RE7Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetDispatchData(string fromDate, string toDate, string reportType, string? magzine, string? indentno, string? customer)
		{
			try
			{
				// Call repository to get dispatch data with filters
				var dispatchData = await _DispatchTransactionRepository.GetDispatchDataAsync(fromDate, toDate, reportType, magzine, indentno, customer);

				// Prepare API response
				_APIResponse.Data = dispatchData;
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRE11statusdata(string fromDate, string toDate, int? re11Status, string? indentno, string? customer)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var data = await _Re11IndentPrdInfoRepository.GetRE11StatusDataAsync(fromDateTime, toDateTime, re11Status, indentno, customer);

				_APIResponse.Data = data;
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRE2statusdata(string fromDate, string toDate, string reportType, int? re2status, string? brand, string? productsize)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var plantData = await _Magzine_StockRepository.GetRE2StatusDataAsync(fromDateTime, toDateTime, reportType, re2status, brand, productsize);

				_APIResponse.Data = plantData;
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getl1boxdata(string fromDate, string toDate, string reportType)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var plantData = await _l1boxdeletionRepository.GetL1BoxDeletionDataAsync(fromDateTime, toDateTime, reportType);

				_APIResponse.Data = plantData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getl1reprintdata(string fromDate, string toDate, string reportType, string? plant)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var plantData = await _l1barcodereprintRepository.GetL1ReprintDataAsync(fromDateTime, toDateTime, reportType, plant);

				_APIResponse.Data = plantData;
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getl2reprintdata(string fromDate, string toDate, string reportType, string? plant)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var plantData = await _reprint_l2barcodeRepository.GetL2ReprintDataAsync(fromDateTime, toDateTime, reportType, plant);

				_APIResponse.Data = plantData;
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getprotransfetchdata(string fromDate, string toDate, string reportType, string? pcode)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var plantData = await _hht_prodtomagtransferRepository.GetTransferDataAsync(fromDateTime, toDateTime, reportType, pcode);

				_APIResponse.Data = plantData;
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
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> Getre7report(string? fromDate, string? toDate, string? magname)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _Magzine_StockRepository.GetRE7Report(fromDateTime, toDateTime, magname);

				_APIResponse.Data = reportData;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return _APIResponse;
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				_APIResponse.Status = false;

				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}


		// Export dispatch data to Excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportDispatchDataToExcel(string fromDate, string toDate, string reportType, string? magzine, string? indentno, string? customer)
		{
			try
			{
				// Fetch report data
				var reportData = await _DispatchTransactionRepository.GetDispatchDataAsync(fromDate, toDate, reportType, magzine, indentno, customer);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("Dispatch Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "Dispatch Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 9].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = DateTime.Parse(fromDate).ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = DateTime.Parse(toDate).ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(magzine) || !string.IsNullOrEmpty(indentno) || !string.IsNullOrEmpty(customer))
					{
						worksheet.Cells[currentRow, 1].Value = "Filters Applied:";
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow++;

						int filterCol = 1;
						if (!string.IsNullOrEmpty(magzine))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Magazine:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = magzine;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(indentno))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Indent No:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = indentno;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(customer))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Customer:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = customer;
						}
						currentRow += 2;
					}

					// Create column headers based on report type - matching REPORT COLUMNS SUMMARY
					var headers = reportType == "Detailed"
						? new[] { "RE11 Indent No.", "Dispt. Dt.", "Truck No.", "L1 Bardcode", "Brand", "Product Size", "Mag Name", "Qty.", "Unit" }
						: new[] { "RE11 Indent No.", "Dispt. Dt.", "Truck No.", "Brand", "Product Size", "Mag Name", "Box Count", "Qty.", "Unit" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							if (reportType == "Detailed")
							{
								worksheet.Cells[currentRow, 1].Value = item.re11indentno;
								worksheet.Cells[currentRow, 2].Value = item.dispatchDt?.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 3].Value = item.truck;
								worksheet.Cells[currentRow, 4].Value = item.L1Barcode;
								worksheet.Cells[currentRow, 5].Value = item.brandname;
								worksheet.Cells[currentRow, 6].Value = item.productsize;
								worksheet.Cells[currentRow, 7].Value = item.magname;

								// Handle numeric values safely
								if (double.TryParse(item.netqty, out double netQty))
								{
									worksheet.Cells[currentRow, 8].Value = netQty;
								}
								else
								{
									worksheet.Cells[currentRow, 8].Value = item.netqty;
								}

								worksheet.Cells[currentRow, 9].Value = item.unit;
							}
							else
							{
								worksheet.Cells[currentRow, 1].Value = item.re11indentno;
								worksheet.Cells[currentRow, 2].Value = item.dispatchDt?.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 3].Value = item.truck;
								worksheet.Cells[currentRow, 4].Value = item.brandname;
								worksheet.Cells[currentRow, 5].Value = item.productsize;
								worksheet.Cells[currentRow, 6].Value = item.magname;

								// For summarized report, box count is calculated in the repository
								worksheet.Cells[currentRow, 7].Value = item.L1Barcode; // This contains the box count in summary view

								// Handle numeric values safely
								if (double.TryParse(item.netqty, out double netQty))
								{
									worksheet.Cells[currentRow, 8].Value = netQty;
								}
								else
								{
									worksheet.Cells[currentRow, 8].Value = item.netqty;
								}

								worksheet.Cells[currentRow, 9].Value = item.unit;
							}

							// Apply borders to data cells
							for (int col = 1; col <= 9; col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= 9; col++)
						{
							worksheet.Column(col).AutoFit();
						}
					}
					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, 9].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 9].Merge = true;

					// Generate file name with timestamp
					var fileName = $"DispatchReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}


		// Export RE11 status data to Excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportRE11StatusDataToExcel(string fromDate, string toDate, int? re11Status, string? indentno, string? customer)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _Re11IndentPrdInfoRepository.GetRE11StatusDataAsync(fromDateTime, toDateTime, re11Status, indentno, customer);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("RE11 Status Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "RE11 Status Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 10].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "From Date:";
					worksheet.Cells[currentRow, 2].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "To Date:";
					worksheet.Cells[currentRow, 5].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					if (re11Status.HasValue)
					{
						worksheet.Cells[currentRow, 7].Value = "Status:";
						worksheet.Cells[currentRow, 8].Value = re11Status.Value;
						worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					}
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(indentno) || !string.IsNullOrEmpty(customer))
					{
						worksheet.Cells[currentRow, 1].Value = "Filters Applied:";
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow++;

						int filterCol = 1;
						if (!string.IsNullOrEmpty(indentno))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Indent No:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = indentno;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(customer))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Customer:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = customer;
						}
						currentRow += 2;
					}

					// Create column headers - matching specified column structure
					var headers = new[] { "RE11 Indent No.", "Indent. Dt.", "Product", "Brand", "Product Size", "Requred Qty.", "Req. Unit", "Remaining Qty.", "Rem. Unit", "Status" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							// Map data to match specified column structure
							worksheet.Cells[currentRow, 1].Value = item.re11indentno;
							worksheet.Cells[currentRow, 2].Value = item.indentDt?.ToString("dd-MMM-yyyy");
							worksheet.Cells[currentRow, 3].Value = item.Ptype; // Product
							worksheet.Cells[currentRow, 4].Value = item.brand;
							worksheet.Cells[currentRow, 5].Value = item.productsize;
							worksheet.Cells[currentRow, 6].Value = item.reqqty; // Required Qty
							worksheet.Cells[currentRow, 7].Value = item.requnit; // Req Unit
							worksheet.Cells[currentRow, 8].Value = item.remqty; // Remaining Qty
							worksheet.Cells[currentRow, 9].Value = item.remunit; // Rem Unit
							worksheet.Cells[currentRow, 10].Value = item.status;

							// Apply borders to data cells
							for (int col = 1; col <= 10; col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= 10; col++)
						{
							worksheet.Column(col).AutoFit();
						}
					}
					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, 10].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 10].Merge = true;

					// Generate file name with timestamp
					var fileName = $"RE11StatusReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}

		// Export RE2 status data to Excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportRE2StatusDataToExcel(string fromDate, string toDate, string reportType, int? re2status, string? brand, string? productsize)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _Magzine_StockRepository.GetRE2StatusDataAsync(fromDateTime, toDateTime, reportType, re2status, brand, productsize);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("RE2 Status Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "RE2 Status Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (re2status.HasValue || !string.IsNullOrEmpty(brand) || !string.IsNullOrEmpty(productsize))
					{
						worksheet.Cells[currentRow, 1].Value = "Filters Applied:";
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow++;

						int filterCol = 1;
						if (re2status.HasValue)
						{
							worksheet.Cells[currentRow, filterCol].Value = "RE2 Status:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = re2status.Value;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(brand))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Brand:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = brand;
							filterCol += 2;
						}

						if (!string.IsNullOrEmpty(productsize))
						{
							worksheet.Cells[currentRow, filterCol].Value = "Product Size:";
							worksheet.Cells[currentRow, filterCol].Style.Font.Bold = true;
							worksheet.Cells[currentRow, filterCol + 1].Value = productsize;
						}
						currentRow += 2;
					}

					// Create column headers based on report type - matching specified column structure
					var headers = reportType == "Detailed"
						? new[] { "Brand Name", "Product Size", "Magazine Name", "Unload Date", "L1 Barcode", "RE2 Status" }
						: new[] { "Brand Name", "Product Size", "Magazine Name", "Unload Date", "Box Count", "RE2 Status" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							if (reportType == "Detailed")
							{
								worksheet.Cells[currentRow, 1].Value = item.brandname;
								worksheet.Cells[currentRow, 2].Value = item.productsize;
								worksheet.Cells[currentRow, 3].Value = item.magname;
								worksheet.Cells[currentRow, 4].Value = item.unloadDt?.ToString("dd-MMM-yyyy");
								worksheet.Cells[currentRow, 5].Value = item.l1barcode;
								worksheet.Cells[currentRow, 6].Value = item.re2status;
							}
							else
							{
								worksheet.Cells[currentRow, 1].Value = item.brandname;
								worksheet.Cells[currentRow, 2].Value = item.productsize;
								worksheet.Cells[currentRow, 3].Value = item.magname;
								worksheet.Cells[currentRow, 4].Value = item.unloadDt?.ToString("dd-MMM-yyyy");

								// For summarized report, box count is calculated in the repository
								worksheet.Cells[currentRow, 5].Value = item.l1barcode; // This contains the box count in summary view

								worksheet.Cells[currentRow, 6].Value = item.re2status;
							}

							// Apply borders to data cells
							for (int col = 1; col <= (reportType == "Detailed" ? 6 : 6); col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= 6; col++)
						{
							worksheet.Column(col).AutoFit();
						}

						// Add summary row for summarized report
						if (reportType != "Detailed")
						{
							currentRow++;
							worksheet.Cells[currentRow, 1].Value = "Total";
							worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Calculate total box count
							var totalBoxCount = reportData.Sum(x => int.TryParse(x.l1barcode, out int boxCount) ? boxCount : 0);
							worksheet.Cells[currentRow, 5].Value = totalBoxCount;
							worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
							worksheet.Cells[currentRow, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

							// Apply background color to summary row
							for (int col = 1; col <= 6; col++)
							{
								worksheet.Cells[currentRow, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
								worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
							}
						}
					}
					else
					{
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, 6].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;

					// Generate file name with timestamp
					var fileName = $"RE2StatusReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}
		// Export production transfer fetch data to Excel
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ExportProTransFetchDataToExcel(string fromDate, string toDate, string reportType, string? pcode)
		{
			try
			{
				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);

				var reportData = await _hht_prodtomagtransferRepository.GetTransferDataAsync(fromDateTime, toDateTime, reportType, pcode);

				// Create a new Excel package
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("Production Transfer Report");

					// Set header information
					int currentRow = 1;

					// Report Title
					worksheet.Cells[currentRow, 1].Value = "Production Transfer Report";
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
					worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
					worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
					worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					currentRow += 2;

					// Report Details
					worksheet.Cells[currentRow, 1].Value = "Report Type:";
					worksheet.Cells[currentRow, 2].Value = reportType;
					worksheet.Cells[currentRow, 1].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 4].Value = "From Date:";
					worksheet.Cells[currentRow, 5].Value = fromDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 4].Style.Font.Bold = true;

					worksheet.Cells[currentRow, 7].Value = "To Date:";
					worksheet.Cells[currentRow, 8].Value = toDateTime.ToString("dd-MMM-yyyy");
					worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
					currentRow += 2;

					// Add filters information if provided
					if (!string.IsNullOrEmpty(pcode))
					{
						worksheet.Cells[currentRow, 1].Value = "Plant Code:";
						worksheet.Cells[currentRow, 2].Value = pcode;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						currentRow += 2;
					}

					// Create column headers based on report type
					var headers = reportType == "Detailed"
						? new[] { "Plant Code", "Truck No", "L1 Barcode", "LUL", "Transfer Date", "Month", "Year", "TD", "Brand", "Product Size", "Mfg Date" }
						: new[] { "Plant Code", "Truck No", "Box Count", "Transfer Date", "Month", "Year" };

					// Apply header styling
					for (int i = 0; i < headers.Length; i++)
					{
						worksheet.Cells[currentRow, i + 1].Value = headers[i];
						worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}
					currentRow++;

					// Add data rows
					if (reportData != null && reportData.Any())
					{
						foreach (var item in reportData)
						{
							if (reportType == "Detailed")
							{
								worksheet.Cells[currentRow, 1].Value = item.pcode;
								worksheet.Cells[currentRow, 2].Value = item.truckno;
								worksheet.Cells[currentRow, 3].Value = item.l1barcode;
								worksheet.Cells[currentRow, 4].Value = item.lul;
								worksheet.Cells[currentRow, 5].Value = item.transferdt;
								worksheet.Cells[currentRow, 6].Value = item.month;
								worksheet.Cells[currentRow, 7].Value = item.year;
								worksheet.Cells[currentRow, 8].Value = item.td;
								worksheet.Cells[currentRow, 9].Value = item.brand;
								worksheet.Cells[currentRow, 10].Value = item.psize;
								worksheet.Cells[currentRow, 11].Value = item.mfgdt;
							}
							else
							{
								worksheet.Cells[currentRow, 1].Value = item.pcode;
								worksheet.Cells[currentRow, 2].Value = item.truckno;

								// For summarized report, we would need to calculate box count
								worksheet.Cells[currentRow, 3].Value = "N/A"; // Would need grouping logic

								worksheet.Cells[currentRow, 4].Value = item.transferdt;
								worksheet.Cells[currentRow, 5].Value = item.month;
								worksheet.Cells[currentRow, 6].Value = item.year;
							}

							// Apply borders to data cells
							for (int col = 1; col <= (reportType == "Detailed" ? 11 : 6); col++)
							{
								worksheet.Cells[currentRow, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
							}
							currentRow++;
						}

						// Auto-fit columns for better visibility
						for (int col = 1; col <= (reportType == "Detailed" ? 11 : 6); col++)
						{
							worksheet.Column(col).AutoFit();
						}
					}
					else
					{
						int columnCount = reportType == "Detailed" ? 11 : 6;
						worksheet.Cells[currentRow, 1].Value = "No data found for the selected criteria";
						worksheet.Cells[currentRow, 1, currentRow, columnCount].Merge = true;
						worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
						worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
					}

					// Add generated timestamp
					currentRow += 2;
					worksheet.Cells[currentRow, 1].Value = $"Generated on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss}";
					worksheet.Cells[currentRow, 1].Style.Font.Italic = true;
					worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;

					// Generate file name with timestamp
					var fileName = $"ProductionTransferReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
					var fileBytes = package.GetAsByteArray();

					return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
				}
			}
			catch (Exception ex)
			{
				// Handle errors and return internal server error
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Errors = new List<string> { ex.Message },
					StatusCode = HttpStatusCode.InternalServerError,
					Status = false
				});
			}
		}
	}
}

