using Discord.Interactions;
using FinderNET.Database;

namespace FinderNET.Modules {
    public class ModuleBase : InteractionModuleBase<SocketInteractionContext> {
        public readonly DataAccessLayer dataAccessLayer;
        public ModuleBase(DataAccessLayer dataAccessLayer) {
            this.dataAccessLayer = dataAccessLayer;
        }
    }
}
