namespace hangfire_service.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        void QueueNotification(int orderId, string message , int receiverUserId);
    }
}
