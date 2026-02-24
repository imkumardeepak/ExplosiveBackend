using System.Linq.Expressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Interface
{
	public interface IGenericRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> FindByNameAsync(Expression<Func<T, bool>> predicate);
		Task<T?> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(int id);
		Task AddRangeAsync(IEnumerable<T> entities);

		Task BulkInsertAsync(IEnumerable<T> entities);

		Task BulkRemoveAsync(IEnumerable<T> entities);
		Task Deletel1Async(string l1barcode);
		// ✅ Expose IQueryable for efficient queries
		IQueryable<T> GetQueryable();
		Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter);
		IQueryable<T> GetAll();
		Task DeleteRangeAsync(Expression<Func<T, bool>> predicate);
		Task DeleteRE11Async(int pid);
		Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetBymagnameIdAsync(string magname);
			

    }
}
