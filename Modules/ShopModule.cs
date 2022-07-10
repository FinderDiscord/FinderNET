using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using FinderNET.Database.Models;
using FinderNET.Database.Repositories;
using FinderNET.Modules.Helpers;
using Newtonsoft.Json;

namespace FinderNET.Modules {
    [Group("shop", "The shop commands to buy items.")]
    public class ShopModule : InteractionModuleBase<ShardedInteractionContext> {
        private readonly ItemInvRepository itemsRepository;
        private readonly EconomyRepository economyRepository;
        public ShopModule(ItemInvRepository _itemsRepository, EconomyRepository _economyRepository) {
            itemsRepository = _itemsRepository;
            economyRepository = _economyRepository;
        }
        public static ItemsRoot? itemsroot = JsonConvert.DeserializeObject<ItemsRoot>(File.ReadAllText(@"items.json"));
        [SlashCommand("buy", "Buy an item from the shop.")]
        public async Task BuyCommand([Autocomplete(typeof(ShopAutocompleteHandler))] string item) {
            Guid itemId = Guid.Parse(item);
            if (itemsroot == null) {
                await ReplyAsync("Could not load items.");
                return;
            }
            var itemToBuy = itemsroot.Items.Find(x => x.Id == itemId);
            if (!itemToBuy.buyable) {
                await RespondAsync("This item is not buyable.");
                return;
            }
            if ((await economyRepository.GetEconomyAsync(Context.Guild.Id, Context.User.Id)).money < itemToBuy.buyPrice) {
                await RespondAsync("You do not have enough money to buy this item.");
                return;
            }
            await economyRepository.AddEconomyAsync(Context.Guild.Id, Context.User.Id, -itemToBuy.buyPrice, 0);
            await itemsRepository.AddItemAsync(Context.Guild.Id, Context.User.Id, itemId);
            await economyRepository.SaveAsync();
            await itemsRepository.SaveAsync();
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "You have purchased an item!",
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = "Item",
                        Value = itemToBuy.name,
                    }
                },
                Color = Color.Green
            }.Build());
        }
        
        [SlashCommand("sell", "Sell an item to the shop.")]
        public async Task SellCommand([Autocomplete(typeof(ShopAutocompleteHandler))] string item) {
            Guid itemId = Guid.Parse(item);
            if (itemsroot == null) {
                await ReplyAsync("Could not load items.");
                return;
            }
            var itemToSell = itemsroot.Items.Find(x => x.Id == itemId);
            if (!itemToSell.sellable) {
                await RespondAsync("This item is not sellable.");
                return;
            }
            if (!await itemsRepository.ItemExistsAsync(Context.Guild.Id, Context.User.Id, itemId)) {
                await RespondAsync("You do not have this item.");
                return;
            }
            await economyRepository.AddEconomyAsync(Context.Guild.Id, Context.User.Id, itemToSell.sellPrice, 0);
            await itemsRepository.RemoveItemAsync(Context.Guild.Id, Context.User.Id, itemId);
            await economyRepository.SaveAsync();
            await itemsRepository.SaveAsync();
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "You have sold an item!",
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = itemToSell.name,
                        Value = "For " + itemToSell.sellPrice,
                    }
                },
                Color = Color.Green
            }.Build());
        }
    }

    public class InventoryModule : InteractionModuleBase<ShardedInteractionContext> {
        private readonly ItemInvRepository itemsRepository;
        private readonly EconomyRepository economyRepository;
        public InventoryModule(ItemInvRepository _itemsRepository, EconomyRepository _economyRepository) {
            itemsRepository = _itemsRepository;
            economyRepository = _economyRepository;
        }
        public static ItemsRoot? itemsroot = JsonConvert.DeserializeObject<ItemsRoot>(File.ReadAllText(@"items.json"));

        [SlashCommand("inventory", "View your inventory.")]
        public async Task InventoryCommand() {
            var items = await itemsRepository.GetItemsAsync((long)Context.Guild.Id, (long)Context.User.Id);
            if (items == null || items.itemIds.Count == 0) {
                await RespondAsync("You do not have any items.");
                return;
            }
            var embed = new EmbedBuilder() {
                Title = "Your inventory",
                Color = Color.Green
            };
            foreach (var item in items.itemIds) {
                var itemToBuy = itemsroot.Items.Find(x => x.Id == item);
                var amount = items.itemIds.Count(x => x == item);
                if (embed.Fields.Find(x => x.Name.Substring(0, itemToBuy.name.Length) == itemToBuy.name) == null) {
                    embed.AddField(itemToBuy.name + " x" + amount, itemToBuy.description);
                }
            }
            await RespondAsync(embed: embed.Build());
        }
    }
    public class ShopAutocompleteHandler : AutocompleteHandler {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services) {
            IEnumerable<AutocompleteResult> results = new List<AutocompleteResult>();
            if (ShopModule.itemsroot == null) {
                return Task.FromResult(AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "Could not load items."));
            }
            results = ShopModule.itemsroot.Items.Aggregate(results, (current, i) => current.Append(new AutocompleteResult(i.name, i.Id.ToString())));
            return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
        }
    }
    
}