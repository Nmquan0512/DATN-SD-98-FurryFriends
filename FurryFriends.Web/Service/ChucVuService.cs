using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.Web.Service.IService;

namespace FurryFriends.Web.Service
{
	public class ChucVuService : IChucVuService
	{
		private readonly IChucVuRepository _chucVuRepository;

		public ChucVuService(IChucVuRepository chucVuRepository)
		{
			_chucVuRepository = chucVuRepository;
		}

		public async Task<IEnumerable<ChucVu>> GetAllAsync()
		{
			return await _chucVuRepository.GetAllAsync();
		}

		public async Task<ChucVu?> GetByIdAsync(Guid chucVuId)
		{
			if (chucVuId == Guid.Empty)
				throw new ArgumentException("ChucVuId không hợp lệ.");

			return await _chucVuRepository.GetByIdAsync(chucVuId);
		}

		public async Task AddAsync(ChucVu chucVu)
		{
			if (chucVu == null)
				throw new ArgumentNullException(nameof(chucVu));

			// Validation
			if (string.IsNullOrWhiteSpace(chucVu.TenChucVu))
				throw new ArgumentException("Tên chức vụ không được để trống.");
			if (chucVu.TenChucVu.Length > 50)
				throw new ArgumentException("Tên chức vụ không được vượt quá 50 ký tự.");
			if (string.IsNullOrWhiteSpace(chucVu.MoTaChucVu))
				throw new ArgumentException("Mô tả chức vụ không được để trống.");
			if (chucVu.MoTaChucVu.Length > 250)
				throw new ArgumentException("Mô tả chức vụ không được vượt quá 250 ký tự.");
			if (chucVu.NgayTao == default)
				chucVu.NgayTao = DateTime.Now;
			if (chucVu.NgayCapNhat == default)
				chucVu.NgayCapNhat = DateTime.Now;

			// Bỏ qua navigation property
			chucVu.NhanViens = null;

			// Repository sẽ thêm ChucVu
			await _chucVuRepository.AddAsync(chucVu);
		}

		public async Task UpdateAsync(ChucVu chucVu)
		{
			if (chucVu == null)
				throw new ArgumentNullException(nameof(chucVu));
			if (chucVu.ChucVuId == Guid.Empty)
				throw new ArgumentException("ChucVuId không hợp lệ.");

			// Validation
			if (string.IsNullOrWhiteSpace(chucVu.TenChucVu))
				throw new ArgumentException("Tên chức vụ không được để trống.");
			if (chucVu.TenChucVu.Length > 50)
				throw new ArgumentException("Tên chức vụ không được vượt quá 50 ký tự.");
			if (string.IsNullOrWhiteSpace(chucVu.MoTaChucVu))
				throw new ArgumentException("Mô tả chức vụ không được để trống.");
			if (chucVu.MoTaChucVu.Length > 250)
				throw new ArgumentException("Mô tả chức vụ không được vượt quá 250 ký tự.");
			if (chucVu.NgayTao == default)
				throw new ArgumentException("Ngày tạo không được để trống.");
			if (chucVu.NgayCapNhat == default)
				chucVu.NgayCapNhat = DateTime.Now;

			// Bỏ qua navigation property
			chucVu.NhanViens = null;

			// Repository sẽ cập nhật ChucVu
			await _chucVuRepository.UpdateAsync(chucVu);
		}

		public async Task DeleteAsync(Guid chucVuId)
		{
			if (chucVuId == Guid.Empty)
				throw new ArgumentException("ChucVuId không hợp lệ.");

			// Repository sẽ kiểm tra liên kết với NhanVien
			await _chucVuRepository.DeleteAsync(chucVuId);
		}

		public async Task<IEnumerable<ChucVu>> FindByTenChucVuAsync(string tenChucVu)
		{
			if (string.IsNullOrWhiteSpace(tenChucVu))
				throw new ArgumentException("Tên chức vụ không được để trống.");

			return await _chucVuRepository.FindByTenChucVuAsync(tenChucVu);
		}
	}
}
