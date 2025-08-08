using FurryFriends.API.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FurryFriends.API.Models.DTO
{
    public class ChatLieuDTO : IValidatableObject
    {
        public Guid ChatLieuId { get; set; }

        [Required(ErrorMessage = "Tên chất liệu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string TenChatLieu { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Kiểm tra tên không được null hoặc rỗng
            if (string.IsNullOrWhiteSpace(TenChatLieu))
            {
                results.Add(new ValidationResult("Tên chất liệu là bắt buộc.", new[] { nameof(TenChatLieu) }));
                return results;
            }

            // Kiểm tra ký tự đặc biệt - chỉ từ chối một số ký tự đặc biệt nhất định
            if (TenChatLieu.Contains("%") || TenChatLieu.Contains("@") || TenChatLieu.Contains("#"))
            {
                results.Add(new ValidationResult("Tên chất liệu không được chứa ký tự đặc biệt.", new[] { nameof(TenChatLieu) }));
                return results;
            }

            // Kiểm tra trùng tên
            try
            {
                var context = (AppDbContext?)validationContext.GetService(typeof(AppDbContext));
                if (context != null)
                {
                    var isDuplicate = context.ChatLieus
                        .Any(x => x.TenChatLieu.ToLower().Trim() == TenChatLieu.ToLower().Trim()
                               && x.ChatLieuId != ChatLieuId);
                    if (isDuplicate)
                    {
                        results.Add(new ValidationResult("Tên chất liệu đã tồn tại.", new[] { nameof(TenChatLieu) }));
                    }
                }
            }
            catch
            {
                // Nếu không thể truy cập database, bỏ qua validation này
            }

            return results;
        }
    }
}