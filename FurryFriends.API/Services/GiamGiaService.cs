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
                SanPhamChiTietIds = x.DotGiamGiaSanPhams?
                    .Select(d => d.SanPhamChiTietId ?? Guid.Empty)
                    .Where(id => id != Guid.Empty)
                    .ToList()
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
                SanPhamChiTietIds = entity.DotGiamGiaSanPhams?
                    .Select(d => d.SanPhamChiTietId ?? Guid.Empty)
                    .Where(id => id != Guid.Empty)
                    .ToList()
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
                    var dot = new DotGiamGiaSanPham
                    {
                        DotGiamGiaSanPhamId = Guid.NewGuid(),
                        GiamGiaId = newId,
                        SanPhamChiTietId = spId
                    };
                    await _dotGiamGiaSanPhamRepo.AddAsync(dot);
                }
            }

<<<<<<< Updated upstream
            dto.GiamGiaId = newId;
            dto.NgayTao = now;
            dto.NgayCapNhat = now;
            return dto;
=======
            // 3. Thêm toàn bộ "biểu đồ đối tượng" vào context
            await _giamGiaRepo.AddAsync(giamGiaEntity);

            // 4. Lưu tất cả thay đổi (cả GiamGia và DotGiamGiaSanPhams) trong MỘT GIAO DỊCH
            await _giamGiaRepo.SaveAsync();

            // Trả về DTO đã được tạo, ánh xạ lại để có đầy đủ thông tin
            return _mapper.Map<GiamGiaDTO>(giamGiaEntity);
        }
        // File: GiamGiaService.cs (API)

        public async Task<GiamGiaDTO> UpdateAsync(GiamGiaDTO dto)
        {
            try
            {
                // 1. Tải đối tượng GiamGia cũ CÙNG VỚI các sản phẩm liên quan
                var existingEntity = await _giamGiaRepo.GetByIdAsync(dto.GiamGiaId, true);
                if (existingEntity == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy chương trình giảm giá để cập nhật.");
                }

                // Validate trùng tên
                if (await _giamGiaRepo.TenGiamGiaExistsAsync(dto.TenGiamGia, dto.GiamGiaId))
                {
                    throw new InvalidOperationException("Tên chương trình giảm giá đã tồn tại.");
                }

                // 2. Cập nhật các thuộc tính chính
                existingEntity.TenGiamGia = dto.TenGiamGia;
                existingEntity.PhanTramKhuyenMai = dto.PhanTramKhuyenMai;
                existingEntity.NgayBatDau = dto.NgayBatDau;
                existingEntity.NgayKetThuc = dto.NgayKetThuc;
                existingEntity.TrangThai = dto.TrangThai;
                existingEntity.NgayCapNhat = DateTime.UtcNow;

                // 3. Xử lý danh sách sản phẩm - Sử dụng cách tiếp cận đơn giản hơn
                var newProductIds = dto.SanPhamChiTietIds ?? new List<Guid>();
                
                // Đảm bảo collection không null
                if (existingEntity.DotGiamGiaSanPhams == null)
                {
                    existingEntity.DotGiamGiaSanPhams = new List<DotGiamGiaSanPham>();
                }

                // Xử lý danh sách sản phẩm bằng cách xóa tất cả và thêm lại
                var currentProductIds = existingEntity.DotGiamGiaSanPhams?.Select(d => d.SanPhamChiTietId).ToList() ?? new List<Guid>();
                
                // Xóa tất cả sản phẩm hiện tại
                if (currentProductIds.Any())
                {
                    await _giamGiaRepo.RemoveProductsFromDiscount(existingEntity.GiamGiaId, currentProductIds);
                }

                // Thêm lại tất cả sản phẩm từ danh sách mới
                if (newProductIds.Any())
                {
                    await _giamGiaRepo.AddProductsToDiscount(existingEntity.GiamGiaId, newProductIds.Distinct().ToList(), existingEntity.PhanTramKhuyenMai);
                }

                // 4. Lưu tất cả các thay đổi
                await _giamGiaRepo.SaveAsync();

                // 5. Trả về DTO đã được cập nhật
                var updatedDto = _mapper.Map<GiamGiaDTO>(existingEntity);
                updatedDto.SanPhamChiTietIds = newProductIds;
                return updatedDto;
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                var errorMessage = $"Lỗi khi cập nhật chương trình giảm giá ID: {dto.GiamGiaId}. " +
                                 $"Sản phẩm mới: {string.Join(",", dto.SanPhamChiTietIds ?? new List<Guid>())}. " +
                                 $"Lỗi: {ex.Message}";
                throw new Exception(errorMessage, ex);
            }
>>>>>>> Stashed changes
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
