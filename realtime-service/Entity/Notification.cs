using System.ComponentModel.DataAnnotations;

namespace realtime_service.Entity
{
    public class Notification
    {
        [Key]
        public int notification_id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public string message_content { get; set; }
        public DateTime created_at { get; set; }
        public bool IsRead { get; set; }
    }
}
