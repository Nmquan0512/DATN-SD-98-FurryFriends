using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.Web.ViewModels;
namespace FurryFriends.Web.ViewModels
{
    public class SanPhamFullCreateViewModel
    {
        public SanPhamDTO SanPham { get; set; } = new();
        public List<SanPhamChiTietCreateViewModel> ChiTietList { get; set; } = new();


    }
}

