    namespace FurryFriends.Web.ViewModels
    {
        public class SanPhamViewModel
        {
            public Guid SanPhamChiTietId { get; set; } //sửa ơ đây thêm guid cho sp chi tiết
            public Guid SanPhamId { get; set; }
            public string TenSanPham { get; set; } = "";
            public string MoTa { get; set; } = "";
            public bool TrangThai { get; set; }

            public string? AnhDaiDienUrl { get; set; } // ảnh đầu tiên của chi tiết

            public List<SanPhamChiTietViewModel> ChiTietList { get; set; } = new();
        }
    }
