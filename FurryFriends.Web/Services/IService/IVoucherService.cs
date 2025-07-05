using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.Web.Services.IService
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(Voucher voucher);
        Task<bool> UpdateAsync(Guid id, Voucher voucher);
        Task<bool> DeleteAsync(Guid id);
    }
}