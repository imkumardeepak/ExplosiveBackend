using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	public class formre2magallotRepository : GenericRepository<formre2magallot>, Iformre2magallotRepository
	{
		private readonly ApplicationDbContext _context;
		public formre2magallotRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}

		public bool ExistsByBarcode(string barcode)
		{
			return _context.FormRe2MagAllot.Any(x => x.l1barcode == barcode);
		}

		public async Task<List<SmallModel>> GetL1MagtransData(DateTime mfgdt, string pcode, string brandId, string pSizeCode, string magname)
		{
			return await _context.Set<formre2magallot>()
				.Where(b => b.mfgdt == mfgdt && b.pcode == pcode && b.bid == brandId && b.psize == pSizeCode && b.magname == magname)
				.Select(b => new SmallModel
				{
					L1Barcode = b.l1barcode,
					L1NetWt = (double?)b.l1netwt,
					//L1NetUnit = b.l1netunit,
                })
				.ToListAsync();
		}


		//public async Task BulkUpdateMag(List<string> barcodes, string newMagValue)
		//{
		//    try
		//    {
		//        // Ensure the list is not empty
		//        if (barcodes == null || !barcodes.Any())
		//            throw new ArgumentException("The list of barcodes cannot be null or empty.");

		//        // Perform bulk update using ExecuteUpdateAsync
		//        await _context.formre2magallot
		//            .Where(x => barcodes.Contains(x.l1barcode))
		//            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.magname, newMagValue));
		//    }
		//    catch (Exception ex)
		//    {
		//        throw new Exception("An error occurred while performing the bulk update.", ex);
		//    }
		//}

	}
}


