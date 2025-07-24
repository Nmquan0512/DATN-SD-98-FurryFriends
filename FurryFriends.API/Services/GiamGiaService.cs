using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;

namespace FurryFriends.API.Services
{
    public class GiamGiaService : IGiamGiaService
    {   
        private readonly IGiamGiaRepository _giamGiaRepo;
        private readonly IDotGiamGiaSanPhamRepository _dotGiamGiaSanPhamRepo;

        public GiamGiaService(
            IGiamGiaRepository giamGiaRepo,
            IDotGiamGiaSanPhamRepository dotGiamGiaSanPhamRepo)
        {
            _giamGiaRepo = giamGiaRepo;
            _dotGiamGiaSanPhamRepo = dotGiamGiaSanPhamRepo;
        }

        public async Task<IEnumerable<GiamGiaDTO>> GetAllAsync()
        {
            var list = await _giamGiaRepo.GetAllWithSanPhamChiTietAsync(); // Ensure this includes DotGiamGiaSanPhams
            return list.Select(x => new GiamGiaDTO
            {
                GiamGiaId = x.GiamGiaId,
                TenGiamGia = x.TenGiamGia,
                PhanTramKhuyenMai = x.PhanTramKhuyenMai,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                NgayTao = x.NgayTao,
                NgayCapNhat = x.NgayCapNhat,
                SanPhamChiTietIds = x.DotGiamGiaSanPhams?.Select(d => d.SanPhamChiTietId).ToList()
            });
        }

        public async Task<GiamGiaDTO?> GetByIdAsync(Guid id)
        {
            var entity = await _giamGiaRepo.GetByIdWithSanPhamChiTietAsync(id);
            if (entity == null) return null;

            return new GiamGiaDTO
            {
                GiamGiaId = entity.GiamGiaId,
                TenGiamGia = entity.TenGiamGia,
                PhanTramKhuyenMai = entity.PhanTramKhuyenMai,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,
                TrangThai = entity.TrangThai,
                NgayTao = entity.NgayTao,
                NgayCapNhat = entity.NgayCapNhat,
                SanPhamChiTietIds = entity.DotGiamGiaSanPhams?.Select(d => d.SanPhamChiTietId).Where(id => id != Guid.Empty).ToList()
            };
        }

        public async Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto)
        {
            var newId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var entity = new GiamGia
            {
                GiamGiaId = newId,
                TenGiamGia = dto.TenGiamGia,
                PhanTramKhuyenMai = dto.PhanTramKhuyenMai,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TrangThai = dto.TrangThai,
                NgayTao = now,
                NgayCapNhat = now,
            };

            await _giamGiaRepo.AddAsync(entity);

            if (dto.SanPhamChiTietIds != null && dto.SanPhamChiTietIds.Any())
            {
                foreach (var spId in dto.SanPhamChiTietIds)
                {
                    // Lấy SanPhamId từ SanPhamChiTiet
                    var chiTiet = await _dotGiamGiaSanPhamRepo.GetByIdAsync(spId);
                    if (chiTiet == null) throw new Exception($"SanPhamChiTietId {spId} không tồn tại!");
                    var dot = new DotGiamGiaSanPham
                    {
                        DotGiamGiaSanPhamId = Guid.NewGuid(),
                        GiamGiaId = newId,
                        SanPhamId = chiTiet.SanPhamId, // Lấy đúng SanPhamId
                        SanPhamChiTietId = spId
                    };
                    await _dotGiamGiaSanPhamRepo.AddAsync(dot);
                }
            }

            dto.GiamGiaId = newId;
            dto.NgayTao = now;
            dto.NgayCapNhat = now;
            return dto;
        }

        public async Task<GiamGiaDTO?> UpdateAsync(Guid id, GiamGiaDTO dto)
        {
            var entity = await _giamGiaRepo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.TenGiamGia = dto.TenGiamGia;
            entity.PhanTramKhuyenMai = dto.PhanTramKhuyenMai;
            entity.NgayBatDau = dto.NgayBatDau;
            entity.NgayKetThuc = dto.NgayKetThuc;
            entity.TrangThai = dto.TrangThai;
            entity.NgayCapNhat = DateTime.UtcNow;

            await _giamGiaRepo.UpdateAsync(entity);

            return new GiamGiaDTO
            {
                GiamGiaId = entity.GiamGiaId,
                TenGiamGia = entity.TenGiamGia,
                PhanTramKhuyenMai = entity.PhanTramKhuyenMai,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,
                TrangThai = entity.TrangThai,
                NgayTao = entity.NgayTao,
                NgayCapNhat = entity.NgayCapNhat
            };
        }

        public async Task<bool> AddSanPhamChiTietToGiamGiaAsync(Guid giamGiaId, List<Guid> sanPhamChiTietIds)
        {
            var giamGia = await _giamGiaRepo.GetByIdAsync(giamGiaId);
            if (giamGia == null) return false;

            var existing = await _dotGiamGiaSanPhamRepo.GetByGiamGiaIdAsync(giamGiaId);
            var existingIds = existing.Select(d => d.SanPhamChiTietId).ToHashSet();

            foreach (var spId in sanPhamChiTietIds)
            {
                if (existingIds.Contains(spId)) continue;

                var dot = new DotGiamGiaSanPham
                {
                    DotGiamGiaSanPhamId = Guid.NewGuid(),
                    GiamGiaId = giamGiaId,
                    SanPhamChiTietId = spId
                };

                await _dotGiamGiaSanPhamRepo.AddAsync(dot);
            }

            return true;
        }
    }
}
