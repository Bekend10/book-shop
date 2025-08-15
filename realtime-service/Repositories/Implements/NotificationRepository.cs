using Microsoft.EntityFrameworkCore;
using realtime_service.Data;
using realtime_service.Entity;
using realtime_service.Repositories.Interfaces;

namespace realtime_service.Repositories.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _db;

        public NotificationRepository(NotificationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId)
        {
            return await _db.Notifications
                .Where(n => n.user_id == userId)
                .OrderByDescending(n => n.created_at)
                .ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _db.Notifications.FindAsync(id);
        }

        public async Task UpdateAsync(Notification notification)
        {
            _db.Notifications.Update(notification);
            await _db.SaveChangesAsync();
        }

        public async Task MarkAllNotification(int userId)
        {
            var notifications = await _db.Notifications
                .Where(n => n.user_id == userId)
                .ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            _db.Notifications.UpdateRange(notifications);
            await _db.SaveChangesAsync();
        }

        public async Task MarkNotificationById(int id)
        {
            var notification = await _db.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _db.Notifications.Update(notification);
                await _db.SaveChangesAsync();
            }
        }

        public Task<int> CountNumberOfNotiUnRead(int userId)
        {
            return _db.Notifications
                .CountAsync(n => n.user_id == userId && !n.IsRead);
        }

        public async Task DeleteMarkById(int id)
        {
            var notification = await _db.Notifications.FindAsync(id);
            if (notification == null)
                throw new Exception("Notification not found");
             _db.Notifications.Remove(notification);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAllMarkByUserId(int userId)
        {
            var notifications = await _db.Notifications
                .Where(n => n.user_id == userId)
                .ToListAsync();
            if (notifications == null || notifications.Count == 0)
            {
                throw new Exception("No notifications found for this user");
            }
            _db.Notifications.RemoveRange(notifications);
            await _db.SaveChangesAsync();
        }
    }

}
