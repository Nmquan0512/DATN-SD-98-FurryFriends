using FurryFriends.API.Models.DTO;
using FurryFriends.API.Models.Enums;
namespace FurryFriends.Web.ViewModels
{
    public class SanPhamChiTietCreateViewModel
    {
        public Guid SanPhamChiTietId { get; set; } = Guid.NewGuid();
        public Guid MauSacId { get; set; }
        public Guid KichCoId { get; set; }
        public int SoLuongTon { get; set; }
        public decimal GiaBan { get; set; }
        public string? MoTa { get; set; }
        public List<IFormFile>? Files { get; set; } = new();
        public List<KichCoDTO> KichCoList { get; set; } = new();
        public List<MauSacDTO> MauSacList { get; set; } = new();
    }

}
