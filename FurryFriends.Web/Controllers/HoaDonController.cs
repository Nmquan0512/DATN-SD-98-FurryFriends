using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace FurryFriends.Web.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IConfiguration _configuration;

        public HoaDonController(IHoaDonService hoaDonService, IConfiguration configuration)
        {
            _hoaDonService = hoaDonService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailInvoice(Guid id)
        {
            try
            {
                // Lấy thông tin hóa đơn
                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(id);
                if (hoaDon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                // Kiểm tra email khách hàng
                if (string.IsNullOrEmpty(hoaDon.EmailCuaKhachHang))
                {
                    return Json(new { success = false, message = "Khách hàng chưa cung cấp email" });
                }

                // Tạo PDF hóa đơn
                var pdfBytes = await _hoaDonService.ExportHoaDonToPdfAsync(id);

                // Gửi email
                await SendInvoiceEmailAsync(hoaDon.EmailCuaKhachHang, hoaDon.TenCuaKhachHang, 
                    id.ToString().Substring(0, 8).ToUpper(), pdfBytes);

                return Json(new { success = true, message = "Hóa đơn đã được gửi thành công" });
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error sending invoice email: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi gửi email" });
            }
        }

        private async Task SendInvoiceEmailAsync(string toEmail, string customerName, string invoiceNumber, byte[] pdfAttachment)
        {
            try
            {
                // Email configuration - you should store these in appsettings.json
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "your-email@gmail.com";
                var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? "your-app-password";
                var senderName = _configuration["EmailSettings:SenderName"] ?? "FurryFriends Store";

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = $"Hóa đơn #{invoiceNumber} - FurryFriends Store",
                    Body = CreateEmailBody(customerName, invoiceNumber),
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                // Attach PDF
                if (pdfAttachment != null && pdfAttachment.Length > 0)
                {
                    var attachment = new Attachment(new MemoryStream(pdfAttachment), $"HoaDon_{invoiceNumber}.pdf", "application/pdf");
                    message.Attachments.Add(attachment);
                }

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }

        private string CreateEmailBody(string customerName, string invoiceNumber)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .logo {{ font-size: 24px; font-weight: bold; margin-bottom: 10px; }}
                        .footer {{ margin-top: 30px; text-align: center; color: #666; font-size: 14px; }}
                        .highlight {{ color: #667eea; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>🐾 FurryFriends</div>
                            <h2>Cảm ơn bạn đã đặt hàng!</h2>
                        </div>
                        <div class='content'>
                            <p>Xin chào <strong>{customerName}</strong>,</p>
                            
                            <p>Cảm ơn bạn đã tin tưởng và mua sắm tại <span class='highlight'>FurryFriends Store</span>!</p>
                            
                            <p>Hóa đơn điện tử cho đơn hàng <strong>#{invoiceNumber}</strong> của bạn đã được đính kèm trong email này.</p>
                            
                            <div style='background: white; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #667eea;'>
                                <h3 style='margin-top: 0; color: #667eea;'>📋 Thông tin đơn hàng</h3>
                                <p><strong>Mã đơn hàng:</strong> #{invoiceNumber}</p>
                                <p><strong>Ngày đặt:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                                <p><strong>Trạng thái:</strong> Đang xử lý</p>
                            </div>
                            
                            <p>Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận và giao hàng.</p>
                            
                            <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi:</p>
                            <ul>
                                <li>📞 Hotline: <strong>0968596808</strong></li>
                                <li>📧 Email: <strong>info@furryfriends.vn</strong></li>
                                <li>🏪 Địa chỉ: <strong>142 Nguyễn Đổng Chi, Nam Từ Liêm, TP. Hà Nội</strong></li>
                            </ul>
                            
                            <p>Một lần nữa, cảm ơn bạn đã lựa chọn FurryFriends!</p>
                            
                            <p>Trân trọng,<br>
                            <strong>Đội ngũ FurryFriends Store</strong> 🐾</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 FurryFriends Store. All rights reserved.</p>
                            <p>www.furryfriends.vn | Powered by FurryFriends System</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}
