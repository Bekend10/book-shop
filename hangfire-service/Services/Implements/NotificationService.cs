using hangfire_service.Repositories.Interfaces;
using hangfire_service.Services.Interfaces;

namespace hangfire_service.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public void SendNotification(int orderId, string message, int receiverUserId)
        {
            _repo.QueueNotification(orderId, message , receiverUserId);
        }
    }
}
