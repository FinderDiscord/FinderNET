using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class SettingsModel {
        [Key]
        public Int64 guildId { get; set; }
        public List<IDictionary<string, string>> settings { get; set; } = new List<IDictionary<string, string>>();
    }
}