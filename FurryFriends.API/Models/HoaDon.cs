using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class HoaDon : IValidatableObject
    {
        [Key]
        public Guid HoaDonId { get; set; }

        public Guid? VoucherId { get; set; }

        [Required]
        public Guid  KhachHangId { get; set; }

        [Required]
        public Guid HinhThucThanhToanId { get; set; }

        //[Required(ErrorMessage = "Tên khách hàng không được để trống")]
        //[StringLength(100, ErrorMessage = "Tên khách hàng tối đa 100 ký tự")]
        public string TenCuaKhachHang { get; set; }
        //[Required(ErrorMessage = "Số điện thoại không được để trống")]
        //[Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SdtCuaKhachHang { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EmailCuaKhachHang { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }
        public DateTime? NgayNhanHang { get; set; }
        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không được âm")]
        public decimal TongTien { get; set; }

        [Required(ErrorMessage = "Tổng tiền sau khi giảm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền sau khi giảm không được âm")]
        public decimal TongTienSauKhiGiam { get; set; }

        [Required]
        public int TrangThai { get; set; }

        public string ? GhiChu { get; set; }

        [ForeignKey("VoucherId")]
        public virtual Voucher Voucher { get; set; }

        [ForeignKey("KhachHangId")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("HinhThucThanhToanId")]
        public virtual HinhThucThanhToan HinhThucThanhToan { get; set; }
        
        // Hợp nhất: Địa chỉ giao hàng có thể có hoặc không tùy vào loại hóa đơn
        public Guid? DiaChiGiaoHangId { get; set; }

        [ForeignKey("DiaChiGiaoHangId")]
        public virtual DiaChiKhachHang? DiaChiGiaoHang { get; set; }
        
        // Hợp nhất: NhanVienId có thể có hoặc không
        public Guid? NhanVienId { get; set; }

        [ForeignKey("NhanVienId")]
        public virtual NhanVien? NhanVien { get; set; }

        public string LoaiHoaDon { get; set; } // Loại hóa đơn (ví dụ: "BanTaiQuay", "Online", ...)

        // ✅ Snapshot thông tin voucher lúc mua
        public string? ThongTinVoucherLucMua { get; set; }

        // ✅ Snapshot địa chỉ giao hàng lúc mua
        public string? DiaChiGiaoHangLucMua { get; set; }

        // ✅ Thời gian thay đổi trạng thái - lưu thời gian thực
        public DateTime? ThoiGianThayDoiTrangThai { get; set; }

        public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; }

        // ✅ Lịch sử thay đổi trạng thái chi tiết
        public virtual ICollection<LichSuTrangThaiHoaDon> LichSuTrangThaiHoaDons { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TongTienSauKhiGiam > TongTien)
            {
                yield return new ValidationResult(
                    "Tổng tiền sau khi giảm không được lớn hơn tổng tiền.",
                    new[] { nameof(TongTienSauKhiGiam) });
            }

            if (NgayNhanHang.HasValue && NgayNhanHang.Value < NgayTao)
            {
                yield return new ValidationResult(
                    "Ngày nhận hàng không được trước ngày tạo.",
                    new[] { nameof(NgayNhanHang) });
            }

            // Validation tùy chỉnh cho từng loại hóa đơn
            if (LoaiHoaDon == "BanTaiQuay")
            {
                // Bắt buộc phải có nhân viên tạo hóa đơn
                if (NhanVienId == Guid.Empty || NhanVienId == null)
                {
                    yield return new ValidationResult(
                        "Hóa đơn bán tại quầy phải có nhân viên tạo.",
                        new[] { nameof(NhanVienId) });
                }
                // Địa chỉ giao hàng không bắt buộc
                if (DiaChiGiaoHangId != null && DiaChiGiaoHangId != Guid.Empty)
                {
                    yield return new ValidationResult(
                        "Hóa đơn bán tại quầy không được có địa chỉ giao hàng.",
                        new[] { nameof(DiaChiGiaoHangId) });
                }
            }
            else if (LoaiHoaDon == "Online")
            {
                // Bắt buộc phải có địa chỉ giao hàng
                if (DiaChiGiaoHangId == Guid.Empty || DiaChiGiaoHangId == null)
                {
                    yield return new ValidationResult(
                        "Hóa đơn online phải có địa chỉ giao hàng.",
                        new[] { nameof(DiaChiGiaoHangId) });
                }
            }
        }
    }
}