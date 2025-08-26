using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services
{
	public class PhieuHoanTraService : IPhieuHoanTraService
	{
		private readonly HttpClient _httpClient;
		private const string baseUrl = "https://localhost:7289/api/PhieuHoanTra"; // URL API

		public PhieuHoanTraService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<PhieuHoanTraViewModel>> GetAllAsync()
		{
			var response = await _httpClient.GetAsync(baseUrl);
			if (!response.IsSuccessStatusCode) return new List<PhieuHoanTraViewModel>();

			return await response.Content.ReadFromJsonAsync<IEnumerable<PhieuHoanTraViewModel>>();
		}

		public async Task<PhieuHoanTraViewModel> GetByIdAsync(Guid id)
		{
			var response = await _httpClient.GetAsync($"{baseUrl}/{id}");
			if (!response.IsSuccessStatusCode) return null;

			return await response.Content.ReadFromJsonAsync<PhieuHoanTraViewModel>();
		}

		public async Task<IEnumerable<PhieuHoanTraViewModel>> GetByKhachHangAsync(Guid khachHangId)
		{
			var response = await _httpClient.GetAsync($"{baseUrl}/khachhang/{khachHangId}");
			if (!response.IsSuccessStatusCode) return new List<PhieuHoanTraViewModel>();

			return await response.Content.ReadFromJsonAsync<IEnumerable<PhieuHoanTraViewModel>>();
		}

		public async Task<bool> CreateAsync(PhieuHoanTraCreateRequest request)
		{
			var response = await _httpClient.PostAsJsonAsync(baseUrl, request);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> UpdateAsync(Guid id, PhieuHoanTraUpdateRequest request)
		{
			var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/{id}", request);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var response = await _httpClient.DeleteAsync($"{baseUrl}/{id}");
			return response.IsSuccessStatusCode;
		}
	}
}
