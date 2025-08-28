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

namespace UnitTest.AuthTest
{
    public class AuthControllerTests
    {
        #region Controller Tests

        public class AuthControllerUnitTests
        {
            private readonly Mock<ITaiKhoanService> _mockService;
            private readonly Mock<ILogger<AuthController>> _mockLogger;
            private readonly Mock<HttpContext> _mockHttpContext;
            private readonly Mock<ISession> _mockSession;
            private readonly Mock<ITempDataProvider> _mockTempDataProvider;
            private readonly AuthController _controller;

            public AuthControllerUnitTests()
            {
                _mockService = new Mock<ITaiKhoanService>();
                _mockLogger = new Mock<ILogger<AuthController>>();
                _mockHttpContext = new Mock<HttpContext>();
                _mockSession = new Mock<ISession>();
                _mockTempDataProvider = new Mock<ITempDataProvider>();
                
                _controller = new AuthController(_mockService.Object, _mockLogger.Object);
                
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
            public async Task AU001_DangNhap_Post_WithValidAdminCredentials_ShouldReturnRedirect()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "admin",
                    Password = "adminpassword"
                };

                var adminResponse = new LoginResponse
                {
                    TaiKhoanId = Guid.NewGuid(),
                    Role = "Admin",
                    HoTen = "Admin User",
                    TrangThai = true
                };

                _mockService.Setup(s => s.DangNhapAdminAsync(loginRequest))
                          .ReturnsAsync(adminResponse);

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<RedirectToActionResult>();
            }

            [Fact]
            public async Task AU002_DangNhap_Post_WithInvalidCredentials_ShouldReturnView()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "invalid",
                    Password = "wrongpassword"
                };

                _mockService.Setup(s => s.DangNhapAdminAsync(loginRequest))
                          .ReturnsAsync((LoginResponse?)null);
                _mockService.Setup(s => s.DangNhapKhachHangAsync(loginRequest))
                          .ReturnsAsync((LoginResponse?)null);

                // Act
                var result = await _controller.DangNhap(loginRequest);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public async Task AU003_DangNhap_Post_WithInvalidModelState_ShouldReturnView()
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
            public void AU004_DangNhap_Get_ShouldReturnView()
            {
                // Arrange
                // Không cần mock Session.GetString vì controller sẽ tự xử lý null

                // Act
                var result = _controller.DangNhap();

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public void AU005_Logout_ShouldReturnRedirect()
            {
                // Act
                var result = _controller.Logout();

                // Assert
                result.Should().BeOfType<RedirectToActionResult>();
            }
        }

        #endregion

        #region Service Tests

        public class AuthServiceTests
        {
            [Fact]
            public void LoginRequest_WithValidData_ShouldBeValid()
            {
                // Arrange
                var request = new LoginRequest
                {
                    UserName = "testuser",
                    Password = "testpassword"
                };

                // Act & Assert
                request.UserName.Should().NotBeNullOrEmpty();
                request.Password.Should().NotBeNullOrEmpty();
            }

            [Fact]
            public void LoginResponse_WithValidData_ShouldBeValid()
            {
                // Arrange
                var response = new LoginResponse
                {
                    TaiKhoanId = Guid.NewGuid(),
                    Role = "Admin",
                    HoTen = "Test User",
                    TrangThai = true
                };

                // Act & Assert
                response.TaiKhoanId.Should().NotBeEmpty();
                response.Role.Should().NotBeNullOrEmpty();
                response.HoTen.Should().NotBeNullOrEmpty();
            }
        }

        #endregion
    }
}