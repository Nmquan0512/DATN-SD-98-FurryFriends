using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Services
{
    public class PhieuHoanTraService : IPhieuHoanTraService
    {
        private readonly IPhieuHoanTraRepository _repo;

        public PhieuHoanTraService(IPhieuHoanTraRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<PhieuHoanTraDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync(); // repo đã Include đủ quan hệ
            return list.Select(MapToDto);
        }

        public async Task<PhieuHoanTraDto?> GetByIdAsync(Guid id)
        {
            // nên lấy kèm quan hệ để map được thông tin sản phẩm/khách đầy đủ
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<PhieuHoanTraDto>> GetByKhachHangAsync(Guid khachHangId)
        {
            var list = await _repo.GetByKhachHangAsync(khachHangId);
            return list.Select(MapToDto);
        }

        // ✅ Tạo phiếu có validate số lượng còn có thể hoàn
        public async Task<bool> CreateAsync(PhieuHoanTraCreateRequest request)
        {
            // 1) Lấy chi tiết hóa đơn (kèm HoaDon…) để có SoLuongSanPham
            var ct = await _repo.GetHoaDonChiTietWithRelationsAsync(request.HoaDonChiTietId);
            if (ct == null) throw new InvalidOperationException("Không tìm thấy chi tiết hóa đơn.");

            // 2) Tính số lượng đã 'giữ chỗ' hoặc đã hoàn xong (TrangThai != 2)
            var daHoanHoacDangCho = await _repo.GetTongSoLuongDaHoanAsync(request.HoaDonChiTietId);
            var soConTheHoan = ct.SoLuongSanPham - daHoanHoacDangCho;

            if (soConTheHoan <= 0)
                throw new InvalidOperationException("Sản phẩm này đã hết số lượng có thể hoàn.");

            if (request.SoLuongHoan < 1 || request.SoLuongHoan > soConTheHoan)
                throw new InvalidOperationException($"Bạn chỉ có thể hoàn tối đa {soConTheHoan} sản phẩm.");

            // 3) Tạo phiếu
            var entity = new PhieuHoanTra
            {
                PhieuHoanTraId = Guid.NewGuid(),
                HoaDonChiTietId = request.HoaDonChiTietId,
                SoLuongHoan = request.SoLuongHoan,
                LyDoHoanTra = request.LyDoHoanTra,
                NgayHoanTra = DateTime.Now,
                TrangThai = 0 // 0 = Yêu cầu
            };

            await _repo.AddAsync(entity);
            return true;
        }

        // ✅ Admin: CHỈ đổi trạng thái (không cho sửa số lượng / lý do để tránh “lách” validate)
        public async Task<bool> UpdateAsync(Guid id, PhieuHoanTraUpdateRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            // Nếu rất cần cho sửa số lượng, phải validate lại y như Create (không khuyến khích).
            // Ở đây chỉ cho đổi trạng thái:
            entity.TrangThai = request.TrangThai;

            await _repo.UpdateAsync(entity);
            return true;
        }

        // Có thể tắt hẳn xóa nếu không muốn mất lịch sử; tạm để nguyên
        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

        // Map → nếu bạn muốn trả thêm thông tin khách trong DTO, thêm các field tương ứng vào PhieuHoanTraDto
        private static PhieuHoanTraDto MapToDto(PhieuHoanTra p)
        {
            // an toàn null
            var ct = p.HoaDonChiTiet;
            var hd = ct?.HoaDon;

            int tongSoLuongMua = ct?.SoLuongSanPham ?? 0;

            int soLuongDaHoan = 0;
            var phieuCungCT = ct?.PhieuHoanTras;
            if (phieuCungCT != null)
            {

                soLuongDaHoan = phieuCungCT
                    .Where(x => x.TrangThai != 2)
                    .Sum(x => x.SoLuongHoan);
            }

            return new PhieuHoanTraDto
            {
                PhieuHoanTraId = p.PhieuHoanTraId,
                HoaDonChiTietId = p.HoaDonChiTietId,
                SoLuongHoan = p.SoLuongHoan,
                LyDoHoanTra = p.LyDoHoanTra,
                NgayHoanTra = p.NgayHoanTra,
                TrangThai = p.TrangThai,


                TenSanPham = ct?.TenSanPhamLucMua,
                MauSac = ct?.MauSacLucMua,
                KichCo = ct?.KichCoLucMua,
                AnhSanPham = ct?.AnhSanPhamLucMua,


                KhachHangId = hd?.KhachHangId ?? Guid.Empty,
                HoTenKhach = hd?.TenCuaKhachHang,
                SdtKhach = hd?.SdtCuaKhachHang,
                EmailKhach = hd?.EmailCuaKhachHang,
                DiaChiNhanHang = hd?.DiaChiGiaoHangLucMua,


                TongSoLuongMua = tongSoLuongMua,
                SoLuongDaHoan = soLuongDaHoan

            };
        }
    }
}
