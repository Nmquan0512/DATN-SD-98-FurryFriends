namespace FurryFriends.API.Models.DTO
{
    public class AddToCartDTO
    {
        public Guid KhachHangId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public int SoLuong { get; set; }

        public Guid? VoucherId { get; set; }
    }
}
