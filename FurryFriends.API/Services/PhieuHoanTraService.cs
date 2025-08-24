using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;

namespace FurryFriends.API.Services
{
	public class PhieuHoanTraService : IPhieuHoanTraService
	{
		private readonly IPhieuHoanTraRepository _repository;
		public PhieuHoanTraService(IPhieuHoanTraRepository repository)
		{
			_repository = repository;
		}
		public async Task<PhieuHoanTra?> GetByIdAsync(Guid id, bool includeHoaDonChiTiet = false)
		{
			return await _repository.GetByIdAsync(id, includeHoaDonChiTiet);
		}

		public async Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonIdAsync(Guid hoaDonId)
		{
			return await _repository.GetByHoaDonIdAsync(hoaDonId);
		}

		public async Task<IReadOnlyList<PhieuHoanTra>> GetByHoaDonChiTietIdAsync(Guid hoaDonChiTietId)
		{
			return await _repository.GetByHoaDonChiTietIdAsync(hoaDonChiTietId);
		}

		public async Task<int> GetTongSoLuongHoanByHdctAsync(Guid hoaDonChiTietId)
		{
			return await _repository.GetTongSoLuongHoanByHdctAsync(hoaDonChiTietId);
		}

		public async Task<PhieuHoanTra> AddAsync(PhieuHoanTra entity)
		{
			// 👉 có thể thêm validate trước khi insert
			if (entity.SoLuongHoan <= 0)
				throw new ArgumentException("Số lượng hoàn trả phải lớn hơn 0");

			return await _repository.AddAsync(entity);
		}

		public async Task UpdateAsync(PhieuHoanTra entity)
		{
			if (!await _repository.ExistsAsync(entity.PhieuHoanTraId))
				throw new KeyNotFoundException("Không tìm thấy phiếu hoàn trả để cập nhật");

			await _repository.UpdateAsync(entity);
		}

		public async Task UpdateTrangThaiAsync(Guid id, int trangThai)
		{
			if (!await _repository.ExistsAsync(id))
				throw new KeyNotFoundException("Không tìm thấy phiếu hoàn trả để đổi trạng thái");

			await _repository.UpdateTrangThaiAsync(id, trangThai);
		}

		public async Task DeleteAsync(Guid id)
		{
			if (!await _repository.ExistsAsync(id))
				throw new KeyNotFoundException("Không tìm thấy phiếu hoàn trả để xóa");

			await _repository.DeleteAsync(id);
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _repository.ExistsAsync(id);
		}
	}
}
