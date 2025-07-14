using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.Service.IService;
using FurryFriends.API.Models;

namespace FurryFriends.Web.Services
{
	public class ThongTinCaNhanService : IThongTinCaNhanService
	{
		private readonly HttpClient _httpClient;
		private readonly string _baseApiUrl = "https://localhost:7289/api/ThongTinCaNhan";
		private readonly IDiaChiKhachHangService _diaChiService;
		private readonly ITaiKhoanService _taiKhoanService;

		public ThongTinCaNhanService(HttpClient httpClient, IDiaChiKhachHangService diaChiService, ITaiKhoanService taiKhoanService)
		{
			_httpClient = httpClient;
			_diaChiService = diaChiService;
			_taiKhoanService = taiKhoanService;
		}

		public async Task<ThongTinCaNhanViewModel?> GetThongTinCaNhanAsync(Guid taiKhoanId)
		{
			var response = await _httpClient.GetAsync($"api/thongtincanhan/{taiKhoanId}");
			if (!response.IsSuccessStatusCode) return null;

			var dto = await response.Content.ReadFromJsonAsync<ThongTinCaNhanDTO>();
			if (dto == null) return null;

			// Lấy KhachHangId từ tài khoản
			var taiKhoan = await _taiKhoanService.GetByIdAsync(taiKhoanId);
			Guid? khachHangId = taiKhoan?.KhachHangId;
			var diaChis = new List<DiaChiKhachHangViewModel>();
			DiaChiKhachHangViewModel? diaChiChinh = null;
			if (khachHangId != null && khachHangId != Guid.Empty)
			{
				var diaChiEntities = await _diaChiService.GetByKhachHangIdAsync(khachHangId.Value);
				if (diaChiEntities != null)
				{
					diaChis = diaChiEntities.Select(dc => new DiaChiKhachHangViewModel
					{
						DiaChiId = dc.DiaChiId,
						TenDiaChi = dc.TenDiaChi,
						MoTa = dc.MoTa,
						PhuongXa = dc.PhuongXa,
						ThanhPho = dc.ThanhPho,
						SoDienThoai = dc.SoDienThoai,
						LaMacDinh = dc.TrangThai == 1,
						GhiChu = dc.GhiChu
					}).ToList();
					diaChiChinh = diaChis.FirstOrDefault(x => x.LaMacDinh);
				}
			}

			return new ThongTinCaNhanViewModel
			{
				TaiKhoanId = dto.TaiKhoanId,
				UserName = dto.UserName,
				HoTen = dto.HoTen,
				Email = dto.Email,
				SoDienThoai = dto.SoDienThoai,
				NgaySinh = dto.NgaySinh,
				GioiTinh = dto.GioiTinh,
				Role = dto.Role,
				DiaChis = diaChis,
				DiaChiChinh = diaChiChinh
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
				GioiTinh = model.GioiTinh
				// Không set UserName, Role, DiaChi
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

		public async Task<List<DiaChiKhachHangViewModel>> GetDanhSachDiaChiAsync(Guid taiKhoanId)
		{
			var result = new List<DiaChiKhachHangViewModel>();
			if (taiKhoanId == Guid.Empty) return result;
			var diaChiEntities = await _diaChiService.GetByKhachHangIdAsync(taiKhoanId);
			if (diaChiEntities != null)
			{
				result = diaChiEntities.Select(dc => new DiaChiKhachHangViewModel
				{
					DiaChiId = dc.DiaChiId,
					TenDiaChi = dc.TenDiaChi,
					MoTa = dc.MoTa,
					PhuongXa = dc.PhuongXa,
					ThanhPho = dc.ThanhPho,
					SoDienThoai = dc.SoDienThoai,
					LaMacDinh = dc.TrangThai == 1,
					GhiChu = dc.GhiChu
				}).ToList();
			}
			return result;
		}
	}
}
