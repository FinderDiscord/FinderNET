using System;
using Discord;
using Discord.Commands;
using FinderNET.Database;

namespace FinderNet.Modules {
    public class EconomyModule : ModuleBase {
        public EconomyModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
    }
}
