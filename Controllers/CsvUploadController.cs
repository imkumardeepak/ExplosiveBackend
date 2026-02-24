using Peso_Baseed_Barcode_Printing_System_API.Interface;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing.BarCodes;

using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using System.Globalization;
using System.Net;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class CsvUploadController : ControllerBase
	{
		private readonly ILogger<CsvUploadController> _logger;
		private APIResponse _APIResponse;
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IProductMasterRepository _productMasterRepository;
		private readonly IL2barcodegenerationRepository _l2BarcodegenerationRepository;
		private readonly IL3barcodegenerationRepository _l3BarcodegenerationRepository;
		private readonly IBarcodeDataRepository _barcodeDataRepository;
		private readonly L1DetailsService _l1DetailsService;
		private readonly ICountryMasterRepository _countryMasterRepository;
		private readonly IMfgLocationMasterRepository _mfgLocationMasterRepository;
		private readonly ApplicationDbContext _context;

		public CsvUploadController(ILogger<CsvUploadController> logger,
			IL1barcodegenerationRepository l1BarcodegenerationRepository,
			IProductMasterRepository productMasterRepository,
			IL2barcodegenerationRepository l2BarcodegenerationRepository,
			IL3barcodegenerationRepository l3BarcodegenerationRepository,
			IBarcodeDataRepository barcodeDataRepository,
			L1DetailsService l1DetailsService,
			ICountryMasterRepository countryMasterRepository,
			IMfgLocationMasterRepository mfgLocationMasterRepository,
			ApplicationDbContext context)
		{
			_logger = logger;
			_APIResponse = new APIResponse();
			_l1BarcodegenerationRepository = l1BarcodegenerationRepository;
			_productMasterRepository = productMasterRepository;
			_l2BarcodegenerationRepository = l2BarcodegenerationRepository;
			_l3BarcodegenerationRepository = l3BarcodegenerationRepository;
			_barcodeDataRepository = barcodeDataRepository;
			_l1DetailsService = l1DetailsService;
			_countryMasterRepository = countryMasterRepository;
			_mfgLocationMasterRepository = mfgLocationMasterRepository;
			_context = context;
		}

		/// <summary>
		/// Upload and parse CSV file containing L1, L2, L3 barcode data
		/// </summary>
		/// <param name="file">CSV file to upload</param>
		/// <returns>Parsed CSV data</returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> UploadCsvFile(IFormFile file)
		{
			try
			{
				// Validate file
				if (file == null || file.Length == 0)
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "No file uploaded or file is empty.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Errors.Add("File is required");
					return BadRequest(_APIResponse);
				}

				// Validate file extension
				var fileExtension = Path.GetExtension(file.FileName).ToLower();
				if (fileExtension != ".csv")
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "Invalid file format. Only CSV files are allowed.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Errors.Add("File must be a CSV file");
					return BadRequest(_APIResponse);
				}

				var csvRecords = new List<CsvUploadDto>();
				var errors = new List<string>();
				int lineNumber = 0;

				using (var stream = new StreamReader(file.OpenReadStream()))
				{
					while (!stream.EndOfStream)
					{
						lineNumber++;
						var line = await stream.ReadLineAsync();

						// Skip empty lines
						if (string.IsNullOrWhiteSpace(line))
						{
							continue;
						}

						// Parse CSV line
						var values = line.Split(',');

						// Validate that we have exactly 3 columns
						if (values.Length != 3)
						{
							errors.Add($"Line {lineNumber}: Invalid format. Expected 3 columns (L1, L2, L3), found {values.Length}");
							continue;
						}

						// Trim whitespace from values
						var l1 = values[0].Trim();
						var l2 = values[1].Trim();
						var l3 = values[2].Trim();

						// Validate that all columns have values and l1 must 27 characters
						if (string.IsNullOrWhiteSpace(l1) || string.IsNullOrWhiteSpace(l2) || string.IsNullOrWhiteSpace(l3) || l1.Length != 27)
						{
							errors.Add($"Line {lineNumber}: One or more columns are empty");
							continue;
						}

						csvRecords.Add(new CsvUploadDto
						{
							L1 = l1,
							L2 = l2,
							L3 = l3
						});
					}
				}

				List<string> l1List = csvRecords.Select(x => x.L1).Distinct().ToList();
				var existingL1s = await _l1BarcodegenerationRepository.GetBulkExistingL1sAsync(l1List);
				if (existingL1s.Any())
				{
					errors.AddRange(existingL1s.Select(l1 => $"L1 already exists in database: {l1}"));
					csvRecords.RemoveAll(x => existingL1s.Contains(x.L1));
				}

				if (csvRecords.Count == 0)
				{
					_APIResponse.Status = false;
					_APIResponse.Message = "No valid records found in the file.";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Errors.Add("No valid records found in the file.");
					return BadRequest(_APIResponse);
				}
				if (csvRecords.Count > 0)
				{
					// Get common info
					var countries = await _countryMasterRepository.GetAllAsync();
					var countryInfo = countries.FirstOrDefault();
					var mfgLocations = await _mfgLocationMasterRepository.GetAllAsync();
					var mfgInfo = mfgLocations.FirstOrDefault();

					var allL1Data = new List<L1barcodegeneration>();
					var allL2Data = new List<L2barcodegeneration>();
					var allL3Data = new List<L3barcodegeneration>();
					var allBarcodeData = new List<BarcodeData>();

					// Group by L1 to process each box
					var l1Groups = csvRecords.GroupBy(x => x.L1).ToList();
					var processedCsvRecords = new List<CsvUploadDto>();

					foreach (var l1Group in l1Groups)
					{
						string l1Barcode = l1Group.Key;
						if (!TryParseL1Barcode(l1Barcode, out var mfgDt, out var shift, out var brandId, out var sizeCode, out var plantCode, out var serialNoStr))
						{
							errors.Add($"Invalid L1 format: {l1Barcode}");
							continue;
						}

						var product = await _productMasterRepository.GetProductDetailsAsync(plantCode, brandId, sizeCode);
						if (product == null)
						{
							errors.Add($"Product not found for L1: {l1Barcode}");
							continue;
						}

						var mfgDateStr = mfgDt.ToString("dd/MM/yyyy");
						string quarter = _l1DetailsService.GetQuarter(mfgDateStr);
						string month = mfgDateStr.Substring(3, 2);
						string year = mfgDateStr.Substring(6, 4);

						var currentBatch = await _l1DetailsService.GetOrCreateBatchAsync(plantCode, mfgDt, product.l1netwt);

						int l1SrNo = int.Parse(serialNoStr);

						// Group group records by L2
						var l2Groups = l1Group.GroupBy(x => x.L2).ToList();

						if (string.IsNullOrEmpty(l2Groups.First().Key) || l2Groups.First().Key.Length < 8)
						{
							errors.Add($"Invalid L2 format for box {l1Barcode}");
							continue;
						}

						// Create L1 data
						var l1data = new L1barcodegeneration
						{
							L1Barcode = l1Barcode,
							SrNo = l1SrNo,
							Country = countryInfo?.cname ?? "INDIA",
							CountryCode = countryInfo?.code ?? "IN",
							MfgName = mfgInfo?.mfgname ?? "",
							MfgLoc = mfgInfo?.mfgloc ?? "",
							MfgCode = mfgInfo?.maincode ?? "",
							PlantName = product.ptype,
							PCode = plantCode,
							MCode = l2Groups.First().Key.Substring(7, 1), // Extract MCode from first L2
							Shift = shift,
							BrandName = product.bname,
							BrandId = brandId,
							ProductSize = product.psize,
							PSizeCode = sizeCode,
							SdCat = product.sdcat,
							UnNoClass = product.unnoclass,
							MfgDt = mfgDt,
							MfgTime = DateTime.Now,
							L1NetWt = product.l1netwt,
							L1NetUnit = product.unit,
							NoOfL2 = l2Groups.Count,
							NoOfL3 = l1Group.Count(),
							MFlag = false,
							CheckFlag = true
						};
						allL1Data.Add(l1data);

						foreach (var l2Group in l2Groups)
						{
							string l2Barcode = l2Group.Key;
							if (string.IsNullOrEmpty(l2Barcode) || l2Barcode.Length < 9)
							{
								errors.Add($"Invalid L2 format: {l2Barcode}");
								continue;
							}
							long l2SrNo = long.Parse(l2Barcode.Substring(8));
							string mCode = l2Barcode.Substring(7, 1);

							var l2data = new L2barcodegeneration
							{
								L2Barcode = l2Barcode,
								SrNo = l2SrNo,
								L1Barcode = l1Barcode
							};
							allL2Data.Add(l2data);

							foreach (var record in l2Group)
							{
								string l3Barcode = record.L3;
								if (string.IsNullOrEmpty(l3Barcode) || l3Barcode.Length < 9)
								{
									errors.Add($"Invalid L3 format: {l3Barcode}");
									continue;
								}
								long l3SrNo = long.Parse(l3Barcode.Substring(8));

								var l3data = new L3barcodegeneration
								{
									L3Barcode = l3Barcode,
									SrNo = l3SrNo,
									L1Barcode = l1Barcode,
									L2Barcode = l2Barcode
								};
								allL3Data.Add(l3data);

								allBarcodeData.Add(new BarcodeData
								{
									L1 = l1Barcode,
									L2 = l2Barcode,
									L3 = l3Barcode,
									Batch = currentBatch?.BatchCode ?? "",
									Re2 = false,
									Re12 = false,
									IsFinal = false
								});
								processedCsvRecords.Add(record);
							}
						}

						// Deduct batch capacity
						if (currentBatch != null)
						{
							currentBatch.RemainingCapacity -= product.l1netwt;
							_context.BatchDetails.Update(currentBatch);
						}
					}

					if (allL1Data.Count > 0)
					{
						await _l1BarcodegenerationRepository.BulkInsertAsync(allL1Data);
						await _l2BarcodegenerationRepository.BulkInsertAsync(allL2Data);
						await _l3BarcodegenerationRepository.BulkInsertAsync(allL3Data);
						await _barcodeDataRepository.BulkInsertAsync(allBarcodeData);
						await _context.SaveChangesAsync();
					}

					csvRecords = processedCsvRecords;
				}


				// Create response
				var response = new CsvUploadResponseDto
				{
					Success = csvRecords.Count > 0,
					Message = csvRecords.Count > 0
						? $"Successfully Uploaded {csvRecords.Count} records"
						: "No valid records found in the file",
					TotalRecords = lineNumber,
					TotalCases = csvRecords.Select(c => c.L1).Distinct().Count(),
					SuccessfulRecords = csvRecords.Count,
					FailedRecords = errors.Count,
					Data = csvRecords,
					Errors = errors
				};

				_APIResponse.Data = response;
				_APIResponse.Status = true;
				_APIResponse.Message = response.Message;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				_logger.LogInformation($"CSV file '{file.FileName}' processed successfully. " + $"Total: {lineNumber}, Success: {csvRecords.Count}, Failed: {errors.Count}");

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing CSV file");
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.Message = "An error occurred while processing the CSV file.";
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
			}
		}

		private static bool TryParseL1Barcode(string l1, out DateTime mfgDate, out string shift, out string brandId, out string sizeCode, out string plantCode, out string serialNo)
		{
			mfgDate = default;
			shift = brandId = sizeCode = plantCode = serialNo = string.Empty;

			if (string.IsNullOrWhiteSpace(l1) || l1.Length != 27)
				return false;

			var datePart = l1.Substring(5, 6);
			shift = l1.Substring(11, 1);
			brandId = l1.Substring(12, 4);
			sizeCode = l1.Substring(16, 3);
			plantCode = l1.Substring(19, 2);
			serialNo = l1.Substring(21, 6);

			// Safe date parsing
			if (!DateTime.TryParseExact(
					datePart,
					"ddMMyy",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out mfgDate))
				return false;

			// Normalize year (optional but recommended)
			if (mfgDate.Year < 2000)
				mfgDate = mfgDate.AddYears(100);

			return true;
		}
	}
}


