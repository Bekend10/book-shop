using hangfire_service.Services.Interfaces;

namespace hangfire_service.Jobs
{
    public class NotificationJob
    {
        private readonly INotificationService _service;

        public NotificationJob(INotificationService service)
        {
            _service = service;
        }

        public void Execute(int orderId, string message , int receiverUserId)
        {
            _service.SendNotification(orderId, message , receiverUserId);
        }
    }
}
