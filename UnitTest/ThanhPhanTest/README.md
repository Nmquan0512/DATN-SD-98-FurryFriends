# Unit Test cho ThanhPhan (Thành phần)

## Tổng quan

Bộ unit test này được thiết kế để kiểm tra tính đúng đắn của module Quản lý thành phần (ThanhPhan) dựa trên các test case đã được định nghĩa.

## Cấu trúc Test

### 1. Controller Tests (ThanhPhanControllerTests)
Kiểm tra các endpoint API của ThanhPhanController:

- **TP001**: Thêm thành phần hợp lệ
- **TP002**: Thêm thành phần trùng tên
- **TP003**: Thêm thành phần thiếu tên
- **TP004**: Sửa thành phần hợp lệ
- **TP005**: Sửa mô tả thành phần hợp lệ
- **TP006**: Sửa thành phần thành tên trùng
- **TP007**: Sửa thành phần thiếu tên
- **TP008**: Xóa thành phần thành công
- **TP009**: Hủy xóa thành phần
- **TP010**: Tìm kiếm thành phần theo tên
- **TP011**: Tìm kiếm không có kết quả
- **TP012**: Phân trang/Hiển thị tất cả thành phần
- **TP013**: Thêm thành phần với mô tả dài
- **TP014**: Thêm thành phần với ký tự đặc biệt
- **TP015**: Sửa trạng thái thành phần
- **TP016**: Sửa tên thành phần thành tên đã tồn tại
- **TP017**: Thêm thành phần với tên có ký tự đặc biệt
- **TP018**: Sắp xếp thành phần theo tên A-Z
- **TP019**: Thêm thành phần với tên độ dài tối đa
- **TP020**: Hiển thị thông báo xác nhận khi xóa thành phần

### 2. Service Tests (ThanhPhanServiceTests)
Kiểm tra logic nghiệp vụ trong ThanhPhanService:

- Lấy tất cả thành phần
- Lấy thành phần theo ID (hợp lệ/không hợp lệ)
- Tạo thành phần mới
- Tạo thành phần trùng tên
- Cập nhật thành phần
- Xóa thành phần

### 3. DTO Validation Tests (ThanhPhanDTOValidationTests)
Kiểm tra validation rules của ThanhPhanDTO:

- **TP003**: Validate tên thành phần rỗng
- Validate thành phần hợp lệ
- Validate độ dài tên thành phần (tối đa 255 ký tự)
- Validate ký tự đặc biệt trong tên
- Validate độ dài mô tả (tối đa 1000 ký tự)

### 4. Integration Tests (ThanhPhanIntegrationTests)
Kiểm tra end-to-end với database in-memory:

- **TP001**: Thêm thành phần hợp lệ (end-to-end)
- **TP003**: Thêm thành phần thiếu tên (end-to-end)
- **TP004**: Sửa thành phần hợp lệ (end-to-end)
- **TP006**: Sửa thành phần thành trùng (end-to-end)
- **TP008**: Xóa thành phần (end-to-end)
- **TP010**: Tìm kiếm thành phần (end-to-end)
- **TP013**: Thêm thành phần với mô tả dài (end-to-end)
- **TP014**: Thêm thành phần với ký tự đặc biệt (end-to-end)
- **TP017**: Thêm thành phần với tên có ký tự đặc biệt (end-to-end)
- **TP019**: Thêm thành phần với tên độ dài tối đa (end-to-end)
- **TP020**: Hiển thị thông báo xác nhận xóa (end-to-end)

## Validation Rules

### Tên thành phần (TenThanhPhan):
- **Bắt buộc**: Không được null hoặc rỗng
- **Độ dài**: Tối đa 255 ký tự
- **Ký tự đặc biệt**: Không được chứa các ký tự: %, @, #, $, !, _
- **Tính duy nhất**: Tên phải là duy nhất trong hệ thống

### Mô tả (MoTa):
- **Độ dài**: Tối đa 1000 ký tự
- **Không bắt buộc**: Có thể null hoặc rỗng

### Trạng thái (TrangThai):
- **Bắt buộc**: Phải có giá trị boolean

## Cách chạy test

### Chạy tất cả test:
```bash
dotnet test UnitTest/ThanhPhanTest/ThanhPhanTests.cs
```

### Chạy test theo nhóm:
```bash
# Chạy Controller Tests
dotnet test --filter "FullyQualifiedName~ThanhPhanControllerTests"

# Chạy Service Tests
dotnet test --filter "FullyQualifiedName~ThanhPhanServiceTests"

# Chạy Validation Tests
dotnet test --filter "FullyQualifiedName~ThanhPhanDTOValidationTests"

# Chạy Integration Tests
dotnet test --filter "FullyQualifiedName~ThanhPhanIntegrationTests"
```

### Chạy test cụ thể:
```bash
# Chạy test TP001
dotnet test --filter "FullyQualifiedName~TP001_ThemThanhPhanHopLe_ShouldReturnCreated"

# Chạy test TP003
dotnet test --filter "FullyQualifiedName~TP003_ThemThanhPhanThieuTen_ShouldReturnBadRequest"
```

## Dependencies

- **xUnit**: Framework test
- **FluentAssertions**: Thư viện assertion
- **Moq**: Thư viện mock
- **Microsoft.EntityFrameworkCore.InMemory**: Database in-memory cho integration test

## Test Cases được implement
- **Controller Tests**: TP001-TP020 (20 test cases)
- **Service Tests**: 8 test cases
- **DTO Validation Tests**: 7 test cases
- **Integration Tests**: TP001-TP020 (20 test cases)
- **Tổng cộng**: 55 test cases

## Lưu ý

1. Tất cả test case đều dựa trên test case document đã được cung cấp
2. Validation rules đã được cập nhật để phù hợp với yêu cầu test case
3. Integration tests sử dụng in-memory database để đảm bảo test độc lập
4. Mock tests được sử dụng để test logic nghiệp vụ mà không cần database thật
5. Bao phủ đầy đủ các scenario từ bảng test case gốc 