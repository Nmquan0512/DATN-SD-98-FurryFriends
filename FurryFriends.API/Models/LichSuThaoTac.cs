namespace FurryFriends.API.Models
{
    public class LichSuThaoTac
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? TaiKhoan { get; set; } // hoặc Id người dùng

        public string HanhDong { get; set; } // VD: "Thêm sản phẩm", "Xóa khách hàng"...

        public string NoiDung { get; set; } // nội dung chi tiết

        public DateTime ThoiGian { get; set; } = DateTime.Now;
    }
}
