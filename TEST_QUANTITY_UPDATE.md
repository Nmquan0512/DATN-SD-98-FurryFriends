# Test Cập Nhật Số Lượng

## Mục đích
Kiểm tra xem việc tăng/giảm số lượng sản phẩm có cập nhật thành tiền đúng không.

## Các thay đổi đã thực hiện
1. **Thêm logging cho API updateQty**: Log request và response
2. **Thêm logging cho event handler**: Log khi cập nhật số lượng
3. **Thêm cơ chế fallback**: Reload dữ liệu nếu API call thất bại
4. **Thêm API getInvoiceDetails**: Để reload dữ liệu khi cần
5. **Sửa renderInvoiceItems**: Đảm bảo thành tiền được tính đúng

## Cách test
1. Thêm một sản phẩm vào hóa đơn
2. Tăng số lượng sản phẩm đó
3. Kiểm tra console log để xem:
   - "Updating quantity for product: [ID] to: [số lượng]"
   - "API call - updateQty: { hoaDonId, spId, request }"
   - "Sending updateQty request to: [URL]"
   - "Request data: { soLuongMoi: [số lượng] }"
   - "Quantity update response: [response]"
   - "UI updated successfully"

## Kết quả mong đợi
- Thành tiền phải được cập nhật đúng (Đơn giá x Số lượng)
- Console log phải hiển thị đầy đủ thông tin
- Không có lỗi JavaScript

## Nếu vẫn có vấn đề
1. Kiểm tra console log để xem lỗi cụ thể
2. Kiểm tra Network tab trong DevTools để xem API call có thành công không
3. Kiểm tra response của API có đầy đủ dữ liệu không

## Debug steps
1. Mở DevTools (F12)
2. Chuyển sang tab Console
3. Tăng số lượng sản phẩm
4. Kiểm tra các log messages
5. Nếu có lỗi, kiểm tra Network tab để xem API call 