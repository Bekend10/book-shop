using realtime_service.Models;

namespace realtime_service.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<List<MessageModel>> GetMessagesAsync(int senderId, int receiverId, int page, int pageSize);
        Task<List<MessageModel>> GetMyAllConversationAsync(int userId, int page, int pageSize);
        Task<MessageModel> GetMessageByIdAsync(int messageId);
        Task<MessageModel> AddMessageAsync(CreateMessageModel model);
        Task<MessageModel> MarkAllAsRead(int userId);
        Task<MessageModel> UpdateMessageAsync(int messageId, UpdateMessageModel model);
        Task<bool> DeleteMessageAsync(int messageId);
        Task<bool> DeleteAllConversation(List<int> messageIds);
    }
}
