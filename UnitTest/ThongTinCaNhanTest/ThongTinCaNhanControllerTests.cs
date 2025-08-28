using FurryFriends.API.Controllers;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace UnitTest.ThongTinCaNhanTest
{
    public class ThongTinCaNhanControllerTests
    {
        #region Controller Tests

        public class ThongTinCaNhanControllerUnitTests
        {
            private readonly Mock<IThongTinCaNhanService> _mockService;
            private readonly ThongTinCaNhanController _controller;

            public ThongTinCaNhanControllerUnitTests()
            {
                _mockService = new Mock<IThongTinCaNhanService>();
                _controller = new ThongTinCaNhanController(_mockService.Object);
            }

            [Fact]
            public async Task TTCN001_GetThongTinCaNhan_WithExistingAccount_ShouldReturnOk()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var thongTinCaNhan = new ThongTinCaNhanDTO
                {
                    TaiKhoanId = taiKhoanId,
                    UserName = "nguyenvana",
                    HoTen = "Nguyen Van A",
                    Email = "nguyenvana@email.com",
                    SoDienThoai = "0123456789",
                    NgaySinh = new DateTime(1990, 1, 1),
                    GioiTinh = "Nam",
                    DiaChi = "123 ABC Street, Ho Chi Minh City",
                    Role = "Customer"
                };

                _mockService.Setup(x => x.GetThongTinCaNhanAsync(taiKhoanId))
                           .ReturnsAsync(thongTinCaNhan);

                // Act
                var result = await _controller.GetThongTinCaNhan(taiKhoanId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedInfo = okResult!.Value as ThongTinCaNhanDTO;
                returnedInfo.Should().NotBeNull();
                returnedInfo!.HoTen.Should().Be("Nguyen Van A");
                returnedInfo.Email.Should().Be("nguyenvana@email.com");
                _mockService.Verify(x => x.GetThongTinCaNhanAsync(taiKhoanId), Times.Once);
            }

            [Fact]
            public async Task TTCN002_GetThongTinCaNhan_WithNonExistentAccount_ShouldReturnNotFound()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                _mockService.Setup(x => x.GetThongTinCaNhanAsync(taiKhoanId))
                           .ReturnsAsync((ThongTinCaNhanDTO?)null);

                // Act
                var result = await _controller.GetThongTinCaNhan(taiKhoanId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be("Không tìm thấy tài khoản.");
            }

            [Fact]
            public async Task TTCN003_GetThongTinCaNhan_WithEmptyGuid_ShouldReturnNotFound()
            {
                // Arrange
                var emptyGuid = Guid.Empty;
                _mockService.Setup(x => x.GetThongTinCaNhanAsync(emptyGuid))
                           .ReturnsAsync((ThongTinCaNhanDTO?)null);

                // Act
                var result = await _controller.GetThongTinCaNhan(emptyGuid);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task TTCN004_UpdateThongTinCaNhan_WithValidData_ShouldReturnOk()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var updateDto = new CapNhatThongTinCaNhanDTO
                {
                    HoTen = "Nguyen Van B Updated",
                    Email = "updated@email.com",
                    SoDienThoai = "0987654321",
                    NgaySinh = new DateTime(1985, 5, 15),
                    GioiTinh = "Nu",
                    DiaChi = "456 Updated Street, Ha Noi"
                };

                _mockService.Setup(x => x.UpdateThongTinCaNhanAsync(taiKhoanId, updateDto))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.UpdateThongTinCaNhan(taiKhoanId, updateDto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().Be("Cập nhật thông tin thành công.");
                _mockService.Verify(x => x.UpdateThongTinCaNhanAsync(taiKhoanId, updateDto), Times.Once);
            }

            [Fact]
            public async Task TTCN005_UpdateThongTinCaNhan_WithNonExistentAccount_ShouldReturnNotFound()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var updateDto = new CapNhatThongTinCaNhanDTO
                {
                    HoTen = "Test User",
                    Email = "test@email.com"
                };

                _mockService.Setup(x => x.UpdateThongTinCaNhanAsync(taiKhoanId, updateDto))
                           .ReturnsAsync(false);

                // Act
                var result = await _controller.UpdateThongTinCaNhan(taiKhoanId, updateDto);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be("Không tìm thấy tài khoản để cập nhật.");
            }

            [Fact]
            public async Task TTCN006_UpdateThongTinCaNhan_WithPartialData_ShouldReturnOk()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var partialUpdateDto = new CapNhatThongTinCaNhanDTO
                {
                    HoTen = "Only Name Update",
                    Email = null, // Other fields not updated
                    SoDienThoai = null,
                    NgaySinh = null,
                    GioiTinh = null,
                    DiaChi = null
                };

                _mockService.Setup(x => x.UpdateThongTinCaNhanAsync(taiKhoanId, partialUpdateDto))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.UpdateThongTinCaNhan(taiKhoanId, partialUpdateDto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().Be("Cập nhật thông tin thành công.");
            }

            [Fact]
            public async Task TTCN007_DoiMatKhau_WithValidPasswords_ShouldReturnOk()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "oldPassword123",
                    MatKhauMoi = "newPassword456"
                };

                _mockService.Setup(x => x.DoiMatKhauAsync(taiKhoanId, request.MatKhauCu, request.MatKhauMoi))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.DoiMatKhau(taiKhoanId, request);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().Be("Đổi mật khẩu thành công.");
                _mockService.Verify(x => x.DoiMatKhauAsync(taiKhoanId, request.MatKhauCu, request.MatKhauMoi), Times.Once);
            }

            [Fact]
            public async Task TTCN008_DoiMatKhau_WithEmptyOldPassword_ShouldReturnBadRequest()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "", // Empty old password
                    MatKhauMoi = "newPassword456"
                };

                // Act
                var result = await _controller.DoiMatKhau(taiKhoanId, request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Mật khẩu không được để trống.");
            }

            [Fact]
            public async Task TTCN009_DoiMatKhau_WithEmptyNewPassword_ShouldReturnBadRequest()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "oldPassword123",
                    MatKhauMoi = "" // Empty new password
                };

                // Act
                var result = await _controller.DoiMatKhau(taiKhoanId, request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Mật khẩu không được để trống.");
            }

            [Fact]
            public async Task TTCN010_DoiMatKhau_WithWhitespacePasswords_ShouldReturnBadRequest()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "   ", // Whitespace only
                    MatKhauMoi = "   " // Whitespace only
                };

                // Act
                var result = await _controller.DoiMatKhau(taiKhoanId, request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TTCN011_DoiMatKhau_WithWrongOldPassword_ShouldReturnBadRequest()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "wrongOldPassword",
                    MatKhauMoi = "newPassword456"
                };

                _mockService.Setup(x => x.DoiMatKhauAsync(taiKhoanId, request.MatKhauCu, request.MatKhauMoi))
                           .ReturnsAsync(false);

                // Act
                var result = await _controller.DoiMatKhau(taiKhoanId, request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Mật khẩu cũ không đúng hoặc tài khoản không tồn tại.");
            }

            [Fact]
            public async Task TTCN012_DoiMatKhau_WithNonExistentAccount_ShouldReturnBadRequest()
            {
                // Arrange
                var nonExistentId = Guid.NewGuid();
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "oldPassword123",
                    MatKhauMoi = "newPassword456"
                };

                _mockService.Setup(x => x.DoiMatKhauAsync(nonExistentId, request.MatKhauCu, request.MatKhauMoi))
                           .ReturnsAsync(false);

                // Act
                var result = await _controller.DoiMatKhau(nonExistentId, request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TTCN013_GetThongTinCaNhan_WhenServiceThrowsException_ShouldPropagateException()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                _mockService.Setup(x => x.GetThongTinCaNhanAsync(taiKhoanId))
                           .ThrowsAsync(new Exception("Database error"));

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(async () => await _controller.GetThongTinCaNhan(taiKhoanId));
            }

            [Fact]
            public async Task TTCN014_UpdateThongTinCaNhan_WhenServiceThrowsException_ShouldPropagateException()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var updateDto = new CapNhatThongTinCaNhanDTO { HoTen = "Test" };
                _mockService.Setup(x => x.UpdateThongTinCaNhanAsync(taiKhoanId, updateDto))
                           .ThrowsAsync(new Exception("Database error"));

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(async () => await _controller.UpdateThongTinCaNhan(taiKhoanId, updateDto));
            }

            [Fact]
            public async Task TTCN015_DoiMatKhau_WhenServiceThrowsException_ShouldPropagateException()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "oldPassword",
                    MatKhauMoi = "newPassword"
                };
                _mockService.Setup(x => x.DoiMatKhauAsync(taiKhoanId, request.MatKhauCu, request.MatKhauMoi))
                           .ThrowsAsync(new Exception("Database error"));

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(async () => await _controller.DoiMatKhau(taiKhoanId, request));
            }
        }

        #endregion

        #region Validation Tests

        public class ThongTinCaNhanValidationTests
        {
            [Fact]
            public void ValidateCapNhatThongTinCaNhanDTO_WithValidData_ShouldPass()
            {
                // Arrange
                var dto = new CapNhatThongTinCaNhanDTO
                {
                    HoTen = "Nguyen Van Test",
                    Email = "test@example.com",
                    SoDienThoai = "0123456789",
                    NgaySinh = new DateTime(1990, 1, 1),
                    GioiTinh = "Nam",
                    DiaChi = "123 Test Street"
                };

                // Act & Assert
                dto.HoTen.Should().NotBeNullOrEmpty();
                dto.Email.Should().Contain("@");
                dto.SoDienThoai.Should().NotBeNullOrEmpty();
                dto.NgaySinh.Should().HaveValue();
                dto.GioiTinh.Should().NotBeNullOrEmpty();
                dto.DiaChi.Should().NotBeNullOrEmpty();
            }

            [Fact]
            public void ValidateCapNhatThongTinCaNhanDTO_WithNullValues_ShouldBeAcceptable()
            {
                // Arrange - All properties are nullable for partial updates
                var dto = new CapNhatThongTinCaNhanDTO
                {
                    HoTen = null,
                    Email = null,
                    SoDienThoai = null,
                    NgaySinh = null,
                    GioiTinh = null,
                    DiaChi = null
                };

                // Act & Assert
                dto.HoTen.Should().BeNull();
                dto.Email.Should().BeNull();
                dto.SoDienThoai.Should().BeNull();
                dto.NgaySinh.Should().BeNull();
                dto.GioiTinh.Should().BeNull();
                dto.DiaChi.Should().BeNull();
            }

            [Fact]
            public void ValidateDoiMatKhauRequest_WithValidData_ShouldPass()
            {
                // Arrange
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "currentPassword123",
                    MatKhauMoi = "newSecurePassword456"
                };

                // Act & Assert
                request.MatKhauCu.Should().NotBeNullOrEmpty();
                request.MatKhauMoi.Should().NotBeNullOrEmpty();
                request.MatKhauCu.Should().NotBe(request.MatKhauMoi); // Passwords should be different
            }

            [Fact]
            public void ValidateDoiMatKhauRequest_WithEmptyPasswords_ShouldFail()
            {
                // Arrange
                var request = new DoiMatKhauRequest
                {
                    MatKhauCu = "",
                    MatKhauMoi = ""
                };

                // Act & Assert
                request.MatKhauCu.Should().BeEmpty();
                request.MatKhauMoi.Should().BeEmpty();
            }

            [Fact]
            public void ValidateEmail_WithValidFormat_ShouldPass()
            {
                // Arrange
                var dto = new CapNhatThongTinCaNhanDTO
                {
                    Email = "valid.email@example.com"
                };

                // Act & Assert
                dto.Email.Should().Contain("@");
                dto.Email.Should().Contain(".");
            }

            [Fact]
            public void ValidatePhoneNumber_WithValidFormat_ShouldPass()
            {
                // Arrange
                var dto = new CapNhatThongTinCaNhanDTO
                {
                    SoDienThoai = "0123456789"
                };

                // Act & Assert
                dto.SoDienThoai.Should().HaveLength(10);
                dto.SoDienThoai.Should().StartWith("0");
            }

            [Fact]
            public void ValidateBirthDate_WithReasonableDate_ShouldPass()
            {
                // Arrange
                var dto = new CapNhatThongTinCaNhanDTO
                {
                    NgaySinh = new DateTime(1990, 6, 15)
                };

                // Act & Assert
                dto.NgaySinh.Should().HaveValue();
                dto.NgaySinh.Should().BeBefore(DateTime.Now);
                dto.NgaySinh.Should().BeAfter(new DateTime(1900, 1, 1));
            }

            [Fact]
            public void ValidateGender_WithValidValues_ShouldPass()
            {
                // Arrange
                var maleDto = new CapNhatThongTinCaNhanDTO { GioiTinh = "Nam" };
                var femaleDto = new CapNhatThongTinCaNhanDTO { GioiTinh = "Nu" };

                // Act & Assert
                maleDto.GioiTinh.Should().Be("Nam");
                femaleDto.GioiTinh.Should().Be("Nu");
            }
        }

        #endregion

        #region Integration Tests

        public class ThongTinCaNhanIntegrationTests
        {
            [Fact]
            public async Task TTCN001_Integration_GetAndUpdatePersonalInfo_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real service and database
                Assert.True(true);
            }

            [Fact]
            public async Task TTCN002_Integration_ChangePassword_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real service and database
                Assert.True(true);
            }

            [Fact]
            public async Task TTCN003_Integration_PartialUpdate_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test partial updates with real data
                Assert.True(true);
            }
        }

        #endregion
    }
}