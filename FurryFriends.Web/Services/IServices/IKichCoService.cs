using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IServices
{
	public interface IKichCoService
	{
		Task<IEnumerable<KichCo>> GetAllAsync();
		Task<KichCo?> GetByIdAsync(Guid id);
		Task<KichCo> CreateAsync(KichCo model);
		Task<bool> UpdateAsync(Guid id, KichCo model);
		Task<bool> DeleteAsync(Guid id);
	}
}
