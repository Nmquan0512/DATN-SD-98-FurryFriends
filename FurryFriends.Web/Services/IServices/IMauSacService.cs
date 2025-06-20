using FurryFriends.API.Models;

namespace FurryFriends.Web.Services.IServices
{
	public interface IMauSacService
	{
		Task<IEnumerable<MauSac>> GetAllAsync();
		Task<MauSac?> GetByIdAsync(Guid id);
		Task<MauSac> CreateAsync(MauSac model);
		Task<bool> UpdateAsync(Guid id, MauSac model);
		Task<bool> DeleteAsync(Guid id);
	}
}
