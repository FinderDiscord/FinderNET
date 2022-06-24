using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using FinderNET.Database.Repositories;

namespace FinderNET.Modules {
    [Group("shop", "The shop commands to buy items.")]
    public class ShopModule : InteractionModuleBase<ShardedInteractionContext> {
        private readonly ItemsRepository itemsRepository;
        public ShopModule(ItemsRepository _itemsRepository) {
            itemsRepository = _itemsRepository;
        }
        [SlashCommand("buy", "Buy an item from the shop.")]
        public async Task BuyCommand([Autocomplete(typeof(ShopAutocompleteHandler))] string parameterWithAutocompletion) {
            await RespondAsync("Not Implemented.");
        }
        public class ShopAutocompleteHandler : AutocompleteHandler {
            public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services) {
                IEnumerable<AutocompleteResult> results = new [] {
                    new AutocompleteResult("Name1", "value111"),
                    new AutocompleteResult("Name2", "value2")
                };
                
                return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
            }
        }
    }
    
}