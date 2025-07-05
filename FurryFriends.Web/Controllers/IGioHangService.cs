
namespace FurryFriends.Web.Controllers
{
    public interface IGioHangService
    {
        Task<string?> GetGioHangByKhachHangIdAsync(Guid id);
    }
}