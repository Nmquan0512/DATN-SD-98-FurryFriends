using FurryFriends.API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    public interface IGiamGiaService
    {
        // Tạo mới giảm giá
        Task<GiamGiaDTO> CreateAsync(GiamGiaDTO dto);

        // Lấy thông tin giảm giá theo ID
        Task<GiamGiaDTO?> GetByIdAsync(Guid id);

        // Lấy tất cả giảm giá
        Task<IEnumerable<GiamGiaDTO>> GetAllAsync();

        // Cập nhật giảm giá
        Task<GiamGiaDTO?> UpdateAsync(Guid id, GiamGiaDTO dto); // ← Cập nhật để khớp class

        // Thêm sản phẩm chi tiết vào đợt giảm giá
        Task<bool> AddSanPhamChiTietToGiamGiaAsync(Guid giamGiaId, List<Guid> sanPhamChiTietIds);

        Task<bool> DeleteAsync(Guid id);
    }
}
