using FinderNET.Modules.Helpers;
using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class ItemsModel {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public Int64 userId { get; set; }
        public List<Items> items { get; set; } = new List<Items>();
    }
}