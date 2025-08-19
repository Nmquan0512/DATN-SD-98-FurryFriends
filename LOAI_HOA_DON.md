# Logic Phân Loại Hóa Đơn

## 📋 Các Loại Hóa Đơn

### 1. **BanTaiQuay** (Bán tại quầy - Offline)
- **Nguồn**: Bán hàng offline tại quầy
- **Đặc điểm**:
  - Khách hàng đến trực tiếp cửa hàng
  - Thanh toán trực tiếp (tiền mặt/chuyển khoản)
  - Nhận hàng ngay tại quầy
  - Không có giao hàng
- **Workflow**: `ChuaThanhToan` → `DaThanhToan`

### 2. **Online** (Bán hàng online)
- **Nguồn**: Giỏ hàng online
- **Đặc điểm**:
  - Khách hàng đặt hàng qua website
  - Thanh toán online (VNPay) hoặc COD
  - Có giao hàng đến địa chỉ khách hàng
  - Cần duyệt và xử lý giao hàng
- **Workflow**: `ChuaThanhToan` → `ChoDuyet` → `DaDuyet` → `DangGiaoHang` → `DaGiaoHang`

### 3. **GiaoHang** (Giao hàng - Đã loại bỏ)
- **Lưu ý**: Loại này đã được loại bỏ khỏi logic hiện tại

## 🔧 Logic Code

### Bán hàng Offline (BanHangRepository.cs)
```csharp
LoaiHoaDon = "BanTaiQuay" // Luôn là "BanTaiQuay"
```

### Bán hàng Online (GioHangRepository.cs)
```csharp
LoaiHoaDon = dto.LoaiHoaDon ?? "Online" // Luôn là "Online"
```

## 📊 Thống Kê

### Trong HoaDonController.cs
```csharp
ViewBag.BanTaiQuayCount = hoaDons?.Count(h => h.LoaiHoaDon == "BanTaiQuay") ?? 0;
ViewBag.OnlineCount = hoaDons?.Count(h => h.LoaiHoaDon == "Online") ?? 0;
```

## 🎯 Mục Đích Phân Loại

1. **Thống kê**: Phân biệt doanh thu online vs offline
2. **Quản lý**: Workflow xử lý khác nhau
3. **Báo cáo**: Báo cáo riêng cho từng kênh bán hàng
4. **Tối ưu**: Chiến lược marketing và quản lý kho khác nhau

## ✅ Kết Luận

- **BanTaiQuay**: Bán hàng offline tại quầy
- **Online**: Bán hàng online qua website
- **Không còn**: GiaoHang (đã loại bỏ)

Logic này giúp phân biệt rõ ràng nguồn gốc hóa đơn và có workflow xử lý phù hợp. 