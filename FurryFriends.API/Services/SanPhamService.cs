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

        public async Task<IEnumerable<SanPhamDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDTO);
        }

        public async Task<SanPhamDTO> GetByIdAsync(Guid id)
        {
            var sp = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            return MapToDTO(sp);
        }

        public async Task<SanPhamDTO> CreateAsync(SanPhamDTO dto)
        {
            var sanPham = new SanPham
            {
                SanPhamId = Guid.NewGuid(),
                TenSanPham = dto.TenSanPham,
                ThuongHieuId = dto.ThuongHieuId,
                TrangThai = dto.TrangThai,
                SanPhamThanhPhans = new List<SanPhamThanhPhan>(),
                SanPhamChatLieus = new List<SanPhamChatLieu>()
            };

            // Xử lý thành phần nếu là đồ ăn
            if (dto.LoaiSanPham == "DoAn" && dto.ThanhPhanIds != null)
            {
                foreach (var tpId in dto.ThanhPhanIds)
                {
                    sanPham.SanPhamThanhPhans.Add(new SanPhamThanhPhan
                    {
                        SanPhamId = sanPham.SanPhamId,
                        ThanhPhanId = tpId
                    });
                }
            }
            // Xử lý chất liệu nếu là đồ dùng
            else if (dto.LoaiSanPham == "DoDung" && dto.ChatLieuIds != null)
            {
                foreach (var clId in dto.ChatLieuIds)
                {
                    sanPham.SanPhamChatLieus.Add(new SanPhamChatLieu
                    {
                        SanPhamId = sanPham.SanPhamId,
                        ChatLieuId = clId
                    });
                }
            }

            await _repository.AddAsync(sanPham);
            await _context.SaveChangesAsync();

            dto.SanPhamId = sanPham.SanPhamId;
            return dto;
        }

        public async Task UpdateAsync(Guid id, SanPhamDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Không tìm thấy sản phẩm với ID {id}");

            existing.TenSanPham = dto.TenSanPham;
            existing.ThuongHieuId = dto.ThuongHieuId;
            existing.TrangThai = dto.TrangThai;

            // Xóa quan hệ cũ
            _context.SanPhamThanhPhans.RemoveRange(existing.SanPhamThanhPhans);
            _context.SanPhamChatLieus.RemoveRange(existing.SanPhamChatLieus);

            // Thêm quan hệ mới
            if (dto.LoaiSanPham == "DoAn" && dto.ThanhPhanIds != null)
            {
                existing.SanPhamThanhPhans = dto.ThanhPhanIds
                    .Select(tpId => new SanPhamThanhPhan
                    {
                        SanPhamId = id,
                        ThanhPhanId = tpId
                    }).ToList();
            }
            else if (dto.LoaiSanPham == "DoDung" && dto.ChatLieuIds != null)
            {
                existing.SanPhamChatLieus = dto.ChatLieuIds
                    .Select(clId => new SanPhamChatLieu
                    {
                        SanPhamId = id,
                        ChatLieuId = clId
                    }).ToList();
            }

            await _repository.UpdateAsync(existing);
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
            var hoaDonChiTiets = await _context.HoaDonChiTiets
                .GroupBy(ct => ct.SanPhamId)
                .Select(g => new
                {
                    SanPhamId = g.Key,
                    TotalSold = g.Sum(x => x.SoLuongSanPham)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(top)
                .ToListAsync();

            var ids = hoaDonChiTiets.Select(x => x.SanPhamId).ToList();
            var sanPhams = await _repository.GetAllAsync();

            return sanPhams
                .Where(sp => ids.Contains(sp.SanPhamId))
                .Select(MapToDTO);
        }

        private static SanPhamDTO MapToDTO(SanPham x)
        {
            return new SanPhamDTO
            {
                SanPhamId = x.SanPhamId,
                TenSanPham = x.TenSanPham,
                ThuongHieuId = x.ThuongHieuId ?? Guid.Empty,
                TenThuongHieu = x.ThuongHieu?.TenThuongHieu,
                LoaiSanPham = x.SanPhamThanhPhans.Any() ? "DoAn" : "DoDung",
                ThanhPhanIds = x.SanPhamThanhPhans?.Select(tp => tp.ThanhPhanId).ToList(),
                ChatLieuIds = x.SanPhamChatLieus?.Select(cl => cl.ChatLieuId).ToList(),
                TenThanhPhans = x.SanPhamThanhPhans?.Select(tp => tp.ThanhPhan?.TenThanhPhan).ToList(),
                TenChatLieus = x.SanPhamChatLieus?.Select(cl => cl.ChatLieu?.TenChatLieu).ToList(),
                TrangThai = x.TrangThai
            };
        }
    }
}