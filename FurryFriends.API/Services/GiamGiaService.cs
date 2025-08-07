using AutoMapper;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class GiamGiaService : IGiamGiaService
    {
        private readonly IGiamGiaRepository _giamGiaRepo;
        private readonly IDotGiamGiaSanPhamRepository _dotGiamGiaRepo; // Vẫn cần để truy vấn
        private readonly IMapper _mapper;

        public GiamGiaService(
            IGiamGiaRepository giamGiaRepo,
            IDotGiamGiaSanPhamRepository dotGiamGiaRepo,
            IMapper mapper)
        {
            _giamGiaRepo = giamGiaRepo;
            _dotGiamGiaRepo = dotGiamGiaRepo;
            _mapper = mapper;
        }

        public async Task<GiamGiaDTO> GetByIdAsync(Guid id)
        {
            var entity = await _giamGiaRepo.GetByIdAsync(id, true);
            if (entity == null) return null;

            var dto = _mapper.Map<GiamGiaDTO>(entity);
            // Lấy danh sách ID sản phẩm từ các đối tượng con
            dto.SanPhamChiTietIds = entity.DotGiamGiaSanPhams.Select(d => d.SanPhamChiTietId).ToList();
            return dto;
        }

        public async Task<IEnumerable<GiamGiaDTO>> GetAllAsync()
        {
            var entities = await _giamGiaRepo.GetAllAsync(true);
            var dtos = _mapper.Map<IEnumerable<GiamGiaDTO>>(entities).ToList();

            // Gán lại số lượng sản phẩm áp dụng cho mỗi DTO
            foreach (var dto in dtos)
            {
                var entity = entities.FirstOrDefault(e => e.GiamGiaId == dto.GiamGiaId);
                if (entity != null)
                {
                    dto.SanPhamChiTietIds = entity.DotGiamGiaSanPhams.Select(d => d.SanPhamChiTietId).ToList();
                }
            }
            return dtos;
        }

        public async Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto)
        {
            // Validate
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (await _giamGiaRepo.TenGiamGiaExistsAsync(dto.TenGiamGia))
            {
                throw new InvalidOperationException("Tên chương trình giảm giá đã tồn tại.");
            }

            // 1. Ánh xạ thuộc tính chính của GiamGia
            var giamGiaEntity = _mapper.Map<GiamGia>(dto);

            // 2. Xây dựng danh sách các sản phẩm liên quan trong bộ nhớ
            if (dto.SanPhamChiTietIds?.Any() == true)
            {
                foreach (var productId in dto.SanPhamChiTietIds)
                {
                    giamGiaEntity.DotGiamGiaSanPhams.Add(new DotGiamGiaSanPham
                    {
                        SanPhamChiTietId = productId,
                        PhanTramGiamGia = giamGiaEntity.PhanTramKhuyenMai,
                        TrangThai = true
                    });
                }
            }

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
            // 1. Tải đối tượng GiamGia cũ CÙNG VỚI các sản phẩm liên quan
            var existingEntity = await _giamGiaRepo.GetByIdAsync(dto.GiamGiaId, includeProducts: true);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException("Không tìm thấy chương trình giảm giá để cập nhật.");
            }

            // Validate trùng tên
            if (await _giamGiaRepo.TenGiamGiaExistsAsync(dto.TenGiamGia, dto.GiamGiaId))
            {
                throw new InvalidOperationException("Tên chương trình giảm giá đã tồn tại.");
            }

            // 2. Chỉ cần gọi Mapper.Map
            // AutoMapper sẽ tự động:
            // - Cập nhật các thuộc tính chính (TenGiamGia, PhanTramKhuyenMai...).
            // - Xóa các DotGiamGiaSanPhams không còn trong dto.SanPhamChiTietIds.
            // - Thêm các DotGiamGiaSanPhams mới từ dto.SanPhamChiTietIds.
            // - Cập nhật PhanTramGiamGia cho các DotGiamGiaSanPhams còn lại.
            _mapper.Map(dto, existingEntity);

            // 3. Lưu tất cả các thay đổi mà Mapper đã chuẩn bị
            await _giamGiaRepo.SaveAsync();

            return _mapper.Map<GiamGiaDTO>(existingEntity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Tải đối tượng cần xóa cùng với các liên kết
            var entityToDelete = await _giamGiaRepo.GetByIdAsync(id, includeProducts: true);
            if (entityToDelete == null)
            {
                return false; // Không tìm thấy để xóa
            }

            // Xóa các liên kết con trước
            // EF Core sẽ tự động xử lý việc này khi bạn cấu hình Cascade Delete,
            // nhưng xóa tường minh sẽ an toàn hơn.
            entityToDelete.DotGiamGiaSanPhams.Clear();

            // Xóa đối tượng cha
            _giamGiaRepo.Delete(entityToDelete);

            // Lưu lại các thay đổi
            await _giamGiaRepo.SaveAsync();

            return true;
        }
    }
}