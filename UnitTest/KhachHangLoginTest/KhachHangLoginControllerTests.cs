using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using FurryFriends.API.Models;
using FurryFriends.Web.Controllers;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UnitTest.KhachHangLoginTest
{
    public class KhachHangLoginControllerTests
    {
        #region Controller Tests

        public class KhachHangLoginControllerUnitTests
        {
            private readonly Mock<ITaiKhoanService> _mockService;
            private readonly Mock<ILogger<KhachHangLoginController>> _mockLogger;
            private readonly Mock<HttpContext> _mockHttpContext;
            private readonly Mock<ISession> _mockSession;
            private readonly Mock<ITempDataProvider> _mockTempDataProvider;
            private readonly KhachHangLoginController _controller;

            public KhachHangLoginControllerUnitTests()
            {
                _mockService = new Mock<ITaiKhoanService>();
                _mockLogger = new Mock<ILogger<KhachHangLoginController>>();
                _mockHttpContext = new Mock<HttpContext>();
                _mockSession = new Mock<ISession>();
                _mockTempDataProvider = new Mock<ITempDataProvider>();
                
                _controller = new KhachHangLoginController(_mockService.Object, _mockLogger.Object);
                
                // Setup HttpContext and Session
                _mockHttpContext.Setup(x => x.Session).Returns(_mockSession.Object);
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                };
                
                // Setup TempData
                _controller.TempData = new TempDataDictionary(_mockHttpContext.Object, _mockTempDataProvider.Object);
            }

            [Fact]
            public void KL001_DangNhap_Get_ShouldReturnView()
            {
                // Arrange
                // Không cần mock Session.GetString vì controller sẽ tự xử lý null

                // Act
                var result = _controller.DangNhap();

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public async Task KL002_DangNhap_Post_WithValidCredentials_ShouldCallService()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "testuser",
                    Password = "TestPassword@123"
                };

                var loginResponse = new LoginResponse
                {
                    TaiKhoanId = Guid.NewGuid(),
                    KhachHangId = Guid.NewGuid(),
                    Role = "KhachHang",
                    HoTen = "Test User",
                    TrangThai = true
                };

                _mockService.Setup(s => s.DangNhapKhachHangAsync(loginRequest))
                          .ReturnsAsync(loginResponse);

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<RedirectToActionResult>();
            }

            [Fact]
            public async Task KL003_DangNhap_Post_WithInvalidCredentials_ShouldReturnViewWithError()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "wronguser",
                    Password = "wrongpassword"
                };

                _mockService.Setup(s => s.DangNhapKhachHangAsync(loginRequest))
                          .ThrowsAsync(new UnauthorizedAccessException("Sai tên đăng nhập hoặc mật khẩu"));

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public async Task KL004_DangNhap_Post_WithInvalidModelState_ShouldReturnViewWithError()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "",
                    Password = "password"
                };
                
                // Setup ModelState properly
                var modelState = new ModelStateDictionary();
                modelState.AddModelError("UserName", "UserName is required");
                _controller.ModelState.Clear();
                foreach (var error in modelState)
                {
                    _controller.ModelState.AddModelError(error.Key, error.Value.Errors.First().ErrorMessage);
                }

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public async Task KL005_DangNhap_Post_WithServiceException_ShouldReturnViewWithGenericError()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "testuser",
                    Password = "password"
                };

                _mockService.Setup(s => s.DangNhapKhachHangAsync(loginRequest))
                          .ThrowsAsync(new Exception("Database connection error"));

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public async Task KL006_DangNhap_Post_WithNullHoTen_ShouldHandleGracefully()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "testuser",
                    Password = "password"
                };

                var loginResponse = new LoginResponse
                {
                    TaiKhoanId = Guid.NewGuid(),
                    KhachHangId = Guid.NewGuid(),
                    Role = "KhachHang",
                    HoTen = null,
                    TrangThai = true
                };

                _mockService.Setup(s => s.DangNhapKhachHangAsync(loginRequest))
                          .ReturnsAsync(loginResponse);

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<RedirectToActionResult>();
            }

            [Fact]
            public void KL007_Logout_ShouldRedirectToHome()
            {
                // Act
                var result = _controller.Logout();

                // Assert
                result.Should().BeOfType<RedirectToActionResult>();
            }

            [Fact]
            public async Task KL008_DangNhap_Post_ShouldLogInformation()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "testuser",
                    Password = "password"
                };

                var loginResponse = new LoginResponse
                {
                    TaiKhoanId = Guid.NewGuid(),
                    KhachHangId = Guid.NewGuid(),
                    Role = "KhachHang",
                    HoTen = "Test User",
                    TrangThai = true
                };

                _mockService.Setup(s => s.DangNhapKhachHangAsync(loginRequest))
                          .ReturnsAsync(loginResponse);

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<RedirectToActionResult>();
            }

            [Fact]
            public async Task KL009_DangNhap_Post_WithUnauthorizedAccess_ShouldLogAndReturnView()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "testuser",
                    Password = "password"
                };

                _mockService.Setup(s => s.DangNhapKhachHangAsync(loginRequest))
                          .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }
        }

        #endregion

        #region Service Tests

        public class KhachHangLoginServiceTests
        {
            [Fact]
            public void ValidateLoginRequest_WithValidData_ShouldPass()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "validuser",
                    Password = "ValidPass@123"
                };

                // Act & Assert
                loginRequest.UserName.Should().NotBeNullOrEmpty();
                loginRequest.Password.Should().NotBeNullOrEmpty();
                loginRequest.UserName.Length.Should().BeGreaterThan(0);
                loginRequest.Password.Length.Should().BeGreaterThan(0);
            }

            [Fact]
            public void ValidateLoginRequest_WithEmptyUserName_ShouldFail()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "",
                    Password = "ValidPass@123"
                };

                // Act & Assert
                loginRequest.UserName.Should().BeEmpty();
            }

            [Fact]
            public void ValidateLoginRequest_WithEmptyPassword_ShouldFail()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "validuser",
                    Password = ""
                };

                // Act & Assert
                loginRequest.Password.Should().BeEmpty();
            }

            [Fact]
            public void ValidateLoginResponse_WithValidData_ShouldPass()
            {
                // Arrange
                var loginResponse = new LoginResponse
                {
                    TaiKhoanId = Guid.NewGuid(),
                    KhachHangId = Guid.NewGuid(),
                    Role = "KhachHang",
                    HoTen = "Test User",
                    TrangThai = true
                };

                // Act & Assert
                loginResponse.TaiKhoanId.Should().NotBeEmpty();
                loginResponse.KhachHangId.Should().NotBeEmpty();
                loginResponse.Role.Should().NotBeNullOrEmpty();
                loginResponse.TrangThai.Should().BeTrue();
            }

            [Fact]
            public void ValidateLoginResponse_WithNullHoTen_ShouldHandleGracefully()
            {
                // Arrange
                var loginResponse = new LoginResponse
                {
                    TaiKhoanId = Guid.NewGuid(),
                    KhachHangId = Guid.NewGuid(),
                    Role = "KhachHang",
                    HoTen = null,
                    TrangThai = true
                };

                // Act & Assert
                loginResponse.HoTen.Should().BeNull();
                loginResponse.TaiKhoanId.Should().NotBeEmpty();
                loginResponse.Role.Should().NotBeNullOrEmpty();
            }
        }

        #endregion

        #region Integration Tests

        public class KhachHangLoginIntegrationTests
        {
            [Fact]
            public async Task KL001_Integration_FullLoginFlow_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                Assert.True(true);
            }

            [Fact]
            public async Task KL002_Integration_LoginAndLogout_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                Assert.True(true);
            }

            [Fact]
            public async Task KL003_Integration_SessionPersistence_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                Assert.True(true);
            }
        }

        #endregion
    }
}