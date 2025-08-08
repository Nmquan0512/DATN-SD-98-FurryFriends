# Unit Test Quản lý Thương hiệu (Brand Management)

## Tổng quan
File test này chứa các unit test toàn diện cho module Quản lý thương hiệu (Brand Management) dựa trên các test case được cung cấp. Tất cả test cases đã được việt hoá hoàn toàn.

## Cấu trúc Test

### 1. ThuongHieuControllerTests (21 test cases)
Các test case cho Controller layer:
- **TH001**: Thêm thương hiệu hợp lệ
- **TH002**: Thêm thương hiệu trùng tên
- **TH003**: Thêm thương hiệu thiếu tên
- **TH004**: Thêm thương hiệu với email không hợp lệ
- **TH005**: Thêm thương hiệu với SDT không hợp lệ
- **TH006**: Sửa tên thương hiệu hợp lệ
- **TH007**: Sửa email thương hiệu hợp lệ
- **TH008**: Sửa tên thương hiệu thành trùng
- **TH009**: Sửa thương hiệu thiếu tên
- **TH010**: Xóa thương hiệu thành công
- **TH011**: Hủy xóa thương hiệu
- **TH012**: Tìm kiếm theo tên thương hiệu
- **TH013**: Tìm kiếm theo email
- **TH014**: Tìm kiếm không có kết quả
- **TH015**: Phân trang/Hiển thị tất cả thương hiệu
- **TH016**: Thêm thương hiệu với địa chỉ dài
- **TH017**: Sửa địa chỉ của thương hiệu thành công
- **TH018**: Sắp xếp danh sách thương hiệu theo tên từ Z-A
- **TH019**: Thêm mới thương hiệu với tên có độ dài vượt quá giới hạn
- **TH020**: Kiểm tra chức năng thống kê thương hiệu theo trạng thái
- **TH021**: Sửa thương hiệu với tên chứa ký tự đặc biệt

### 2. ThuongHieuServiceTests (8 test cases)
Các test case cho Service layer:
- **GetAllAsync**: Lấy tất cả thương hiệu
- **GetByIdAsync_WithValidId**: Lấy thương hiệu theo ID hợp lệ
- **GetByIdAsync_WithInvalidId**: Lấy thương hiệu theo ID không hợp lệ
- **CreateAsync**: Tạo thương hiệu mới
- **UpdateAsync_WithValidId**: Cập nhật thương hiệu với ID hợp lệ
- **UpdateAsync_WithInvalidId**: Cập nhật thương hiệu với ID không hợp lệ
- **DeleteAsync_WithValidId**: Xóa thương hiệu với ID hợp lệ
- **DeleteAsync_WithInvalidId**: Xóa thương hiệu với ID không hợp lệ

### 3. ThuongHieuDTOValidationTests (7 test cases)
Các test case cho validation logic:
- **TH003**: Validate tên thương hiệu rỗng
- **TH004**: Validate email không hợp lệ
- **TH005**: Validate số điện thoại không hợp lệ
- **ValidateValidBrand**: Validate thương hiệu hợp lệ
- **ValidateBrandNameLength**: Validate độ dài tên thương hiệu
- **ValidateBrandNameLength_ShouldFailWhenExceedsMaxLength**: Validate tên thương hiệu vượt quá giới hạn
- **ValidateSpecialCharactersInName**: Validate ký tự đặc biệt trong tên

### 4. ThuongHieuIntegrationTests (22 test cases)
Các test case cho integration testing:
- **TH001-TH005**: Test thêm thương hiệu với các trường hợp khác nhau
- **TH006-TH008**: Test sửa thương hiệu với các trường hợp khác nhau
- **TH009-TH011**: Test xóa thương hiệu
- **TH012-TH014**: Test lấy danh sách và chi tiết thương hiệu
- **TH015-TH022**: Test các trường hợp đặc biệt (mô tả dài, ký tự đặc biệt, trạng thái, v.v.)

## Các vấn đề đã sửa

### 1. Validation trong Controller
- Thêm try-catch blocks trong `Create` và `Update` methods
- Xử lý exceptions từ service layer
- Trả về `BadRequest` với message lỗi phù hợp

### 2. Validation trong Service
- Thêm validation cho tên thương hiệu rỗng
- Thêm validation cho email rỗng
- Thêm validation cho số điện thoại rỗng
- Thêm kiểm tra trùng tên thương hiệu

### 3. DTO Validation
- Sử dụng `DataAnnotations` cho validation
- `[Required]` cho các trường bắt buộc
- `[EmailAddress]` cho email
- `[RegularExpression]` cho số điện thoại
- `[StringLength]` cho độ dài tên

### 4. Việt hoá hoàn toàn
- Tất cả tên test methods đã được việt hoá
- Tất cả comments đã được việt hoá
- Tất cả error messages đã được việt hoá

## Cách chạy test

```bash
# Chạy tất cả test
dotnet test

# Chạy chỉ test cho ThuongHieu
dotnet test --filter "ThuongHieuTests"

# Chạy test với verbosity cao
dotnet test --verbosity normal

# Chạy test theo từng loại
dotnet test --filter "ThuongHieuControllerTests"
dotnet test --filter "ThuongHieuServiceTests"
dotnet test --filter "ThuongHieuDTOValidationTests"
dotnet test --filter "ThuongHieuIntegrationTests"
```

## Dependencies
- xUnit
- Moq
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory

## Test Cases được implement
- **Controller Tests**: TH001-TH021 (21 test cases)
- **Service Tests**: 8 test cases
- **DTO Validation Tests**: 7 test cases
- **Integration Tests**: TH001-TH022 (22 test cases)
- **Tổng cộng**: 58 test cases

## Lưu ý
- Các test sử dụng in-memory database cho integration tests
- Mock được sử dụng cho unit tests
- Validation được test ở cả DTO và Service layer
- Tất cả test cases đã được việt hoá hoàn toàn
- Bao phủ đầy đủ các scenario từ bảng test case gốc 