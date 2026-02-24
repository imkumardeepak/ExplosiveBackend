using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using DocumentFormat.OpenXml.Office.MetaAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;
using System.Net.Sockets;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class L1ReprintReportController : ControllerBase
	{


		private readonly Il1barcodereprintRepository _l1barcodereprintRepository;
		private readonly IL1barcodegenerationRepository _L1barcodegenerationRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
		private readonly IProductMasterRepository _ProductMasterRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly IConfiguration _configuration;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "L1boxDeletionReport";
		private const string CacheKey = "L1boxDeletionReport";


		public L1ReprintReportController(IDistributedCache distributedCache, IMapper mapper, Il1barcodereprintRepository l1barcodereprintRepository, IL1barcodegenerationRepository l1BarcodegenerationRepository, IDispatchTransactionRepository dispatchTransactionRepository, IWebHostEnvironment webHostEnvironment, IProductMasterRepository productMasterRepository, IConfiguration configuration)
		{
			_mapper = mapper;

			_l1barcodereprintRepository = l1barcodereprintRepository;
			_L1barcodegenerationRepository = l1BarcodegenerationRepository;
			_distributedCache = distributedCache;
			_DispatchTransactionRepository = dispatchTransactionRepository;
			_webHostEnvironment = webHostEnvironment;
			_configuration = configuration;
			_ProductMasterRepository = productMasterRepository;
			_APIResponse = new APIResponse();


		}
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetL1Reprintbarcd(DateTime? MfgDt, string? plant, string? plantcode, string? mcode, string? shift, string? Brandname, string? BrandId, string? productsize, string? psizecode, string? l1barcode)
		{
			try
			{
				var plantData = await _L1barcodegenerationRepository.GetL1ReprintBarcodesAsync(
					MfgDt, plant, plantcode, mcode, Brandname, BrandId, productsize, psizecode, l1barcode, shift, mcode
				);

				if (plantData == null || plantData.Count == 0)
				{
					_APIResponse.Data = plantData;
					_APIResponse.Status = false;
					_APIResponse.Message = "No Data Found";
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					return Ok(_APIResponse);
				}

				_APIResponse.Data = plantData;
				_APIResponse.Status = true;
				_APIResponse.Message = "Data Found";
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

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> REPrintdetailslone(ReprintRequestDto request)
		{
			if (request.ReprintData.Any() && !string.IsNullOrWhiteSpace(request.Reason))
			{
				StringBuilder printBuilder = new StringBuilder();

				foreach (var row in request.ReprintData.Where(r => !string.IsNullOrEmpty(r.L1Barcode)))
				{
					string selBarcode = row.L1Barcode;

					bool isDispatched = await _DispatchTransactionRepository
						.AnyAsync(d => selBarcode.Contains(d.L1Barcode) && d.Re12 == true);

					if (isDispatched)
					{
						return BadRequest("One or more selected L1-Box barcodes were already dispatched! You can't take reprint of it!");
					}

					string boxno = row.SrNo.PadLeft(6, '0');
					DateTime reprintDate = DateTime.Now;

					// INSERT into L1BarcodeReprint
					var reprintEntry = new l1barcodereprint
					{
						l1barcode = selBarcode,
						srno = Convert.ToInt32(boxno),
						country = row.Country,
						countrycode = row.CountryCode,
						mfgname = row.MfgName,
						mfgloc = "-",
						mfgcode = row.MfgCode,
						mcode = row.MCode,
						plantname = row.PlantName,
						pcode = row.PCode,
						shift = row.Shift,
						brandname = row.Brand,
						brandid = row.BrandId,
						productsize = row.PSize,
						psizecode = row.PSizeCode,
						sdcat = row.SDCat,
						unnoclass = row.UNClass,
						mfgdt = row.MfgDt ?? DateTime.Now,
						rp_dt_time = reprintDate,
						month = reprintDate.Month.ToString("D2"),
						genyear = reprintDate.Year.ToString(),
						reason = request.Reason
					};
					await _l1barcodereprintRepository.AddAsync(reprintEntry);
					// List<ProductMaster> datalist = new List<ProductMaster>();
					var datalist = await _ProductMasterRepository.GetAllAsync();

					var product = datalist
						.Where(p => p.bid == row.BrandId && p.ptypecode == row.PCode && p.psizecode == row.PSizeCode)
						.Select(p => new
						{
							p.noofl3perl1,
							p.Class,
							p.Division
						})
						.FirstOrDefault();

					if (product == null)
					{
						continue; // skip if no matching product
					}


					string unitDisplay = row.L1NetUnit == "Mtrs" ? $"{product.noofl3perl1} {row.L1NetUnit}" : $"{product.noofl3perl1} Nos.";
					string ddmmyy = selBarcode.Substring(5, 6);
					// File paths
					string basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "PRNFile");
					string printFilePath = row.PCode == "G3" || row.PCode == "G6"
						? Path.Combine(basePath, "KEL_ARGOX_Var_Script.txt")
						: Path.Combine(basePath, "KEL_ARGOX_Var_Script_Old.txt");

					string fileToBeRead = row.PCode == "G3" || row.PCode == "G6"
						? Path.Combine(basePath, "KEL_ARGOX_Value_Script.txt")
						: Path.Combine(basePath, "KEL_ARGOX_Value_Script_Old.txt");

					if (!System.IO.File.Exists(printFilePath))
						return NotFound("Template script not found.");

					// Copy template to working file
					System.IO.File.Copy(printFilePath, fileToBeRead, true);

					string content = await System.IO.File.ReadAllTextAsync(fileToBeRead);

					var replacements = new Dictionary<string, string>
					{
						["<D001>"] = row.L1Barcode,
						["<D002>"] = row.MfgName,
						["<D003>"] = row.Brand,
						["<D004>"] = row.PSize,
						["<D005>"] = row.PSizeCode,
						["<D006>"] = Convert.ToString(row.MfgDt),
						["<D007>"] = boxno,
						["<D008>"] = unitDisplay,
						["<D009>"] = row.L1NetWt,
						["<D010>"] = row.L1NetUnit,
						["<D011>"] = row.MfgCode,
						["<D012>"] = row.PCode,
						["<D013>"] = product.Class.ToString(),
						["<D014>"] = product.Division.ToString(),
						["<D015>"] = row.SDCat,
						["<D016>"] = row.UNClass,
						["<D017>"] = row.CountryCode,
						["<D018>"] = ddmmyy,
						["<D019>"] = row.BrandId,
						["<D020>"] = row.Shift,
						["<D021>"] = "1"
					};

					if (!string.IsNullOrEmpty(row.PCode) && row.PCode.Length >= 2)
					{
						replacements["<D022>"] = row.PCode.Substring(0, 1);
						replacements["<D023>"] = row.PCode.Substring(1, 1);
					}

					foreach (var pair in replacements)
					{
						content = content.Replace(pair.Key, pair.Value);
					}

					await System.IO.File.WriteAllTextAsync(fileToBeRead, content);

					// Read final content
					string finalContent = await System.IO.File.ReadAllTextAsync(fileToBeRead);
					printBuilder.AppendLine(finalContent);

					// Clean up file
					if (System.IO.File.Exists(fileToBeRead)) ;
					//System.IO.File.Delete(fileToBeRead);
				}

				// Send to printer
				// Send to printer
				if (printBuilder.Length > 0)
				{
					try
					{
						// Get printer IP from config
						//string printerIp = _configuration["AppSettings:loc1_printer"];
						//int printerPort = 9100;

						//using TcpClient client = new TcpClient();
						//await client.ConnectAsync(IPAddress.Parse(printerIp), printerPort);

						//byte[] bytes = Encoding.ASCII.GetBytes(printBuilder.ToString());
						//using NetworkStream stream = client.GetStream();
						//await stream.WriteAsync(bytes, 0, bytes.Length);
						//await stream.FlushAsync();
					}
					catch (Exception ex)
					{
						return StatusCode(500, new APIResponse
						{
							Status = false,

							Data = null
						});
					}
				}

				return Ok(new APIResponse
				{
					Status = true,
					StatusCode = HttpStatusCode.OK,
					Message = "Reprint done successfully",
					Data = null
				});
			}

			return BadRequest(new APIResponse
			{
				Status = false,

				Data = null
			});

		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetMissingLone(string? MfgDt, string? plant, string? plantcode, string? mode, string? Brandname, string? BrandId, string? productsize, string? psizecode, int? fromsrno, int? tosrno)
		{
			try
			{
				// Convert string dates to DateTime
				//DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
				//DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
				DateTime MfgDts = DateTime.Parse(MfgDt);
				// Fetch all records from repository
				var barcodeList = await _L1barcodegenerationRepository.GetAllAsync();
				List<L1barcodegeneration> plantData = new List<L1barcodegeneration>();

				plantData = barcodeList
					.Where(x =>
						x.PlantName.Trim() == plant.Trim() &&
						x.PCode.Trim() == plantcode.Trim() &&
						x.MfgDt == MfgDts &&  // ✅ safer date comparison
						x.BrandName.Trim() == Brandname.Trim() &&
						x.BrandId.Trim() == BrandId.Trim() &&
						x.ProductSize == productsize &&
						x.PSizeCode.Trim() == psizecode.Trim() &&
						x.SrNo >= fromsrno &&
						x.SrNo <= tosrno
					)
					.Select(x => new L1barcodegeneration
					{
						L1Barcode = x.L1Barcode,
						SrNo = x.SrNo,

					})
					.OrderBy(x => x.L1Barcode)
					.ThenBy(x => x.SrNo)
					.ToList();

				// Prepare API response
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
	}
}


