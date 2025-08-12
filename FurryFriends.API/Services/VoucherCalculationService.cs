using FurryFriends.API.Models;

namespace FurryFriends.API.Services
{
    public class VoucherCalculationService
    {
        /// <summary>
        /// Kiểm tra voucher có hợp lệ để áp dụng hay không
        /// </summary>
        public bool IsVoucherValid(Voucher voucher, decimal tongTienHang, decimal phiVanChuyen = 0)
        {
            Console.WriteLine($"🔍 [IsVoucherValid] Bắt đầu kiểm tra voucher: {voucher?.MaVoucher}");
            
            if (voucher == null) 
            {
                Console.WriteLine($"❌ [IsVoucherValid] Voucher null");
                return false;
            }
            
            // Kiểm tra trạng thái active
            if (voucher.TrangThai != 1) 
            {
                Console.WriteLine($"❌ [IsVoucherValid] Trạng thái không hợp lệ: {voucher.TrangThai}");
                return false;
            }
            if (string.IsNullOrWhiteSpace(voucher.MaVoucher)) 
            {
                Console.WriteLine($"❌ [IsVoucherValid] Mã voucher rỗng");
                return false;
            }
            
            // Kiểm tra thời gian hiệu lực
            var now = DateTime.Now;
            Console.WriteLine($"🔍 [IsVoucherValid] Thời gian hiện tại: {now:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"🔍 [IsVoucherValid] Ngày bắt đầu: {voucher.NgayBatDau:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"🔍 [IsVoucherValid] Ngày kết thúc: {voucher.NgayKetThuc:dd/MM/yyyy HH:mm:ss}");
            
            if (now < voucher.NgayBatDau || now > voucher.NgayKetThuc) 
            {
                Console.WriteLine($"❌ [IsVoucherValid] Thời gian không hợp lệ");
                return false;
            }
            
            // Kiểm tra số lượng còn lại
            if (voucher.SoLuong <= 0) 
            {
                Console.WriteLine($"❌ [IsVoucherValid] Số lượng không hợp lệ: {voucher.SoLuong}");
                return false;
            }
            
            // Kiểm tra số tiền áp dụng tối thiểu (bao gồm phí ship)
            var tongDonHang = tongTienHang + phiVanChuyen;
            Console.WriteLine($"🔍 [IsVoucherValid] Tổng tiền hàng: {tongTienHang:N0}, Phí ship: {phiVanChuyen:N0}, Tổng đơn hàng: {tongDonHang:N0}");
            Console.WriteLine($"🔍 [IsVoucherValid] Điều kiện tối thiểu: {voucher.SoTienApDungToiThieu?.ToString() ?? "Không có"}");
            
            if (voucher.SoTienApDungToiThieu.HasValue && tongDonHang < voucher.SoTienApDungToiThieu.Value)
            {
                Console.WriteLine($"❌ [IsVoucherValid] Không đủ điều kiện tối thiểu: {tongDonHang:N0} < {voucher.SoTienApDungToiThieu.Value:N0}");
                return false;
            }
            
            Console.WriteLine($"✅ [IsVoucherValid] Voucher hợp lệ");
            return true;
        }

        /// <summary>
        /// Tính toán số tiền giảm từ voucher
        /// </summary>
        public decimal CalculateVoucherDiscount(Voucher voucher, decimal tongTienHang, decimal phiVanChuyen = 0)
        {
            Console.WriteLine($"🔍 [CalculateVoucherDiscount] Bắt đầu tính toán giảm giá cho voucher: {voucher.MaVoucher}");
            
            if (!IsVoucherValid(voucher, tongTienHang, phiVanChuyen)) 
            {
                Console.WriteLine($"❌ [CalculateVoucherDiscount] Voucher không hợp lệ");
                return 0;
            }
            
            // Tính số tiền giảm theo phần trăm (dựa trên tổng đơn hàng bao gồm phí ship)
            var tongDonHang = tongTienHang + phiVanChuyen;
            decimal tienGiam = tongDonHang * (voucher.PhanTramGiam / 100);
            
            Console.WriteLine($"🔍 [CalculateVoucherDiscount] Tổng đơn hàng: {tongDonHang:N0}, Phần trăm giảm: {voucher.PhanTramGiam}%, Tiền giảm trước giới hạn: {tienGiam:N0}");
            
            // Áp dụng giới hạn giảm tối đa nếu có
            if (voucher.GiaTriGiamToiDa.HasValue && tienGiam > voucher.GiaTriGiamToiDa.Value)
            {
                Console.WriteLine($"🔍 [CalculateVoucherDiscount] Áp dụng giới hạn tối đa: {tienGiam:N0} -> {voucher.GiaTriGiamToiDa.Value:N0}");
                tienGiam = voucher.GiaTriGiamToiDa.Value;
            }
            
            var ketQua = Math.Round(tienGiam, 0, MidpointRounding.AwayFromZero);
            Console.WriteLine($"✅ [CalculateVoucherDiscount] Kết quả cuối cùng: {ketQua:N0}");
            
            return ketQua;
        }

        /// <summary>
        /// Lấy thông tin voucher với trạng thái áp dụng
        /// </summary>
        public VoucherApplicationResult GetVoucherApplication(Voucher voucher, decimal tongTienHang, decimal phiVanChuyen = 0)
        {
            var result = new VoucherApplicationResult
            {
                VoucherId = voucher?.VoucherId,
                TenVoucher = voucher?.TenVoucher,
                IsValid = false,
                SoTienGiam = 0,
                LyDoKhongHopLe = ""
            };

            if (voucher == null)
            {
                result.LyDoKhongHopLe = "Voucher không tồn tại";
                return result;
            }

            Console.WriteLine($"🔍 [VoucherCalculationService] Kiểm tra voucher: {voucher.MaVoucher}, TrangThai: {voucher.TrangThai}");

            // Kiểm tra trạng thái
            if (voucher.TrangThai != 1)
            {
                Console.WriteLine($"❌ [VoucherCalculationService] Voucher bị vô hiệu hóa: TrangThai = {voucher.TrangThai}");
                result.LyDoKhongHopLe = "Voucher đã bị vô hiệu hóa";
                return result;
            }

            // Kiểm tra thời gian
            var now = DateTime.Now;
            Console.WriteLine($"🔍 [VoucherCalculationService] Thời gian hiện tại: {now:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"🔍 [VoucherCalculationService] Ngày bắt đầu: {voucher.NgayBatDau:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"🔍 [VoucherCalculationService] Ngày kết thúc: {voucher.NgayKetThuc:dd/MM/yyyy HH:mm:ss}");
            
            if (now < voucher.NgayBatDau)
            {
                Console.WriteLine($"❌ [VoucherCalculationService] Voucher chưa có hiệu lực");
                result.LyDoKhongHopLe = $"Voucher chưa có hiệu lực (Bắt đầu: {voucher.NgayBatDau:dd/MM/yyyy})";
                return result;
            }

            if (now > voucher.NgayKetThuc)
            {
                Console.WriteLine($"❌ [VoucherCalculationService] Voucher đã hết hạn");
                result.LyDoKhongHopLe = $"Voucher đã hết hạn (Hết hạn: {voucher.NgayKetThuc:dd/MM/yyyy})";
                return result;
            }

            // Kiểm tra số lượng
            if (voucher.SoLuong <= 0)
            {
                Console.WriteLine($"❌ [VoucherCalculationService] Voucher hết số lượng: {voucher.SoLuong}");
                result.LyDoKhongHopLe = "Voucher đã hết số lượng sử dụng";
                return result;
            }

            // Kiểm tra số tiền tối thiểu (bao gồm phí ship)
            var tongDonHang = tongTienHang + phiVanChuyen;
            Console.WriteLine($"🔍 [VoucherCalculationService] Tổng tiền hàng: {tongTienHang:N0}, Phí ship: {phiVanChuyen:N0}, Tổng đơn hàng: {tongDonHang:N0}");
            Console.WriteLine($"🔍 [VoucherCalculationService] Điều kiện tối thiểu: {voucher.SoTienApDungToiThieu?.ToString() ?? "Không có"}");
            
            if (voucher.SoTienApDungToiThieu.HasValue && tongDonHang < voucher.SoTienApDungToiThieu.Value)
            {
                Console.WriteLine($"❌ [VoucherCalculationService] Không đủ điều kiện tối thiểu: {tongDonHang:N0} < {voucher.SoTienApDungToiThieu.Value:N0}");
                result.LyDoKhongHopLe = $"Đơn hàng tối thiểu {voucher.SoTienApDungToiThieu.Value:N0} VNĐ để áp dụng voucher này";
                return result;
            }

            Console.WriteLine($"✅ [VoucherCalculationService] Voucher hợp lệ - tính toán giảm giá");

            // Voucher hợp lệ - tính toán giảm giá
            result.IsValid = true;
            result.SoTienGiam = CalculateVoucherDiscount(voucher, tongTienHang, phiVanChuyen);
            result.PhanTramGiam = voucher.PhanTramGiam;
            result.GiaTriGiamToiDa = voucher.GiaTriGiamToiDa;
            result.SoTienApDungToiThieu = voucher.SoTienApDungToiThieu;

            return result;
        }

        /// <summary>
        /// Tính phí vận chuyển dựa trên tổng tiền hàng
        /// </summary>
        public decimal CalculateShippingFee(decimal tongTienHang, decimal phiVanChuyenMacDinh = 30000, decimal nguongFreeship = 500000)
        {
            if (tongTienHang >= nguongFreeship)
                return 0; // Freeship
            return phiVanChuyenMacDinh; // Phí ship mặc định
        }
    }

    public class VoucherApplicationResult
    {
        public Guid? VoucherId { get; set; }
        public string? TenVoucher { get; set; }
        public bool IsValid { get; set; }
        public decimal SoTienGiam { get; set; }
        public decimal PhanTramGiam { get; set; }
        public decimal? GiaTriGiamToiDa { get; set; }
        public decimal? SoTienApDungToiThieu { get; set; }
        public string LyDoKhongHopLe { get; set; } = "";
    }
}
