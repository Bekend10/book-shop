namespace hangfire_service.Services.Interfaces
{
    public interface INotificationService
    {
        void SendNotification(int orderId, string message , int receiverUserId);
    }
}
