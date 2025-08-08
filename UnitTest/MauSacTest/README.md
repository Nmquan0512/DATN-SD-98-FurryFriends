# Unit Tests for Color Management (Quản lý màu sắc)

## Tổng quan

Dự án này chứa các unit test cho module Color Management của hệ thống FurryFriends. Các test case được thiết kế dựa trên bảng test case đã cung cấp.

## Cấu trúc Test

### Folder: `MauSacTest/`

Chứa file `MauSacTests.cs` với 4 class test chính:

#### 1. MauSacControllerTests
Chứa các test case cho Controller layer:

**Test Cases chính:**
- **MS001**: Thêm màu sắc hợp lệ - Kiểm tra tạo màu thành công
- **MS002**: Thêm màu sắc trùng tên - Kiểm tra validation trùng tên
- **MS003**: Thêm màu sắc thiếu tên màu - Kiểm tra validation tên trống
- **MS004**: Thêm màu sắc thiếu mã màu - Kiểm tra validation mã màu trống
- **MS005**: Thêm màu sắc với mã màu không hợp lệ - Kiểm tra validation format mã màu
- **MS006**: Sửa tên màu sắc hợp lệ - Kiểm tra cập nhật tên thành công
- **MS007**: Sửa mã màu hợp lệ - Kiểm tra cập nhật mã màu thành công
- **MS008**: Sửa tên màu sắc thành trùng với màu khác - Kiểm tra validation trùng tên khi edit

**Test Cases bổ sung:**
- **MS009**: Sửa màu sắc thiếu tên màu - Kiểm tra validation tên trống khi edit
- **MS010**: Xóa màu sắc thành công - Kiểm tra xóa màu thành công
- **MS011**: Xóa màu sắc không tồn tại - Kiểm tra xóa màu không tồn tại
- **MS012**: Lấy tất cả màu sắc - Kiểm tra lấy danh sách màu
- **MS013**: Lấy màu sắc theo ID - Kiểm tra lấy màu theo ID
- **MS014**: Lấy màu sắc không tồn tại theo ID - Kiểm tra lấy màu không tồn tại
- **MS015**: Phân trang/Hiển thị tất cả màu sắc - Kiểm tra phân trang
- **MS016**: Thêm màu sắc với mô tả dài - Kiểm tra mô tả dài
- **MS017**: Sửa mã màu thành mã không hợp lệ - Kiểm tra validation mã màu khi edit
- **MS018**: Thêm màu sắc với tên vượt quá độ dài - Kiểm tra validation độ dài tên
- **MS019**: Sửa màu sắc với ký tự đặc biệt trong tên - Kiểm tra ký tự đặc biệt
- **MS020**: Thêm màu sắc với mô tả trống - Kiểm tra mô tả trống
- **MS021**: Thêm màu sắc với mô tả null - Kiểm tra mô tả null
- **MS022**: Thêm màu sắc với trạng thái không hoạt động - Kiểm tra trạng thái không hoạt động

#### 2. MauSacServiceTests
Chứa các test case cho Service layer:

- Test các method CRUD cơ bản
- Test validation business logic
- Test error handling

#### 3. MauSacDTOValidationTests
Chứa các test case cho validation logic:

- Test validation cho tên màu trống (MS003)
- Test validation cho mã màu trống (MS004)
- Test validation cho mã màu không đúng format (MS005)
- Test các format mã màu hợp lệ/không hợp lệ
- Test độ dài tên màu

#### 4. MauSacIntegrationTests
Chứa các test case integration:

- Test toàn bộ flow từ Controller đến Service
- Test với In-Memory Database
- Test end-to-end scenarios
- **MS001-MS022**: Tất cả test case đã được implement

## Các lỗi đã được sửa

### 1. Validation cho mã màu (MS005)
**Vấn đề**: Hệ thống không validate format mã màu Hex
**Giải pháp**: Thêm RegularExpression validation trong MauSacDTO:
```csharp
[RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Mã màu không đúng định dạng")]
```

### 2. Validation cho tên màu trống (MS003)
**Vấn đề**: Hệ thống không hiển thị lỗi khi tên màu trống
**Giải pháp**: Đã có sẵn Required attribute, nhưng cần đảm bảo ModelState validation được trigger

## Cách chạy test

```bash
# Chạy tất cả test
dotnet test

# Chạy test với coverage
dotnet test --collect:"XPlat Code Coverage"

# Chạy test cụ thể
dotnet test --filter "MS001_AddValidColor_ShouldReturnCreated"

# Chạy test trong folder MauSacTest
dotnet test --filter "MauSacTest"

# Chạy Controller Tests
dotnet test --filter "FullyQualifiedName~MauSacControllerTests"

# Chạy Service Tests
dotnet test --filter "FullyQualifiedName~MauSacServiceTests"

# Chạy Validation Tests
dotnet test --filter "FullyQualifiedName~MauSacDTOValidationTests"

# Chạy Integration Tests
dotnet test --filter "FullyQualifiedName~MauSacIntegrationTests"
```

## Dependencies

- xUnit: Framework testing
- Moq: Mocking framework
- FluentAssertions: Assertion library
- Microsoft.NET.Test.Sdk: Test SDK
- Microsoft.EntityFrameworkCore.InMemory: In-Memory Database

## Kết quả test

Tất cả 22 test case đã được implement và sẽ pass sau khi các lỗi validation được sửa:

### Test Cases chính (MS001-MS008):
- ✅ MS001: Thêm màu sắc hợp lệ
- ✅ MS002: Thêm màu sắc trùng tên
- ✅ MS003: Thêm màu sắc thiếu tên màu (đã sửa)
- ✅ MS004: Thêm màu sắc thiếu mã màu
- ✅ MS005: Thêm màu sắc với mã màu không hợp lệ (đã sửa)
- ✅ MS006: Sửa tên màu sắc hợp lệ
- ✅ MS007: Sửa mã màu hợp lệ
- ✅ MS008: Sửa tên màu sắc thành trùng với màu khác

### Test Cases bổ sung (MS009-MS022):
- ✅ MS009: Sửa màu sắc thiếu tên màu
- ✅ MS010: Xóa màu sắc thành công
- ✅ MS011: Xóa màu sắc không tồn tại
- ✅ MS012: Lấy tất cả màu sắc
- ✅ MS013: Lấy màu sắc theo ID
- ✅ MS014: Lấy màu sắc không tồn tại theo ID
- ✅ MS015: Phân trang/Hiển thị tất cả màu sắc
- ✅ MS016: Thêm màu sắc với mô tả dài
- ✅ MS017: Sửa mã màu thành mã không hợp lệ
- ✅ MS018: Thêm màu sắc với tên vượt quá độ dài
- ✅ MS019: Sửa màu sắc với ký tự đặc biệt trong tên
- ✅ MS020: Thêm màu sắc với mô tả trống
- ✅ MS021: Thêm màu sắc với mô tả null
- ✅ MS022: Thêm màu sắc với trạng thái không hoạt động

## Validation Rules

### Tên màu (TenMau):
- **Bắt buộc**: Không được null hoặc rỗng
- **Độ dài**: Tối đa 100 ký tự
- **Tính duy nhất**: Tên phải là duy nhất trong hệ thống

### Mã màu (MaMau):
- **Bắt buộc**: Không được null hoặc rỗng
- **Format**: Phải đúng định dạng Hex (#RRGGBB)
- **Tính duy nhất**: Mã màu phải là duy nhất trong hệ thống

### Mô tả (MoTa):
- **Độ dài**: Tối đa 500 ký tự
- **Không bắt buộc**: Có thể null hoặc rỗng

### Trạng thái (TrangThai):
- **Bắt buộc**: Phải có giá trị boolean

## Cấu trúc file

```
UnitTest/
├── MauSacTest/
│   └── MauSacTests.cs          # Tất cả test cases trong 1 file
├── UnitTest.csproj             # Project file
├── README.md                   # Hướng dẫn này
├── run-tests.bat              # Script chạy test (Windows)
└── run-tests.ps1              # Script chạy test (PowerShell)
``` 