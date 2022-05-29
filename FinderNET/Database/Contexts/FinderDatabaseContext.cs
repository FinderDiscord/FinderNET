using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;


namespace FinderNET.Database.Contexts
{
    public class FinderDatabaseContext : DbContext
    {
        public FinderDatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Addons> addons { get; set; }
    }
}