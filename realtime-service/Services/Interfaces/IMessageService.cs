using realtime_service.Models;

namespace realtime_service.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageModel>> GetConversationAsync(int senderId, int receiverId, int page, int pageSize);
        Task<List<MessageModel>> GetMyAllConversationAysnc(GetMyMessageModel model , int page, int pageSize);
        Task<MessageModel> GetMessageByIdAsync(int userId ,int messageId);
        Task<MessageModel> SendMessageAsync(CreateMessageModel message);
        Task<MessageModel> MarkAllAsReadAsync(int userId);
        Task<MessageModel> UpdateMessageAsync(int messageId, UpdateMessageModel model);
        Task<bool> DeleteMessageAsync(int messageId);
        Task<bool> DeleteAllConversationAsync(List<int> messageIds);
    }
}
