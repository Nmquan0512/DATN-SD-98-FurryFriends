using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services
{
	public class PhieuHoanTraService : IPhieuHoanTraService
	{
		private readonly HttpClient _httpClient;
		public PhieuHoanTraService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<PhieuHoanTraViewModel>> GetByHoaDonIdAsync(Guid hoaDonId)
		{
			return await _httpClient.GetFromJsonAsync<IEnumerable<PhieuHoanTraViewModel>>($"api/PhieuHoanTra/GetByHoaDon/{hoaDonId}");
		}

		public async Task<PhieuHoanTraViewModel?> GetByIdAsync(Guid id)
		{
			return await _httpClient.GetFromJsonAsync<PhieuHoanTraViewModel>($"api/PhieuHoanTra/{id}");
		}

		public async Task<bool> CreateAsync(PhieuHoanTraViewModel model)
		{
			var response = await _httpClient.PostAsJsonAsync("api/PhieuHoanTra", model);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateTrangThaiAsync(Guid id, int trangThai)
		{
			var response = await _httpClient.PutAsJsonAsync($"api/PhieuHoanTra/{id}/trang-thai", trangThai);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var response = await _httpClient.DeleteAsync($"api/PhieuHoanTra/{id}");
			return response.IsSuccessStatusCode;
		}
	}
}
