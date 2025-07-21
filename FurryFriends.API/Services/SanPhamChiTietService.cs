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
                SanPhamChiTietId = x.SanPhamChiTietId,
                SanPhamId = x.SanPhamId,
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
            var x = await _repository.GetByIdAsync(id);
            if (x == null) return null;

            return new SanPhamChiTietDTO
            {
                SanPhamChiTietId = x.SanPhamChiTietId,
                SanPhamId = x.SanPhamId,
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
            };
        }

        public async Task<Guid> CreateAsync(SanPhamChiTietDTO dto)
        {
            var entity = new SanPhamChiTiet
            {
                SanPhamChiTietId = Guid.NewGuid(),
                SanPhamId = dto.SanPhamId,
                KichCoId = dto.KichCoId,
                MauSacId = dto.MauSacId,
                Gia = dto.Gia,
                SoLuong = dto.SoLuong,
                MoTa = dto.MoTa,
                AnhId = dto.AnhId,
                NgayTao = DateTime.UtcNow,
                TrangThai = dto.TrangThai // hoặc mặc định là 1 nếu cần
            };

            await _repository.AddAsync(entity);
            await _repository.SaveAsync();

            return entity.SanPhamChiTietId;
        }

        public async Task<bool> UpdateAsync(Guid id, SanPhamChiTietDTO dto)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return false;

            e.KichCoId = dto.KichCoId;
            e.MauSacId = dto.MauSacId;
            e.Gia = dto.Gia;
            e.SoLuong = dto.SoLuong;
            e.MoTa = dto.MoTa;
            e.AnhId = dto.AnhId;
            e.NgaySua = DateTime.UtcNow;
            e.TrangThai = dto.TrangThai;

            _repository.Update(e);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return false;

            _repository.Delete(e);
            await _repository.SaveAsync();
            return true;
        }
    }
}
