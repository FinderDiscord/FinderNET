using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database {
    public class Settings {
        [Key]
        public Int64 guildId { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
}