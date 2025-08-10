using AutoMapper;
using Microsoft.EntityFrameworkCore;
using realtime_service.Data;
using realtime_service.Entity;
using realtime_service.Models;
using realtime_service.Repositories.Interfaces;
using System;

namespace realtime_service.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly NotificationDbContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(NotificationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MessageModel>> GetMessagesAsync(int senderId, int receiverId, int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            var messages = await _context.Messages
                .Where(m =>
                    (m.sender_id == senderId && m.receiver_id == receiverId) ||
                    (m.sender_id == receiverId && m.receiver_id == senderId))
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<MessageModel>>(messages);
        }


        public async Task<MessageModel> AddMessageAsync(CreateMessageModel model)
        {
            var entity = _mapper.Map<Message>(model);
            await _context.Messages.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<MessageModel>(entity);
        }

        public async Task<List<MessageModel>> GetMyAllConversationAsync(int userId, int page, int pageSize)
        {
            var rawMessages = await _context.Messages
                .Where(m => m.sender_id == userId || m.receiver_id == userId)
                .ToListAsync();

            var latestMessages = rawMessages
                .Select(m => new
                {
                    Message = m,
                    PartnerId = m.sender_id == userId ? m.receiver_id : m.sender_id
                })
                .GroupBy(x => x.PartnerId)
                .Select(g => g
                    .OrderByDescending(x => x.Message.sent_at)
                    .First().Message)
                .OrderByDescending(m => m.sent_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return _mapper.Map<List<MessageModel>>(latestMessages);
        }

        public async Task<MessageModel> GetMessageByIdAsync(int messageId)
        {
            var message = await _context.Messages
                .Where(m => m.message_id == messageId)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                throw new ArgumentException("Message not found", nameof(messageId));
            }

            return _mapper.Map<MessageModel>(message);
        }

        public async Task<MessageModel> UpdateMessageAsync(int messageId, UpdateMessageModel model)
        {
            var message = _context.Messages.FirstOrDefault(m => m.message_id == messageId);
            try
            {
                if (message == null)
                {
                    throw new ArgumentException("Message not found", nameof(messageId));
                }

                if (model.message_content != null)
                {
                    message.message_content = model.message_content;
                }

                message.is_read = true;
                message.read_at = DateTime.UtcNow.AddHours(7);

                _context.Messages.Update(message);
                await _context.SaveChangesAsync();

                return await Task.FromResult(_mapper.Map<MessageModel>(message));
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the message", ex);
            }
        }

        public async Task<MessageModel> MarkAllAsRead(int userId)
        {
            try
            {
                var messages = await _context.Messages
                    .Where(m => m.receiver_id == userId && m.sender_id != userId && !m.is_read)
                    .ToListAsync();

                if (messages.Count == 0)
                {
                    throw new ArgumentException("No unread messages found for the given criteria", nameof(messages));
                }

                foreach (var message in messages)
                {
                    message.is_read = true;
                    message.read_at = DateTime.UtcNow;
                }

                _context.Messages.UpdateRange(messages);
                await _context.SaveChangesAsync();

                return _mapper.Map<MessageModel>(messages.Last());

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while marking messages as read", ex);
            }
        }

        public Task<bool> DeleteMessageAsync(int messageId)
        {
            try
            {
                var message = _context.Messages.FirstOrDefault(m => m.message_id == messageId);
                if (message == null)
                {
                    throw new ArgumentException("Message not found", nameof(messageId));
                }

                _context.Messages.Remove(message);
                return _context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the message", ex);
            }
        }

        public async Task<bool> DeleteAllConversation(List<int> messageIds)
        {
            try
            {
                var messages = _context.Messages
                    .Where(m => messageIds.Contains(m.message_id))
                    .ToList();
                if (messages.Count == 0)
                {
                    throw new ArgumentException("No messages found for the given IDs", nameof(messageIds));
                }
                _context.Messages.RemoveRange(messages);
                return await _context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting all conversations", ex);
            }
        }
    }
}
