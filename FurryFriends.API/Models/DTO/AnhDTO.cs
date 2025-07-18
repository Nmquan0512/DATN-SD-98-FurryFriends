using System;

namespace FurryFriends.API.Models.DTO
{
    public class AnhDTO
    {
        public Guid AnhId { get; set; }
        public Guid SanPhamChiTietId { get; set; }
        public string DuongDan { get; set; }

        public string TenAnh { get; set; }

        public bool TrangThai { get; set; }
    }
}
