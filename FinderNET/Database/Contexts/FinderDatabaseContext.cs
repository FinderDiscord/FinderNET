using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;


namespace FinderNET.Database {
    public class FinderDatabaseContext: DbContext {       
        public IServiceProvider services { get; }
        private readonly IConfiguration configuration;
        public FinderDatabaseContext(DbContextOptions<FinderDatabaseContext> options, IConfiguration _configuration, IServiceProvider _services) : base(options) {
            configuration = _configuration; 
            services = _services;
        }
            
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }

        public Task<GuildConfig> GetGuildConfigAsync(ulong guildId) => GuildConfigurations.GetOrCreateObjectAsync(guildId);
    }
}