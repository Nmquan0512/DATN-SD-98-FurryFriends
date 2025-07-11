using FurryFriends.API.Models;
using FurryFriends.API.Repositories;

namespace FurryFriends.API.Repository.IRepository
{
	public interface ISanPhamRepository:IRepository<SanPham>
	{
		Task DeleteAsync(Guid id);
        Task UpdateAsync(SanPham existing);
    }
	
	
}
