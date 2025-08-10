using System.ComponentModel.DataAnnotations;

namespace realtime_service.Models
{
    public class MessageModel
    {
        public int message_id { get; set; }
        public int sender_id { get; set; }
        public int receiver_id { get; set; }
        public string message_content { get; set; } = string.Empty;
        public DateTime sent_at { get; set; }
        public bool is_read { get; set; } = false;
        public DateTime? read_at { get; set; }
        public string? sender_snapshot_name { get; set; }
        public string? receiver_snapshot_name { get; set; }
    }

    public class CreateMessageModel
    {
        public int sender_id { get; set; }
        public int receiver_id { get; set; }
        public string message_content { get; set; } = string.Empty;
        public DateTime sent_at { get; set; } = DateTime.UtcNow.AddHours(7);
        public string? sender_snapshot_name { get; set; }
        public string? receiver_snapshot_name { get; set; }
    }

    public class UpdateMessageModel
    {
        public string? message_content { get; set; }
        public bool? is_read { get; set; }
        public DateTime? read_at { get; set; }
    }

    public class GetMyMessageModel
    {
        public int userId { get; set; }
    }
}
