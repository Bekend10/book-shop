using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using realtime_service.Models;
using realtime_service.Services.Interfaces;

namespace realtime_service.Controllers
{
    [ApiController]
    [Route("api/v1/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMessageService _messageService;

        public MessagesController(INotificationService notificationService, IMessageService messageService)
        {
            _notificationService = notificationService;
            _messageService = messageService;
        }

        /// <summary>
        /// Gửi tin nhắn đến một người dùng cụ thể (vừa lưu DB, vừa gửi real-time).
        /// </summary>
        [HttpPost("send-mess-to-user")]
        public async Task<IActionResult> SendToUser([FromBody] CreateMessageModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _notificationService.SendToUserAsync(model);

            return Ok(new
            {
                status = "Sent",
                message = model.message_content,
                to = model.receiver_id
            });
        }

        /// <summary>
        /// Gửi broadcast đến tất cả người dùng
        /// </summary>
        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return BadRequest(new { error = "Message is required" });

            await _notificationService.BroadcastAsync(message);

            return Ok(new
            {
                status = "Broadcasted",
                message
            });
        }

        /// <summary>
        /// Lấy tin nhắn giữa 2 người
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversation([FromQuery] int senderId, [FromQuery] int receiverId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var messages = await _messageService.GetConversationAsync(senderId, receiverId, page, pageSize);
            return Ok(messages);
        }

        /// <summary>
        /// Lấy danh sách nhắn tin
        /// </summary>
        [HttpPost("my-conversations")]
        public async Task<IActionResult> GetMyAllConversations([FromBody] GetMyMessageModel model, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var conversations = await _messageService.GetMyAllConversationAysnc(model, page, pageSize);
            return Ok(conversations);
        }
        /// <summary>
        /// lấy tin nhắn theo id
        /// </summary>  
        [HttpGet("get-converstation-by-id")]
        public async Task<IActionResult> GetMessageById([FromQuery] int userId, [FromQuery] int messageId)
        {
            var message = await _messageService.GetMessageByIdAsync(userId, messageId);
            if (message == null)
            {
                return NotFound(new { error = "Message not found" });
            }
            return Ok(message);
        }
        /// <summary>
        /// Đánh dấu tất cả tin nhắn là đã đọc
        /// </summary>
        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead([FromBody] GetMyMessageModel model)
        {
            if (model.userId <= 0)
            {
                return BadRequest(new { error = "Invalid user ID" });
            }

            var result = await _messageService.MarkAllAsReadAsync(model.userId);
            return Ok(result);
        }

        /// <summary>
        /// Xoá đoạn tin nhắn
        /// </summary>
        [HttpDelete("delete-message")]
        public async Task<IActionResult> DeleteMessage([FromQuery] int messageId)
        {
            if (messageId <= 0)
            {
                return BadRequest(new { error = "Invalid message ID" });
            }

            var result = await _messageService.DeleteMessageAsync(messageId);
            if (!result)
            {
                return NotFound(new { error = "Message not found or could not be deleted" });
            }

            return Ok(new { status = "Deleted", messageId });
        }
        /// <summary>
        /// Xoá nhiều tin nhắn
        /// </summary>
        [HttpDelete("delete-all-conversation")]
        public async Task<IActionResult> DeleteAllConversation([FromBody] List<int> messageIds)
        {
            if (messageIds == null || !messageIds.Any())
            {
                return BadRequest(new { error = "Message IDs are required" });
            }

            var result = await _messageService.DeleteAllConversationAsync(messageIds);
            if (!result)
            {
                return NotFound(new { error = "Some messages not found or could not be deleted" });
            }

            return Ok(new { status = "Deleted", messageIds });
        }
    }
}
