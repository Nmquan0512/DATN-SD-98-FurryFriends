using FurryFriends.API.Data;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Services
{
	public class ThongTinCaNhanService : IThongTinCaNhanService
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ThongTinCaNhanService(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		public async Task<ThongTinCaNhanDTO?> GetThongTinCaNhanAsync(Guid taiKhoanId)
		{
			var tk = await _context.TaiKhoans
				.Include(t => t.NhanVien)
					.ThenInclude(nv => nv.ChucVu)
				.Include(t => t.KhachHang)
				.FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId);

			if (tk == null) return null;

			var dto = new ThongTinCaNhanDTO
			{
				TaiKhoanId = tk.TaiKhoanId,
				UserName = tk.UserName,
				Role = tk.NhanVien?.ChucVu?.TenChucVu ?? "KhachHang"
			};

			if (tk.NhanVien != null)
			{
				var nv = tk.NhanVien;
				dto.HoTen = nv.HoVaTen;
				dto.Email = nv.Email;
				dto.NgaySinh = nv.NgaySinh;
				dto.DiaChi = nv.DiaChi;
				dto.GioiTinh = nv.GioiTinh;
				dto.SoDienThoai = nv.SDT;
			}
			else if (tk.KhachHang != null)
			{
				var kh = tk.KhachHang;
				dto.HoTen = kh.TenKhachHang;
				dto.Email = kh.EmailCuaKhachHang;
				dto.SoDienThoai = kh.SDT;

				// Tìm địa chỉ đầu tiên nếu có
				dto.DiaChi = await _context.DiaChiKhachHangs
				.Where(d => d.KhachHangId == kh.KhachHangId)
				.OrderByDescending(d => d.NgayCapNhat) // Lấy địa chỉ mới nhất
				.Select(d =>
					string.Join(", ", new[]
					{
						d.MoTa,
						d.PhuongXa,
						d.QuanHuyen,
						d.ThanhPho
					}.Where(x => !string.IsNullOrWhiteSpace(x)))
				)
				.FirstOrDefaultAsync();
			}

			return dto;
		}

		public async Task<bool> UpdateThongTinCaNhanAsync(Guid taiKhoanId, CapNhatThongTinCaNhanDTO dto)
		{
			var tk = await _context.TaiKhoans
				.Include(t => t.NhanVien)
				.Include(t => t.KhachHang)
				.FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId);

			if (tk == null) return false;

			if (tk.NhanVien != null)
			{
				var nv = tk.NhanVien;
				nv.HoVaTen = dto.HoTen ?? nv.HoVaTen;
				nv.Email = dto.Email ?? nv.Email;
				nv.NgaySinh = dto.NgaySinh ?? nv.NgaySinh;
				nv.SDT = dto.SoDienThoai ?? nv.SDT;
				nv.GioiTinh = dto.GioiTinh ?? nv.GioiTinh;
				nv.DiaChi = dto.DiaChi ?? nv.DiaChi;
				nv.NgayCapNhat = DateTime.Now;
			}
			else if (tk.KhachHang != null)
			{
				var kh = tk.KhachHang;
				kh.TenKhachHang = dto.HoTen ?? kh.TenKhachHang;
				kh.EmailCuaKhachHang = dto.Email ?? kh.EmailCuaKhachHang;
				kh.SDT = dto.SoDienThoai ?? kh.SDT;
				kh.NgayCapNhatCuoiCung = DateTime.Now;

				// Cập nhật địa chỉ đầu tiên nếu có
				var diaChi = await _context.DiaChiKhachHangs
				.Where(d => d.KhachHangId == kh.KhachHangId)
				.OrderByDescending(d => d.NgayCapNhat)
				.FirstOrDefaultAsync();

							if (diaChi != null && !string.IsNullOrWhiteSpace(dto.DiaChi))
							{
								diaChi.MoTa = dto.DiaChi;
								diaChi.NgayCapNhat = DateTime.Now;
							}
			}

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DoiMatKhauAsync(Guid taiKhoanId, string matKhauCu, string matKhauMoi)
		{
			var tk = await _context.TaiKhoans.FindAsync(taiKhoanId);
			if (tk == null) return false;

			if (tk.Password != matKhauCu) // Sau này nên dùng BCrypt.Verify
				return false;

			// Sau này nên mã hóa: tk.Password = BCrypt.Net.BCrypt.HashPassword(matKhauMoi);
			tk.Password = matKhauMoi;
			tk.NgayCapNhatCuoiCung = DateTime.Now;
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
