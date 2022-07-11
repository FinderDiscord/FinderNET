using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinderNET.Database {
    public class Repository<T> where T : class {
        protected readonly FinderDatabaseContext context;
        public Repository(FinderDatabaseContext _context) {
            context = _context;
        }
        
        public async Task<T?> Get(ulong id) {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetASync(ulong id, ulong id2) {
            return await context.Set<T>().FindAsync(id, id2);
        }

        public async Task<T?> GetAsync(ulong id, ulong id2, ulong id3) {
            return await context.Set<T>().FindAsync(id, id2, id3);
        }

        public async Task<IEnumerable<T>> GetAllAsync() {
            return await context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate) {
            return await context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity) {
            await context.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities) {
            await context.Set<T>().AddRangeAsync(entities);
        }

        public void Remove(T entity) {
            context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities) {
            context.Set<T>().RemoveRange(entities);
        }

        public void Dispose() {
            context.Dispose();
        }

        public int Save() {
            return context.SaveChanges();
        }

        public async Task<int> SaveAsync() {
            return await context.SaveChangesAsync();
        }
    }
}