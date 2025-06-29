namespace ShoppingWeb.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
        Task SendWelcomeEmailAsync(string toEmail,string username, string? confirmationLink);
        Task SendPasswordResetEmailAsync(string toEmail, string token, string username);
        Task SendEmailWhenUserChangePasswordAsync(string toEmail, string token, string username);
    }
}
