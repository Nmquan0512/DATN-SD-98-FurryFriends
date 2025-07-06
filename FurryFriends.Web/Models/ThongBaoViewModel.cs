using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Models
{
    public class ThongBaoViewModel
    {
        public Guid ThongBaoId { get; set; }
        [Required]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }
        [Required]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }
        [Display(Name = "Đã đọc")]
        public bool DaDoc { get; set; }
        [Required]
        [Display(Name = "Loại")]
        public string Loai { get; set; }
        [Required]
        [Display(Name = "Người gửi")]
        public string UserName { get; set; }
    }
} 