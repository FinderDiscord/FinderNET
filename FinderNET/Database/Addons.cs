using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Finder.Database.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinderNET.Database {
    public class GuildConfig : IAsyncCreatable {
        public List<string> addons { get; set; }
            
        public ValueTask OnCreatingAsync(IConfiguration config)
        {
            addons = new List<string>;
            return default;
        }
    }
}