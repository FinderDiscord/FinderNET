using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinderNET.Database
{
    public class Addons
    {
        public Int64 Id { get; set; }
        public List<string> addons { get; set; }
    }
}