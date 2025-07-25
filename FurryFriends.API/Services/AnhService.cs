﻿using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Services
{
    public class AnhService : IAnhService
    {
        private readonly IAnhRepository _repository;
        private readonly IWebHostEnvironment _env;

        public AnhService(IAnhRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<IEnumerable<AnhDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(x => new AnhDTO
            {
                AnhId = x.AnhId,
                SanPhamChiTietId = x.SanPhamChiTietId,
                DuongDan = x.DuongDan,
                TenAnh = x.TenAnh,
                TrangThai = x.TrangThai
            });
        }

        public async Task<AnhDTO?> GetByIdAsync(Guid id)
        {
            var anh = await _repository.GetByIdAsync(id);
            if (anh == null) return null;

            return new AnhDTO
            {
                AnhId = anh.AnhId,
                SanPhamChiTietId = anh.SanPhamChiTietId,
                DuongDan = anh.DuongDan,
                TenAnh = anh.TenAnh,
                TrangThai = anh.TrangThai
            };
        }

        public async Task<AnhDTO?> UploadAsync(IFormFile file, Guid? sanPhamChiTietId = null)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var anh = new Anh
            {
                AnhId = Guid.NewGuid(),
                SanPhamChiTietId = sanPhamChiTietId ?? Guid.Empty,
                DuongDan = $"/images/{fileName}",
                TenAnh = Path.GetFileNameWithoutExtension(file.FileName),
                TrangThai = true
            };

            await _repository.AddAsync(anh);
            await _repository.SaveAsync();

            return new AnhDTO
            {
                AnhId = anh.AnhId,
                SanPhamChiTietId = anh.SanPhamChiTietId,
                DuongDan = anh.DuongDan,
                TenAnh = anh.TenAnh,
                TrangThai = anh.TrangThai
            };
        }

        public async Task<bool> UpdateAsync(Guid id, AnhDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.TenAnh = dto.TenAnh;
            entity.TrangThai = dto.TrangThai;
            entity.DuongDan = dto.DuongDan;
            entity.SanPhamChiTietId = dto.SanPhamChiTietId;

            _repository.Update(entity);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            // Xoá file vật lý nếu tồn tại
            var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", entity.DuongDan.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);

            _repository.Delete(entity);
            await _repository.SaveAsync();
            return true;
        }
    }
}
