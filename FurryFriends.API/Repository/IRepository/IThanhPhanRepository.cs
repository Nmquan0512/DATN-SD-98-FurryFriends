using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repositories
{
	public interface IThanhPhanRepository : IRepository<ThanhPhan>
	{
		Task<IEnumerable<ThanhPhan>> GetActiveAsync(); // Lấy danh sách trạng thái active (TrangThai == true)
	}
}
