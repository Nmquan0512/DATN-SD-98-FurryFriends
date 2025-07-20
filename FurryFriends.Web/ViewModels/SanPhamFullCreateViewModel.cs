using FurryFriends.API.Models.DTO;
using System.ComponentModel;

namespace FurryFriends.Web.ViewModels
{
    public class SanPhamFullCreateViewModel
    {
        public SanPhamDTO SanPham { get; set; } = new();
        
        [DisplayName("Danh sách biến thể")]
        public List<SanPhamChiTietCreateViewModel> ChiTietList { get; set; } = new();
    }
}
