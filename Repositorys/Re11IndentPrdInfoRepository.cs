using Peso_Baseed_Barcode_Printing_System_API.Interface;
using Microsoft.EntityFrameworkCore;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Entities;
using Peso_Baseed_Barcode_Printing_System_API.Models.DTO;
using Peso_Baseed_Barcode_Printing_System_API.Repositories;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositorys
{

	public class Re11IndentPrdInfoRepository : GenericRepository<Re11IndentPrdInfo>, IRe11IndentPrdInfoRepository
	{
		private readonly ApplicationDbContext _context;
		public Re11IndentPrdInfoRepository(ApplicationDbContext context) : base(context){
			_context = context;
			}
		public async Task<List<DispatchStatusDto>> GetRE11Listdetails()
		{
			try
			{
				var result = await _context.AllLoadingSheet.Include(ls => ls.IndentDetails)
					.GroupBy(x => x.Compflag == 1 ? "Completed" : "Pending")
					.Select(g => new DispatchStatusDto
					{
						Status = g.Key,
						Count = g.Count()
					})
					.ToListAsync();

				return result;
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving dispatch status.", ex);
			}
		}

		public async Task<List<DispatchDetailsDto>> GetDispatchdeails()
		{
			DateTime today = DateTime.Today;
			DateTime yesterday = today.AddDays(-1);

			var result = await (from dispatch in _context.Re11IndentPrdInfo
								join customer in _context.Re11IndentInfo on dispatch.IndentNo equals customer.IndentNo
								where dispatch.IndentDt >= yesterday && dispatch.IndentDt <= today // Date filter
								select new DispatchDetailsDto
								{
									CustomerName = customer.CustName, // Customer Name from Re11IndentInfos
									IndentNo = dispatch.IndentNo,     // Indent Number from Re11IndentPrdInfos
									BrandName = dispatch.Bname,        // Brand Name
									ProductSize = dispatch.Psize,     // Product Size
									RequestedWeight = (double)dispatch.ReqWt,           // Requested Weight
									Status =  0        // Completion Status
								}).ToListAsync();

			return result;
		}
		public async Task<List<dynamic>> GetGroupedDispatchData(string plantName, DateTime startDate)
		{
			var disresult = await _context.Re11IndentPrdInfo
				.Where(a => a.Ptype.ToUpper() == plantName.ToUpper() && a.IndentDt >= startDate)
				.GroupBy(a => new { a.IndentDt.Date, a.Bname, a.Ptype })
				.Select(g => new
				{
					Date = g.Key.Date,
					BrandName = g.Key.Bname,
					Count = g.Sum(x => x.L1NetWt)
				})
				.ToListAsync();

			return disresult.Cast<dynamic>().ToList();
		}

		public async Task<List<RE11StatusReport>> GetRE11StatusDataAsync(DateTime fromDate, DateTime toDate, int? status, string? indentno, string? customer)
		{

            var query = from dt in _context.Re11IndentPrdInfo.AsNoTracking()
                        join cm in _context.Re11IndentInfo.AsNoTracking()
                            on dt.IndentNo equals cm.IndentNo
                        where dt.IndentDt >= fromDate && dt.IndentDt <= toDate &&
                              (!status.HasValue || cm.CompletedIndent == status.Value) &&
                              (string.IsNullOrEmpty(indentno) || dt.IndentNo == indentno) &&
                              (string.IsNullOrEmpty(customer) || cm.CustName == customer)
                        orderby dt.IndentNo, dt.Ptype
                        select new RE11StatusReport
                        {
                            re11indentno = dt.IndentNo,
                            indentDt = dt.IndentDt.Date,
                            pesore11Dt = cm.PesoDt,
                            customer = cm.CustName,
                            Ptype = dt.Ptype,
                            brand = dt.Bname,
                            productsize = dt.Psize,
                            reqqty = dt.ReqWt.ToString(),
                            requnit = dt.Unit.ToString(),
                            remqty = dt.RemWt.ToString(),
                            remunit = dt.Unit.ToString(),
                            status = cm.CompletedIndent.ToString(),
                        };

            return await query.ToListAsync();


        }




        public async Task<Re11IndentPrdInfo> GetByIndentNO(string indentNo)
		{
			return _context.Set<Re11IndentPrdInfo>().FirstOrDefault(i => i.IndentNo == indentNo);
		}
	}
}



