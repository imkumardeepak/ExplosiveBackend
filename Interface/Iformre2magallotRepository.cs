using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
  
    public interface Iformre2magallotRepository : IGenericRepository<formre2magallot>
    {
        bool ExistsByBarcode(string barcode);

        Task<List<SmallModel>> GetL1MagtransData(DateTime mfgdt, string pcode, string brandId, string pSizeCode, string magname);
    }
}
