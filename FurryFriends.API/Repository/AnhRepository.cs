using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Repository
{
	public class AnhRepository:IAnhRepository
	{
		private readonly AppDbContext _context;
		private readonly string _uploadsFolder;
		private readonly IWebHostEnvironment _env;

		public AnhRepository(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
			_uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads", "Images");
		}

		async Task<List<Anh>> IAnhRepository.GetAllAsync()
		{
			return await _context.Anhs.ToListAsync();
		}

		public async Task<Anh> GetByIdAsync(Guid id)
		{
			return await _context.Anhs.FindAsync(id);
		}

		public async Task<Anh> UploadAsync(IFormFile file, string tenAnh)
		{
			if (file == null || file.Length == 0) return null;

			Directory.CreateDirectory(_uploadsFolder);
			var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(_uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			var anh = new Anh
			{
				DuongDan = Path.Combine("Uploads/Images", fileName).Replace("\\", "/"),
				TenAnh = tenAnh
			};
			_context.Anhs.Add(anh);
			await _context.SaveChangesAsync();
			return anh;
		}

		public async Task<Anh> UpdateAsync(Anh anh)
		{
			var existing = await _context.Anhs.FindAsync(anh.AnhId);
			if (existing == null) return null;

			existing.TenAnh = anh.TenAnh;
			existing.TrangThai = anh.TrangThai;
			await _context.SaveChangesAsync();
			return existing;
		}

		public async Task<Anh> UpdateFileAsync(Guid id, IFormFile file, string tenAnh)
		{
			var existing = await _context.Anhs.FindAsync(id);
			if (existing == null || file == null || file.Length == 0) return null;

			// Xóa file cũ
			var oldPath = Path.Combine(_env.ContentRootPath, existing.DuongDan);
			if (File.Exists(oldPath))
				File.Delete(oldPath);

			// Lưu file mới
			Directory.CreateDirectory(_uploadsFolder);
			var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(_uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			existing.DuongDan = Path.Combine("Uploads/Images", fileName).Replace("\\", "/");
			existing.TenAnh = tenAnh ?? existing.TenAnh;
			await _context.SaveChangesAsync();
			return existing;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var anh = await _context.Anhs.FindAsync(id);
			if (anh == null) return false;

			// Xóa file vật lý
			var fullPath = Path.Combine(_env.ContentRootPath, anh.DuongDan);
			if (File.Exists(fullPath))
				File.Delete(fullPath);

			_context.Anhs.Remove(anh);
			await _context.SaveChangesAsync();
			return true;
		}

		
	}
}
