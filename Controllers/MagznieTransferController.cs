using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using Peso_Baseed_Barcode_Printing_System_API.Services;
using System.Net;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class MagznieTransferController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly APIResponse _APIResponse;
		private readonly WebSocketService _webSocketService;
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IMagzine_StockRepository _StockRepository;

		public MagznieTransferController(ApplicationDbContext context, APIResponse aPIResponse, WebSocketService webSocketService, IL1barcodegenerationRepository l1BarcodegenerationRepository, IMagzine_StockRepository stockRepository)
		{
			_context = context;
			_APIResponse = aPIResponse;
			_webSocketService = webSocketService;
			_l1BarcodegenerationRepository = l1BarcodegenerationRepository;
			_StockRepository = stockRepository;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<ProductionTransfer>>> GetTransferProdToMagzine()
		{
			try
			{
				var data = await _context.ProductionTransfers.Where(a => a.Allotflag == 0).Include(a => a.Barcodes).ToListAsync();
				_APIResponse.Data = data;
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ProductionMagzineAllocation>> PostProductionMagzineAllocation(List<ProductionMagzineAllocation> transferToMazgnie)
		{
			try
			{
				
				_context.ProductionMagzineAllocations.AddRange(transferToMazgnie);
				var productionTransfers = _context.ProductionTransfers.Where(x => x.TransId == transferToMazgnie.Select(y => y.TransferId).FirstOrDefault()).FirstOrDefault();
				productionTransfers.Allotflag = 1;
				await _context.SaveChangesAsync();
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
	}
}

