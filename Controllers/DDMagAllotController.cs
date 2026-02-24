using Peso_Baseed_Barcode_Printing_System_API.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DDMagAllotController : ControllerBase
    {
        private readonly IDispatchTransactionRepository _dispatchTransactionRepository;
        private readonly IMagzineMasterRepository _magzineMasterRepository;
        private readonly IMagzine_StockRepository _magzine_StockRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private const string CacheKey = "RE2DDAllot";

        public DDMagAllotController(IDispatchTransactionRepository dispatchTransactionRepository, IDistributedCache distributedCache, IMapper mapper, IMagzineMasterRepository magzineMasterRepository, IMagzine_StockRepository magzine_StockRepository)
        {
            _dispatchTransactionRepository = dispatchTransactionRepository;
            _distributedCache = distributedCache;
            _mapper = mapper;
            _magzineMasterRepository = magzineMasterRepository;
            _magzine_StockRepository = magzine_StockRepository;
        }

        // GET: api/RE2DDAllot/GetAll
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetIndentsMagAllot()
        {
            try
            {
                RE2DDAllot allotList = new RE2DDAllot
                {
                    MfgDt=DateTime.Now.Date,
                    Indentlist = await _dispatchTransactionRepository.GetIndentNoForMagAllotAsync(),
                    Mlist = await _magzineMasterRepository.GetQueryable()
         .Select(e => new SelectListItem
         {
             Value = e.mcode,
             Text = e.mcode
         })
         .ToListAsync() ?? new List<SelectListItem>()  // Fallback to empty list if null
                };


                return Ok(new APIResponse { Status = true, StatusCode = HttpStatusCode.OK, Data = allotList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse { Status = false, Errors = new List<string> { ex.Message } });
            }
        }


        // POST: api/RE2DDAllot/Create
        [HttpPost]
        public async Task<ActionResult<APIResponse>> Create(RE2DDAllot allot)
        {
            try
            {
                if (allot == null) return BadRequest(new APIResponse { Status = false, Errors = new List<string> { "Invalid data." } });

                if (allot == null)
                    return BadRequest(new APIResponse
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Errors = new List<string> { "Invalid request: magzineAllotment cannot be null" }
                    });

                // Map ManualMagAllot to List<Magzine_Stock>
                var magzinestock = _mapper.Map<List<Magzine_Stock>>(allot);

                // Get unique values for better query optimization
                var l1Barcodes = magzinestock.Select(k => k.L1Barcode).Distinct().ToList();


                var existingKeys = await _dispatchTransactionRepository
                    .GetQueryable()
                    .Where(t => l1Barcodes.Contains(t.L1Barcode))
                    .Select(t => new { t.L1Barcode, t.Bid, t.PSizeCode })
                    .ToListAsync(); // ✅ Runs efficiently in SQL

                // Convert results to a HashSet for fast lookups
                var existingKeysSet = new HashSet<string>(existingKeys.Select(e => $"{e.L1Barcode}|{e.Bid}|{e.PSizeCode}"));

                // Filter out existing records before bulk insert
                var newTransactions = magzinestock
                    .Where(t => !existingKeysSet.Contains($"{t.L1Barcode}|{t.BrandId}|{t.PSizeCode}")) // ✅ Fast HashSet lookup
                    .ToList();

                // Save mapped data to database
                await _magzine_StockRepository.AddRangeAsync(magzinestock);

                await _dispatchTransactionRepository.BulkUpdateMag(l1Barcodes, allot.Magname);


                return new APIResponse { Status = true, StatusCode = HttpStatusCode.OK, Data = allot };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse { Status = false, Errors = new List<string> { ex.Message } });
            }
        }

        //// PUT: api/RE2DDAllot/Update/5
        //[HttpPut("{id}")]
        //public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] RE2DDAllot allot)
        //{
        //    try
        //    {
        //        if (id != allot.Id) return BadRequest(new APIResponse { Status = false, Errors = new List<string> { "ID mismatch." } });

        //        var existingAllot = await _re2ddAllotRepository.GetByIdAsync(id);
        //        if (existingAllot == null)
        //            return NotFound(new APIResponse { Status = false, Errors = new List<string> { "Record not found." } });

        //        _mapper.Map(allot, existingAllot);
        //        await _re2ddAllotRepository.UpdateAsync(existingAllot);
        //        await _distributedCache.RemoveAsync(CacheKey);

        //        return Ok(new APIResponse { Status = true, StatusCode = HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new APIResponse { Status = false, Errors = new List<string> { ex.Message } });
        //    }
        //}

        //// DELETE: api/RE2DDAllot/Delete/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<APIResponse>> Delete(int id)
        //{
        //    try
        //    {
        //        if (id <= 0) return BadRequest(new APIResponse { Status = false, Errors = new List<string> { "Invalid ID." } });

        //        var allot = await _re2ddAllotRepository.GetByIdAsync(id);
        //        if (allot == null)
        //            return NotFound(new APIResponse { Status = false, Errors = new List<string> { "Record not found." } });

        //        await _re2ddAllotRepository.DeleteAsync(id);
        //        await _distributedCache.RemoveAsync(CacheKey);

        //        return Ok(new APIResponse { Status = true, StatusCode = HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new APIResponse { Status = false, Errors = new List<string> { ex.Message } });
        //    }
        //}

        //// BULK DELETE: api/RE2DDAllot/DeleteBulk
        //[HttpPost]
        //public async Task<ActionResult<APIResponse>> DeleteBulk([FromBody] List<int> ids)
        //{
        //    try
        //    {
        //        if (ids == null || !ids.Any()) return BadRequest(new APIResponse { Status = false, Errors = new List<string> { "Invalid IDs." } });

        //        await _re2ddAllotRepository.DeleteBulkAsync(ids);
        //        await _distributedCache.RemoveAsync(CacheKey);

        //        return Ok(new APIResponse { Status = true, StatusCode = HttpStatusCode.OK });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new APIResponse { Status = false, Errors = new List<string> { ex.Message } });
        //    }
        //}
    }
}

