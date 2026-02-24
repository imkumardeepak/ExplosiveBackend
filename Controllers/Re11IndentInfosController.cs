using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Core.Types;
using Peso_Based_Barcode_Printing_System_API.Services;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class Re11IndentInfosController : ControllerBase
    {
        private readonly IRe11IndentInfoRepository _indentRepository;
        private readonly ICustomerMastersRepository _customerMastersRepository;
        private readonly ICustMemberDetailsRepository _custMemberDetailsRepository;
        private readonly ICustMagazineDetailsRepository _custMagazineDetailsRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IPlantMasterRepository _plantMasterRepository;
        private readonly IBrandMasterRepository _brandMasterRepository;
        private readonly IRe11IndentPrdInfoRepository _indentPrdInfoRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceProvider _serviceProvider1;
        private readonly IServiceProvider _serviceProvider2;
        private readonly IServiceProvider _serviceProvider3;
        private readonly IServiceProvider _serviceProvider4;
        private readonly IDistributedCache _distributedCache;
        private readonly APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private const string CacheKey = "Re11IndentInfo";
        private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
        private readonly PdfReaderService _pdfReaderService;


        public Re11IndentInfosController(IRe11IndentInfoRepository indentRepository, IDistributedCache distributedCache, IMapper mapper, ICustomerMastersRepository customerMastersRepository, ICustMemberDetailsRepository custMemberDetailsRepository, ICustMagazineDetailsRepository custMagazineDetailsRepository, IProductMasterRepository productMasterRepository, IServiceProvider serviceProvider, IPlantMasterRepository plantMasterRepository, IBrandMasterRepository brandMasterRepository, IRe11IndentPrdInfoRepository indentPrdInfoRepository, IDispatchTransactionRepository dispatchTransactionRepository, PdfReaderService pdfReaderService)
        {
            _indentRepository = indentRepository;
            _distributedCache = distributedCache;
            _APIResponse = new APIResponse();
            _mapper = mapper;
            _customerMastersRepository = customerMastersRepository;
            _custMemberDetailsRepository = custMemberDetailsRepository;
            _custMagazineDetailsRepository = custMagazineDetailsRepository;
            _productMasterRepository = productMasterRepository;
            _serviceProvider = serviceProvider;
            _serviceProvider1 = serviceProvider;
            _serviceProvider2 = serviceProvider;
            _serviceProvider3 = serviceProvider;
            _serviceProvider4 = serviceProvider;
            _plantMasterRepository = plantMasterRepository;
            _brandMasterRepository = brandMasterRepository;
            _indentPrdInfoRepository = indentPrdInfoRepository;
            _DispatchTransactionRepository = dispatchTransactionRepository;
            _pdfReaderService = pdfReaderService;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllIndents()
        {
            try
            {
                var indents = await _indentRepository.GetAllIndentInfoAlongProduct();
                _APIResponse.Data = indents;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetOnlyIndentsNo()
        {
            try
            {
                var indents = await _indentRepository.GetOnlyIndentNo();
                _APIResponse.Data = indents;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCreateIndents()
        {
            try
            {
                var customers = await _customerMastersRepository.GetCustomerDetailsWithAllDetails();
                List<string> allindetno = _indentRepository.GetOnlyIndentNo().Result.ToList();
                List<ProductMaster> products = await _productMasterRepository.GetAllAsync();

                var re11indent = new Re11IndentInfoViewModel()
                {
                    AllExistingIndentno = allindetno,
                    IndentNo = "RE-11/" + DateTime.Now.Year + "/",
                    CustomerslIST = customers,
                    ProductList = products,
                };

                _APIResponse.Data = re11indent;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetIndentByNo(string id)
        {
            try
            {
                var indent = _indentRepository.GetByIndentNO(id);
                if (indent == null)
                {
                    _APIResponse.Status = false;
                    _APIResponse.StatusCode = HttpStatusCode.NotFound;
                    _APIResponse.Errors.Add("Indent not found.");
                    return NotFound(_APIResponse);
                }

                _APIResponse.Data = indent;
                _APIResponse.Status = true;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateIndent(Re11IndentInfoViewModel indent)
        {
            try
            {

                // Assign the customer and contact names to the indent ViewModel
                indent.CustName = indent.CustName;
                indent.ConName = indent.ConName;
                indent.IndentDt = DateTime.Now.Date;
                // Map ViewModel to Entity using AutoMapper
                var re11indentinfo = _mapper.Map<Re11IndentInfo>(indent);

                // Map product info list to the Re11IndentPrdInfo
                re11indentinfo.IndentItems = _mapper.Map<List<Re11IndentPrdInfo>>(indent);

                // Optionally, save to the database (uncomment the save line when the repository is set up)
                await _indentRepository.AddAsync(re11indentinfo);

                // Return the API response with the created indent data
                _APIResponse.Data = re11indentinfo;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;

                // Return the CreatedAtAction response
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                // Handle any errors that occur and return a 500 Internal Server Error
                _APIResponse.Errors.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }


        //Commented By Niraj
        /*[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> RE11FileUpload(CombinedIndentRecord file)
		{
			try
			{

				var customers = await _customerMastersRepository.GetCustomerDetailsWithAllDetails();

				var result = await _customerMastersRepository.GetCustomerMasterByCname(file.IndentDetails.Cname.ToUpper());

				if (result != null)
				{

					var re11indent = new Re11IndentInfoViewModel()
					{
						IndentNo = file.IndentDetails.IndentNo,
						PesoDt = Convert.ToDateTime(file.IndentDetails.Date),
						CustomerslIST = customers,
						CustName = result.Id.ToString(),
						ConName = result.Members.Where(e => e.Name.ToUpper() == file.IndentDetails.OccupierName.ToUpper().Replace("\n", " ")).Select(e => e.Id).FirstOrDefault() == 0 ? null : result.Members.Where(e => e.Name.ToUpper() == file.IndentDetails.OccupierName.ToUpper().Replace("\n", " ")).Select(e => e.Id).FirstOrDefault().ToString(),
						ConNo = result.Members.Where(e => e.Name.ToUpper() == (file.IndentDetails.OccupierName.ToUpper().Replace("\n", " "))).Select(c => c.ContactNo).FirstOrDefault() == null ? null : result.Members.Where(e => e.Name.ToUpper() == (file.IndentDetails.OccupierName.ToUpper().Replace("\n", " "))).Select(c => c.ContactNo).FirstOrDefault().ToString(),
						Clic = file.IndentDetails.License.Replace(" ", ""),
						IndentDt = DateTime.Now.Date,
						Month = DateTime.Now.ToString("MM"),
						Year = DateTime.Now.ToString("yyyy"),
						CompFlag = 0,
						PrdInfoList = new List<Re11IndentPrdInfoViewModel>(),


					};

					List<string> plants = _plantMasterRepository.GetAllAsync().Result.Select(p => p.PName).ToList();

					var re11indentprd = (await Task.WhenAll(
						file.ExtractedRecords.Select(async item =>
						{
							using (var scope = _serviceProvider.CreateScope()) // New scope for each call
							{
								var productRepository = scope.ServiceProvider.GetRequiredService<IProductMasterRepository>();
								var productdata = (await productRepository.GetProductByBidAndPsize(item.Bid.PadLeft(4, '0'), item.PSizeCode));


								using (var scope1 = _serviceProvider1.CreateScope()) // New scope for each call
								{
									var bnameRepository = scope1.ServiceProvider.GetRequiredService<IBrandMasterRepository>();

								}

								using (var scope2 = _serviceProvider2.CreateScope()) // New scope for each call
								{
									var pRepository = scope2.ServiceProvider.GetRequiredService<IProductMasterRepository>();

								}



								return productdata != null ? new Re11IndentPrdInfoViewModel
								{

									Ptype = productdata.ptype,
									ptypelist = plants.Select(pname => new SelectListItem { Value = pname, Text = pname }).ToList(),
									PtypeCode = productdata.ptypecode,
									Bid = productdata.bid,
									Bname = productdata.bname,
									Psize = productdata.psize,
									SizeCode = productdata.psizecode,
									Class = productdata.Class,
									Div = productdata.Division,
									L1NetWt = productdata.l1netwt,
									ReqWt = Convert.ToDouble(item.Quantity),
									ReqUnit = productdata.unit,
									RemWt = 0,
									RemUnit = productdata.unit,
									LoadWt = 0,
									LoadUnit = productdata.unit,
									CompFlag = 0
								} : null;
							}
						})
					)).Where(x => x != null).ToList();





					// Assign to PrdInfoList
					re11indent.PrdInfoList = re11indentprd;



					_APIResponse.Data = re11indent;
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
					return Ok(_APIResponse);
				}

				_APIResponse.Data = "Customer Not Found";
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.OK;

				return Ok(_APIResponse);
			}
			catch (Exception ex)
			{
				// Handle any errors that occur and return a 500 Internal Server Error
				_APIResponse.Errors.Add(ex.Message);
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}*/



        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UploadRe11Pdf(IFormFile file)
        {
            var _apiResponse = new APIResponse();

            if (file == null || file.Length == 0)
            {
                _apiResponse.Status = false;
                _apiResponse.Message = "No file uploaded.";
                _apiResponse.Data = "No file uploaded.";
				_apiResponse.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(_apiResponse);
            }

            if (file.ContentType != "application/pdf")
            {
                _apiResponse.Status = false;
				_apiResponse.Data = "Invalid file type.";
                _apiResponse.Message = "Only PDF files are allowed.";
				_apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                string pdfText = _pdfReaderService.ReadPdfFromStream(memoryStream);

                string value = ExtractValue(pdfText, "(if any)", "I/We hold");
                var indentDetails = ExtractIndentData(pdfText);
                var tableData = ExtractDynamicData(value);
				List<string> allindetno = _indentRepository.GetOnlyIndentNo().Result.ToList();
				var combinedRecord = new CombinedIndentRecord
                {
                    IndentDetails = indentDetails,
                    ExtractedRecords = tableData
                };

                if (allindetno.Contains(combinedRecord.IndentDetails.IndentNo))
                {
					_apiResponse.Status = false;
					_apiResponse.Data = "Indent No Already Exists";
					_apiResponse.Message = "Indent No Already Exists";
					_apiResponse.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_apiResponse);
                }

                var customers = await _customerMastersRepository.GetCustomerDetailsWithAllDetails();
                var result = await _customerMastersRepository.GetCustomerMasterByCname(combinedRecord.IndentDetails.Cname.ToUpper());
				List<ProductMaster> products = await _productMasterRepository.GetAllAsync();

				if (result == null)
                {
                    _apiResponse.Status = false;
                    _apiResponse.Data = "Customer Not Found";
					_apiResponse.Message = "Customer not found. Please add the customer to the master list first.";
					_apiResponse.StatusCode = HttpStatusCode.BadRequest;
					return Ok(_apiResponse);
                }

                var re11indent = new Re11IndentInfoViewModel
                {
                    AllExistingIndentno = allindetno,
                    IndentNo = combinedRecord.IndentDetails.IndentNo,
                    PesoDt = DateTime.ParseExact(combinedRecord.IndentDetails.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    CustomerslIST = customers,
                    CustName = result.CName.ToString(),
                    ConName = result.Members.FirstOrDefault(e => e.Name.ToUpper().Contains(combinedRecord.IndentDetails.OccupierName.ToUpper().Replace("\n", " ")))?.Name.ToString(),
                    ConNo = result.Members.FirstOrDefault(e => e.Name.ToUpper().Contains(combinedRecord.IndentDetails.OccupierName.ToUpper().Replace("\n", " ")))?.ContactNo?.ToString(),
                    Clic = combinedRecord.IndentDetails.License.Replace(" ", ""),
                    IndentDt = DateTime.Now.Date,
                    Month = DateTime.Now.ToString("MM"),
                    Year = DateTime.Now.ToString("yyyy"),
                    CompFlag = 0,
                    ProductList= products,
                    PrdInfoList = new List<Re11IndentPrdInfoViewModel>()
                };

                var plants = _plantMasterRepository.GetAllAsync().Result.Select(p => p.PName).ToList();

                var prdList = await Task.WhenAll(combinedRecord.ExtractedRecords.Select(async item =>
                {
                    item.PSizeCode = item.PSizeCode.Substring(0, 3).Trim();
                    using var scope = _serviceProvider.CreateScope();
                    var productRepo = scope.ServiceProvider.GetRequiredService<IProductMasterRepository>();
                    var productdata = await productRepo.GetProductByBidAndPsize(item.Bid.PadLeft(4, '0'), item.PSizeCode);

                    if (productdata == null) return null;

                    return new Re11IndentPrdInfoViewModel
                    {
                        Ptype = productdata.ptype,
                        ptypelist = plants.Select(p => new SelectListItem { Value = p, Text = p }).ToList(),
                        PtypeCode = productdata.ptypecode,
                        Bid = productdata.bid,
                        Bname = productdata.bname,
                        Psize = productdata.psize,
                        SizeCode = productdata.psizecode,
                        Class = productdata.Class,
                        Div = productdata.Division,
                        L1NetWt = (double)productdata.l1netwt,
                        ReqWt = Convert.ToDouble(item.Quantity),
                        ReqUnit = productdata.unit,
                        reqCase=Convert.ToInt32(Convert.ToDouble(item.Quantity) / (double)productdata.l1netwt),
                        RemWt = Convert.ToDouble(item.Quantity),
						RemUnit = productdata.unit,
                        remCase= Convert.ToInt32(Convert.ToDouble(item.Quantity) / (double)productdata.l1netwt),
						LoadWt = 0,
                        LoadCase=0,
                        LoadUnit = productdata.unit,
                        CompFlag = 0
                    };
                }));

                re11indent.PrdInfoList = prdList.Where(p => p != null).ToList();

				var re11indentinfo = _mapper.Map<Re11IndentInfo>(re11indent);

				// Map product info list to the Re11IndentPrdInfo
				re11indentinfo.IndentItems = _mapper.Map<List<Re11IndentPrdInfo>>(re11indent);

				// Optionally, save to the database (uncomment the save line when the repository is set up)
				await _indentRepository.AddAsync(re11indentinfo);

				_apiResponse.Status = true;
                _apiResponse.Data = re11indent;
                _apiResponse.Message = "File uploaded successfully.";
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, _apiResponse);
            }
        }




        //[HttpPut("{id}")]
        //public async Task<ActionResult<APIResponse>> UpdateIndent(string id, Re11IndentInfo indent)
        //{
        //    try
        //    {
        //        if (id != indent.IndentNo)
        //            return BadRequest();

        //        await _distributedCache.RemoveAsync(CacheKey);
        //        var existingIndent = await _indentRepository.GetByIdAsync(id);
        //        if (existingIndent == null)
        //        {
        //            _APIResponse.Status = false;
        //            _APIResponse.StatusCode = HttpStatusCode.NotFound;
        //            _APIResponse.Errors.Add("Indent not found.");
        //            return NotFound(_APIResponse);
        //        }

        //        existingIndent = _mapper.Map(indent, existingIndent);
        //        await _indentRepository.UpdateAsync(existingIndent);

        //        _APIResponse.Data = existingIndent;
        //        _APIResponse.Status = true;
        //        return Ok(_APIResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _APIResponse.Errors.Add(ex.Message);
        //        return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<APIResponse>> DeleteIndent(string id)
        //{
        //    try
        //    {
        //        var existingIndent = await _indentRepository.GetByIdAsync(id);
        //        if (existingIndent == null)
        //        {
        //            _APIResponse.Status = false;
        //            _APIResponse.StatusCode = HttpStatusCode.NotFound;
        //            _APIResponse.Errors.Add("Indent not found.");
        //            return NotFound(_APIResponse);
        //        }

        //        await _distributedCache.RemoveAsync(CacheKey);
        //        await _indentRepository.DeleteAsync(id);

        //        _APIResponse.Data = true;
        //        _APIResponse.Status = true;
        //        return Ok(_APIResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _APIResponse.Errors.Add(ex.Message);
        //        return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
        //    }
        //}

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
                var dispatchList = await _indentPrdInfoRepository.GetAllAsync();
                var customerList = await _indentRepository.GetAllAsync(); // Fetch customer data

                // Define date range: from one day before today to today
                DateTime today = DateTime.Today;
                DateTime yesterday = today.AddDays(-1);

                // Join both lists based on a common key (assuming IndentNo) and filter by date
                var joinedData = from dispatch in dispatchList
                                 join customer in customerList on dispatch.IndentNo equals customer.IndentNo
                                 where dispatch.IndentDt >= yesterday && dispatch.IndentDt <= today // Date filter
                                 select new
                                 {
                                     CustomerName = customer.CustName, // Customer Name from Re11IndentInfos
                                     indentno = dispatch.IndentNo,     // Indent Number from Re11IndentPrdInfos
                                     brandnam = dispatch.Bname,        // Brand Name
                                     productsize = dispatch.Psize,     // Product Size
                                     reqwt = dispatch.ReqWt,           // Requested Weight
                                     status = 0        // Completion Status
                                 };

                // Prepare API response
                var response = new APIResponse
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    Data = joinedData.ToList() // Convert to list
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



        [HttpGet("{cName}")]
        public async Task<ActionResult<APIResponse>> GetRE11indentno(string cName)
        {
            var response = new APIResponse();
            try
            {
                var psizeNames = await _indentRepository.GetIndentsByCName(cName);

                if (psizeNames.Any())
                {
                    response.Data = psizeNames;
                    response.Status = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                response.Status = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        // DELETE: api/PlantMaster/5
        [HttpDelete("{indent}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Deleteindent(string indent)
        {
            try
            {
                if (indent == null)
                    return BadRequest();

                var indentno = indent.Replace(".", "/");

                //var data = await _indentPrdInfoRepository.GetByIndentNO(indentno);

                //await _indentPrdInfoRepository.DeleteAsync(data.Id);

                var plantMaster = _indentRepository.GetByIndentNO(indentno);
                if (plantMaster == null)
                    return NotFound();

                await _indentRepository.Deletel1Async(indentno);

                // Delete from cache
                //await _distributedCache.RemoveAsync(KeyName);

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


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetOnlyIndents()
        {
            try
            {
                var indents = await _indentRepository.GetOnlyIndentNo();
                _APIResponse.Data = indents;
                _APIResponse.Status = true;
                _APIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.Errors.Add(ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
            }
        }



        static string ExtractValue(string input, string startKeyword, string endKeyword)
        {
            int startIndex = input.IndexOf(startKeyword) + startKeyword.Length;
            int endIndex = input.IndexOf(endKeyword, startIndex);
            if (startIndex < startKeyword.Length || endIndex == -1)
            {
                return "Not Found"; // Handle cases where keywords are missing
            }
            return input.Substring(startIndex, endIndex - startIndex).Trim();
        }

        private List<ExtractedRecord> ExtractDynamicData(string inputText)
        {
            var records = new List<ExtractedRecord>();

            // Regular expression pattern to match each line in the table
            var regex = new Regex(@"(?<srno>\d+)\s*(?<bname>[\w\s\-]+)\s*\((?<bid>\d+)\)\s*(?<psizecode>\w+)\s*(?<psizedesc>[\w\s\-\.]+)\s*(?:Nos\.\s*=\s*(?<l1netwt>\d+))?\s*(?<unit>[A-Za-z]+)\s*(?<class>\d+)\s*(?<div>\d+)\s*(?<quantity>[\d\.,]+)\s*(?<quantityUnit>[A-Za-z]+)", RegexOptions.IgnoreCase);

            var matches = regex.Matches(inputText);

            foreach (Match match in matches)
            {
                var record = new ExtractedRecord
                {
                    SrNo = match.Groups["srno"].Value,
                    BName = match.Groups["bname"].Value.Trim(),
                    Bid = match.Groups["bid"].Value,
                    PSizeCode = match.Groups["psizecode"].Value,
                    PSizeDesc = match.Groups["psizedesc"].Value,
                    L1NetWt = string.IsNullOrEmpty(match.Groups["l1netwt"].Value) ? 0 : int.Parse(match.Groups["l1netwt"].Value),
                    Unit = match.Groups["unit"].Value,
                    Class = match.Groups["class"].Value,
                    Div = match.Groups["div"].Value,
                    Quantity = match.Groups["quantity"].Value,
                    QuantityUnit = match.Groups["quantityUnit"].Value
                };

                records.Add(record);
            }

            return records;
        }

        private IndentDetails ExtractIndentData(string inputText)
        {
            // Regular expressions to extract Indent No, Date, Occupier Name, and License
            var indentNoRegex = new Regex(@"Indent No:\s*(RE-\d+/\d+/\d+)");
            var dateRegex = new Regex(@"Date:\s*(\d{2}/\d{2}/\d{4})");
            // Regular expressions to extract Occupier Name and License

            var indentNoMatch = indentNoRegex.Match(inputText);
            var dateMatch = dateRegex.Match(inputText);
            string occupierNameMatch = ExtractValue(inputText, "Signature attested by :", "(Authorized Signatory)"); // (Occupier)
            string licenseMatch = ExtractValue(inputText, "licence number", "in LE-3");
            string customername = ExtractValuecust(inputText, "(Authorized Signatory) M/s.", "Licence Number :");

            if (customername == "")
            {
                customername = ExtractValuecust(inputText, "(Authorized Signatory)M/s.", "Licence Number :");
            }

            if (customername == "")
            {
                customername = ExtractValuecust(inputText, "(Occupier)M/s.", "Licence Number :");
            }



            if (occupierNameMatch == "Not Found")
            {
                occupierNameMatch = ExtractValue(inputText, "Signature attested by :", "(Authorized");
            }




            if (indentNoMatch.Success && dateMatch.Success && occupierNameMatch != null && licenseMatch != null)
            {
                var indentDetails = new IndentDetails
                {
                    Cname = customername,
                    IndentNo = indentNoMatch.Groups[1].Value,
                    Date = dateMatch.Groups[1].Value,
                    OccupierName = occupierNameMatch,
                    License = licenseMatch
                };

                return indentDetails;
            }

            return null;
        }

        public static string ExtractValuecust(string inputText, string startPattern, string endPattern)
        {
            // Create a dynamic regex pattern using start and end patterns passed as parameters
            string pattern = $@"(?<={Regex.Escape(startPattern)})(.*?)(?={Regex.Escape(endPattern)})";
            Regex regex = new Regex(pattern, RegexOptions.Singleline);

            // Perform the matching
            Match match = regex.Match(inputText);

            // Return the matched value if successful, otherwise an empty string
            return match.Success ? match.Value.Trim() : string.Empty;
        }

    }
}
