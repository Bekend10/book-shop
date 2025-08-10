using Microsoft.AspNetCore.SignalR;
using realtime_service.Hubs;
using realtime_service.Entity;
using realtime_service.Services.Interfaces;
using realtime_service.Models;

namespace realtime_service.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMessageService _messageService;

        public NotificationService(IHubContext<NotificationHub> hubContext, IMessageService messageService)
        {
            _hubContext = hubContext;
            _messageService = messageService;
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
    }
}
