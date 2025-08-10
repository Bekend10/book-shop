using System.ComponentModel.DataAnnotations;

namespace realtime_service.Entity
{
    public class Message
    {
        [Key]
        public int message_id { get; set; }
        [Required]
        public int sender_id { get; set; }
        [Required]
        public int receiver_id { get; set; }
        [Required]
        public string message_content { get; set; } = string.Empty;
        public DateTime sent_at { get; set; } = DateTime.UtcNow;
        public bool is_read { get; set; } = false;
        public DateTime? read_at { get; set; }
        public string? sender_snapshot_name { get; set; }
        public string? receiver_snapshot_name { get; set; }
    }
}
