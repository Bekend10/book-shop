using System.Net.Mail;
using book_shop.Helpers.EmailHelper;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
namespace book_shop.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly string senderEmail;
        private readonly string senderPassword;
        private readonly ILogger<EmailService> _logger;
        private readonly EmailTemplateLoader _templateLoader;


        public EmailService(IConfiguration configuration, ILogger<EmailService> logger , EmailTemplateLoader emailTemplateLoader)
        {
            var smtpSettings = configuration.GetSection("SmtpSettings");
            smtpServer = smtpSettings["SmtpServer"];
            smtpPort = int.Parse(smtpSettings["SmtpPort"]);
            senderEmail = smtpSettings["SenderEmail"];
            senderPassword = smtpSettings["SenderPassword"];
            _logger = logger;
            _templateLoader = emailTemplateLoader;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string name, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Book Shop", senderEmail));
                message.To.Add(new MailboxAddress(name, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, senderPassword);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi gửi email: " + ex.Message);
            }
        }

        public async Task<bool> SendResetPassword(string toEmail, string userName , string newPassword)
        {
            var bodyContent = _templateLoader.LoadTemplate("Account/ResetPassword.html", new Dictionary<string, string>
            {
                { "UserName", userName },
                { "newPassword", newPassword },
            });

            var finalBody = _templateLoader.WrapWithLayout(bodyContent);

            return await SendEmailAsync(toEmail, userName, "Khôi phục mật khẩu", finalBody);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var bodyContent = _templateLoader.LoadTemplate("Account/Welcome.html", new Dictionary<string, string>
            {
                { "UserName", userName },
                { "email", toEmail }
            });

            var finalBody = _templateLoader.WrapWithLayout(bodyContent);

            return await SendEmailAsync(toEmail, userName, "Chào mừng đến với Book Shop", finalBody);
        }
    }
}
