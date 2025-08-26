using FurryFriends.API.Models;
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
            // 1. Tìm tài khoản theo email
            var account = await _taiKhoanRepository.FindByEmailAsync(request.Email);

            // 2. Nếu KHÔNG tìm thấy, vẫn trả về thông báo "thành công" giống hệt nhau
            if (account == null)
            {
                // Ghi log để developer biết là có ai đó đang thử với email không tồn tại
                _logger.LogWarning($"Yêu cầu đặt lại mật khẩu cho email không tồn tại: {request.Email}");
                // Nhưng vẫn trả về response y hệt trường hợp thành công
                return Ok(new { message = "Nếu email của bạn tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu." });
            }

            // 3. NẾU TÌM THẤY tài khoản, thì mới tạo mã và gửi email
            var code = new Random().Next(100000, 999999).ToString();
            _resetCodes[request.Email.ToLower()] = (code, DateTime.UtcNow.AddMinutes(10));

            try
            {
                var subject = "Yêu cầu đặt lại mật khẩu cho tài khoản Furry Friends";
                var body = $"<p>Xin chào,</p><p>Mã xác nhận để đặt lại mật khẩu của bạn là: <strong>{code}</strong></p><p>Mã này sẽ hết hạn sau 10 phút.</p>";
                await _mailService.SendEmailAsync(request.Email, subject, body);

                _logger.LogInformation($"Đã gửi mã xác nhận đến email: {request.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email đặt lại mật khẩu cho {Email}", request.Email);
                // NGAY CẢ KHI GỬI EMAIL LỖI, vẫn trả về thông báo thành công cho người dùng
                // return StatusCode(500, new { message = "Đã có lỗi xảy ra trong quá trình gửi email." }); // DÒNG NÀY SAI
                // Sửa lại: Kể cả lỗi gửi mail cũng không được tiết lộ cho client
                return Ok(new { message = "Nếu email của bạn tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu." });
            }

            // 4. Trả về thông báo thành công (giống hệt trường hợp không tìm thấy email)
            return Ok(new { message = "Nếu email của bạn tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var emailKey = request.Email.ToLower();

            if (!_resetCodes.TryGetValue(emailKey, out var value) || value.Code != request.Code || value.Expiry < DateTime.UtcNow)
            {
                if (_resetCodes.ContainsKey(emailKey) && _resetCodes[emailKey].Expiry < DateTime.UtcNow)
                {
                    _resetCodes.Remove(emailKey);
                }
                return BadRequest(new { message = "Mã xác nhận không đúng hoặc đã hết hạn." });
            }

            var account = await _taiKhoanRepository.FindByEmailAsync(request.Email);
            if (account == null)
            {
                return BadRequest(new { message = "Tài khoản không hợp lệ." });
            }

            var newHashedPassword = HashPassword(request.NewPassword);
            await _taiKhoanRepository.UpdatePasswordAsync(account.TaiKhoanId, newHashedPassword);

            _resetCodes.Remove(emailKey);

            return Ok(new { message = "Mật khẩu của bạn đã được đặt lại thành công." });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}