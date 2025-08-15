using realtime_service.Entity;

namespace realtime_service.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);
        Task AddAsync(Notification notification);
        Task<Notification?> GetByIdAsync(int id);
        Task UpdateAsync(Notification notification);
        Task MarkAllNotification(int userId);
        Task MarkNotificationById(int id);
        Task<int> CountNumberOfNotiUnRead(int userId);
        Task DeleteMarkById(int id);
        Task DeleteAllMarkByUserId(int userId);
    }
}
