using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services
{
    public class DiscountService
    {
        /// <summary>
        /// Tính toán giảm giá với % cao nhất khi có nhiều giảm giá
        /// </summary>
        /// <param name="discountPercentages">Danh sách các % giảm giá</param>
        /// <param name="originalPrice">Giá gốc</param>
        /// <returns>Tuple chứa (có giảm giá, % giảm giá cao nhất, giá sau giảm)</returns>
        public static (bool coGiamGia, decimal? phanTramGiamGia, decimal? giaSauGiam) CalculateMaxDiscount(List<decimal> discountPercentages, decimal originalPrice)
        {
            if (!discountPercentages.Any() || discountPercentages.All(p => p <= 0))
                return (false, null, null);

            // Lấy % giảm giá cao nhất
            var maxDiscountPercentage = discountPercentages.Max();
            var giaSauGiam = originalPrice * (1 - maxDiscountPercentage / 100);

            return (true, maxDiscountPercentage, giaSauGiam);
        }

        /// <summary>
        /// Áp dụng giảm giá cho một sản phẩm chi tiết
        /// </summary>
        /// <param name="chiTiet">Sản phẩm chi tiết</param>
        /// <param name="discountPercentages">Danh sách % giảm giá</param>
        /// <returns>Sản phẩm chi tiết đã được cập nhật giảm giá</returns>
        public static SanPhamChiTietViewModel ApplyDiscount(SanPhamChiTietViewModel chiTiet, List<decimal> discountPercentages)
        {
            var (coGiamGia, phanTramGiamGia, giaSauGiam) = CalculateMaxDiscount(discountPercentages, chiTiet.GiaBan);
            
            chiTiet.CoGiamGia = coGiamGia;
            chiTiet.PhanTramGiamGia = phanTramGiamGia;
            chiTiet.GiaSauGiam = giaSauGiam;

            return chiTiet;
        }

        /// <summary>
        /// Tính toán tổng quan giảm giá cho một sản phẩm
        /// </summary>
        /// <param name="sanPham">Sản phẩm</param>
        /// <returns>Sản phẩm đã được cập nhật thông tin giảm giá</returns>
        public static SanPhamViewModel CalculateProductDiscount(SanPhamViewModel sanPham)
        {
            var chiTietCoGiamGia = sanPham.ChiTietList.Where(c => c.CoGiamGia).ToList();
            
            if (!chiTietCoGiamGia.Any())
            {
                sanPham.CoGiamGia = false;
                sanPham.PhanTramGiamGia = null;
                sanPham.GiaSauGiam = null;
                sanPham.NgayKetThucGiamGia = null;
                return sanPham;
            }

            // Lấy % giảm giá cao nhất trong tất cả biến thể
            var maxPhanTramGiamGia = chiTietCoGiamGia.Max(c => c.PhanTramGiamGia ?? 0);
            var minGiaSauGiam = chiTietCoGiamGia.Min(c => c.GiaSauGiam ?? c.GiaBan);
            
            // Lấy ngày hết hạn sớm nhất trong các biến thể có giảm giá
            var ngayKetThucSomNhat = chiTietCoGiamGia
                .Where(c => c.NgayKetThucGiamGia.HasValue)
                .Min(c => c.NgayKetThucGiamGia.Value);

            sanPham.CoGiamGia = true;
            sanPham.PhanTramGiamGia = maxPhanTramGiamGia;
            sanPham.GiaSauGiam = minGiaSauGiam;
            sanPham.NgayKetThucGiamGia = ngayKetThucSomNhat;

            return sanPham;
        }

        /// <summary>
        /// Tính toán giá sau giảm an toàn cho một collection
        /// </summary>
        /// <param name="chiTietList">Danh sách sản phẩm chi tiết</param>
        /// <returns>Tuple chứa (có giảm giá, giá sau giảm min, giá sau giảm max)</returns>
        public static (bool coGiamGia, decimal? minGiaSauGiam, decimal? maxGiaSauGiam) CalculateSafeDiscountRange(List<SanPhamChiTietViewModel> chiTietList)
        {
            var chiTietCoGiamGia = chiTietList.Where(c => c.CoGiamGia).ToList();
            
            if (!chiTietCoGiamGia.Any())
                return (false, null, null);

            var minGiaSauGiam = chiTietCoGiamGia.Min(c => c.GiaSauGiam ?? c.GiaBan);
            var maxGiaSauGiam = chiTietCoGiamGia.Max(c => c.GiaSauGiam ?? c.GiaBan);

            return (true, minGiaSauGiam, maxGiaSauGiam);
        }
    }
}
