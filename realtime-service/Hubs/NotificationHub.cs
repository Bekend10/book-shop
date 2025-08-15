using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace realtime_service.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"];
            var role = httpContext?.Request.Query["role"];

            if (!string.IsNullOrEmpty(role) && role.ToString() == "admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
            }

            await Clients.All.SendAsync("UserOnline", userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"];
            await Clients.All.SendAsync("UserOffline", userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartTyping(string conversationId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Group($"conversation_{conversationId}")
                         .SendAsync("UserStartedTyping", userId, conversationId);
        }

        public async Task StopTyping(string conversationId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Group($"conversation_{conversationId}")
                         .SendAsync("UserStoppedTyping", userId, conversationId);
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public async Task SendNotificationToUser(string targetUserId, string message)
        {
            await Clients.Group($"user_{targetUserId}").SendAsync("ReceiveNotification", message);
        }

        public async Task SendNotificationToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendNotificationToAdmins(string message)
        {
            await Clients.Group("admin").SendAsync("ReceiveNotification", message);
        }

    }
}
