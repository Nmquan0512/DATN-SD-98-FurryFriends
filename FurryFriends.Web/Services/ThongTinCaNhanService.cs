using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;

namespace FurryFriends.Web.Services
{
	public class ThongTinCaNhanService : IThongTinCaNhanService
	{
		private readonly HttpClient _httpClient;
		private readonly string _baseApiUrl = "https://localhost:7289/api/ThongTinCaNhan";

		public ThongTinCaNhanService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<ThongTinCaNhanViewModel?> GetThongTinCaNhanAsync(Guid taiKhoanId)
		{
			var response = await _httpClient.GetAsync($"api/thongtincanhan/{taiKhoanId}");
			if (!response.IsSuccessStatusCode) return null;

			var dto = await response.Content.ReadFromJsonAsync<ThongTinCaNhanDTO>();
			if (dto == null) return null;

			return new ThongTinCaNhanViewModel
			{
				TaiKhoanId = dto.TaiKhoanId,
				UserName = dto.UserName,
				HoTen = dto.HoTen,
				Email = dto.Email,
				SoDienThoai = dto.SoDienThoai,
				NgaySinh = dto.NgaySinh,
				GioiTinh = dto.GioiTinh,
				DiaChi = dto.DiaChi,
				Role = dto.Role
			};
		}

		public async Task<bool> UpdateThongTinCaNhanAsync(Guid taiKhoanId, ThongTinCaNhanViewModel model)
		{
			var dto = new ThongTinCaNhanDTO
			{
				HoTen = model.HoTen,
				Email = model.Email,
				SoDienThoai = model.SoDienThoai,
				NgaySinh = model.NgaySinh,
				GioiTinh = model.GioiTinh,
				DiaChi = model.DiaChi
				// Không set UserName hoặc Role vì không được sửa
			};

			var response = await _httpClient.PutAsJsonAsync($"api/thongtincanhan/{taiKhoanId}", dto);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DoiMatKhauAsync(Guid taiKhoanId, string matKhauCu, string matKhauMoi)
		{
			var payload = new
			{
				matKhauCu,
				matKhauMoi
			};

			var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/doimatkhau/{taiKhoanId}", payload);
			return response.IsSuccessStatusCode;
		}

	}
}
