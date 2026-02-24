using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Data;
using System.Net;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class L2ReprintReportController : ControllerBase
	{

		private readonly IConfiguration _configuration;
		private readonly Ireprint_l2barcodeRepository _reprint_l2barcodeRepository;
		private readonly Iformre2magallotRepository _formre2magallotRepository;
		private readonly IProductMasterRepository _ProductMasterRepository;
		private readonly IL3barcodegenerationRepository _L3barcodegenerationRepository;
		private readonly IL2barcodegenerationRepository _L2barcodegenerationRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IDistributedCache _distributedCache;

		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "L2ReprintReport";
		private const string CacheKey = "L2ReprintReport";


		public L2ReprintReportController(IDistributedCache distributedCache, IMapper mapper, Ireprint_l2barcodeRepository reprint_l2barcodeRepository, Iformre2magallotRepository formre2magallotRepository, IProductMasterRepository ProductMasterRepository, IL3barcodegenerationRepository L3barcodegenerationRepository, IL2barcodegenerationRepository L2barcodegenerationRepository, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
		{
			_mapper = mapper;

			_reprint_l2barcodeRepository = reprint_l2barcodeRepository;
			_L3barcodegenerationRepository = L3barcodegenerationRepository;
			_L2barcodegenerationRepository = L2barcodegenerationRepository;
			_formre2magallotRepository = formre2magallotRepository;
			_ProductMasterRepository = ProductMasterRepository;
			_distributedCache = distributedCache;
			_webHostEnvironment = webHostEnvironment;
			_APIResponse = new APIResponse();
            _configuration = configuration;


		}

		//[HttpGet]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		//[ProducesResponseType(StatusCodes.Status403Forbidden)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> GetAllProductionreport()
		//{
		//	try
		//	{
		//		string serializedList = string.Empty;
		//		var encodedList = await _distributedCache.GetAsync(KeyName);

		//		if (encodedList != null)
		//		{
		//			_APIResponse.Data = JsonConvert.DeserializeObject<List<ProductionReport>>(Encoding.UTF8.GetString(encodedList));
		//			_APIResponse.Status = true;
		//			_APIResponse.StatusCode = HttpStatusCode.OK;
		//		}
		//		else
		//		{
		//			var countries = await _StorageMagazineReportRepository.GetAllAsync();
		//			if (countries != null)
		//			{
		//				serializedList = JsonConvert.SerializeObject(countries);
		//				encodedList = Encoding.UTF8.GetBytes(serializedList);

		//				var options = new DistributedCacheEntryOptions()
		//					.SetSlidingExpiration(TimeSpan.FromDays(30))
		//					.SetAbsoluteExpiration(TimeSpan.FromDays(30));
		//				await _distributedCache.SetAsync(KeyName, encodedList, options);

		//				_APIResponse.Data = countries;
		//				_APIResponse.Status = true;
		//				_APIResponse.StatusCode = HttpStatusCode.OK;
		//			}
		//		}

		//		return Ok(_APIResponse);
		//	}
		//	catch (Exception ex)
		//	{
		//		_APIResponse.Errors.Add(ex.Message);
		//		_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
		//		_APIResponse.Status = false;
		//		return _APIResponse;
		//	}
		//}


		//[HttpGet("{magname}")]
		//public async Task<ActionResult<APIResponse>> Getmagcode(string magname)
		//{
		//    var response = new APIResponse();
		//    try
		//    {
		//        var psizenames = await _StorageMagazineReportRepository.GetAllmagzine(magname);

		//        if (psizenames.Any())
		//        {
		//            response.Data = psizenames;
		//            response.Status = true;
		//            response.StatusCode = HttpStatusCode.OK;
		//            return Ok(response);
		//        }
		//        else
		//        {
		//            response.Status = false;
		//            response.StatusCode = HttpStatusCode.NotFound;
		//            return NotFound(response);
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        response.Errors.Add(ex.Message);
		//        response.Status = false;
		//        response.StatusCode = HttpStatusCode.InternalServerError;
		//        return StatusCode((int)HttpStatusCode.InternalServerError, response);
		//    }
		//}

		//[HttpGet]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> Getl2reprintdata(string fromDate, string toDate, string reportType, string? plant)
		//{
		//    try
		//    {
		//        // Convert string dates to DateTime
		//        //DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		//        //DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		//        DateTime fromDateTime = DateTime.Parse(fromDate);
		//        DateTime toDateTime = DateTime.Parse(toDate);
		//        // Fetch all records from repository
		//        var barcodeList = await _reprint_l2barcodeRepository.GetAllAsync();
		//        List<L2ReprintReport> plantData = new List<L2ReprintReport>();

		//        if (reportType == "Detailed")
		//        {
		//            plantData = barcodeList
		//                 .Where(x => DateTime.Parse(x.rptdt) >= fromDateTime && DateTime.Parse(x.rptdt) <= toDateTime &&
		//                             (string.IsNullOrEmpty(plant) || x.plantcode == plant))
		//                 .Select(x => new L2ReprintReport
		//                 {
		//                     plantcode = x.plantcode,
		//                     L2Barcode = x.l2barcode,
		//                     reprintDt = DateTime.Parse(x.rptdt),
		//                     reason = x.reason,

		//                 }).OrderBy(x => x.plantcode)
		//                  .ThenBy(x => x.L2Barcode)   // Order by barcode count

		//                 .ToList();
		//        }
		//        else
		//        {
		//            plantData = barcodeList
		//                .Where(x => DateTime.Parse(x.rptdt) >= fromDateTime && DateTime.Parse(x.rptdt) <= toDateTime &&
		//                            (string.IsNullOrEmpty(plant) || x.plantcode == plant))
		//                .GroupBy(x => x.plantcode)  // Group only by plantcode
		//                .Select(g => new L2ReprintReport
		//                {
		//                    plantcode = g.Key,
		//                    L2Barcode = g.Select(x => x.l2barcode).Distinct().Count().ToString(), // Count distinct L2Barcode per plantcode
		//                })
		//                .OrderBy(x => x.plantcode) // Order by plantcode
		//                .ToList();

		//        }


		//        // Prepare API response
		//        _APIResponse.Data = plantData;
		//        _APIResponse.Status = true;
		//        _APIResponse.StatusCode = HttpStatusCode.OK;

		//        return Ok(_APIResponse);
		//    }
		//    catch (Exception ex)
		//    {
		//        _APIResponse.Errors.Add(ex.Message);
		//        _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
		//        _APIResponse.Status = false;

		//        return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
		//    }
		//}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRe2formdata(string fromDate, string toDate, string? plant)
		{
			var response = new APIResponse();

			try
			{

				string strcon = _configuration["ConnectionStrings:DefaultConnection"];
                NpgsqlConnection con = new NpgsqlConnection(strcon);

				DateTime fromDateTime = DateTime.Parse(fromDate);
				DateTime toDateTime = DateTime.Parse(toDate);


				string query = $@"
                                SELECT gendt,shift,""class"",div,bname,productsize,maglic,sum(l1netwt) AS netwt,dateoftest,COUNT(l1barcode) AS srno_count,
                                    COALESCE(
                                        LEFT(
                                            STRING_AGG(
                                                CASE
                                                    WHEN next_srno IS NULL OR next_srno - srno > 1 THEN srno::TEXT || ' , '
                                                    WHEN prev_srno IS NULL AND next_srno IS NULL THEN srno::TEXT
                                                    WHEN prev_srno IS NULL OR srno - prev_srno > 1 THEN  srno::TEXT || ' to '
                                                    WHEN (prev_srno IS NULL OR srno - prev_srno > 1) AND (next_srno IS NULL OR next_srno - srno > 1) THEN  srno::TEXT || ' , ' || COALESCE(next_srno::TEXT, '')
                                                END, ''
                                            ), LENGTH(STRING_AGG(srno::TEXT, ''))
                                        ), ''
                                    ) AS srno_ranges
                                FROM (
                                    SELECT gendt,shift,""class"",div,bname,productsize,maglic,l1netwt,dateoftest,l1barcode,srno,
                                        LEAD(srno) OVER (PARTITION BY gendt, shift, ""class"", div, bname, productsize, maglic ORDER BY srno) AS next_srno,
                                        LAG(srno) OVER (PARTITION BY gendt, shift, ""class"", div, bname, productsize, maglic ORDER BY srno) AS prev_srno
                                    FROM ""FormRe2MagAllot""
                                    WHERE pcode = '{plant}' 
                                    AND gendt BETWEEN '{fromDateTime:yyyy-MM-dd}' AND '{toDateTime:yyyy-MM-dd}'
                                ) AS subquery
                                GROUP BY gendt, shift, ""class"", div, bname, productsize, maglic, dateoftest
                                ORDER BY gendt, shift, ""class"", div, bname, productsize, MIN(l1barcode), MIN(gendt), maglic";



				DataTable dt = new DataTable();
				NpgsqlDataAdapter sda = new NpgsqlDataAdapter(query, con);
				sda.Fill(dt);

				List<Re2FormDataResult> re2FormData = dt.AsEnumerable()
						  .Select(row => new Re2FormDataResult
						  {
							  gendt = (DateTime)row["gendt"],
							  shift = row["shift"]?.ToString(),
							  bname = row["bname"]?.ToString(),
							  Class = row["class"]?.ToString(),
							  div = row["div"]?.ToString(),
							  productsize = row["productsize"]?.ToString(),
							  srno_count = Convert.ToInt32(row["srno_count"]),
							  netwt = Convert.ToDouble(row["netwt"]),
							  dateoftest = row["dateoftest"] == DBNull.Value ? null : (DateTime?)row["dateoftest"],
							  srno_ranges = row["srno_ranges"]?.ToString(),
							  maglic = row["maglic"]?.ToString(),
						  })
						  .ToList();


                // Get license using ExecuteScalar - Much more efficient for single values
                string license = null;
                string licenseQuery = @"select ""License"" from ""PlantMasters"" where ""PCode"" = @plant";
				con.Open();
                using (var cmd = new NpgsqlCommand(licenseQuery, con))
                {
                    cmd.Parameters.AddWithValue("@plant", plant ?? (object)DBNull.Value);
                    var result = await cmd.ExecuteScalarAsync();
                    license = result?.ToString();
                }

                var responseData = new
                {
                    re2FormData = re2FormData,
                    license = license
                };

                response.Data = responseData;
                response.Status = true;
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


		//[HttpGet]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> GetReprintbar(int? fromDate, int? toDate, string? mode, string? plant, string? plantcode, string? mcode, string? l2barcode)
		//{
		//    try
		//    {
		//        // Convert string dates to DateTime
		//        //DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		//        //DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

		//        // Fetch all records from repository
		//        var barcodeList = await _L2barcodegenerationRepository.GetAllAsync();
		//        List<L2barcodegeneration> plantData = new List<L2barcodegeneration>();

		//        if (mode == "barcode")
		//        {
		//            plantData = barcodeList
		//                 .Where(x => x.L2Barcode.ToString().Trim() == l2barcode.Trim())

		//                 .Select(x => new L2barcodegeneration
		//                 {
		//                     L2Barcode = x.L2Barcode,
		//                     SrNo = x.SrNo,
		//                     PlantName = x.PlantName,
		//                     PCode = x.PCode,
		//                     MfgDt = x.MfgDt,
		//                     L1Barcode = x.L1Barcode,
		//                     MfgName = x.MfgName,
		//                     MfgCode = x.MfgCode,
		//                     MCode = x.MCode,
		//                     BrandName = x.BrandName,
		//                     BrandId = x.BrandId,
		//                     ProductSize = x.ProductSize,
		//                     PSizeCode = x.PSizeCode,
		//                     SdCat = x.SdCat,
		//                     UnNoClass = x.UnNoClass,

		//                 }).OrderBy(x => x.L2Barcode)
		//                  .ThenBy(x => x.SrNo)   // Order by barcode count

		//                 .ToList();
		//        }
		//        else
		//        {
		//            plantData = barcodeList
		//                 .Where(x => x.PlantName.Trim() == plant.Trim() && x.PCode.Trim() == plantcode.Trim() && x.MCode.Trim() == mcode.Trim() && x.SrNo >= fromDate && x.SrNo <= toDate)
		//                 .Select(x => new L2barcodegeneration
		//                 {
		//                     L2Barcode = x.L2Barcode,
		//                     SrNo = x.SrNo,
		//                     PlantName = x.PlantName,
		//                     PCode = x.PCode,
		//                     MfgDt = x.MfgDt,
		//                     L1Barcode = x.L1Barcode,
		//                     MfgName = x.MfgName,
		//                     MfgCode = x.MfgCode,
		//                     MCode = x.MCode,
		//                     BrandName = x.BrandName,
		//                     BrandId = x.BrandId,
		//                     ProductSize = x.ProductSize,
		//                     PSizeCode = x.PSizeCode,
		//                     SdCat = x.SdCat,
		//                     UnNoClass = x.UnNoClass,

		//                 }).OrderBy(x => x.L2Barcode)
		//                  .ThenBy(x => x.SrNo)   // Order by barcode count

		//                 .ToList();

		//        }


		//        // Prepare API response
		//        _APIResponse.Data = plantData;
		//        _APIResponse.Status = true;
		//        _APIResponse.StatusCode = HttpStatusCode.OK;

		//        return Ok(_APIResponse);
		//    }
		//    catch (Exception ex)
		//    {
		//        _APIResponse.Errors.Add(ex.Message);
		//        _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
		//        _APIResponse.Status = false;

		//        return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
		//    }
		//}
		//[HttpGet]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> GetReprintL3barcode(int? from, int? to, string? plant, string? plantcode, string? mcode)
		//{
		//    try
		//    {
		//        // Convert string dates to DateTime
		//        //DateTime fromDateTime = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		//        //DateTime toDateTime = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

		//        // Fetch all records from repository
		//        var barcodeList = await _L3barcodegenerationRepository.GetAllAsync();
		//        List<L3barcodegeneration> plantData = new List<L3barcodegeneration>();


		//        plantData = barcodeList
		//               .Where(x => x.SrNo >= from && x.SrNo <= to &&
		//                         (string.IsNullOrEmpty(plant) || x.PlantName == plant) &&
		//                         (string.IsNullOrEmpty(plantcode) || x.PCode == plantcode) &&
		//                         (string.IsNullOrEmpty(mcode) || x.MCode == mcode))

		//             .Select(x => new L3barcodegeneration
		//             {
		//                 L1Barcode = x.L1Barcode,
		//                 L2Barcode = x.L2Barcode,
		//                 L3Barcode = x.L3Barcode,


		//             }).OrderBy(x => x.L1Barcode)
		//              .ThenBy(x => x.L2Barcode)   // Order by barcode count
		//              .ThenBy(x => x.L3Barcode)   // Order by barcode count

		//             .ToList();


		//        // Prepare API response
		//        _APIResponse.Data = plantData;
		//        _APIResponse.Status = true;
		//        _APIResponse.StatusCode = HttpStatusCode.OK;

		//        return Ok(_APIResponse);
		//    }
		//    catch (Exception ex)
		//    {
		//        _APIResponse.Errors.Add(ex.Message);
		//        _APIResponse.StatusCode = HttpStatusCode.InternalServerError;
		//        _APIResponse.Status = false;

		//        return StatusCode(StatusCodes.Status500InternalServerError, _APIResponse);
		//    }
		//}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetReprintbar(
		  DateTime mfgdt, string? plant, string? plantcode, string? mcode, int fromsrno, int tosrno, string? l2barcode)
		{
			try
			{
				var plantData = await _L2barcodegenerationRepository.GetReprintBarcodesAsync(
					mfgdt, plant, plantcode, mcode, fromsrno, tosrno, l2barcode);

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
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> REPrintdetailsLTwo(L2ReprintRequestDto request)
		{
			if (string.IsNullOrWhiteSpace(request.Reason))
			{
				return BadRequest(new { message = "Please provide a reason for reprint." });
			}

			var datalist = await _ProductMasterRepository.GetAllAsync();
			StringBuilder printBuilder = new StringBuilder();

			foreach (var item in request.ReprintData)
			{
				try
				{
					// Fetch class and division from ProductMaster
					var product = datalist
						.Where(p => p.bid == item.ParentL1?.BrandId && p.psizecode == item.ParentL1?.PSizeCode)
						.Select(p => new { p.Class, p.Division })
						.FirstOrDefault();

					string clsDiv = product != null ? $"{product.Class.ToString().Trim()}/{product.Division.ToString().Trim()}/{item.ParentL1?.SdCat}" : $"N/A/N/A/{item.ParentL1?.SdCat}";
					string plantMac = $"{item.ParentL1?.MfgCode}{item.ParentL1?.PCode}{item.ParentL1?.MCode}";

					string basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "PRNFile");
					string printFile = Path.Combine(basePath, "KEG_L2_100x75_Var_Script.prn");
					string fileToBeRead = Path.Combine(basePath, "KEG_L2_100x75_Value_Script.prn");

					if (System.IO.File.Exists(printFile))
					{
						System.IO.File.Copy(printFile, fileToBeRead, true);
					}

					if (!System.IO.File.Exists(fileToBeRead))
						continue; // Skip if copy failed or file not found

					var l21stPart = item.L2Barcode.Substring(0, 8);
					var l22ndPart = item.L2Barcode.Substring(8, 12);

					string content = await System.IO.File.ReadAllTextAsync(fileToBeRead);
					content = content.Replace("<D001>", item.L2Barcode?.Trim())
									 .Replace("<D002>", l21stPart)
									 .Replace("<D003>", l22ndPart)
									 .Replace("<D004>", item.ParentL1?.BrandName?.Trim())
									 .Replace("<D005>", item.ParentL1?.BrandId?.Trim())
									 .Replace("<D006>", item.ParentL1?.ProductSize?.Trim())
									 .Replace("<D007>", item.ParentL1?.PSizeCode?.Trim())
									 .Replace("<D008>", clsDiv)
									 .Replace("<D009>", plantMac?.Trim())
									 .Replace("<D010>", item.ParentL1?.UnNoClass);

					await System.IO.File.WriteAllTextAsync(fileToBeRead, content);

					printBuilder.AppendLine(content);

					// System.IO.File.Delete(fileToBeRead);
				}
				catch (Exception ex)
				{
					// Log error with context (e.g., item.L2Barcode)
					//  _logger.LogError(ex, $"Error processing L2Barcode: {item.L2Barcode}");
					continue;
				}
			}

			if (printBuilder.Length > 0)
			{
				try
				{
					//string printerIp = _configuration["AppSettings:loc1_printer"];
					//int printerPort = 9100;

					//using TcpClient client = new TcpClient();
					//await client.ConnectAsync(IPAddress.Parse(printerIp), printerPort);

					//byte[] bytes = Encoding.ASCII.GetBytes(printBuilder.ToString());
					//using NetworkStream stream = client.GetStream();
					//await stream.WriteAsync(bytes, 0, bytes.Length);
					//await stream.FlushAsync();

					return Ok(new APIResponse
					{
						Status = true,
						StatusCode = HttpStatusCode.OK,
						Message = "Reprint done successfully",
						Data = null
					});
				}
				catch (Exception ex)
				{
					//_logger.LogError(ex, "Failed to send data to printer.");
					return StatusCode(500, new APIResponse
					{
						Status = false,
						Message = "Error while sending data to printer.",
						Data = null
					});
				}
			}

			return BadRequest(new APIResponse
			{
				Status = false,
				Message = "No data to print or an error occurred.",
				Data = null
			});
		}

		//private async Task PrintL3BarcodeAsync(string fromL3, string toL3, string totL3perL2, string bname)
		//{
		//    try
		//    {
		//        StringBuilder printBuilder = new StringBuilder();
		//        string basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "PRNFile");
		//        string printFile = Path.Combine(basePath, "DFL3_100x25_var_script.prn");
		//        string fileToBeRead = Path.Combine(basePath, "DFL3_100x25_value_script.prn");

		//        if (System.IO.File.Exists(printFile))
		//        {
		//            System.IO.File.Copy(printFile, fileToBeRead, true);
		//        }

		//        if (!System.IO.File.Exists(fileToBeRead))
		//            return;

		//        string content = await System.IO.File.ReadAllTextAsync(fileToBeRead);
		//        content = content.Replace("<D001>", fromL3.Trim())
		//                         .Replace("<D002>", toL3.Trim())
		//                         .Replace("<D003>", totL3perL2)
		//                         .Replace("<D004>", bname.Trim());

		//        await System.IO.File.WriteAllTextAsync(fileToBeRead, content);

		//        printBuilder.AppendLine(content);

		//        System.IO.File.Delete(fileToBeRead);

		//    }
		//    catch (Exception ex)
		//    {
		//        //_logger.LogError(ex, "Error while printing L3 barcode.");
		//    }

		//}
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> REPrintdetailsLThree([FromBody] L3ReprintRequestDto request)
		{
			StringBuilder printBuilder = new StringBuilder();
			if (request?.ReprintData == null || !request.ReprintData.Any())
				return BadRequest("No data provided for reprint.");

			if (!request.Plant.Trim().Contains("DETONATING FUSE", StringComparison.OrdinalIgnoreCase))
			{
				foreach (var item in request.ReprintData)
				{
					string l1 = item.L1Barcode;

					if (string.IsNullOrWhiteSpace(l1) || l1.Length < 24)
					{
						//_logger.LogWarning("Invalid L1Barcode format: {Barcode}", l1);
						continue;
					}

					string l1Box = l1.Substring(21).Trim();
					string dop = l1.Substring(5, 6).Trim();
					string brandId = l1.Substring(12, 4).Trim();
					string scode = l1.Substring(16, 3).Trim();

					string brandName = await _ProductMasterRepository.GetBrandnameAsync(brandId);
					string size = await _ProductMasterRepository.GetSizeAsync(scode);
					string l3 = item.L3Barcode;

					// Start of inlined L3BarcodePrint logic
					try
					{
						string basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "PRNFile");
						string printFile = Path.Combine(basePath, "L3_100x25_var_script.prn");
						string valueFile = Path.Combine(basePath, "L3_100x25_value_script.prn");

						if (!Directory.Exists(basePath))
							Directory.CreateDirectory(basePath);

						if (System.IO.File.Exists(valueFile))
						{
							System.IO.File.Copy(printFile, valueFile, true);
						}

						if (!System.IO.File.Exists(valueFile))
						{
							// _logger.LogError("Failed to find or create PRN file: {File}", valueFile);
							continue;
						}

						string content = await System.IO.File.ReadAllTextAsync(valueFile);
						content = content.Replace("<D001>", l3)
										 .Replace("<D002>", brandId)
										 .Replace("<D003>", scode)
										 .Replace("<D004>", brandName)
										 .Replace("<D005>", size)
										 .Replace("<D006>", dop)
										 .Replace("<D007>", l1Box);

						await System.IO.File.WriteAllTextAsync(valueFile, content);
						// await System.IO.File.WriteAllTextAsync(fileToBeRead, content);

						printBuilder.AppendLine(content);

						//System.IO.File.Delete(valueFile);
						// Optional: Send to printer
						// RawPrinterHelper.SendFileToPrinter("YourPrinterName", valueFile);

						// Optional: Delete after printing
						// System.IO.File.Delete(valueFile);
					}
					catch (Exception ex)
					{
						// _logger.LogError(ex, "Error occurred while printing L3 barcode.");
					}
					if (printBuilder.Length > 0)
					{
						try
						{
							//string printerIp = _configuration["AppSettings:loc1_printer"];
							//int printerPort = 9100;

							//using TcpClient client = new TcpClient();
							//await client.ConnectAsync(IPAddress.Parse(printerIp), printerPort);

							//byte[] bytes = Encoding.ASCII.GetBytes(printBuilder.ToString());
							//using NetworkStream stream = client.GetStream();
							//await stream.WriteAsync(bytes, 0, bytes.Length);
							//await stream.FlushAsync();

							return Ok(new APIResponse
							{
								Status = true,
								Message = "Reprint done successfully",
								Data = null
							});
						}
						catch (Exception ex)
						{
							//_logger.LogError(ex, "Failed to send data to printer.");
							return StatusCode(500, new APIResponse
							{
								Status = false,
								Message = "Error while sending data to printer.",
								Data = null
							});
						}
					}

					return BadRequest(new APIResponse
					{
						Status = false,
						Message = "No data to print or an error occurred.",
						Data = null
					});
				}
			}

			return Ok("Reprint completed.");
		}


	}
}

