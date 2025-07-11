using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
    public class AnhRepository : IAnhRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _folder = "Uploads";

        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public AnhRepository(AppDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Anh>> GetAllAsync()
        {
            return _context.Anhs.ToList();
        }

        public async Task<Anh> GetByIdAsync(Guid id)
        {
            return await _context.Anhs.FindAsync(id);
        }

        public async Task<Anh> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                return null;

            string uploadsRoot = Path.Combine(_env.WebRootPath, _folder);
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            string fileName = Guid.NewGuid() + extension;
            string fullPath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo URL tuyệt đối (full URL)
            string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            string fullUrl = $"{baseUrl}/{_folder}/{fileName}";

            var anh = new Anh
            {
                TenAnh = fileName,
                DuongDan = fullUrl, // <-- URL đầy đủ
                TrangThai = true
            };

            _context.Anhs.Add(anh);
            await _context.SaveChangesAsync();
            return anh;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var anh = await _context.Anhs.FindAsync(id);
            if (anh == null) return false;

            // Trích tên file từ TenAnh để xóa file vật lý
            var fullPath = Path.Combine(_env.WebRootPath, _folder, anh.TenAnh);
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.Anhs.Remove(anh);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
