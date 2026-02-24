using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	class Re12Repository : GenericRepository<RE12Gen>, IRe12Repository
	{
		private readonly ApplicationDbContext _context;
		public Re12Repository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public List<string> GetL1DataRE12(string indentno, string truckno, string mag, string bid, string psize, int flag)
		{
			try
			{
				return _context.DispatchTransaction
					.Where(x => x.IndentNo == indentno && x.TruckNo == truckno && x.MagName == mag && x.Bid == bid && x.PSizeCode == psize && x.Re12 == (flag != 0))
					.Select(x => x.L1Barcode)
					.ToList();
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while retrieving L1 data for RE12.", ex);
			}
		}

		public async Task UpdateRe12(List<string> barcodes, string IndentNo, string brandid, string sizecode, string magname)
		{
			try
			{
				if (barcodes == null || !barcodes.Any())
					throw new ArgumentException("The list of barcodes cannot be null or empty.");

				await _context.DispatchTransaction
					.Where(x => barcodes.Contains(x.L1Barcode) && x.IndentNo == IndentNo)
					.ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Re12, true));

				await _context.Magzine_Stock
				   .Where(x => barcodes.Contains(x.L1Barcode) && x.BrandId == brandid)
				   .ExecuteUpdateAsync(setters => setters
				   .SetProperty(x => x.Re12, true).SetProperty(x => x.Re2, true));

				await _context.BarcodeData
				  .Where(x => barcodes.Contains(x.L1))
				  .ExecuteUpdateAsync(setters => setters
				  .SetProperty(x => x.Re12, true).SetProperty(x => x.Re2, true));

				await _context.AllLoadingIndentDeatils
				  .Where(x => x.IndentNo == IndentNo && x.Bid == brandid && x.SizeCode == sizecode && x.mag == magname)
				  .ExecuteUpdateAsync(setters => setters
				  .SetProperty(x => x.iscompleted, 2));
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while performing the bulk update.", ex);
			}
		}
	}
}



