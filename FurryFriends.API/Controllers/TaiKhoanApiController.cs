using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanApiController : ControllerBase
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly ILogger<TaiKhoanApiController> _logger;

        public TaiKhoanApiController(ITaiKhoanRepository taiKhoanRepository, ILogger<TaiKhoanApiController> logger)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _logger = logger;
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
                if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
                {
                    return Ok(new List<object>()); // Không trả về gì nếu chưa đủ ký tự
                }
                var all = await _taiKhoanRepository.GetAllAsync();
                var result = all
                    .Where(tk => !string.IsNullOrEmpty(tk.UserName) && tk.UserName.ToLower().Contains(keyword.ToLower()))
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
                    return Unauthorized("Tài khoản đã bị khóa.");
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
                    return Unauthorized("Tài khoản đã bị khóa.");
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

    }
}