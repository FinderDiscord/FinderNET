using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database {
    public class Leveling {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public Int64 userId { get; set; }
        public int level { get; set; }
        public int exp { get; set; }
    }
}