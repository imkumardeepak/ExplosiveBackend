using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class Re11IndentPrdInfosController : ControllerBase
	{
		private readonly IRe11IndentPrdInfoRepository _repository;
		private readonly Icanre11indent_prd_infoRepository _canre11indent_prd_infoRepository;
		private readonly Icanre11indentinfoRepository _canre11indentinfoRepository;
		private readonly IDispatchTransactionRepository _DispatchTransactionRepository;
		private readonly IRe11IndentInfoRepository _re11IndentInfoRepository;
		private readonly IDistributedCache _distributedCache;
		private readonly APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private const string CacheKey = "Re11IndentPrdInfo";

		public Re11IndentPrdInfosController(IRe11IndentPrdInfoRepository repository, IDistributedCache distributedCache, IMapper mapper, IRe11IndentInfoRepository re11IndentInfoRepository, Icanre11indent_prd_infoRepository canre11indent_prd_infoRepository, Icanre11indentinfoRepository canre11indentinfoRepository, IDispatchTransactionRepository DispatchTransactionRepository)
		{
			_repository = repository;
			_re11IndentInfoRepository = re11IndentInfoRepository;
			_canre11indent_prd_infoRepository = canre11indent_prd_infoRepository;
			_canre11indentinfoRepository = canre11indentinfoRepository;
			_DispatchTransactionRepository = DispatchTransactionRepository;
			_distributedCache = distributedCache;
			_APIResponse = new APIResponse();
			_mapper = mapper;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllIndentProducts()
		{
			try
			{
				var cachedData = await _distributedCache.GetAsync(CacheKey);
				if (cachedData != null)
				{
					var products = JsonConvert.DeserializeObject<List<Re11IndentPrdInfo>>(Encoding.UTF8.GetString(cachedData));
					_APIResponse.Data = products;
				}
				else
				{
					var products = await _repository.GetAllAsync();
					var serializedData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(products));
					var cacheOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(30), AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) };
					await _distributedCache.SetAsync(CacheKey, serializedData, cacheOptions);
					_APIResponse.Data = products;
				}
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
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

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<APIResponse>> GetIndentProductById(int id)
		{
			try
			{
				var product = await _repository.GetByIdAsync(id);

				if (product == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Indent Product not found.");
					return NotFound(_APIResponse);
				}
				_APIResponse.Data = product;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
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

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<ActionResult<APIResponse>> CreateIndentProduct(Re11IndentPrdInfo product)
		{
			try
			{
				if (product == null)
					return BadRequest();

				await _distributedCache.RemoveAsync(CacheKey);
				await _repository.AddAsync(product);
				_APIResponse.Data = product;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;
				return CreatedAtAction("GetIndentProductById", new { id = product.Id }, _APIResponse);
			}
			catch (Exception ex)
			{
				_APIResponse.Errors.Add(ex.Message);
				_APIResponse.Status = false;
				_APIResponse.StatusCode = HttpStatusCode.InternalServerError;
				return StatusCode((int)HttpStatusCode.InternalServerError, _APIResponse);
			}
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> UpdateIndentProduct(int id, Re11IndentPrdInfo product)
		{
			try
			{
				if (id != product.Id)
					return BadRequest();

				var existingProduct = await _repository.GetByIdAsync(id);

				if (existingProduct == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Indent Product not found.");
					return NotFound(_APIResponse);
				}
				await _distributedCache.RemoveAsync(CacheKey);
				existingProduct = _mapper.Map(product, existingProduct);
				await _repository.UpdateAsync(existingProduct);
				_APIResponse.Data = existingProduct;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
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

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> DeleteIndentProduct(int id)
		{
			try
			{
				var existingProduct = await _repository.GetByIdAsync(id);
				if (existingProduct == null)
				{
					_APIResponse.Status = false;
					_APIResponse.StatusCode = HttpStatusCode.NotFound;
					_APIResponse.Errors.Add("Indent Product not found.");
					return NotFound(_APIResponse);
				}
				await _distributedCache.RemoveAsync(CacheKey);
				await _repository.DeleteAsync(id);
				_APIResponse.Data = true;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.OK;
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


		//[HttpGet]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		//public async Task<ActionResult<APIResponse>> GetRE11statusdata(string fromDate, string toDate, string? status, string? indentno, string? Customer)
		//{
		//    try
		//    {
		//        // Convert string dates to DateTime



		//        // Fetch all records from repositories
		//        var barcodeList = await _repository.GetAllAsync();
		//        var custoList = await _re11IndentInfoRepository.GetAllAsync();

		//        List<RE11StatusReport> plantData = new List<RE11StatusReport>();

		//        DateTime fromDateTime = DateTime.Parse(fromDate);
		//        DateTime toDateTime = DateTime.Parse(toDate);
		//        plantData = (from dt in barcodeList
		//                     join cm in custoList on dt.IndentNo equals cm.IndentNo
		//                     where dt.IndentDt >= fromDateTime && dt.IndentDt <= toDateTime &&
		//                            (string.IsNullOrEmpty(status) || dt.CompFlag == status) &&
		//                           (string.IsNullOrEmpty(indentno) || dt.IndentNo == indentno) &&
		//                           (string.IsNullOrEmpty(Customer) || cm.CustName == Customer)
		//                     orderby dt.IndentNo
		//                     select new RE11StatusReport
		//                     {
		//                         re11indentno = dt.IndentNo,
		//                         indentDt = dt.IndentDt.Date,
		//                         pesore11Dt = cm.PesoDt,
		//                         customer = cm.CustName,
		//                         Ptype = dt.Ptype,
		//                         brand = dt.Bname,
		//                         productsize = dt.Psize,
		//                         reqqty = dt.ReqWt.ToString(),
		//                         requnit = dt.ReqUnit.ToString(),
		//                         remqty = dt.RemWt.ToString(),
		//                         remunit = dt.RemUnit.ToString(),
		//                         status = dt.CompFlag.ToString(),
		//                     }).OrderBy(x => x.re11indentno).ThenBy(x => x.Ptype)

		//                     .ToList();


		//        //else
		//        //{
		//        //    DateTime fromDateTime = DateTime.Parse(fromDate);
		//        //    DateTime toDateTime = DateTime.Parse(toDate);

		//        //    plantData = barcodeList
		//        //         .Join(custoList, dt => dt.IndentNo, cm => cm.IndentNo, (dt, cm) => new { dt, cm })
		//        //         .Where(x => x.dt.DispDt >= fromDateTime && x.dt.DispDt <= toDateTime &&
		//        //                     (string.IsNullOrEmpty(magzine) || x.dt.MagName == magzine) &&
		//        //                     (string.IsNullOrEmpty(indentno) || x.dt.IndentNo == indentno) &&
		//        //                     (string.IsNullOrEmpty(Customer) || x.cm.CustName == Customer))
		//        //         .GroupBy(x => new { x.dt.IndentNo, x.dt.TruckNo, x.dt.Brand, x.dt.PSize, x.dt.MagName, x.dt.L1NetUnit, x.dt.DispDt })
		//        //         .Select(g => new DispatchReport
		//        //         {
		//        //             re11indentno = g.Key.IndentNo,
		//        //             dispatchDt = g.Key.DispDt,
		//        //             truck = g.Key.TruckNo,
		//        //             brandname = g.Key.Brand,
		//        //             productsize = g.Key.PSize,
		//        //             magname = g.Key.MagName,
		//        //             L1Barcode = g.Select(x => x.dt.L1Barcode).Distinct().Count().ToString(), // Count unique L1Barcodes
		//        //             netqty = g.Sum(x => Convert.ToDecimal(x.dt.L1NetWt)).ToString("0.00"), // Sum net quantity
		//        //             unit = g.Key.L1NetUnit
		//        //         })
		//        //         .OrderBy(x => x.magname)
		//        //         .ThenBy(x => x.re11indentno)
		//        //         .ToList();
		//        //}

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

		public async Task<ActionResult<APIResponse>> GetRE11cancellation(string indentno)
		{
			try
			{
				// Fetch all or filtered records from barcode repository
				var barcodeList = await _repository.GetAllAsync();

				var plantData = barcodeList
					.Where(dt => string.IsNullOrEmpty(indentno) || dt.IndentNo == indentno)
					.OrderBy(dt => dt.IndentNo)

					.Select(dt => new Re11IndentPrdInfo
					{

						Ptype = dt.Ptype,
						Bname = dt.Bname,         // assuming available in barcodeList
						Bid = dt.Bid,         // assuming available in barcodeList
						Psize = dt.Psize,
						SizeCode = dt.SizeCode,
						Class = dt.Class,
						Div = dt.Div,
						L1NetWt = dt.L1NetWt,
						Unit = dt.Unit

					})
					.ToList();

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
		public async Task<IActionResult> CancelIndent([FromBody] CancelIndentRequest model)
		{
			var response = new APIResponse();

			try
			{
				model.indentNo = Uri.UnescapeDataString(model.indentNo);

				if (string.IsNullOrWhiteSpace(model.indentNo) || string.IsNullOrWhiteSpace(model.reason))
				{
					response.Status = false;
					response.Errors.Add("Indent number and reason are required.");
					return BadRequest(response);
				}

				var indentInfo = await _re11IndentInfoRepository
					.GetAll()
					.FirstOrDefaultAsync(x => x.IndentNo == model.indentNo);

				if (indentInfo == null)
				{
					response.Status = false;
					response.Errors.Add("Indent not found.");
					return NotFound(response);
				}

				var today = DateTime.Now;

				// Optional: prevent duplicate entry in backup table
				var existingCancel = await _canre11indentinfoRepository
					.GetAll()
					.FirstOrDefaultAsync(x => x.indentno == model.indentNo);

				if (existingCancel == null)
				{
					var cancelInfo = new canre11indentinfo
					{
						indentno = indentInfo.IndentNo,
						indentdt = indentInfo.IndentDt,
						custname = indentInfo.CustName,
						pesodt = indentInfo.PesoDt,
						conname = indentInfo.ConName,
						conno = indentInfo.ConNo,
						clic = indentInfo.Clic,
						month = indentInfo.Month,
						year = indentInfo.Year,
						compflag = 0,
						reason = model.reason,
						canceldttime = today,
						cmonth = today.Month.ToString(),
						cyear = today.Year.ToString()
					};

					await _canre11indentinfoRepository.AddAsync(cancelInfo);
				}

				if (model.cancelType == "Full")
				{
					var allProducts = await _repository
						.GetAll()
						.Where(p => p.IndentNo == model.indentNo)
						.ToListAsync();

					if (allProducts.Any())
					{
						var backupProducts = allProducts.Select(p => new canre11indent_prd_info
						{
							pid = 0,
							indentno = p.IndentNo,
							ptype = p.Ptype,
							ptypecode = p.PtypeCode,
							bname = p.Bname,
							bid = p.Bid,
							psize = p.Psize,
							sizecode = p.SizeCode,
							Class = Convert.ToInt32(p.Class),
							div = Convert.ToInt32(p.Div),
							l1netwt = p.L1NetWt,
							unit = p.Unit,
							compflag = 1,
							remunit = p.Unit,
							remwt = Convert.ToInt32(p.RemWt),
							indentdt = p.IndentDt,
							loadunit = p.Unit?.ToString(),
							reqwt = p.ReqWt,
							requnit = p.Unit
						}).ToList();

						await _canre11indent_prd_infoRepository.AddRangeAsync(backupProducts);
						await _repository.DeleteRangeAsync(x => x.IndentNo == model.indentNo);
						await _DispatchTransactionRepository.DeleteRangeAsync(x => x.IndentNo == model.indentNo);
					}

					await _re11IndentInfoRepository.DeleteAsync(Convert.ToInt32(indentInfo.IndentNo));
					response.Status = true;
					response.Data = $"RE-11 Indent {model.indentNo} cancelled completely!";
					return Ok(response);
				}

				if (model.cancelType == "Partial")
				{
					if (model.selectedProductIds == null || !model.selectedProductIds.Any())
					{
						response.Status = false;
						response.Errors.Add("Please select at least one product for partial cancellation.");
						return BadRequest(response);
					}

					bool anyCancelled = false;

					foreach (var pid in model.selectedProductIds)
					{
						var product = await _repository
							.GetAll()
							.FirstOrDefaultAsync(p => p.IndentNo == model.indentNo);

						if (product != null)
						{
							var backup = new canre11indent_prd_info
							{
								pid = 0,
								indentno = product.IndentNo,
								ptype = product.Ptype,
								ptypecode = product.PtypeCode,
								bname = product.Bname,
								bid = product.Bid,
								psize = product.Psize,
								sizecode = product.SizeCode,
								Class = Convert.ToInt32(product.Class),
								div = Convert.ToInt32(product.Div),
								l1netwt = product.L1NetWt,
								unit = product.Unit,
								reqwt = product.ReqWt,
								requnit = product.Unit,
								compflag = 0,
								remunit = product.Unit,
								remwt = Convert.ToInt32(product.RemWt),
								loadunit = product.Unit,
								indentdt = product.IndentDt,
								loadwt = Convert.ToInt32(product.LoadWt),
							};

							await _canre11indent_prd_infoRepository.AddAsync(backup);
							await _repository.DeleteRE11Async(pid);
							anyCancelled = true;
						}
					}

					if (anyCancelled)
					{
						await _DispatchTransactionRepository.RemoveByPidsAsync(model.indentNo);
						response.Status = true;
						response.Data = $"RE-11 Indent {model.indentNo} cancelled partly!";
						return Ok(response);
					}

					response.Status = false;
					response.Errors.Add("No valid products found to cancel.");
					return BadRequest(response);
				}

				response.Status = false;
				response.Errors.Add("Invalid cancellation type.");
				return BadRequest(response);
			}
			catch (Exception ex)
			{
				response.Status = false;
				response.Errors.Add($"An error occurred: {ex.Message}");
				return StatusCode(500, response);
			}
		}


		public class CancelIndentRequest
		{
			public string indentNo { get; set; }
			public string reason { get; set; }
			public string cancelType { get; set; } // "Full" or "Partial"
			public List<int> selectedProductIds { get; set; }
		}

	}
}

