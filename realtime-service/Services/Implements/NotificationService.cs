using Microsoft.AspNetCore.SignalR;
using realtime_service.Entity;
using realtime_service.Hubs;
using realtime_service.Models;
using realtime_service.Repositories.Interfaces;
using realtime_service.Services.Interfaces;

namespace realtime_service.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMessageService _messageService;
        private readonly INotificationRepository _repo;

        public NotificationService(IHubContext<NotificationHub> hubContext, IMessageService messageService, INotificationRepository repo)
        {
            _hubContext = hubContext;
            _messageService = messageService;
            _repo = repo;
        }

        public async Task SendToUserAsync(CreateMessageModel model)
        {
            var savedMessage = await _messageService.SendMessageAsync(model);

            // Gửi tin nhắn đến người nhận qua SignalR
            await _hubContext.Clients.Group($"user_{model.receiver_id}")
                                                 .SendAsync("ReceiveMessage", savedMessage.message_content);

            // Gửi lại cho sender để cập nhật giao diện
            await _hubContext.Clients.Group($"user_{model.sender_id}")
                                     .SendAsync("MessageSent", savedMessage.message_content);
        }

        public async Task BroadcastAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendOrderNotificationToAdminAsync(int orderId, int userId)
        {
            // Gửi cho admin
            await _hubContext.Clients.Group("admin").SendAsync("ReceiveOrderNotification", orderId, userId);

            // Gửi riêng cho khách hàng
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveOrderNotification", orderId, userId);
        }
        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task AddNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                user_id = userId,
                message_content = message,
                created_at = DateTime.UtcNow,
                IsRead = false
            };

            await _repo.AddAsync(notification);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null)
                throw new Exception("Notification not found");

            await _repo.MarkNotificationById(id);
        }

        public async Task MarkAllNotificationsAsync(int userId)
        {
            await _repo.MarkAllNotification(userId);
        }

        public Task<int> CountNumberOfNotiUnReadAsync(int userId)
        {
            return _repo.CountNumberOfNotiUnRead(userId);
        }

        public Task DeleteMarkById(int id)
        {
            return _repo.DeleteMarkById(id);
        }

        public Task DeleteAllMarkByUserId(int userId)
        {
            return _repo.DeleteAllMarkByUserId(userId);
        }
    }
}
