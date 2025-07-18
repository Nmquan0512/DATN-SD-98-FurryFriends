using FurryFriends.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Repository.IRepository
{
	public interface ISanPhamThanhPhanRepository : IRepository<SanPhamThanhPhan>
	{
		Task<IEnumerable<SanPhamThanhPhan>> GetBySanPhamIdAsync(Guid sanPhamId);
		Task<IEnumerable<SanPhamThanhPhan>> GetByThanhPhanIdAsync(Guid thanhPhanId);
		Task DeleteBySanPhamIdAsync(Guid sanPhamId);
		Task DeleteByThanhPhanIdAsync(Guid thanhPhanId);
	}
}
