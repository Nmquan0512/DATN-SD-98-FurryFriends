using FurryFriends.API.Models;
using FurryFriends.API.Models.Dtos;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanApiController : ControllerBase
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
		private readonly ITaiKhoanService _taiKhoanService;
		public TaiKhoanApiController(
		ITaiKhoanService taiKhoanService,
		ITaiKhoanRepository taiKhoanRepository)
		{
			_taiKhoanService = taiKhoanService;
			_taiKhoanRepository = taiKhoanRepository;
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

		[HttpPost("dang-nhap-admin")]
		public async Task<IActionResult> DangNhapAdmin([FromBody] LoginRequest model)
		{
			var result = await _taiKhoanService.DangNhapAdminNhanVienAsync(model);
			if (result == null)
				return Unauthorized("Sai tên đăng nhập hoặc mật khẩu hoặc không có quyền.");

			return Ok(result);
		}
		[HttpPost("dang-nhap-khachhang")]
		public async Task<IActionResult> DangNhapKhachHang([FromBody] LoginRequest model)
		{
			var result = await _taiKhoanService.DangNhapKhachHangAsync(model);
			if (result == null)
				return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");

			return Ok(result);
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
        public async Task<IActionResult> SearchByUserName([FromQuery] string userName)
        {
            try
            {
                IEnumerable<TaiKhoan> taiKhoans;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    taiKhoans = await _taiKhoanRepository.GetAllAsync();
                }
                else
                {
                    var tk = await _taiKhoanRepository.FindByUserNameAsync(userName);
                    taiKhoans = tk != null ? new List<TaiKhoan> { tk } : new List<TaiKhoan>();
                }

                return Ok(taiKhoans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}