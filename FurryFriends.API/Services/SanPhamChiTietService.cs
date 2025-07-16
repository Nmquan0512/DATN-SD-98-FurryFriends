using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class SanPhamChiTietService : ISanPhamChiTietService
    {
        private readonly ISanPhamChiTietRepository _repository;

        public SanPhamChiTietService(ISanPhamChiTietRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SanPhamChiTietDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(x => new SanPhamChiTietDTO
            {
                Id = x.SanPhamChiTietId,
                SanPhamChiTietId = x.SanPhamChiTietId,
                KichCoId = x.KichCoId,
                TenKichCo = x.KichCo?.TenKichCo,
                MauSacId = x.MauSacId,
                TenMau = x.MauSac?.TenMau,
                Gia = x.Gia,
                SoLuong = x.SoLuong,
                MoTa = x.MoTa,
                AnhId = x.AnhId,
                DuongDan = x.Anh?.DuongDan,
                NgayTao = x.NgayTao,
                NgaySua = x.NgaySua,
                TrangThai = x.TrangThai
            });
        }

        public async Task<SanPhamChiTietDTO?> GetByIdAsync(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return null;

            return new SanPhamChiTietDTO
            {
                Id = item.SanPhamChiTietId,
                SanPhamChiTietId = item.SanPhamChiTietId,
                KichCoId = item.KichCoId,
                TenKichCo = item.KichCo?.TenKichCo,
                MauSacId = item.MauSacId,
                TenMau = item.MauSac?.TenMau,
                Gia = item.Gia,
                SoLuong = item.SoLuong,
                MoTa = item.MoTa,
                AnhId = item.AnhId,
                DuongDan = item.Anh?.DuongDan,
                NgayTao = item.NgayTao,
                NgaySua = item.NgaySua,
                TrangThai = item.TrangThai
            };
        }

        public async Task<bool> CreateAsync(SanPhamChiTietDTO dto)
        {
            var entity = new SanPhamChiTiet
            {
                SanPhamChiTietId = Guid.NewGuid(),
                SanPhamId = dto.Id,
                KichCoId = dto.KichCoId,
                MauSacId = dto.MauSacId,
                Gia = dto.Gia,
                SoLuong = dto.SoLuong,
                MoTa = dto.MoTa,
                AnhId = dto.AnhId, // chỉ lưu Id, không upload ảnh
                NgayTao = DateTime.Now,
                TrangThai = dto.TrangThai ?? 1
            };

            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, SanPhamChiTietDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.KichCoId = dto.KichCoId;
            entity.MauSacId = dto.MauSacId;
            entity.Gia = dto.Gia;
            entity.SoLuong = dto.SoLuong;
            entity.MoTa = dto.MoTa;
            entity.AnhId = dto.AnhId; // cập nhật ảnh nếu có
            entity.NgaySua = DateTime.Now;
            entity.TrangThai = dto.TrangThai ?? entity.TrangThai;

            _repository.Update(entity);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveAsync();
            return true;
        }
    }
}
