using Microsoft.AspNetCore.Mvc;
using realtime_service.Repositories.Interfaces;
using realtime_service.Services.Interfaces;

namespace realtime_service.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách thông báo của người dùng
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetNotificationsByUserId(int userId)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Đánh dấu thông báo là đã đọc
        /// </summary>
        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { message = "Thông báo đã đuợc đọc" });
        }

        /// <summary>
        /// Đánh dấu toàn bộ thông báo là đã đọc
        /// </summary>
        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            await _notificationService.MarkAllNotificationsAsync(userId);
            return Ok(new { message = "Tất cả thông báo đã được đánh dấu là đã đọc" });
        }
        /// <summary>
        /// Xoá toàn bộ thông báo
        /// </summary>
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllNotifications(int userId)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            foreach (var notification in notifications)
            {
                await _notificationService.DeleteAllMarkByUserId(userId);
            }
            return Ok(new { message = "Tất cả thông báo đã được xoá" });
        }

        /// <summary>
        /// Xoá thông báo theo ID
        /// </summary> 
        [HttpDelete("delete-notification-by-id")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _notificationService.GetUserNotificationsAsync(id);
            if (notification == null)
            {
                return NotFound(new { message = "Thông báo không tồn tại" });
            }
            await _notificationService.DeleteMarkById(id);
            return Ok(new { message = "Thông báo đã được xoá" });
        }

        /// <summary>
        /// Đếm số lượng thông báo chưa đọc
        /// </summary>
        [HttpGet("count-unread")]
        public async Task<IActionResult> CountUnreadNotifications(int userId)
        {
            var count = await _notificationService.CountNumberOfNotiUnReadAsync(userId);
            return Ok(new { count });
        }
    }
}
