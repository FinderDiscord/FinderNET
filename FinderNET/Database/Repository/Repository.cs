using FinderNET.Database.Contexts;
using System.Linq.Expressions;

namespace FinderNET.Database.Repositories {
    public class Repository<T> where T : class {
        protected readonly FinderDatabaseContext context;
        public Repository(FinderDatabaseContext _context) {
            context = _context;
        }
        
        public T Get(ulong id) {
            return context.Set<T>().Find(id);
        }

        public T Get(ulong id, ulong id2) {
            return context.Set<T>().Find(id, id2);
        }

        public T Get(ulong id, ulong id2, ulong id3) {
            return context.Set<T>().Find(id, id2, id3);
        }

        public IEnumerable<T> GetAll() {
            return context.Set<T>().ToList();
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate) {
            return context.Set<T>().Where(predicate);
        }

        public void Add(T entity) {
            context.Set<T>().Add(entity);
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