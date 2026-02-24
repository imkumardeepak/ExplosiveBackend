using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PdfSharpCore.Drawing;
using Peso_Based_Barcode_Printing_System_APIBased.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using DocumentFormat.OpenXml.Bibliography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class RE6GenerationController : ControllerBase
	{


		private readonly IMagzineMasterRepository _MagzineMasterRepository;
		private readonly IRe11IndentInfoRepository _Re11IndentInfoRepository;
		private readonly ICustomerMastersRepository _CustomerMastersRepository;
		private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
		private readonly IProductMasterRepository _ProductMasterRepository;
		private readonly ICustMagazineDetailsRepository _CustMagazineDetailsRepository;
		private readonly ITransportMasterRepository _TransportMasterRepository;
		private readonly ITransVehicleDetailRepository _TransVehicleDetailRepository;
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _webHostEnvironment;


		private readonly IDistributedCache _distributedCache;

		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "L2ReprintReport";
		private const string CacheKey = "L2ReprintReport";


		public RE6GenerationController(IDistributedCache distributedCache, IMapper mapper, IMagzineMasterRepository MagzineMasterRepository, ICustMagazineDetailsRepository CustMagazineDetailsRepository, IRe11IndentInfoRepository Re11IndentInfoRepository, ICustomerMastersRepository CustomerMastersRepository, IProductMasterRepository productMasterRepository, IWebHostEnvironment WebHostEnvironment, IConfiguration configuration, IDispatchTransactionRepository dispatchTransactionRepository, ITransportMasterRepository transportMasterRepository, ITransVehicleDetailRepository transVehicleDetailRepository)
		{
			_mapper = mapper;
			_Re11IndentInfoRepository = Re11IndentInfoRepository;
			_MagzineMasterRepository = MagzineMasterRepository;
			_CustMagazineDetailsRepository = CustMagazineDetailsRepository;
			_CustomerMastersRepository = CustomerMastersRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_ProductMasterRepository = productMasterRepository;
			_webHostEnvironment = WebHostEnvironment;
			_configuration = configuration;
			_DispatchTransactionRepository = dispatchTransactionRepository;
			_TransportMasterRepository = transportMasterRepository;
			_TransVehicleDetailRepository = transVehicleDetailRepository;
		}


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


		//[HttpGet]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> GetRe2formdata(string fromDate, string toDate, string? plant)
		//{
		//    var response = new APIResponse();

		//    try
		//    {
		//        // Convert string dates to DateTime
		//        DateTime fromDateTime = DateTime.Parse(fromDate);
		//        DateTime toDateTime = DateTime.Parse(toDate);
		//        var barcodeList = await _formre2magallotRepository.GetAllAsync();
		//        // Fetch filtered and grouped data using LINQ
		//        var groupedData = barcodeList
		//            .Where(x => x.pcode == plant && x.gendt >= fromDateTime && x.gendt <= toDateTime)
		//            .GroupBy(x => new { x.gendt, x.shift, x.Class, x.div, x.bname, x.productsize, x.maglic, x.dateoftest })
		//            .Select(g => new
		//            {
		//                g.Key.gendt,
		//                g.Key.shift,
		//                g.Key.bname,
		//                g.Key.Class,
		//                g.Key.div,
		//                g.Key.productsize,
		//                SrNoList = g.OrderBy(x => x.srno).Select(x => x.srno).ToList(),
		//                NetWt = g.Sum(x => x.l1netwt),
		//                g.Key.dateoftest,
		//                SrNoCount = g.Count(),
		//                g.Key.maglic,




		//            }).ToList();


		//        //// Helper to generate serial number ranges
		//        //string GetSrNoRanges(List<int> srnos)
		//        //{
		//        //    if (srnos == null || srnos.Count == 0) return "";

		//        //    srnos.Sort();
		//        //    var ranges = new List<string>();
		//        //    int start = srnos[0], end = srnos[0];

		//        //    for (int i = 1; i < srnos.Count; i++)
		//        //    {
		//        //        if (srnos[i] == end + 1)
		//        //        {
		//        //            end = srnos[i];
		//        //        }
		//        //        else
		//        //        {
		//        //            ranges.Add(start == end ? start.ToString() : $"{start} to {end}");
		//        //            start = end = srnos[i];
		//        //        }
		//        //    }

		//        //    ranges.Add(start == end ? start.ToString() : $"{start} to {end}");
		//        //    return string.Join(", ", ranges);
		//        //}

		//        string GetSrNoRanges(List<int> srnos)
		//        {
		//            if (srnos == null || srnos.Count == 0) return "";

		//            int start = srnos.Min();
		//            int end = srnos.Max();

		//            return start == end ? start.ToString() : $"{start} to {end}";
		//        }


		//        // Add srno_ranges to each result
		//        var finalResult = groupedData.Select(x => new
		//        {
		//            x.gendt,
		//            x.shift,
		//            x.bname,
		//            x.Class,
		//            x.div,
		//            x.productsize,
		//            SrNoRanges = GetSrNoRanges(x.SrNoList),
		//            x.NetWt,
		//            x.dateoftest,
		//            x.SrNoCount,
		//            x.maglic,

		//        }).OrderBy(x => x.gendt)
		//                .ThenBy(x => x.shift)
		//                .ToList();

		//        // Prepare API response
		//        response.Data = finalResult;
		//        response.Status = true;
		//        response.StatusCode = HttpStatusCode.OK;

		//        return Ok(response);
		//    }
		//    catch (Exception ex)
		//    {
		//        response.Errors.Add(ex.Message);
		//        response.Status = false;
		//        response.StatusCode = HttpStatusCode.InternalServerError;

		//        return StatusCode(StatusCodes.Status500InternalServerError, response);
		//    }
		//}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetRE6Details(string dispDate, string indentNo)
		{
			try
			{
				var dispatchList = await _DispatchTransactionRepository.GetAllAsync();
				var productList = await _ProductMasterRepository.GetAllAsync();

				var indentPrefix = $"RE-11/{DateTime.Now.Year}/";
				if (!indentNo.StartsWith(indentPrefix))
				{
					//return BadRequest("");
					_APIResponse.Message = "Invalid Indent No Format!";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Status = false;
					return _APIResponse;
				}

				var dispatches = dispatchList
					.Where(dt => dt.IndentNo == indentNo && dt.DispDt.ToString("yyyy-MM-dd") == dispDate && dt.Re12 == true)
					.Select(dt => new { dt.Brand, dt.PSize, dt.L1NetUnit, dt.MagName })
					.Distinct()
					.ToList();

				if (!dispatches.Any())
				{
					_APIResponse.Message = "Please Check The Indent No!";
					_APIResponse.StatusCode = HttpStatusCode.BadRequest;
					_APIResponse.Status = false;
					return _APIResponse;
				}


				var resultRows = new List<DataViewModel>();

				foreach (var item in dispatches)
				{
					var count = dispatchList
						.Where(dt => dt.Brand == item.Brand && dt.PSize == item.PSize &&
									 dt.IndentNo == indentNo && dt.DispDt.ToString("yyyy-MM-dd") == dispDate && dt.Re12 == true)
						.Select(dt => dt.L1Barcode)
						.Distinct()
						.Count();

					if (count == 0)
						continue;

					var product = productList
						.Where(p => p.bname == item.Brand && p.psize == item.PSize)
						.Select(p => new
						{
							p.bname,
							p.psize,
							p.noofl3perl1,
							p.l1netwt,
							p.unit,
							p.Class,
							p.Division
						})
						.FirstOrDefault();

					if (product != null)
					{
						resultRows.Add(new DataViewModel
						{
							BrandName = $"{product.bname}({product.psize}X{product.noofl3perl1}Nos)",
							StrClass = $"{product.Class}-{product.Division}",
							Quantity = count * (double)product.l1netwt,
							Unit = product.unit,
							Count = count,
							IndentNo = indentNo
						});
					}
				}

				// -----------------------
				// Add RE-12 Indent No Build Logic
				// -----------------------

				//string re12Prefix = $"RE-12/{DateTime.Now.Year}/";
				//string re12IndentSuffix = indentNo.Length >= 11 ? indentNo.Substring(11) : indentNo;

				//string currentRe12 = re12Prefix;
				//if (currentRe12 == $"RE-12/{DateTime.Now.Year}/")
				//{
				//    currentRe12 += re12IndentSuffix;
				//}
				//else
				//{
				//    currentRe12 += "," + re12IndentSuffix;
				//}

				// Optionally include the generated RE-12 string in the API response
				_APIResponse.Data = resultRows;
				//{
				//    Details =
				//    // RE12String = currentRe12
				//};
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
		public async Task<ActionResult<APIResponse>> GetConsigneeData(string indentNo)
		{
			try
			{
				var dispatchList = await _DispatchTransactionRepository.GetAllAsync();
				var magData = await _MagzineMasterRepository.GetAllAsync();
				var indentList = await _Re11IndentInfoRepository.GetAllAsync();
				var custList = await _CustomerMastersRepository.GetAllAsync();
				var custMagList = await _CustMagazineDetailsRepository.GetAllAsync();
				var transList = await _TransportMasterRepository.GetAllAsync();

				// 1. Get Licenses from Dispatch & Magazine
				var consignorLicenses = (from dt in dispatchList
										 join mm in magData on dt.MagName.Trim() equals mm.mcode
										 where dt.IndentNo == indentNo
										 select mm.licno).Distinct().ToList();

				var licenseNumbers = consignorLicenses
					.Select(l => l?.Split('(').ElementAtOrDefault(1)?.Replace(")", ""))
					.Where(s => !string.IsNullOrEmpty(s))
					.Distinct()
					.ToList();

				// 2. Get Customer Name from RE11
				var customerInfo = indentList
							 .Where(i => i.IndentNo == indentNo)
							 .Select(i => new { i.CustName, i.ConName })
							 .FirstOrDefault();


				// 3. Get Customer Info
				var customer = custList
					.FirstOrDefault(c => c.CName == customerInfo.CustName);

				// 4. Get License from Customer Magazine Mapping
				var licenseFromCustomer = custMagList
					.Where(c => c.Cid == customer?.Id)
					.Select(c => c.License)
					.ToList();

				var licenseList = licenseFromCustomer
					.Select(l => l?.Split('(').ElementAtOrDefault(1)?.Replace(")", ""))
					.Where(s => !string.IsNullOrEmpty(s))
					.ToList();

				// 5. Get All Transport Names
				var transportNames = transList
					.Select(t => t.TName)
					.ToList();

				// 6. Prepare response
				var resultData = new
				{
					ConsignorLicenses = licenseNumbers,
					CustomerName = customerInfo.CustName,
					ConnName = customerInfo.ConName,
					Address = customer?.Addr,
					District = customer?.District,
					State = customer?.State,
					Tashil = customer?.Tahsil,
					City = customer?.City,
					CustomerId = customer?.Id,
					LicenseNumbers = licenseList,
					TransportNames = transportNames
				};

				_APIResponse.Data = resultData;
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

		public async Task<ActionResult<APIResponse>> GetTransData(string tname)
		{
			var transList = await _TransportMasterRepository.GetAllAsync();
			var transVehicleList = await _TransVehicleDetailRepository.GetAllAsync();
			var transport = transList
				.FirstOrDefault(t => t.TName == tname);

			if (transport == null)
				return NotFound();

			var vehicles = transVehicleList
				.Where(v => v.Cid == transport.Id)
				.Select(v => new
				{
					v.VehicleNo,
					v.License,
					v.Validity
				}).ToList();


			// 6. Prepare response
			var resultData = new
			{
				TransportId = transport.Id,
				Vehicles = vehicles

			};

			_APIResponse.Data = resultData;
			_APIResponse.Status = true;
			_APIResponse.StatusCode = HttpStatusCode.OK;

			return Ok(_APIResponse);
		}

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
		//[HttpPost]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<IActionResult> REPrintdetailsLTwo(L2ReprintRequestDto request)
		//{
		//    if (string.IsNullOrWhiteSpace(request.Reason))
		//    {
		//        return BadRequest(new { message = "Please provide a reason for reprint." });
		//    }

		//    var datalist = await _ProductMasterRepository.GetAllAsync();
		//    StringBuilder printBuilder = new StringBuilder();

		//    foreach (var item in request.ReprintData)
		//    {
		//        try
		//        {
		//            // Fetch class and division from ProductMaster
		//            var product = datalist
		//                .Where(p => p.bid == item.BrandId && p.psizecode == item.PSizeCode)
		//                .Select(p => new { p.Class, p.Division })
		//                .FirstOrDefault();

		//            string clsDiv = product != null ? $"{product.Class.ToString().Trim()}/{product.Division.ToString().Trim()}/{item.SDCat}" : $"N/A/N/A/{item.SDCat}";
		//            string plantMac = $"{item.MfgCode}{item.PCode}{item.MCode}";

		//            string basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "PRNFile");
		//            string printFile = Path.Combine(basePath, "KEG_L2_100x75_Var_Script.prn");
		//            string fileToBeRead = Path.Combine(basePath, "KEG_L2_100x75_Value_Script.prn");

		//            if (System.IO.File.Exists(printFile))
		//            {
		//                System.IO.File.Copy(printFile, fileToBeRead, true);
		//            }

		//            if (!System.IO.File.Exists(fileToBeRead))
		//                continue; // Skip if copy failed or file not found

		//            var l21stPart = item.L2Barcode.Substring(0, 8);
		//            var l22ndPart = item.L2Barcode.Substring(8, 12);

		//            string content = await System.IO.File.ReadAllTextAsync(fileToBeRead);
		//            content = content.Replace("<D001>", item.L2Barcode?.Trim())
		//                             .Replace("<D002>", l21stPart)
		//                             .Replace("<D003>", l22ndPart)
		//                             .Replace("<D004>", item.Brand?.Trim())
		//                             .Replace("<D005>", item.BrandId?.Trim())
		//                             .Replace("<D006>", item.PSize?.Trim())
		//                             .Replace("<D007>", item.PSizeCode?.Trim())
		//                             .Replace("<D008>", clsDiv)
		//                             .Replace("<D009>", plantMac?.Trim())
		//                             .Replace("<D010>", item.UNClass);

		//            await System.IO.File.WriteAllTextAsync(fileToBeRead, content);

		//            printBuilder.AppendLine(content);

		//            // System.IO.File.Delete(fileToBeRead);
		//        }
		//        catch (Exception ex)
		//        {
		//            // Log error with context (e.g., item.L2Barcode)
		//            //  _logger.LogError(ex, $"Error processing L2Barcode: {item.L2Barcode}");
		//            continue;
		//        }
		//    }

		//    if (printBuilder.Length > 0)
		//    {
		//        try
		//        {
		//            //string printerIp = _configuration["AppSettings:loc1_printer"];
		//            //int printerPort = 9100;

		//            //using TcpClient client = new TcpClient();
		//            //await client.ConnectAsync(IPAddress.Parse(printerIp), printerPort);

		//            //byte[] bytes = Encoding.ASCII.GetBytes(printBuilder.ToString());
		//            //using NetworkStream stream = client.GetStream();
		//            //await stream.WriteAsync(bytes, 0, bytes.Length);
		//            //await stream.FlushAsync();

		//            return Ok(new APIResponse
		//            {
		//                Status = true,
		//                Message = "Reprint done successfully",
		//                Data = null
		//            });
		//        }
		//        catch (Exception ex)
		//        {
		//            //_logger.LogError(ex, "Failed to send data to printer.");
		//            return StatusCode(500, new APIResponse
		//            {
		//                Status = false,
		//                Message = "Error while sending data to printer.",
		//                Data = null
		//            });
		//        }
		//    }

		//    return BadRequest(new APIResponse
		//    {
		//        Status = false,
		//        Message = "No data to print or an error occurred.",
		//        Data = null
		//    });
		//}

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
		public async Task<ActionResult> RE6barcodeprint(Re6BarcodeModel request)
		{
			try
			{
				var d = DateTime.Now.ToString("dd-MM-yyyy");
				string[] add = Regex.Split(request.Address ?? "", "District", RegexOptions.IgnoreCase);
				string addressPart = add[0];
				int l = addressPart.Length;
				int l2 = l % 2 == 0 ? l / 2 : (l / 2) + 1;

				string addLine1 = addressPart.Substring(0, l2);
				string addLine2 = addressPart.Substring(l2);
				StringBuilder printBuilder = new StringBuilder();
				string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "PRNFile");
				string printFile = Path.Combine(basePath, "RE-6_100x150_Var_script.prn");
				string fileToBeRead = Path.Combine(basePath, "RE-6_100x150_Value_script.prn");

				if (System.IO.File.Exists(printFile))
				{
					System.IO.File.Copy(printFile, fileToBeRead, true);
				}


				string content = await System.IO.File.ReadAllTextAsync(fileToBeRead);

				//string prnContent = await System.IO.File.ReadAllTextAsync(valTemplatePath);

				content = content.Replace("<D001>", d)
									   .Replace("<D002>", request.VehicleNumber)
									   .Replace("<D003>", request.VehicleLicense)
									   .Replace("<D004>", request.RE12)
									   .Replace("<D030>", request.VehicleValue)
									   .Replace("<D005>", request.ConsigneeName)
									   .Replace("<D006>", addLine1)
									   .Replace("<D019>", addLine2)
									   .Replace("<D007>", $"District -{request.District}, {request.State}")
									   .Replace("<D008>", request.LicenseNumber)
									   .Replace("<D041>", request.ConsignorLicense);

				var products = request.Products ?? new List<Re6ProductModel>();

				for (int i = 0; i < 2; i++)
				{
					if (i < request.Products?.Count)
					{
						content = content.Replace($"<D00{9 + i * 5}>", request.Products[i].ProductName)
											   .Replace($"<D0{10 + i * 5}>", request.Products[i].CD)
											   .Replace($"<D0{11 + i * 5}>", request.Products[i].Qty)
											   .Replace($"<D0{12 + i * 5}>", request.Products[i].UOM)
											   .Replace($"<D0{13 + i * 5}>", request.Products[i].Cases);
					}
					else
					{
						for (int j = 0; j < 5; j++)
						{
							content = content.Replace($"<D0{9 + i * 5 + j}>", "");
						}
					}
				}

				for (int i = 20; i <= 40; i++)
				{
					content = content.Replace($"<D0{i}>", "");
				}

				await System.IO.File.WriteAllTextAsync(fileToBeRead, content);

				// Uncomment when printer is available
				/*
                string printerIp = _configuration["AppSettings:loc1_printer"];
                int printerPort = 9100;

                using TcpClient client = new TcpClient();
                await client.ConnectAsync(IPAddress.Parse(printerIp), printerPort);

                byte[] bytes = Encoding.ASCII.GetBytes(prnContent);
                using NetworkStream stream = client.GetStream();
                await stream.WriteAsync(bytes, 0, bytes.Length);
                await stream.FlushAsync();
                */

				_APIResponse.Data = "RE6 Print done successfully";
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
				return Ok(_APIResponse);

				return Ok(new APIResponse
				{
					Status = true,
					Message = "RE6 Print done successfully"

				});
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Status = false,
					Message = "Error while processing the print request: " + ex.Message
				});
			}
		}


		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> RE6PrintpdfDetails(Re6BarcodeModel request)
		{
			try
			{
				var d = DateTime.Now.ToString("dd-MM-yyyy");
				string[] add = Regex.Split(request.Address ?? "", "District", RegexOptions.IgnoreCase);
				string addressPart = add[0];
				int l = addressPart.Length;
				int l2 = l % 2 == 0 ? l / 2 : (l / 2) + 1;

				string addLine1 = addressPart.Substring(0, l2);
				string addLine2 = addressPart.Substring(l2);
				StringBuilder printBuilder = new StringBuilder();
				string basePath = Path.Combine(_webHostEnvironment.ContentRootPath, "PRNFile");
				string printFile = Path.Combine(basePath, "RE-6_100x150_Var_script.prn");
				string fileToBeRead = Path.Combine(basePath, "RE-6_100x150_Value_script.prn");

				if (System.IO.File.Exists(printFile))
				{
					System.IO.File.Copy(printFile, fileToBeRead, true);
				}


				string content = await System.IO.File.ReadAllTextAsync(fileToBeRead);

				//string prnContent = await System.IO.File.ReadAllTextAsync(valTemplatePath);

				content = content.Replace("<D001>", d)
									   .Replace("<D002>", request.VehicleNumber)
									   .Replace("<D003>", request.VehicleLicense)
									   .Replace("<D004>", request.RE12)
									   .Replace("<D030>", request.VehicleValue)
									   .Replace("<D005>", request.ConsigneeName)
									   .Replace("<D006>", addLine1)
									   .Replace("<D019>", addLine2)
									   .Replace("<D007>", $"District -{request.District}, {request.State}")
									   .Replace("<D008>", request.LicenseNumber)
									   .Replace("<D041>", request.ConsignorLicense);

				var products = request.Products ?? new List<Re6ProductModel>();

				for (int i = 0; i < 2; i++)
				{
					if (i < request.Products?.Count)
					{
						content = content.Replace($"<D00{9 + i * 5}>", request.Products[i].ProductName)
											   .Replace($"<D0{10 + i * 5}>", request.Products[i].CD)
											   .Replace($"<D0{11 + i * 5}>", request.Products[i].Qty)
											   .Replace($"<D0{12 + i * 5}>", request.Products[i].UOM)
											   .Replace($"<D0{13 + i * 5}>", request.Products[i].Cases);
					}
					else
					{
						for (int j = 0; j < 5; j++)
						{
							content = content.Replace($"<D0{9 + i * 5 + j}>", "");
						}
					}
				}

				for (int i = 20; i <= 40; i++)
				{
					content = content.Replace($"<D0{i}>", "");
				}

				await System.IO.File.WriteAllTextAsync(fileToBeRead, content);

				var imageBytes = await ConvertZplPrnToImage(content);



				using var imageStream = new MemoryStream(imageBytes);
				using var image = Image.Load(imageStream, out var format); // Removed <Rgba32> for compatibility
																		   //using var image = Image.Load<Rgba64>(imageStream);


				// Manually set the desired image width
				int desiredWidth = 500; // Set your desired width in pixels

				// Resize image while maintaining the aspect ratio
				image.Mutate(x => x.Resize(desiredWidth, (int)(desiredWidth * (image.Height / (float)image.Width)))); // Calculate the height based on the aspect ratio
				image.Mutate(x => x.Rotate(90));


				using var pdfDoc = new PdfDocument();
				var pdfPage = pdfDoc.AddPage();
				pdfPage.Orientation = PdfSharpCore.PageOrientation.Landscape;

				//// Set page size based on rotated image
				//pdfPage.Width = image.Height;
				//pdfPage.Height = image.Width;
				pdfPage.Width = 595.28f;  // A4 Landscape width (in points)
				pdfPage.Height = 900.89f; // A4 Landscape height (in points)

				using var gfx = XGraphics.FromPdfPage(pdfPage);
				using var ms = new MemoryStream();
				image.Save(ms, new PngEncoder()); // Convert to PNG in-memory
				ms.Position = 0;

				using var xImage = XImage.FromStream(() => ms);
				gfx.DrawImage(xImage, 0, 0);

				using var pdfStream = new MemoryStream();
				pdfDoc.Save(pdfStream, false);
				pdfStream.Position = 0;

				return File(pdfStream.ToArray(), "application/pdf", "sticker.pdf");


				return Ok(new APIResponse
				{
					Status = true,
					Message = "RE6 Print done successfully"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
				{
					Status = false,
					Message = "Error while processing the print request: " + ex.Message
				});
			}
		}

		[HttpGet]
		public async Task<byte[]> ConvertZplPrnToImage(string zpl)
		{
			using (var http = new HttpClient())
			{
				var content = new StringContent(zpl, Encoding.UTF8, "application/x-www-form-urlencoded");
				var response = await http.PostAsync("http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/", content);

				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsByteArrayAsync(); // PNG bytes
				}
				else
				{
					throw new Exception("Failed to convert PRN to image: " + response.StatusCode);
				}
			}
		}





	}
}


