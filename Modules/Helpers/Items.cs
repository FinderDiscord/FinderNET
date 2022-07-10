using FinderNET.Modules.Helpers.Enums;

namespace FinderNET.Modules.Helpers {
    public class ItemsRoot {
        public List<Items> Items { get; set; }
    }
    public class Items {
        public Guid Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool tradeable { get; set; }
        public bool buyable { get; set; }
        public bool sellable { get; set; }
        public ItemRarity rarity { get; set; }
        public int buyPrice { get; set; }
        public int sellPrice { get; set; }
    }
}