using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
    public interface IGiamGiaRepository
    {
        Task<IEnumerable<GiamGia>> GetAllAsync(bool includeProducts = false);
        Task<GiamGia> GetByIdAsync(Guid id, bool includeProducts = false);
        Task AddAsync(GiamGia entity);
        void Update(GiamGia entity); // Sửa thành phương thức đồng bộ, không cần Save
        void Delete(GiamGia entity); // Sửa thành phương thức đồng bộ
        Task<bool> ExistsAsync(Guid id);
        Task<bool> TenGiamGiaExistsAsync(string tenGiamGia, Guid? excludeId = null);
        Task SaveAsync(); // Phương thức quan trọng để lưu tất cả thay đổi
    }
}