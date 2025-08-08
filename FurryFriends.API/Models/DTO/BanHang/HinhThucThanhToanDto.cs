using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO.BanHang
{
    public class HinhThucThanhToanDto
    {
        public Guid HinhThucThanhToanId { get; set; }

        [Required(ErrorMessage = "Tên hình thức thanh toán là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên hình thức không vượt quá 100 ký tự")]
        public string TenHinhThuc { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không vượt quá 500 ký tự")]
        public string MoTa { get; set; }

        // Loại hình thức: 1-Tiền mặt, 2-Chuyển khoản, 3-Thẻ, 4-Ví điện tử
        [Required(ErrorMessage = "Loại hình thức là bắt buộc")]
        [Range(1, 4, ErrorMessage = "Loại hình thức không hợp lệ")]
        public int LoaiHinhThuc { get; set; }

        // Trạng thái hoạt động (true: đang sử dụng, false: ngừng sử dụng)
        public bool TrangThai { get; set; } = true;

        // Thông tin bổ sung cho từng loại
        public string ThongTinBoSung { get; set; }

        // Phí giao dịch (%)
        [Range(0, 100, ErrorMessage = "Phí giao dịch phải từ 0-100%")]
        public decimal PhiGiaoDich { get; set; } = 0;

        // Icon hiển thị (font-awesome hoặc URL)
        public string Icon { get; set; }

        // Phương thức chuyển đổi tên loại
        public string TenLoaiHinhThuc => LoaiHinhThuc switch
        {
            1 => "Tiền mặt",
            2 => "Chuyển khoản",
            3 => "Thẻ",
            4 => "Ví điện tử",
            _ => "Không xác định"
        };
    }
}