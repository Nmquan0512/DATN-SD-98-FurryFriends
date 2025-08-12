using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;

namespace FurryFriends.Web.Services
{
    public class DiscountCalculationService
    {
        private readonly IGiamGiaService _giamGiaService;

        public DiscountCalculationService(IGiamGiaService giamGiaService)
        {
            _giamGiaService = giamGiaService;
        }

        /// <summary>
        /// Tính toán giảm giá cho một sản phẩm chi tiết
        /// </summary>
        /// <param name="sanPhamChiTietId">ID sản phẩm chi tiết</param>
        /// <param name="giaBan">Giá bán gốc</param>
        /// <returns>Thông tin giảm giá</returns>
        public async Task<(bool coGiamGia, decimal? phanTramGiamGia, decimal? giaSauGiam)> CalculateDiscountForProduct(Guid sanPhamChiTietId, decimal giaBan)
        {
            try
            {
                // Lấy tất cả chương trình giảm giá
                var allGiamGia = await _giamGiaService.GetAllAsync();
                
                // Tìm các chương trình giảm giá có chứa sản phẩm này
                var applicableDiscounts = new List<decimal>();
                
                foreach (var giamGia in allGiamGia)
                {
                    // Kiểm tra xem sản phẩm có trong danh sách giảm giá không
                    if (giamGia.SanPhamChiTietIds?.Contains(sanPhamChiTietId) == true)
                    {
                        // Kiểm tra thời gian hiệu lực
                        var now = DateTime.UtcNow;
                        if (giamGia.NgayBatDau <= now && giamGia.NgayKetThuc >= now && giamGia.TrangThai)
                        {
                            applicableDiscounts.Add(giamGia.PhanTramKhuyenMai);
                        }
                    }
                }

                // Nếu không có giảm giá nào
                if (!applicableDiscounts.Any())
                {
                    return (false, null, null);
                }

                // Lấy % giảm giá cao nhất
                var maxDiscountPercentage = applicableDiscounts.Max();
                var giaSauGiam = giaBan * (1 - maxDiscountPercentage / 100);

                return (true, maxDiscountPercentage, giaSauGiam);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Lỗi khi tính toán giảm giá cho sản phẩm {sanPhamChiTietId}: {ex.Message}");
                return (false, null, null);
            }
        }

        /// <summary>
        /// Cập nhật thông tin giảm giá cho danh sách sản phẩm chi tiết
        /// </summary>
        /// <param name="chiTietList">Danh sách sản phẩm chi tiết</param>
        /// <returns>Danh sách đã được cập nhật</returns>
        public async Task<List<SanPhamChiTietViewModel>> UpdateDiscountInfo(List<SanPhamChiTietViewModel> chiTietList)
        {
            var updatedList = new List<SanPhamChiTietViewModel>();

            foreach (var chiTiet in chiTietList)
            {
                var (coGiamGia, phanTramGiamGia, giaSauGiam) = await CalculateDiscountForProduct(chiTiet.SanPhamChiTietId, chiTiet.GiaBan);
                
                chiTiet.CoGiamGia = coGiamGia;
                chiTiet.PhanTramGiamGia = phanTramGiamGia;
                chiTiet.GiaSauGiam = giaSauGiam;

                updatedList.Add(chiTiet);
            }

            return updatedList;
        }

        /// <summary>
        /// Cập nhật thông tin giảm giá cho một sản phẩm
        /// </summary>
        /// <param name="sanPham">Sản phẩm cần cập nhật</param>
        /// <returns>Sản phẩm đã được cập nhật</returns>
        public async Task<SanPhamViewModel> UpdateProductDiscount(SanPhamViewModel sanPham)
        {
            // Cập nhật thông tin giảm giá cho từng chi tiết
            sanPham.ChiTietList = await UpdateDiscountInfo(sanPham.ChiTietList);

            // Tính toán tổng quan giảm giá cho sản phẩm
            return DiscountService.CalculateProductDiscount(sanPham);
        }
    }
}
