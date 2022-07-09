using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class ItemInvModel {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public Int64 userId { get; set; }
        public List<Guid> itemIds { get; set; }
        
    }
}