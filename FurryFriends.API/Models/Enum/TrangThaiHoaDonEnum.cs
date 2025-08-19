namespace FurryFriends.API.Models
{
    public enum TrangThaiHoaDon
    {
        // === TRẠNG THÁI BÁN HÀNG ONLINE (KHÔNG THAY ĐỔI) ===
        ChuaThanhToan = 0,    // Chưa thanh toán (Online: Chờ duyệt)
        DaThanhToan = 1,      // Đã thanh toán (Online: Đã duyệt)
        DangGiaoHang = 2,     // Đang giao hàng (Online: Đang giao)
        DaGiaoHang = 3,       // Đã giao hàng (Online: Đã giao)
        DaHuy = 4,            // Đã hủy (Online: Đã hủy)
        DaHoanTra = 5,        // Đã hoàn trả (Online: Đã hoàn trả)
        
        // === TRẠNG THÁI BÁN HÀNG OFFLINE (THÊM MỚI) ===
        Offline_ChuaThanhToan = 6,    // Offline: Chưa thanh toán
        Offline_DaThanhToan = 7,      // Offline: Đã thanh toán (không ship)
        Offline_DangGiaoHang = 8,     // Offline: Đang giao hàng (có ship)
        Offline_DaGiaoHang = 9,       // Offline: Đã giao hàng (có ship)
        Offline_DaHuy = 10            // Offline: Đã hủy
    }
}
