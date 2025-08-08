using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepository _repository;
        private readonly AppDbContext _context;

        public SanPhamService(ISanPhamRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // Phương thức này và các phương thức Get khác giữ nguyên, chúng ta chấp nhận vấn đề hiệu năng.
        public async Task<IEnumerable<SanPhamDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync(); // Giả sử GetAllAsync đã Include() các bảng liên quan
            return list.Select(MapToDTO);
        }

        public async Task<SanPhamDTO> GetByIdAsync(Guid id)
        {
            var sp = await _repository.GetByIdAsync(id) // Giả sử GetByIdAsync đã Include()
                ?? throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            return MapToDTO(sp);
        }

        public async Task<SanPhamDTO> CreateAsync(SanPhamDTO dto)
        {
            // Code của bạn ở đây đã tốt
            var sanPham = new SanPham
            {
                SanPhamId = Guid.NewGuid(),
                TenSanPham = dto.TenSanPham,
                ThuongHieuId = dto.ThuongHieuId,
                TrangThai = dto.TrangThai,
                SanPhamThanhPhans = new List<SanPhamThanhPhan>(),
                SanPhamChatLieus = new List<SanPhamChatLieu>()
            };

            if (dto.LoaiSanPham == "DoAn" && dto.ThanhPhanIds != null)
            {
                foreach (var tpId in dto.ThanhPhanIds)
                {
                    sanPham.SanPhamThanhPhans.Add(new SanPhamThanhPhan { SanPhamId = sanPham.SanPhamId, ThanhPhanId = tpId });
                }
            }
            else if (dto.LoaiSanPham == "DoDung" && dto.ChatLieuIds != null)
            {
                foreach (var clId in dto.ChatLieuIds)
                {
                    sanPham.SanPhamChatLieus.Add(new SanPhamChatLieu { SanPhamId = sanPham.SanPhamId, ChatLieuId = clId });
                }
            }

            await _repository.AddAsync(sanPham);
            await _repository.SaveAsync();
            dto.SanPhamId = sanPham.SanPhamId;
            return dto;
        }

        public async Task UpdateAsync(Guid id, SanPhamDTO dto)
        {
            // 1. Tải đối tượng cần cập nhật cùng với các collection liên quan
            var existing = await _context.SanPhams
                .Include(sp => sp.SanPhamThanhPhans)
                .Include(sp => sp.SanPhamChatLieus)
                .FirstOrDefaultAsync(sp => sp.SanPhamId == id)
                ?? throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            // 2. Cập nhật các thuộc tính chính của SanPham
            existing.TenSanPham = dto.TenSanPham;
            existing.ThuongHieuId = dto.ThuongHieuId;
            existing.TrangThai = dto.TrangThai;

            // 3. Xóa các quan hệ cũ một cách an toàn
            // Vì đã có constructor, .Clear() sẽ luôn an toàn, không bao giờ gây lỗi.
            existing.SanPhamThanhPhans.Clear();
            existing.SanPhamChatLieus.Clear();

            // 4. Thêm lại các quan hệ mới dựa trên DTO
            if (dto.LoaiSanPham == "DoAn" && dto.ThanhPhanIds != null)
            {
                foreach (var tpId in dto.ThanhPhanIds)
                {
                    existing.SanPhamThanhPhans.Add(new SanPhamThanhPhan { ThanhPhanId = tpId });
                }
            }
            else if (dto.LoaiSanPham == "DoDung" && dto.ChatLieuIds != null)
            {
                foreach (var clId in dto.ChatLieuIds)
                {
                    existing.SanPhamChatLieus.Add(new SanPhamChatLieu { ChatLieuId = clId });
                }
            }

            // 5. Lưu tất cả các thay đổi (cập nhật thuộc tính, xóa và thêm quan hệ) vào DB trong 1 lần duy nhất.
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _repository.ExistsAsync(id))
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            await _repository.DeleteAsync(id);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<SanPhamDTO> Data, int TotalCount)> GetFilteredAsync(string? loaiSanPham, int page, int pageSize)
        {
            var all = await _repository.GetAllAsync();

            var filtered = all.Where(sp =>
                string.IsNullOrEmpty(loaiSanPham) ||
                (loaiSanPham == "DoAn" && sp.SanPhamThanhPhans.Any()) ||
                (loaiSanPham == "DoDung" && sp.SanPhamChatLieus.Any())
            );

            var totalCount = filtered.Count();
            var paged = filtered
                .OrderByDescending(sp => sp.SanPhamId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDTO);

            return (paged, totalCount);
        }

        public async Task<int> GetTotalProductsAsync()
        {
            var all = await _repository.GetAllAsync();
            return all.Count();
        }

        public async Task<IEnumerable<SanPhamDTO>> GetTopSellingProductsAsync(int top)
        {
            // Bước 1: Bao gồm các quan hệ cần thiết để truy cập đến SanPhamId
            var topProductsInfo = await _context.HoaDonChiTiets
                .Include(ct => ct.SanPhamChiTiet)       // Include SanPhamChiTiet từ HoaDonChiTiet
                    .ThenInclude(spct => spct.SanPham)  // Include SanPham từ SanPhamChiTiet

                // Bước 2: SỬA LỖI Ở ĐÂY - Nhóm theo ID của sản phẩm chung
                .GroupBy(ct => ct.SanPhamChiTiet.SanPhamId)
                .Select(g => new
                {
                    SanPhamId = g.Key,
                    TotalSold = g.Sum(x => x.SoLuongSanPham)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(top)
                .ToListAsync();

            // Các bước còn lại vẫn đúng logic, không cần thay đổi
            var ids = topProductsInfo.Select(x => x.SanPhamId).ToList();

            // Giả sử _repository là repository của SanPham
            var allSanPhams = await _repository.GetAllAsync(); // Hàm này cần trả về tất cả sản phẩm

            var topSellingSanPhams = allSanPhams.Where(sp => ids.Contains(sp.SanPhamId));

            // Giả sử bạn có một hàm MapToDTO để chuyển đổi SanPham -> SanPhamDTO
            return topSellingSanPhams.Select(MapToDTO);
        }
        private static SanPhamDTO MapToDTO(SanPham x)
        {
            return new SanPhamDTO
            {
                SanPhamId = x.SanPhamId,
                TenSanPham = x.TenSanPham,
                ThuongHieuId = x.ThuongHieuId ?? Guid.Empty,
                TenThuongHieu = x.ThuongHieu?.TenThuongHieu,
                LoaiSanPham = (x.SanPhamThanhPhans?.Any() == true) ? "DoAn" : "DoDung",
                ThanhPhanIds = x.SanPhamThanhPhans?.Select(tp => tp.ThanhPhanId).ToList() ?? new List<Guid>(),
                ChatLieuIds = x.SanPhamChatLieus?.Select(cl => cl.ChatLieuId).ToList() ?? new List<Guid>(),
                TenThanhPhans = x.SanPhamThanhPhans?.Select(tp => tp.ThanhPhan?.TenThanhPhan).ToList() ?? new List<string>(),
                TenChatLieus = x.SanPhamChatLieus?.Select(cl => cl.ChatLieu?.TenChatLieu).ToList() ?? new List<string>(),
                TrangThai = x.TrangThai
            };
        }
    }
}