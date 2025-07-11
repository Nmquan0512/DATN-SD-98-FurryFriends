using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class HinhThucThanhToan
    {
        [Key]
        public Guid HinhThucThanhToanId { get; set; }

        [Required(ErrorMessage = "Tên hình thức thanh toán không được để trống")]
        [StringLength(100, ErrorMessage = "Tên hình thức tối đa 100 ký tự")]
        public string TenHinhThuc { get; set; }
        public string MoTa { get; set; }

        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}