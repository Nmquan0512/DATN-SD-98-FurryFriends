using System;

namespace FurryFriends.API.Models.DTO.BanHang
{
    public class VoucherDto
    {
        public Guid VoucherId { get; set; }
        public string MaVoucher { get; set; }
        public string TenVoucher { get; set; }
        public decimal PhanTramGiam { get; set; }
        public decimal GiaTriGiamToiDa { get; set; }
        public DateTime NgayHetHan { get; set; }
        public bool ApDung { get; set; }
    }
}