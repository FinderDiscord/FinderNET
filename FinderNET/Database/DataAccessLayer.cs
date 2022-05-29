using Discord;
using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinderNET.Database
{
    class DataAccessLayer
    {
        private readonly IDbContextFactory<FinderDatabaseContext> contextFactory;

        public DataAccessLayer(IDbContextFactory<FinderDatabaseContext> _contextFactory)
        {
            contextFactory = _contextFactory;
        }

        public List<string> GetAddons(Int64 id)
        {
            using var context = contextFactory.CreateDbContext();
            var addons = context.addons.Find(id);
            if (addons == null)
            {
                addons = context.Add(new Addons { Id = id, addons = new List<string>() }).Entity;
                context.SaveChanges();
            }
            return addons.addons;
        }

        public async Task SetAddons(Int64 id, List<string> addons)
        {
            using var context = contextFactory.CreateDbContext();
            var addonsList = await context.addons.FindAsync(id);
            if (addonsList != null)
            {
                context.Entry(new Addons() { Id = id, addons = addons }).Property(x => x.addons).IsModified = true;
            }
            else
            {
                context.Add(new Addons { Id = id, addons = addons });
            }
            await context.SaveChangesAsync();
        }

        public async Task AddAddons(Int64 id, string addon)
        {
            using var context = contextFactory.CreateDbContext();
            var addons = await context.addons.FindAsync(id);
            if (addons != null)
            {
                addons.addons.Add(addon);
            }
            else
            {
                context.Add(new Addons { Id = id, addons = new List<string> { addon } });
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllAddons(Int64 id)
        {
            using var context = contextFactory.CreateDbContext();
            var addons = await context.addons.FindAsync(id);
            if (addons == null) return;
            context.Remove(addons);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAddon(Int64 id, string addon)
        {
            using var context = contextFactory.CreateDbContext();
            var addons = await context.addons.FindAsync(id);
            if (addons == null) return;
            if (!addons.addons.Contains(addon)) return;
            addons.addons.Remove(addon);
            context.Entry(new Addons() { Id = id, addons = addons.addons }).Property(x => x.addons).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveAddons(Int64 id, List<string> addons)
        {
            using var context = contextFactory.CreateDbContext();
            var addonsList = await context.addons.FindAsync(id);
            if (addonsList == null) return;
            foreach (var addon in addons)
            {
                if (!addonsList.addons.Contains(addon)) continue;
                addonsList.addons.Remove(addon);
            }
            context.Entry(new Addons() { Id = id, addons = addonsList.addons }).Property(x => x.addons).IsModified = true;
            await context.SaveChangesAsync();
        }
    }
}