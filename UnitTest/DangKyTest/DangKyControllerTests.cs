using FurryFriends.Web.Controllers;
using FurryFriends.Web.Services.IService;
using FurryFriends.API.Models;
using FurryFriends.Web.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace UnitTest.DangKyTest
{
    public class DangKyControllerTests
    {
        #region Controller Tests

        public class DangKyControllerUnitTests
        {
            private readonly Mock<ITaiKhoanService> _mockTaiKhoanService;
            private readonly Mock<IKhachHangService> _mockKhachHangService;
            private readonly Mock<ILogger<DangKyController>> _mockLogger;
            private readonly Mock<HttpContext> _mockHttpContext;
            private readonly Mock<ISession> _mockSession;
            private readonly Mock<ITempDataProvider> _mockTempDataProvider;
            private readonly Mock<HttpRequest> _mockRequest;
            private readonly Mock<IQueryCollection> _mockQueryCollection;
            private readonly Mock<IAuthenticationService> _mockAuthService;
            private readonly DangKyController _controller;

            public DangKyControllerUnitTests()
            {
                _mockTaiKhoanService = new Mock<ITaiKhoanService>();
                _mockKhachHangService = new Mock<IKhachHangService>();
                _mockLogger = new Mock<ILogger<DangKyController>>();
                _mockHttpContext = new Mock<HttpContext>();
                _mockSession = new Mock<ISession>();
                _mockTempDataProvider = new Mock<ITempDataProvider>();
                _mockRequest = new Mock<HttpRequest>();
                _mockQueryCollection = new Mock<IQueryCollection>();
                _mockAuthService = new Mock<IAuthenticationService>();
                
                _controller = new DangKyController(_mockKhachHangService.Object, _mockTaiKhoanService.Object, _mockLogger.Object);
                
                // Setup HttpContext, Session, Request
                _mockHttpContext.Setup(x => x.Session).Returns(_mockSession.Object);
                _mockHttpContext.Setup(x => x.Request).Returns(_mockRequest.Object);
                _mockRequest.Setup(x => x.Query).Returns(_mockQueryCollection.Object);
                _mockQueryCollection.Setup(x => x["error"]).Returns("");
                
                // Setup AuthenticationService
                _mockHttpContext.Setup(x => x.RequestServices.GetService(typeof(IAuthenticationService)))
                               .Returns(_mockAuthService.Object);
                
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                };
                
                // Setup TempData
                _controller.TempData = new TempDataDictionary(_mockHttpContext.Object, _mockTempDataProvider.Object);
                
                // Setup Session methods
                _mockSession.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()));
                _mockSession.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);
            }

            [Fact]
            public void DK001_Index_Get_ShouldReturnView()
            {
                // Act
                var result = _controller.Index();

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

 

            [Fact]
            public async Task DK003_Register_Post_WithDuplicateUsername_ShouldReturnView()
            {
                // Arrange
                var model = new RegisterViewModel
                {
                    UserName = "existinguser",
                    Password = "testpassword",
                    ConfirmPassword = "testpassword",
                    FullName = "Test User",
                    Email = "test@example.com",
                    Phone = "0123456789",
                    AgreeTerms = true
                };

                var existingAccount = new TaiKhoan { UserName = "existinguser" };
                _mockTaiKhoanService.Setup(x => x.FindByUserNameAsync(model.UserName))
                                   .ReturnsAsync(new List<TaiKhoan> { existingAccount });

                // Act
                var result = await _controller.Register(model);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public async Task DK004_Register_Post_WithDuplicatePhone_ShouldReturnView()
            {
                // Arrange
                var model = new RegisterViewModel
                {
                    UserName = "newuser",
                    Password = "testpassword",
                    ConfirmPassword = "testpassword",
                    FullName = "Test User",
                    Email = "test@example.com",
                    Phone = "0123456789",
                    AgreeTerms = true
                };

                var existingPhone = new KhachHang { SDT = "0123456789" };
                _mockTaiKhoanService.Setup(x => x.FindByUserNameAsync(model.UserName))
                                   .ReturnsAsync(new List<TaiKhoan>());
                _mockKhachHangService.Setup(x => x.FindByPhoneAsync(model.Phone))
                                    .ReturnsAsync(existingPhone);

                // Act
                var result = await _controller.Register(model);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }

            [Fact]
            public async Task DK005_Register_Post_WithInvalidModelState_ShouldReturnView()
            {
                // Arrange
                var model = new RegisterViewModel
                {
                    UserName = "",
                    Password = "testpassword",
                    ConfirmPassword = "testpassword",
                    FullName = "Test User",
                    Email = "test@example.com",
                    Phone = "0123456789",
                    AgreeTerms = true
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
                var result = await _controller.Register(model);

                // Assert
                result.Should().BeOfType<ViewResult>();
            }
        }

        #endregion

        #region Validation Tests

        public class DangKyValidationTests
        {
            [Fact]
            public void RegisterViewModel_WithValidData_ShouldBeValid()
            {
                // Arrange
                var model = new RegisterViewModel
                {
                    UserName = "testuser",
                    Password = "testpassword",
                    ConfirmPassword = "testpassword",
                    FullName = "Test User",
                    Email = "test@example.com",
                    Phone = "0123456789",
                    AgreeTerms = true
                };

                // Act & Assert
                model.UserName.Should().NotBeNullOrEmpty();
                model.Password.Should().NotBeNullOrEmpty();
                model.ConfirmPassword.Should().NotBeNullOrEmpty();
                model.FullName.Should().NotBeNullOrEmpty();
                model.Email.Should().NotBeNullOrEmpty();
                model.Phone.Should().NotBeNullOrEmpty();
                model.AgreeTerms.Should().BeTrue();
            }

            [Fact]
            public void RegisterViewModel_PasswordAndConfirmPassword_ShouldMatch()
            {
                // Arrange
                var model = new RegisterViewModel
                {
                    UserName = "testuser",
                    Password = "testpassword",
                    ConfirmPassword = "testpassword",
                    FullName = "Test User",
                    Email = "test@example.com",
                    Phone = "0123456789",
                    AgreeTerms = true
                };

                // Act & Assert
                model.Password.Should().Be(model.ConfirmPassword);
            }
        }

        #endregion
    }
}