# Logic Trạng Thái Hóa Đơn Mới - Online vs Offline

## 📋 Enum TrangThaiHoaDon (Đã cập nhật)

```csharp
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
```

## 🏪 Bán Hàng Offline (Tại quầy)

### Workflow đơn giản:
```
Offline_ChuaThanhToan (6) → Offline_DaThanhToan (7) [không ship - hoàn thành]
Offline_ChuaThanhToan (6) → DangGiaoHang (2) + LoaiHoaDon = "Online" [có ship]
```

### Logic:
- **Tạo hóa đơn**: `TrangThai = Offline_ChuaThanhToan (6)`
- **Thanh toán không ship**: `TrangThai = Offline_DaThanhToan (7)` - Hoàn thành
- **Thanh toán có ship**: `TrangThai = DangGiaoHang (2)` + `LoaiHoaDon = "Online"` (coi như đơn hàng online)
- **Hủy**: `TrangThai = Offline_DaHuy (10)`

### Các trường hợp:
1. **Tiền mặt/Chuyển khoản không ship**: `Offline_ChuaThanhToan (6)` → `Offline_DaThanhToan (7)` - Hoàn thành
2. **Tiền mặt/Chuyển khoản có ship**: `Offline_ChuaThanhToan (6)` → `DangGiaoHang (2)` + `LoaiHoaDon = "Online"`

## 🌐 Bán Hàng Online (Giỏ hàng)

### Workflow phức tạp (KHÔNG THAY ĐỔI):
```
ChuaThanhToan (0) → ChoDuyet (0) → DaDuyet (1) → DangGiaoHang (2) → DaGiaoHang (3)
```

### Logic trong GioHangRepository (KHÔNG THAY ĐỔI):
```csharp
if (hinhThucThanhToan.TenHinhThuc.Contains("VNPay"))
{
    trangThai = 1; // DaDuyet - Vì đã thanh toán online thành công
}
else
{
    trangThai = 0; // ChoDuyet - Cần admin xác nhận
}
```

## 🔄 Sự Khác Biệt

| Aspect | Offline (không ship) | Offline (có ship) | Online |
|--------|---------------------|-------------------|--------|
| **Trạng thái chờ** | `Offline_ChuaThanhToan (6)` | `Offline_ChuaThanhToan (6)` | `ChuaThanhToan (0)` |
| **Trạng thái hoàn thành** | `Offline_DaThanhToan (7)` | `DangGiaoHang (2)` | `DaGiaoHang (3)` |
| **Loại hóa đơn** | `BanTaiQuay` | `Online` | `Online` |
| **Quản lý** | Bán hàng offline | Quản lý đơn hàng | Quản lý đơn hàng |
| **Workflow** | Đơn giản (2 bước) | Chuyển thành online | Phức tạp (4-5 bước) |

## 📝 Lưu ý

- **Online**: Không thay đổi gì, vẫn dùng trạng thái 0-5
- **Offline không ship**: Sử dụng trạng thái 6-7, 10
- **Offline có ship**: Chuyển thành trạng thái online (2) và loại "Online"
- **Quản lý đơn hàng**: Chỉ hiển thị hóa đơn trạng thái 0-5 (bao gồm cả offline có ship)
- **Quản lý hóa đơn**: Chỉ hiển thị hóa đơn trạng thái 3 và 7 (đã hoàn thành)
- **Tương thích**: Có thể mở rộng thêm trạng thái mới nếu cần

## ✅ Lợi Ích

1. **Phân biệt rõ ràng**: Online và offline có trạng thái riêng
2. **Không ảnh hưởng**: Online không bị thay đổi
3. **Dễ quản lý**: Mỗi loại có workflow riêng
4. **Báo cáo chính xác**: Thống kê riêng cho từng kênh 