using FurryFriends.API.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FurryFriends.API.Repository.IRepository
{
	public interface IAnhRepository
	{
		Task<List<Anh>> GetAllAsync();
		Task<Anh> GetByIdAsync(Guid id);
		Task<Anh> UploadAsync(IFormFile file, string tenAnh);
		Task<Anh> UpdateAsync(Anh anh);
		Task<Anh> UpdateFileAsync(Guid id, IFormFile file, string tenAnh);
		Task<bool> DeleteAsync(Guid id);
	}
}
