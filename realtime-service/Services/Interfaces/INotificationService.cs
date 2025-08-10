using realtime_service.Entity;
using realtime_service.Models;

namespace realtime_service.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(CreateMessageModel model);
        Task BroadcastAsync(string message);
    }
}
