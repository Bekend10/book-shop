using Hangfire;
using hangfire_service.Jobs;
using hangfire_service.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hangfire_service.Controllers
{
    [Route("api/v1/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        public JobsController(IBackgroundJobClient backgroundJobClient) => _backgroundJobClient = backgroundJobClient;

        /// <summary>
        /// gửi thông báo đến admin
        /// </summary>
        [HttpPost("send-admin-notification")]
        public IActionResult SendAdminNotification([FromBody] AdminNotificationModelCommand cmd)
        {
            _backgroundJobClient.Enqueue<NotificationJob>(job => job.Execute(cmd.order_id, cmd.message_content , cmd.receiver_user_id));
            return Ok(new { success = true });
        }
    }
}
