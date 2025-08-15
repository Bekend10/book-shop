namespace realtime_service.Models
{
    public class AdminNotificationModelCommand
    {
        public int order_id { get; set; }
        public string message_content { get; set; }
        public int receiver_user_id { get; set; }
    }
}
