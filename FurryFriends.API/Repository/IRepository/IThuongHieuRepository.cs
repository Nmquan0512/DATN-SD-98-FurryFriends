using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
	public interface IThuongHieuRepository : IRepository<ThuongHieu>
	{
		Task<IEnumerable<ThuongHieu>> GetActiveBrandsAsync();
	}
}
