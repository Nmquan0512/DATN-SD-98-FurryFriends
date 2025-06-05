using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models
{
    public class ThuongHieu
    {

        [Key]
        public Guid ThuongHieuId { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Tên không được chứa ký tự đặc biệt.")]
        public string TenThuongHieu { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string SDT { get; set; }

        public string DiaChi { get; set; }

        public string MoTa { get; set; }

        public bool TrangThai { get; set; }
    }
}
