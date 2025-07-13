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
            return list.Select(x => new SanPhamDTO
            {
                SanPhamId = x.SanPhamId,
                TenSanPham = x.TenSanPham,
                ThuongHieuId = x.ThuongHieuId,
                TenThuongHieu = x.ThuongHieu?.TenThuongHieu,
                LoaiSanPham = x.SanPhamThanhPhans.Any() ? "DoAn" : "DoDung",
                TenThanhPhans = x.SanPhamThanhPhans?.Select(tp => tp.ThanhPhan?.TenThanhPhan).ToList(),
                TenChatLieus = x.SanPhamChatLieus?.Select(cl => cl.ChatLieu?.TenChatLieu).ToList(),
                NgayTao = x.TaiKhoan?.NgayTaoTaiKhoan ?? DateTime.UtcNow,
                NgaySua = x.TaiKhoan?.NgayCapNhatCuoiCung,
                TrangThai = x.TrangThai
            });
        }

        public async Task<SanPhamDTO> GetByIdAsync(Guid id)
        {
            var sp = await _repository.GetByIdAsync(id);
            if (sp == null) return null;

            return new SanPhamDTO
            {
                SanPhamId = sp.SanPhamId,
                TenSanPham = sp.TenSanPham,
                ThuongHieuId = sp.ThuongHieuId,
                TenThuongHieu = sp.ThuongHieu?.TenThuongHieu,
                LoaiSanPham = sp.SanPhamThanhPhans.Any() ? "DoAn" : "DoDung",
                ThanhPhanIds = sp.SanPhamThanhPhans?.Select(tp => tp.ThanhPhanId).ToList(),
                ChatLieuIds = sp.SanPhamChatLieus?.Select(cl => cl.ChatLieuId).ToList(),
                TenThanhPhans = sp.SanPhamThanhPhans?.Select(tp => tp.ThanhPhan?.TenThanhPhan).ToList(),
                TenChatLieus = sp.SanPhamChatLieus?.Select(cl => cl.ChatLieu?.TenChatLieu).ToList(),
                NgayTao = sp.TaiKhoan?.NgayTaoTaiKhoan ?? DateTime.UtcNow,
                NgaySua = sp.TaiKhoan?.NgayCapNhatCuoiCung,
                TrangThai = sp.TrangThai
            };
        }

        public async Task<SanPhamDTO> CreateAsync(SanPhamDTO dto)
        {
            var sanPham = new SanPham
            {
                SanPhamId = Guid.NewGuid(),
                TenSanPham = dto.TenSanPham,
                ThuongHieuId = dto.ThuongHieuId,
                TrangThai = dto.TrangThai,
                TaiKhoanId = _context.TaiKhoans.First().TaiKhoanId, // TODO: truyền từ tài khoản đăng nhập
                SanPhamThanhPhans = new List<SanPhamThanhPhan>(),
                SanPhamChatLieus = new List<SanPhamChatLieu>()
            };

            if (dto.LoaiSanPham == "DoAn" && dto.ThanhPhanIds != null)
            {
                foreach (var id in dto.ThanhPhanIds)
                {
                    sanPham.SanPhamThanhPhans.Add(new SanPhamThanhPhan
                    {
                        SanPhamId = sanPham.SanPhamId,
                        ThanhPhanId = id
                    });
                }
            }
            else if (dto.LoaiSanPham == "DoDung" && dto.ChatLieuIds != null)
            {
                foreach (var id in dto.ChatLieuIds)
                {
                    sanPham.SanPhamChatLieus.Add(new SanPhamChatLieu
                    {
                        SanPhamId = sanPham.SanPhamId,
                        ChatLieuId = id
                    });
                }
            }

            await _repository.AddAsync(sanPham);
            dto.SanPhamId = sanPham.SanPhamId;
            return dto;
        }

        public async Task<bool> UpdateAsync(Guid id, SanPhamDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.TenSanPham = dto.TenSanPham;
            existing.ThuongHieuId = dto.ThuongHieuId;
            existing.TrangThai = dto.TrangThai;

            existing.SanPhamThanhPhans?.Clear();
            existing.SanPhamChatLieus?.Clear();

            if (dto.LoaiSanPham == "DoAn" && dto.ThanhPhanIds != null)
            {
                existing.SanPhamThanhPhans = dto.ThanhPhanIds.Select(tpId => new SanPhamThanhPhan
                {
                    SanPhamId = existing.SanPhamId,
                    ThanhPhanId = tpId
                }).ToList();
            }
            else if (dto.LoaiSanPham == "DoDung" && dto.ChatLieuIds != null)
            {
                existing.SanPhamChatLieus = dto.ChatLieuIds.Select(clId => new SanPhamChatLieu
                {
                    SanPhamId = existing.SanPhamId,
                    ChatLieuId = clId
                }).ToList();
            }

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (!await _repository.ExistsAsync(id)) return false;
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<(IEnumerable<SanPhamDTO> Data, int TotalCount)> GetFilteredAsync(string? loaiSanPham, int page, int pageSize)
        {
            var all = await _repository.GetAllAsync();

            var filtered = all.Where(sp =>
                string.IsNullOrEmpty(loaiSanPham)
                || (loaiSanPham == "DoAn" && sp.SanPhamThanhPhans.Any())
                || (loaiSanPham == "DoDung" && sp.SanPhamChatLieus.Any())
            );

            var totalCount = filtered.Count();

            var paged = filtered
                .OrderByDescending(sp => sp.TaiKhoan?.NgayTaoTaiKhoan ?? DateTime.UtcNow)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sp => new SanPhamDTO
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    ThuongHieuId = sp.ThuongHieuId,
                    TenThuongHieu = sp.ThuongHieu?.TenThuongHieu,
                    LoaiSanPham = sp.SanPhamThanhPhans.Any() ? "DoAn" : "DoDung",
                    TenThanhPhans = sp.SanPhamThanhPhans?.Select(tp => tp.ThanhPhan?.TenThanhPhan).ToList(),
                    TenChatLieus = sp.SanPhamChatLieus?.Select(cl => cl.ChatLieu?.TenChatLieu).ToList(),
                    NgayTao = sp.TaiKhoan?.NgayTaoTaiKhoan ?? DateTime.UtcNow,
                    NgaySua = sp.TaiKhoan?.NgayCapNhatCuoiCung,
                    TrangThai = sp.TrangThai
                }).ToList();

            return (paged, totalCount);
        }
    }
}
