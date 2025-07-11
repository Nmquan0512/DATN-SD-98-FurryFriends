using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.Models.DTO
{
    public class ChatLieuDTO : IValidatableObject
    {
        public Guid ChatLieuId { get; set; }

        [Required(ErrorMessage = "Tên chất liệu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string TenChatLieu { get; set; }

        public string MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var _context = (AppDbContext)validationContext.GetService(typeof(AppDbContext));

            if (_context != null)
            {
                var isDuplicate = _context.ChatLieus
                    .Any(x => x.TenChatLieu.ToLower().Trim() == TenChatLieu.ToLower().Trim()
                           && x.ChatLieuId != ChatLieuId);

                if (isDuplicate)
                {
                    yield return new ValidationResult(
                        "Tên chất liệu đã tồn tại.",
                        new[] { nameof(TenChatLieu) });
                }
            }
        }
    }
}