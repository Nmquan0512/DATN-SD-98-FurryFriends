using FurryFriends.API.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using FurryFriends.API.Services.IServices;
using Microsoft.Extensions.Logging;

namespace FurryFriends.API.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailService> _logger;

        // Inject IOptions<MailSettings> để đọc cấu hình từ appsettings.json
        public MailService(IOptions<MailSettings> mailSettings, ILogger<MailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                _logger.LogInformation($"Bắt đầu gửi email đến: {toEmail}");
                _logger.LogInformation($"Cấu hình SMTP: {_mailSettings.Host}:{_mailSettings.Port}");
                _logger.LogInformation($"Email gửi từ: {_mailSettings.Mail}");

                var email = new MimeMessage();
                email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = body; // Thiết lập nội dung email là HTML
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                
                _logger.LogInformation("Đang kết nối SMTP...");
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                
                _logger.LogInformation("Đang xác thực...");
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                
                _logger.LogInformation("Đang gửi email...");
                await smtp.SendAsync(email);
                
                _logger.LogInformation("Đang ngắt kết nối...");
                await smtp.DisconnectAsync(true);
                
                _logger.LogInformation($"Email đã được gửi thành công đến: {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi email đến {toEmail}: {ex.Message}");
                throw;
            }
        }
    }
}
