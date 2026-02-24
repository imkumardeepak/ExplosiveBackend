using Peso_Baseed_Barcode_Printing_System_API.Interface;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Peso_Baseed_Barcode_Printing_System_API.DBContext;
using Peso_Baseed_Barcode_Printing_System_API.Repositorys;
using System.Linq.Expressions;

namespace Peso_Baseed_Barcode_Printing_System_API.Repositories
{
    /// <summary>
    /// Generic Repository with backward-compatible constructor support
    /// RECOMMENDATION: Migrate to single-context constructor for new repositories
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        // =====================================================
        // RECOMMENDED: Single context constructor (optimized)
        // =====================================================
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        // =====================================================
        // BACKWARD COMPATIBLE: 8-context constructor (legacy)
        // TODO: Gradually migrate repositories to single-context pattern
        // =====================================================
        public GenericRepository(
            ApplicationDbContext context, 
            ApplicationDbContext context1, 
            ApplicationDbContext context2, 
            ApplicationDbContext context3, 
            ApplicationDbContext context4, 
            ApplicationDbContext context5, 
            ApplicationDbContext context6, 
            ApplicationDbContext context7) : this(context)
        {
            // Legacy support - all operations use _context (first context)
            // Other contexts are ignored but accepted for backward compatibility
        }

        // =====================================================
        // READ OPERATIONS (Use AsNoTracking for performance)
        // =====================================================

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllNAsNoTrackingAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetBydaraIdAsync(string l1barcode)
        {
            return await _dbSet.FindAsync(l1barcode);
        }

        public async Task<T?> GetBymagnameIdAsync(string magname)
        {
            return await _dbSet.FindAsync(magname);
        }

        public async Task<T?> GetByre11IdAsync(int pid)
        {
            return await _dbSet.FindAsync(pid);
        }

        public async Task<IEnumerable<T>> FindByNameAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

        public IQueryable<T> GetAll() => _dbSet.AsQueryable();

        // =====================================================
        // WRITE OPERATIONS
        // =====================================================

        public async Task<T?> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            _context.ChangeTracker.Clear();
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.ChangeTracker.Clear();
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Deletel1Async(string l1barcode)
        {
            var entity = await GetBydaraIdAsync(l1barcode);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRE11Async(int pid)
        {
            var entity = await GetByre11IdAsync(pid);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteRangeAsync(Expression<Func<T, bool>> predicate)
        {
            var items = _dbSet.Where(predicate);
            _dbSet.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        // =====================================================
        // BULK OPERATIONS
        // =====================================================

        public async Task BulkInsertAsync(IEnumerable<T> entities)
        {
            await _context.BulkInsertAsync(entities.ToList());
        }

        public async Task BulkRemoveAsync(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                return;

            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }


    }
}
