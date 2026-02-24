using Peso_Baseed_Barcode_Printing_System_API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Net;
using System.Text;

namespace Peso_Baseed_Barcode_Printing_System_API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	public class MagazineStocksController : ControllerBase
	{
		private readonly IGenericRepository<Magzine_Stock> _magzineStockRepository;
		private readonly IDistributedCache _distributedCache;
		private APIResponse _APIResponse;
		private readonly IMapper _mapper;
		private readonly string KeyName = "MagazineStock";
		private readonly IL1barcodegenerationRepository _l1BarcodegenerationRepository;
		private readonly IProductMasterRepository _ProductMasterRepository;
		private readonly IMagzineMasterRepository _magzineMasterRepository;

		public MagazineStocksController(IGenericRepository<Magzine_Stock> magzineStockRepository, IDistributedCache distributedCache, IMapper mapper, IL1barcodegenerationRepository l1BarcodegenerationRepository, IProductMasterRepository ProductMasterRepository, IMagzineMasterRepository magzineMasterRepository)
		{
			_magzineStockRepository = magzineStockRepository;
			_APIResponse = new();
			_distributedCache = distributedCache;
			_mapper = mapper;
			_l1BarcodegenerationRepository = l1BarcodegenerationRepository;
			_ProductMasterRepository = ProductMasterRepository;
			_magzineMasterRepository = magzineMasterRepository;
		}

		// GET: api/MagazineStock
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetAllMagazineStock()
		{
			try
			{
				string serializedList = string.Empty;
				var encodedList = await _distributedCache.GetAsync(KeyName);

				if (encodedList != null)
				{
					_APIResponse.Data = JsonConvert.DeserializeObject<List<Magzine_Stock>>(Encoding.UTF8.GetString(encodedList));
					_APIResponse.Status = true;
					_APIResponse.StatusCode = HttpStatusCode.OK;
				}
				else
				{
					var magazineStock = await _magzineStockRepository.GetAllAsync();
					if (magazineStock != null)
					{
						serializedList = JsonConvert.SerializeObject(magazineStock);
						encodedList = Encoding.UTF8.GetBytes(serializedList);

						var options = new DistributedCacheEntryOptions()
							.SetSlidingExpiration(TimeSpan.FromDays(30))
							.SetAbsoluteExpiration(TimeSpan.FromDays(30));
						await _distributedCache.SetAsync(KeyName, encodedList, options);

						_APIResponse.Data = magazineStock;
						_APIResponse.Status = true;
						_APIResponse.StatusCode = HttpStatusCode.OK;
					}
				}

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



		// POST: api/MagazineStock
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> CreateMagazineStock(Magzine_Stock magazineStock)
		{
			try
			{
				if (magazineStock == null)
					return BadRequest();

				await _distributedCache.RemoveAsync(KeyName);
				await _magzineStockRepository.AddAsync(magazineStock);

				_APIResponse.Data = magazineStock;
				_APIResponse.Status = true;
				_APIResponse.StatusCode = HttpStatusCode.Created;

				return CreatedAtAction("GetMagazineStockById", new { id = magazineStock.L1Barcode }, _APIResponse);
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
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<APIResponse>> GetMagazinesstock()
		{
			try
			{
				// Fetch all magazine stock data from the repository
				var magazineList = await _magzineStockRepository.GetAllAsync();

				// Group magazines where stock == 1 by MagName
				var groupedMagazines = magazineList
					.Where(m => m.Stock == 1) // Safely parse Stock
					.GroupBy(m => m.MagName)             // Group by magazine name
					.Select(g => new
					{
						Name = g.Key,                    // Magazine Name
						Count = g.Count()                // Count of stocks
					})
					.ToList();

				var magdata = magazineList.Where(m => m.Stock == 1).GroupBy(m => m.MagName).Select(a => new { Count = a.Count() }).ToList();
				_APIResponse.Data = magdata;
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
	}
}
