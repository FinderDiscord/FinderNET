using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class SettingsModel {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public string key { get; set; }
        public string value { get; set; }
    }
}