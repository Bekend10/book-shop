using realtime_service.Models;
using realtime_service.Repositories.Interfaces;
using realtime_service.Services.External;
using realtime_service.Services.Interfaces;

namespace realtime_service.Services.Implements
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IUserService _userService;

        public MessageService(IMessageRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        public Task<bool> DeleteAllConversationAsync(List<int> messageIds)
        {
            if (messageIds == null || !messageIds.Any())
            {
                throw new ArgumentException("Message IDs cannot be null or empty", nameof(messageIds));
            }
            return _repository.DeleteAllConversation(messageIds);
        }

        public Task<bool> DeleteMessageAsync(int messageId)
        {
            return _repository.DeleteMessageAsync(messageId);
        }

        public async Task<List<MessageModel>> GetConversationAsync(int senderId, int receiverId, int page, int pageSize)
        {
            return await _repository.GetMessagesAsync(senderId, receiverId, page, pageSize);
        }

        public async Task<MessageModel> GetMessageByIdAsync(int userId, int messageId)
        {
            var res = await _repository.GetMessageByIdAsync(messageId);
            if (res == null)
            {
                throw new ArgumentException("Message not found", nameof(messageId));
            }

            if (res.receiver_id == userId)
            {
                res.is_read = true;
                res.read_at = DateTime.UtcNow;
                await _repository.UpdateMessageAsync(messageId, new UpdateMessageModel
                {
                    is_read = res.is_read,
                    read_at = res.read_at
                });
            }
            return res;
        }

        public async Task<List<MessageModel>> GetMyAllConversationAysnc(GetMyMessageModel model, int page, int pageSize)
        {
            var user = await _userService.GetUserByIdAsync(model.userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(model.userId));
            }
            return await _repository.GetMyAllConversationAsync(model.userId, page, pageSize);
        }

        public Task<MessageModel> MarkAllAsReadAsync(int userId)
        {
            return _repository.MarkAllAsRead(userId);
        }

        public async Task<MessageModel> SendMessageAsync(CreateMessageModel message)
        {
            var senderIsExisting = await _userService.GetUserByIdAsync(message.sender_id);
            if (senderIsExisting == null)
            {
                throw new ArgumentException("Sender user not found", nameof(message.sender_id));
            }
            var userIsExisting = await _userService.GetUserByIdAsync(message.receiver_id);
            if (userIsExisting == null)
            {
                throw new ArgumentException("Receiver user not found", nameof(message.receiver_id));
            }

            if (message.sender_id == message.receiver_id)
            {
                throw new ArgumentException("Sender and receiver cannot be the same", nameof(message));
            }
            var senderInfor = senderIsExisting.Data;
            message.sender_snapshot_name = senderInfor.Full_Name;
            var receiverInfor = userIsExisting.Data;
            message.receiver_snapshot_name = receiverInfor.Full_Name;
            return await _repository.AddMessageAsync(message);
        }

        public Task<MessageModel> UpdateMessageAsync(int messageId, UpdateMessageModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "Update model cannot be null");
            }
            return _repository.UpdateMessageAsync(messageId, model);
        }
    }
}
