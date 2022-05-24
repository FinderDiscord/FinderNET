using FinderNET.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FinderNET.Database {
    public class FinderDbContext : DbContext {
        public FinderDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Guild> Guilds { get; set; }
    }
}