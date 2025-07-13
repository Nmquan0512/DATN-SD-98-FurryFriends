using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ThongTinCaNhanController : ControllerBase
    {
		private readonly IThongTinCaNhanService _service;

		public ThongTinCaNhanController(IThongTinCaNhanService service)
		{
			_service = service;
		}

		// GET: api/thongtincanhan/{taiKhoanId}
		[HttpGet("{taiKhoanId}")]
		public async Task<IActionResult> GetThongTinCaNhan(Guid taiKhoanId)
		{
			var result = await _service.GetThongTinCaNhanAsync(taiKhoanId);
			if (result == null)
				return NotFound("Không tìm thấy tài khoản.");

			return Ok(result);
		}

		// PUT: api/thongtincanhan/{taiKhoanId}
		[HttpPut("{taiKhoanId}")]
		public async Task<IActionResult> UpdateThongTinCaNhan(Guid taiKhoanId, [FromBody] CapNhatThongTinCaNhanDTO dto)
		{
			var success = await _service.UpdateThongTinCaNhanAsync(taiKhoanId, dto);
			if (!success)
				return NotFound("Không tìm thấy tài khoản để cập nhật.");

			return Ok("Cập nhật thông tin thành công.");
		}

		// PUT: api/thongtincanhan/doimatkhau/{taiKhoanId}
		[HttpPut("doimatkhau/{taiKhoanId}")]
		public async Task<IActionResult> DoiMatKhau(Guid taiKhoanId, [FromBody] DoiMatKhauRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.MatKhauCu) || string.IsNullOrWhiteSpace(request.MatKhauMoi))
				return BadRequest("Mật khẩu không được để trống.");

			var success = await _service.DoiMatKhauAsync(taiKhoanId, request.MatKhauCu, request.MatKhauMoi);
			if (!success)
				return BadRequest("Mật khẩu cũ không đúng hoặc tài khoản không tồn tại.");

			return Ok("Đổi mật khẩu thành công.");
		}
	}

	public class DoiMatKhauRequest
	{
		public string MatKhauCu { get; set; } = null!;
		public string MatKhauMoi { get; set; } = null!;
	}
}

