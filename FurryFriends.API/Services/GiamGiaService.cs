using AutoMapper;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Org.BouncyCastle.Crypto;
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
        private readonly IDotGiamGiaSanPhamRepository _dotGiamGiaRepo;
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
            return entity == null ? null : _mapper.Map<GiamGiaDTO>(entity);
        }

        public async Task<IEnumerable<GiamGiaDTO>> GetAllAsync()
        {
            var entities = await _giamGiaRepo.GetAllAsync(true);
            return _mapper.Map<IEnumerable<GiamGiaDTO>>(entities);
        }

        public async Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto)
        {
            // Validate cơ bản
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.PhanTramKhuyenMai <= 0 || dto.PhanTramKhuyenMai > 100)
                throw new ValidationException("Phần trăm giảm giá phải từ 1 đến 100");
            if (dto.NgayKetThuc <= dto.NgayBatDau)
                throw new ValidationException("Ngày kết thúc phải sau ngày bắt đầu");

            // Kiểm tra trùng tên
            if (await _giamGiaRepo.TenGiamGiaExistsAsync(dto.TenGiamGia))
                throw new InvalidOperationException("Tên chương trình giảm giá đã tồn tại");

            // Tạo mới
            var entity = _mapper.Map<GiamGia>(dto);
            await _giamGiaRepo.AddAsync(entity);

            // Áp dụng cho sản phẩm nếu có
            if (dto.SanPhamChiTietIds?.Any() == true)
            {
                await AssignProductsAsync(entity.GiamGiaId, dto.SanPhamChiTietIds);
            }
            var updatedEntity = await _giamGiaRepo.GetByIdAsync(entity.GiamGiaId, true);
            if (updatedEntity.DotGiamGiaSanPhams.Count != dto.SanPhamChiTietIds?.Count)
            {
                throw new Exception("Số lượng sản phẩm được áp dụng giảm giá không khớp");
            }

            return _mapper.Map<GiamGiaDTO>(entity);
        }

        public async Task<GiamGiaDTO> UpdateAsync(GiamGiaDTO dto)
        {
            // Validate cơ bản
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.PhanTramKhuyenMai <= 0 || dto.PhanTramKhuyenMai > 100)
                throw new ValidationException("Phần trăm giảm giá phải từ 1 đến 100");
            if (dto.NgayKetThuc <= dto.NgayBatDau)
                throw new ValidationException("Ngày kết thúc phải sau ngày bắt đầu");

            var existing = await _giamGiaRepo.GetByIdAsync(dto.GiamGiaId);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy chương trình giảm giá");

            // Kiểm tra trùng tên (trừ chính nó)
            if (await _giamGiaRepo.TenGiamGiaExistsAsync(dto.TenGiamGia, dto.GiamGiaId))
                throw new InvalidOperationException("Tên chương trình giảm giá đã tồn tại");

            // Cập nhật
            _mapper.Map(dto, existing);
            await _giamGiaRepo.UpdateAsync(existing);

            // Cập nhật sản phẩm nếu có
            if (dto.SanPhamChiTietIds != null)
            {
                await AssignProductsAsync(existing.GiamGiaId, dto.SanPhamChiTietIds);
            }

            return _mapper.Map<GiamGiaDTO>(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _dotGiamGiaRepo.DeleteByGiamGiaIdAsync(id);
            await _giamGiaRepo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> AssignProductsAsync(Guid giamGiaId, List<Guid> productIds)
        {
            // Xóa các sản phẩm cũ
            await _dotGiamGiaRepo.DeleteByGiamGiaIdAsync(giamGiaId);

            // Lấy thông tin giảm giá để lấy phần trăm
            var giamGia = await _giamGiaRepo.GetByIdAsync(giamGiaId);
            if (giamGia == null) return false;

            // Thêm các sản phẩm mới
            var dotGiamGias = productIds.Select(productId => new DotGiamGiaSanPham
            {
                GiamGiaId = giamGiaId,
                SanPhamChiTietId = productId,
                PhanTramGiamGia = giamGia.PhanTramKhuyenMai,
                TrangThai = true,
                NgayTao = DateTime.UtcNow,
                NgayCapNhat = DateTime.UtcNow
            }).ToList();

            await _dotGiamGiaRepo.AddRangeAsync(dotGiamGias);
            return true;
        }
    }
}