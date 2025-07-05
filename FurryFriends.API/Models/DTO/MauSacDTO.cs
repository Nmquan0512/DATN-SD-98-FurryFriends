using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class MauSacDTO
    {
        public Guid MauSacId { get; set; }

        [Required]
        [StringLength(50)]
        public string TenMau { get; set; }

        public string MaMau { get; set; }

        public string MoTa { get; set; }

        public bool TrangThai { get; set; }
    }
}
