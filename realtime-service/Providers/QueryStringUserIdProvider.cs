using Microsoft.AspNetCore.SignalR;

namespace realtime_service.Providers
{
    public class QueryStringUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext()?.Request.Query["userId"];
        }
    }
}
