# Unit Test cho KichCo (Kích cỡ)

## Tổng quan

Bộ unit test này được thiết kế để kiểm tra tính đúng đắn của module Quản lý kích cỡ (KichCo) dựa trên các test case đã được định nghĩa.

## Cấu trúc Test

### 1. Controller Tests (KichCoControllerTests)
Kiểm tra các endpoint API của KichCoController:

- **KC001**: Thêm kích cỡ hợp lệ
- **KC002**: Thêm kích cỡ trùng tên
- **KC003**: Thêm kích cỡ thiếu tên
- **KC004**: Sửa kích cỡ hợp lệ
- **KC005**: Sửa mô tả kích cỡ hợp lệ
- **KC006**: Sửa kích cỡ thành tên trùng
- **KC007**: Sửa kích cỡ thiếu tên
- **KC008**: Xóa kích cỡ thành công
- **KC009**: Hủy xóa kích cỡ
- **KC010**: Tìm kiếm kích cỡ theo tên
- **KC011**: Tìm kiếm không có kết quả
- **KC012**: Phân trang/Hiển thị tất cả kích cỡ
- **KC013**: Thêm kích cỡ với mô tả rất dài
- **KC014**: Thêm kích cỡ với ký tự đặc biệt
- **KC015**: Sửa trạng thái kích cỡ
- **KC016**: Thêm kích cỡ với tên độ dài tối đa
- **KC017**: Sắp xếp kích cỡ theo mô tả Z-A
- **KC018**: Sửa kích cỡ mô tả thành rỗng

### 2. Service Tests (KichCoServiceTests)
Kiểm tra logic nghiệp vụ trong KichCoService:

- Lấy tất cả kích cỡ
- Lấy kích cỡ theo ID (hợp lệ/không hợp lệ)
- Tạo kích cỡ mới
- Tạo kích cỡ trùng tên
- Cập nhật kích cỡ
- Xóa kích cỡ

### 3. DTO Validation Tests (KichCoDTOValidationTests)
Kiểm tra validation rules của KichCoDTO:

- **KC003**: Validate tên kích cỡ rỗng
- Validate kích cỡ hợp lệ
- Validate độ dài tên kích cỡ (tối đa 50 ký tự)
- Validate ký tự đặc biệt trong tên
- Validate độ dài mô tả (tối đa 500 ký tự)

### 4. Integration Tests (KichCoIntegrationTests)
Kiểm tra end-to-end với database in-memory:

- **KC001**: Thêm kích cỡ hợp lệ (end-to-end)
- **KC003**: Thêm kích cỡ thiếu tên (end-to-end)
- **KC004**: Sửa kích cỡ hợp lệ (end-to-end)
- **KC006**: Sửa kích cỡ thành trùng (end-to-end)
- **KC008**: Xóa kích cỡ (end-to-end)
- **KC010**: Tìm kiếm kích cỡ (end-to-end)
- **KC014**: Thêm kích cỡ với ký tự đặc biệt (end-to-end)
- **KC016**: Thêm kích cỡ với tên độ dài tối đa (end-to-end)
- **KC018**: Sửa kích cỡ mô tả thành rỗng (end-to-end)

## Validation Rules

### Tên kích cỡ (TenKichCo):
- **Bắt buộc**: Không được null hoặc rỗng
- **Độ dài**: Tối đa 50 ký tự
- **Ký tự đặc biệt**: Không được chứa các ký tự: %, @, #, $, !, _, ^, &, *, (, )
- **Tính duy nhất**: Tên phải là duy nhất trong hệ thống

### Mô tả (MoTa):
- **Độ dài**: Tối đa 500 ký tự
- **Không bắt buộc**: Có thể null hoặc rỗng

### Trạng thái (TrangThai):
- **Bắt buộc**: Phải có giá trị boolean

## Cách chạy test

### Chạy tất cả test:
```bash
dotnet test UnitTest/KichCoTest/KichCoTests.cs
```

### Chạy test theo nhóm:
```bash
# Chạy Controller Tests
dotnet test --filter "FullyQualifiedName~KichCoControllerTests"

# Chạy Service Tests
dotnet test --filter "FullyQualifiedName~KichCoServiceTests"

# Chạy Validation Tests
dotnet test --filter "FullyQualifiedName~KichCoDTOValidationTests"

# Chạy Integration Tests
dotnet test --filter "FullyQualifiedName~KichCoIntegrationTests"
```

### Chạy test cụ thể:
```bash
# Chạy test KC001
dotnet test --filter "FullyQualifiedName~KC001_ThemKichCoHopLe_ShouldReturnCreated"

# Chạy test KC003
dotnet test --filter "FullyQualifiedName~KC003_ThemKichCoThieuTen_ShouldReturnBadRequest"
```

## Dependencies

- **xUnit**: Framework test
- **FluentAssertions**: Thư viện assertion
- **Moq**: Thư viện mock
- **Microsoft.EntityFrameworkCore.InMemory**: Database in-memory cho integration test

## Test Cases được implement
- **Controller Tests**: KC001-KC018 (18 test cases)
- **Service Tests**: 8 test cases
- **DTO Validation Tests**: 7 test cases
- **Integration Tests**: KC001-KC018 (18 test cases)
- **Tổng cộng**: 51 test cases

## Lưu ý

1. Tất cả test case đều dựa trên test case document đã được cung cấp
2. Validation rules đã được cập nhật để phù hợp với yêu cầu test case
3. Integration tests sử dụng in-memory database để đảm bảo test độc lập
4. Mock tests được sử dụng để test logic nghiệp vụ mà không cần database thật
5. Bao phủ đầy đủ các scenario từ bảng test case gốc 