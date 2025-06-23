namespace book_shop.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string name, string subject, string body);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendResetPassword(string toEmail , string userName , string newPassword); 
    }
}
