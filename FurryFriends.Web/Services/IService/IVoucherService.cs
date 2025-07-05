using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IService
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(Guid id);
        Task<Voucher> CreateAsync(Voucher voucher);
        Task<Voucher?> UpdateAsync(Guid id, Voucher voucher);
        Task<bool> DeleteAsync(Guid id);
    }
}
