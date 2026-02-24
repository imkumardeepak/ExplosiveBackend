using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AIMEController : ControllerBase
    {
 
        private readonly IRe11IndentPrdInfoRepository _re11IndentPrdInfoRepository;
        private readonly IProductMasterRepository _productMasterRepository;
        private readonly IDistributedCache _distributedCache;
        private APIResponse _APIResponse;
        private readonly IMapper _mapper;
        private readonly string KeyName = "AIME";

        public AIMEController(IDistributedCache distributedCache, IMapper mapper, IRe11IndentPrdInfoRepository re11IndentPrdInfoRepository, IProductMasterRepository productMasterRepository)
        {
            
            _APIResponse = new();
            _distributedCache = distributedCache;
            _mapper = mapper;
            _re11IndentPrdInfoRepository = re11IndentPrdInfoRepository;
            _productMasterRepository = productMasterRepository;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAIMETableDetails(string indentdate, string indentNo)
        {
            try
            {
                DateTime inddate = DateTime.Parse(indentdate);

                var indentList = await _re11IndentPrdInfoRepository.GetAllAsync();
                var productList = await _productMasterRepository.GetAllAsync();

                var indentPrefix = $"RE-11/{DateTime.Now.Year}/";
                if (!indentNo.StartsWith(indentPrefix))
                {
                    _APIResponse.Message = "Invalid Indent No / Date Format!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                // Filter the indent records by indentNo
                var indentRecords = indentList
                    .Where(x => x.IndentNo == indentNo && x.IndentDt == inddate)
                    .ToList();

                if (!indentRecords.Any())
                {
                    _APIResponse.Message = "Please Check The Indent No!";
                    _APIResponse.StatusCode = HttpStatusCode.BadRequest;
                    _APIResponse.Status = false;
                    return _APIResponse;
                }

                var resultRows = new List<DataViewModel>();

                foreach (var indent in indentRecords)
                {
                    var brand = indent.Bname;
                    var psize = indent.Psize;
                    var req = indent.ReqWt;
                    var psizecode = indent.SizeCode;

                    // Find matching product
                    var product = productList.FirstOrDefault(p =>
                        p.bname == brand &&
                        p.psize == psize &&
                        p.psizecode == psizecode);

                    if (product == null || product.l1netwt == 0)
                        continue;
                    //({product.psizecode})  X {product.noofl3perl1} Nos = {product.l1netwt} {product.unit}";
                    var brandName = $"{product.bname}({product.psize})";
                    var strClass = product.Class;
                    var div = product.Division;
                    var unit = product.l1netwt;
                    var units = product.unit;

                    var quantity = req / unit;

                    // Check if already exists in resultRows
                    var existing = resultRows.FirstOrDefault(r => r.BrandName == brandName);

                    if (existing == null)
                    {
                        resultRows.Add(new DataViewModel
                        {
                            BrandName = brandName,
                            StrClass = $"{strClass}-{div}",
                            Quantity = (double)req,
                            Netl1 = (double)unit,
                            Unit = units,
                            Count = (int)quantity,
                            IndentNo = indentNo
                        });
                    }
                    else
                    {
                        existing.Quantity += (double)req;
                        existing.Count += (int)quantity;
                    }
                }

                _APIResponse.Data = resultRows;
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

