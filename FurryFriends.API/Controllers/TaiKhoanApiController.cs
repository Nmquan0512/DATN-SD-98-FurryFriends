﻿using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanApiController : ControllerBase
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly ILogger<TaiKhoanApiController> _logger;
        private readonly IMailService _mailService;

        private static readonly Dictionary<string, (string Code, DateTime Expiry)> _resetCodes = new();

        public TaiKhoanApiController(ITaiKhoanRepository taiKhoanRepository, ILogger<TaiKhoanApiController> logger, IMailService mailService)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _logger = logger;
            _mailService = mailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var taiKhoans = await _taiKhoanRepository.GetAllAsync();
                return Ok(taiKhoans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("find-by-username")]
        public async Task<IActionResult> FindByUserName([FromQuery] string userName)
        {
            try
            {
                var taiKhoan = await _taiKhoanRepository.FindByUserNameAsync(userName);
                if (taiKhoan == null)
                {
                    return Ok(new List<TaiKhoan>());
                }
                return Ok(new List<TaiKhoan> { taiKhoan });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var taiKhoan = await _taiKhoanRepository.GetByIdAsync(id);
                if (taiKhoan == null)
                {
                    return NotFound($"Tài khoản với TaiKhoanId {id} không tồn tại.");
                }
                return Ok(taiKhoan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


		[HttpPost]
        public async Task<IActionResult> Create([FromBody] TaiKhoan taiKhoan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _taiKhoanRepository.AddAsync(taiKhoan);
                return CreatedAtAction(nameof(GetById), new { id = taiKhoan.TaiKhoanId }, taiKhoan);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.TaiKhoanId)
            {
                return BadRequest("TaiKhoanId không khớp.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _taiKhoanRepository.UpdateAsync(taiKhoan);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
	


		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _taiKhoanRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            try
            {
                var all = await _taiKhoanRepository.GetAllAsync();
                var result = all
                    .Where(tk => !string.IsNullOrEmpty(tk.UserName) && 
                                (string.IsNullOrWhiteSpace(keyword) || tk.UserName.ToLower().Contains(keyword.ToLower())) &&
                                tk.KhachHang == null) // Chỉ trả về tài khoản chưa được liên kết với khách hàng
                    .Select(tk => new { taiKhoanId = tk.TaiKhoanId, userName = tk.UserName })
                    .Take(20)
                    .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search-all")]
        public async Task<IActionResult> SearchAll([FromQuery] string keyword)
        {
            try
            {
                var all = await _taiKhoanRepository.GetAllAsync();
                var result = all
                    .Where(tk => !string.IsNullOrEmpty(tk.UserName) && 
                                (string.IsNullOrWhiteSpace(keyword) || tk.UserName.ToLower().Contains(keyword.ToLower())))
                    .Select(tk => new { taiKhoanId = tk.TaiKhoanId, userName = tk.UserName })
                    .Take(20)
                    .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search-for-edit")]
        public async Task<IActionResult> SearchForEdit([FromQuery] string keyword, [FromQuery] Guid? currentTaiKhoanId = null)
        {
            try
            {
                var all = await _taiKhoanRepository.GetAllAsync();
                var result = all
                    .Where(tk => !string.IsNullOrEmpty(tk.UserName) && 
                                (string.IsNullOrWhiteSpace(keyword) || tk.UserName.ToLower().Contains(keyword.ToLower())) &&
                                (tk.KhachHang == null || tk.TaiKhoanId == currentTaiKhoanId)) // Chỉ trả về tài khoản chưa liên kết HOẶC tài khoản hiện tại
                    .Select(tk => new { taiKhoanId = tk.TaiKhoanId, userName = tk.UserName })
                    .Take(20)
                    .ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("dang-nhap-admin")]
        public async Task<IActionResult> DangNhapAdmin([FromBody] LoginRequest model)
        {
            try
            {
                _logger.LogInformation($"API: Đăng nhập admin với UserName: {model.UserName}");

                if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                {
                    _logger.LogWarning("API: UserName hoặc Password trống");
                    return BadRequest("Tên đăng nhập và mật khẩu không được để trống.");
                }

                var taiKhoan = await _taiKhoanRepository.FindByUserNameAsync(model.UserName);
                _logger.LogInformation($"API: Tìm thấy tài khoản: {(taiKhoan != null ? "Có" : "Không")}");
                
                if (taiKhoan == null)
                {
                    _logger.LogWarning($"API: Không tìm thấy tài khoản với UserName: {model.UserName}");
                    return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");
                }

                _logger.LogInformation($"API: So sánh password - DB: {taiKhoan.Password}, Input: {model.Password}");
                
                if (taiKhoan.Password != model.Password)
                {
                    _logger.LogWarning($"API: Password không khớp cho UserName: {model.UserName}");
                    return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");
                }

                if (!taiKhoan.TrangThai)
                {
                    _logger.LogWarning($"API: Tài khoản bị khóa cho UserName: {model.UserName}");
                    return Unauthorized("Tài khoản đã dừng hoạt động. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                }

                // Kiểm tra trạng thái nhân viên liên kết
                if (taiKhoan.NhanVien != null && !taiKhoan.NhanVien.TrangThai)
                {
                    _logger.LogWarning($"API: Nhân viên liên kết bị khóa cho UserName: {model.UserName}");
                    return Unauthorized("Tài khoản đã dừng hoạt động. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                }

                // Kiểm tra có phải admin/nhân viên không
                if (taiKhoan.NhanVien == null)
                {
                    _logger.LogWarning($"API: Tài khoản không có quyền admin cho UserName: {model.UserName}");
                    return Unauthorized("Tài khoản không có quyền admin.");
                }

                var response = new LoginResponse
                {
                    TaiKhoanId = taiKhoan.TaiKhoanId,
                    Role = taiKhoan.NhanVien.ChucVu?.TenChucVu ?? "NhanVien",
                    HoTen = taiKhoan.NhanVien.HoVaTen
                };

                _logger.LogInformation($"API: Đăng nhập admin thành công cho UserName: {model.UserName}, Role: {response.Role}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API: Lỗi đăng nhập admin cho UserName: {model.UserName}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("dang-nhap-khachhang")]
        public async Task<IActionResult> DangNhapKhachHang([FromBody] LoginRequest model)
        {
            try
            {
                _logger.LogInformation($"API: Đăng nhập khách hàng với UserName: {model.UserName}");

                if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                {
                    _logger.LogWarning("API: UserName hoặc Password trống");
                    return BadRequest("Tên đăng nhập và mật khẩu không được để trống.");
                }

                var taiKhoan = await _taiKhoanRepository.FindByUserNameAsync(model.UserName);
                _logger.LogInformation($"API: Tìm thấy tài khoản khách hàng: {(taiKhoan != null ? "Có" : "Không")}");

                if (taiKhoan == null)
                {
                    _logger.LogWarning($"API: Không tìm thấy tài khoản khách hàng với UserName: {model.UserName}");
                    return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");
                }

                _logger.LogInformation($"API: So sánh password khách hàng - DB: {taiKhoan.Password}, Input: {model.Password}");

                if (taiKhoan.Password != model.Password)
                {
                    _logger.LogWarning($"API: Password khách hàng không khớp cho UserName: {model.UserName}");
                    return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");
                }

                if (!taiKhoan.TrangThai)
                {
                    _logger.LogWarning($"API: Tài khoản khách hàng bị khóa cho UserName: {model.UserName}");
                    return Unauthorized("Tài khoản đã dừng hoạt động. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                }

                // Kiểm tra trạng thái khách hàng/nhân viên liên kết
                if (taiKhoan.KhachHang != null && taiKhoan.KhachHang.TrangThai != 1)
                {
                    _logger.LogWarning($"API: Khách hàng liên kết bị khóa cho UserName: {model.UserName}");
                    return Unauthorized("Tài khoản đã dừng hoạt động. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                }

                if (taiKhoan.NhanVien != null && !taiKhoan.NhanVien.TrangThai)
                {
                    _logger.LogWarning($"API: Nhân viên liên kết bị khóa cho UserName: {model.UserName}");
                    return Unauthorized("Tài khoản đã dừng hoạt động. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                }

                // Kiểm tra quyền thực tế của người dùng
                string actualRole = "KhachHang";
                string hoTen = "";

                // Nếu có thông tin nhân viên, ưu tiên role nhân viên
                if (taiKhoan.NhanVien != null)
                {
                    actualRole = taiKhoan.NhanVien.ChucVu?.TenChucVu ?? "NhanVien";
                    hoTen = taiKhoan.NhanVien.HoVaTen;
                    _logger.LogInformation($"API: Tài khoản có quyền nhân viên với role: {actualRole}");
                }
                // Nếu không có nhân viên, kiểm tra khách hàng
                else if (taiKhoan.KhachHang != null)
                {
                    actualRole = "KhachHang";
                    hoTen = taiKhoan.KhachHang.TenKhachHang;
                    _logger.LogInformation($"API: Tài khoản là khách hàng");
                }
                else
                {
                    _logger.LogWarning($"API: Tài khoản không có thông tin khách hàng hoặc nhân viên cho UserName: {model.UserName}");
                    return Unauthorized("Tài khoản không hợp lệ.");
                }

                var response = new LoginResponse
                {
                    TaiKhoanId = taiKhoan.TaiKhoanId,
                    KhachHangId = taiKhoan.KhachHang?.KhachHangId ?? Guid.Empty, //sửa cho thêm dòng này
                    Role = actualRole,
                    HoTen = hoTen
                };

                _logger.LogInformation($"API: Đăng nhập thành công cho UserName: {model.UserName} với Role: {actualRole}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API: Lỗi đăng nhập khách hàng cho UserName: {model.UserName}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            _logger.LogInformation($"=== BẮT ĐẦU XỬ LÝ FORGOT PASSWORD ===");
            _logger.LogInformation($"Email yêu cầu: {request.Email}");
            
            // 1. Tìm tài khoản theo email
            var account = await _taiKhoanRepository.FindByEmailAsync(request.Email);
            _logger.LogInformation($"Kết quả tìm kiếm email: {(account != null ? "TÌM THẤY" : "KHÔNG TÌM THẤY")}");

            // 2. Nếu KHÔNG tìm thấy, vẫn trả về thông báo "thành công" giống hệt nhau
            if (account == null)
            {
                // Ghi log để developer biết là có ai đó đang thử với email không tồn tại
                _logger.LogWarning($"Yêu cầu đặt lại mật khẩu cho email không tồn tại: {request.Email}");
                // Nhưng vẫn trả về response y hệt trường hợp thành công
                return Ok(new { message = "Nếu email của bạn tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu." });
            }

            _logger.LogInformation($"Tìm thấy tài khoản: TaiKhoanId={account.TaiKhoanId}, UserName={account.UserName}");
            _logger.LogInformation($"Thông tin liên kết: KhachHangId={account.KhachHangId}, NhanVienId={account.NhanVienId}");

            // 3. NẾU TÌM THẤY tài khoản, thì mới tạo mã và gửi email
            var code = new Random().Next(100000, 999999).ToString();
            _logger.LogInformation($"Mã xác nhận được tạo: {code}");
            
            _resetCodes[request.Email.ToLower()] = (code, DateTime.UtcNow.AddMinutes(10));
            _logger.LogInformation($"Mã đã được lưu vào cache với thời gian hết hạn: {DateTime.UtcNow.AddMinutes(10)}");

            try
            {
                var subject = "Yêu cầu đặt lại mật khẩu cho tài khoản Furry Friends";
                var body = $"<p>Xin chào,</p><p>Mã xác nhận để đặt lại mật khẩu của bạn là: <strong>{code}</strong></p><p>Mã này sẽ hết hạn sau 10 phút.</p>";
                
                _logger.LogInformation($"Bắt đầu gửi email đến: {request.Email}");
                _logger.LogInformation($"Subject: {subject}");
                _logger.LogInformation($"Body: {body}");
                
                await _mailService.SendEmailAsync(request.Email, subject, body);

                _logger.LogInformation($"Đã gửi mã xác nhận đến email: {request.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email đặt lại mật khẩu cho {Email}", request.Email);
                _logger.LogError($"Chi tiết lỗi: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                
                // NGAY CẢ KHI GỬI EMAIL LỖI, vẫn trả về thông báo thành công cho người dùng
                // return StatusCode(500, new { message = "Đã có lỗi xảy ra trong quá trình gửi email." }); // DÒNG NÀY SAI
                // Sửa lại: Kể cả lỗi gửi mail cũng không được tiết lộ cho client
                return Ok(new { message = "Nếu email của bạn tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu." });
            }

            _logger.LogInformation($"=== HOÀN THÀNH XỬ LÝ FORGOT PASSWORD ===");
            // 4. Trả về thông báo thành công (giống hệt trường hợp không tìm thấy email)
            return Ok(new { message = "Nếu email của bạn tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            _logger.LogInformation($"=== BẮT ĐẦU XỬ LÝ RESET PASSWORD ===");
            _logger.LogInformation($"Email: {request.Email}, Code: {request.Code}");

            // 1. Kiểm tra mã xác nhận
            var emailKey = request.Email.ToLower();
            if (!_resetCodes.ContainsKey(emailKey))
            {
                _logger.LogWarning($"Không tìm thấy mã xác nhận cho email: {request.Email}");
                return BadRequest(new { message = "Mã xác nhận không hợp lệ hoặc đã hết hạn." });
            }

            var (storedCode, expiry) = _resetCodes[emailKey];
            if (DateTime.UtcNow > expiry)
            {
                _logger.LogWarning($"Mã xác nhận đã hết hạn cho email: {request.Email}");
                    _resetCodes.Remove(emailKey);
                return BadRequest(new { message = "Mã xác nhận đã hết hạn." });
            }

            if (storedCode != request.Code)
            {
                _logger.LogWarning($"Mã xác nhận không đúng cho email: {request.Email}");
                return BadRequest(new { message = "Mã xác nhận không đúng." });
            }

            // 2. Tìm tài khoản
            var account = await _taiKhoanRepository.FindByEmailAsync(request.Email);
            if (account == null)
            {
                _logger.LogWarning($"Không tìm thấy tài khoản cho email: {request.Email}");
                return BadRequest(new { message = "Email không tồn tại trong hệ thống." });
            }

            _logger.LogInformation($"Tìm thấy tài khoản: TaiKhoanId={account.TaiKhoanId}, UserName={account.UserName}");

            try
            {
                // 3. Cập nhật mật khẩu mới (không mã hóa)
                await _taiKhoanRepository.UpdatePasswordAsync(account.TaiKhoanId, request.NewPassword);
                
                // 4. Xóa mã xác nhận đã sử dụng
            _resetCodes.Remove(emailKey);

                _logger.LogInformation($"Đã cập nhật mật khẩu thành công cho tài khoản: {account.TaiKhoanId}");
                _logger.LogInformation($"=== HOÀN THÀNH XỬ LÝ RESET PASSWORD ===");

                return Ok(new { message = "Mật khẩu đã được đặt lại thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật mật khẩu cho {Email}", request.Email);
                return StatusCode(500, new { message = "Đã có lỗi xảy ra trong quá trình đặt lại mật khẩu." });
            }
        }

        // Xóa method HashPassword không cần thiết nữa

        [HttpGet("test-email/{email}")]
        public async Task<IActionResult> TestEmail(string email)
        {
            _logger.LogInformation($"=== TEST EMAIL ENDPOINT ===");
            _logger.LogInformation($"Testing email: {email}");
            
            var account = await _taiKhoanRepository.FindByEmailAsync(email);
            
            if (account != null)
            {
                return Ok(new { 
                    found = true, 
                    taiKhoanId = account.TaiKhoanId,
                    userName = account.UserName,
                    khachHangId = account.KhachHangId,
                    nhanVienId = account.NhanVienId
                });
            }
            
            return Ok(new { found = false });
        }
    }
}