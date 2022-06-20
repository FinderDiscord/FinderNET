using FinderNET.Modules.Helpers.Enums;
using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class AddonsModel {
        [Key]
        public Int64 guildId { get; set; }
        public List<Addons> addons { get; set; } = new List<Addons>();
    }
}