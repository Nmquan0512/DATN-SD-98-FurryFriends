# Unit Test cho ChatLieu (Chất liệu)

## Tổng quan

Bộ unit test này được thiết kế để kiểm tra tính đúng đắn của module Quản lý chất liệu (ChatLieu) dựa trên các test case đã được định nghĩa.

## Cấu trúc Test

### 1. Controller Tests (ChatLieuControllerTests)
Kiểm tra các endpoint API của ChatLieuController:

- **CL001**: Thêm chất liệu hợp lệ
- **CL002**: Thêm chất liệu trùng tên
- **CL003**: Thêm chất liệu thiếu tên
- **CL004**: Sửa chất liệu hợp lệ
- **CL005**: Sửa chất liệu thành trùng
- **CL006**: Sửa chất liệu thiếu tên
- **CL008**: Hủy xóa chất liệu
- **CL009**: Tìm kiếm theo tên chất liệu
- **CL010**: Tìm kiếm không có kết quả
- **CL014**: Sửa trạng thái chất liệu

### 2. Service Tests (ChatLieuServiceTests)
Kiểm tra logic nghiệp vụ trong ChatLieuService:

- Lấy tất cả chất liệu
- Lấy chất liệu theo ID hợp lệ
- Lấy chất liệu theo ID không hợp lệ
- Tạo chất liệu mới
- Cập nhật chất liệu với ID hợp lệ
- Cập nhật chất liệu với ID không hợp lệ
- Xóa chất liệu với ID hợp lệ
- Xóa chất liệu với ID không hợp lệ

### 3. DTO Validation Tests (ChatLieuDTOValidationTests)
Kiểm tra validation logic:

- **CL003**: Validate tên chất liệu rỗng
- Validate chất liệu hợp lệ
- Validate độ dài tên chất liệu
- Validate tên chất liệu vượt quá giới hạn
- Validate ký tự đặc biệt trong tên

### 4. Integration Tests (ChatLieuIntegrationTests)
Kiểm tra end-to-end:

- **CL001**: Thêm chất liệu hợp lệ
- **CL003**: Thêm chất liệu thiếu tên
- **CL004**: Sửa chất liệu hợp lệ
- **CL005**: Sửa chất liệu thành trùng
- **CL010**: Xóa chất liệu
- **CL011**: Xóa chất liệu không tồn tại
- **CL012**: Lấy tất cả chất liệu
- **CL013**: Lấy chất liệu theo ID
- **CL014**: Lấy chất liệu không tồn tại theo ID
- **CL015**: Thêm chất liệu với mô tả dài
- **CL016**: Thêm chất liệu với ký tự đặc biệt
- **CL017**: Tìm kiếm chất liệu không phân biệt chữ hoa/thường
- **CL018**: Thêm chất liệu với tên độ dài tối đa
- **CL019**: Hiển thị thông báo xác nhận khi xóa
- **CL020**: Thêm chất liệu với tên vượt quá giới hạn
- **CL021**: Thêm chất liệu với mô tả rỗng
- **CL022**: Thêm chất liệu với mô tả null
- **CL023**: Thêm chất liệu với trạng thái không hoạt động

## Validation Rules

### TenChatLieu (Tên chất liệu)
- **Required**: Không được để trống
- **MaxLength**: 255 ký tự
- **Special Characters**: Không được chứa ký tự đặc biệt %@#$!_
- **Duplicate Check**: Không được trùng tên với chất liệu khác

### MoTa (Mô tả)
- **MaxLength**: 1000 ký tự
- **Optional**: Có thể để trống hoặc null

### TrangThai (Trạng thái)
- **Required**: Bắt buộc phải có giá trị

## Cách chạy test

```bash
# Chạy tất cả test
dotnet test

# Chạy chỉ test cho ChatLieu
dotnet test --filter "ChatLieuTests"

# Chạy test với verbosity cao
dotnet test --verbosity normal

# Chạy test theo từng loại
dotnet test --filter "ChatLieuControllerTests"
dotnet test --filter "ChatLieuServiceTests"
dotnet test --filter "ChatLieuDTOValidationTests"
dotnet test --filter "ChatLieuIntegrationTests"
```

## Dependencies
- xUnit
- Moq
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory

## Test Cases được implement
- **Controller Tests**: CL001-CL006, CL008-CL010, CL014 (10 test cases)
- **Service Tests**: 8 test cases
- **DTO Validation Tests**: 5 test cases
- **Integration Tests**: CL001-CL023 (23 test cases)
- **Tổng cộng**: 46 test cases

## Lưu ý
- Các test sử dụng in-memory database cho integration tests
- Mock được sử dụng cho unit tests
- Validation được test ở cả DTO và Service layer
- Tất cả test cases đã được việt hoá hoàn toàn
- Bao phủ đầy đủ các scenario từ bảng test case gốc 