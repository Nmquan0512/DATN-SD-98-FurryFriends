# Logic Trạng Thái Hóa Đơn - Online vs Offline

## 📋 Enum TrangThaiHoaDon (Dùng chung)

```csharp
public enum TrangThaiHoaDon
{
    ChuaThanhToan = 0,    // Chưa thanh toán
    DaThanhToan = 1,      // Đã thanh toán  
    DaHuy = 2,            // Đã hủy
    DangGiaoHang = 3,     // Đang giao hàng
    DaGiaoHang = 4,       // Đã giao hàng
    DaHoanTra = 5         // Đã hoàn trả
}
```

## 🏪 Bán Hàng Offline (Tại quầy)

### Workflow đơn giản:
```
ChuaThanhToan (0) → DaThanhToan (1)
```

### Logic:
- **Tạo hóa đơn**: `TrangThai = ChuaThanhToan (0)`
- **Thanh toán**: `TrangThai = DaThanhToan (1)` (dù tiền mặt hay chuyển khoản)
- **Không có giao hàng**: Vì khách hàng nhận hàng trực tiếp tại quầy

### Các trường hợp:
1. **Tiền mặt**: Thanh toán trực tiếp → `DaThanhToan`
2. **Chuyển khoản**: Thanh toán qua VietQR → `DaThanhToan`

## 🌐 Bán Hàng Online (Giỏ hàng)

### Workflow phức tạp:
```
ChuaThanhToan (0) → ChoDuyet (0) → DaDuyet (1) → DangGiaoHang (3) → DaGiaoHang (4)
```

### Logic trong GioHangRepository:
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

### Các trường hợp:
1. **VNPay**: Thanh toán online → `DaDuyet (1)` → Có thể giao hàng ngay
2. **COD/Chuyển khoản**: Chờ admin duyệt → `ChoDuyet (0)` → Admin duyệt → `DaDuyet (1)` → Giao hàng

## 🔄 Sự Khác Biệt

| Aspect | Offline | Online |
|--------|---------|--------|
| **Workflow** | Đơn giản (2 bước) | Phức tạp (4-5 bước) |
| **Giao hàng** | Không có | Có giao hàng |
| **Duyệt** | Không cần | Cần admin duyệt |
| **Thanh toán** | Trực tiếp | Online/COD |
| **Trạng thái cuối** | `DaThanhToan` | `DaGiaoHang` |

## 📝 Lưu ý

- **Cùng enum**: Cả online và offline dùng chung `TrangThaiHoaDon`
- **Logic khác nhau**: Mỗi loại có workflow riêng
- **Tương thích**: Có thể mở rộng thêm trạng thái mới nếu cần 