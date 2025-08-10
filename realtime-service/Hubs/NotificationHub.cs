using Microsoft.AspNetCore.SignalR;

namespace realtime_service.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Gắn UserId để gọi Clients.User(userId)
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

                await Clients.All.SendAsync("UserOnline", userId);

            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"];
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");

                await Clients.All.SendAsync("UserOffline", userId);
            }

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
    }
}
