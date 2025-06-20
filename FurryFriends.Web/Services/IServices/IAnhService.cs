
	using FurryFriends.API.Models;


	namespace FurryFriends.API.Services
	{
		public interface IAnhService
		{
			Task<IEnumerable<Anh>> GetAllAsync();
			Task<Anh?> GetByIdAsync(Guid id);
			Task<Anh> CreateAsync(Anh model);
			Task<bool> UpdateAsync(Guid id, Anh model);
			Task<bool> DeleteAsync(Guid id);
		}
	}

