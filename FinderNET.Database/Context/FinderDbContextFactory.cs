using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinderNET.Database.Context {
    public class FinderDbContextFactory : IDesignTimeDbContextFactory<FinderDbContext> {
        public FinderDbContext CreateDbContext(string[] args) {
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true).Build();
            DbContextOptionsBuilder? optionsBuilder = new DbContextOptionsBuilder().UseMySql(configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(8, 0, 28)));
            return new FinderDbContext(optionsBuilder.Options);
        }
    }
}