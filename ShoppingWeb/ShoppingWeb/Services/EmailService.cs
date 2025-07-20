using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string username, string? confirmationLink)
        {
            string subject = "Welcome to website";
            string body = $@"
            <h2>Welcome!</h2>
            <p>Thanks for registering</p>";
            //<p><a href='{confirmationLink}'>Confirm Email</a></p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendEmailAsync(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["Email:Host"], int.Parse(_configuration["Email:Port"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string? userName)
        {
            try
            {
                var baseUrl = _configuration["App:FrontendBaseUrl"];
                var resetUrl = $"{baseUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(toEmail)}";


                var emailBody = $@"
                <h2>Password Reset Request</h2>
                <p>Hello {userName ?? "User"},</p>
                <p>You requested to reset your password. Click the link below to reset it:</p>
                <a href='{resetUrl}'>Reset Password</a>
                <p>This link will expire in 1 hour.</p>
                <p>If you didn't request this, please ignore this email.</p>
            ";

                // Implement your email sending logic here
                // Example with SMTP, SendGrid, etc.
                await SendEmailAsync(toEmail, "Password Reset Request", emailBody);

                _logger.LogInformation("Password reset email sent to: {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to: {Email}", toEmail);
                throw;
            }
        }

        public Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailWhenUserChangePasswordAsync(string toEmail, string token, string username)
        {
            throw new NotImplementedException();
        }
    }
}
