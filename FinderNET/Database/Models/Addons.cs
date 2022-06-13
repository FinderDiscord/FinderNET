using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class Addons {
        [Key]
        public Int64 guildId { get; set; }
        public List<string> addons { get; set; } = new List<string>();
    }
}