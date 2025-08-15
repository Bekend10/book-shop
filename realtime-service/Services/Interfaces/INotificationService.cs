using realtime_service.Entity;
using realtime_service.Models;

namespace realtime_service.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(CreateMessageModel model);
        Task BroadcastAsync(string message);
        Task SendOrderNotificationToAdminAsync(int orderId, int userId);
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task AddNotificationAsync(int userId, string message);
        Task MarkAsReadAsync(int id);
        Task MarkAllNotificationsAsync(int userId);
        Task<int> CountNumberOfNotiUnReadAsync(int userId);
        Task DeleteMarkById(int id);
        Task DeleteAllMarkByUserId(int userId);
    }
}
