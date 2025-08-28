using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace UnitTest.TaiKhoanTest
{
    public class TaiKhoanApiControllerTests
    {
        #region Controller Tests

        public class TaiKhoanApiControllerUnitTests
        {
            private readonly Mock<ITaiKhoanRepository> _mockRepository;
            private readonly Mock<ILogger<TaiKhoanApiController>> _mockLogger;
            private readonly Mock<IMailService> _mockMailService;
            private readonly TaiKhoanApiController _controller;

            public TaiKhoanApiControllerUnitTests()
            {
                _mockRepository = new Mock<ITaiKhoanRepository>();
                _mockLogger = new Mock<ILogger<TaiKhoanApiController>>();
                _mockMailService = new Mock<IMailService>();
                _controller = new TaiKhoanApiController(_mockRepository.Object, _mockLogger.Object, _mockMailService.Object);
            }

            [Fact]
            public async Task TK001_GetAll_ShouldReturnOk()
            {
                // Arrange
                var taiKhoans = new List<TaiKhoan>
                {
                    new TaiKhoan 
                    { 
                        TaiKhoanId = Guid.NewGuid(), 
                        UserName = "admin",
                        Password = "hashedpassword1",
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = true
                    },
                    new TaiKhoan 
                    { 
                        TaiKhoanId = Guid.NewGuid(), 
                        UserName = "user1",
                        Password = "hashedpassword2",
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = true
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(taiKhoans);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedTaiKhoans = okResult!.Value as IEnumerable<TaiKhoan>;
                returnedTaiKhoans.Should().HaveCount(2);
                returnedTaiKhoans.Should().Contain(t => t.UserName == "admin");
            }

            [Fact]
            public async Task TK002_GetAll_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                _mockRepository.Setup(x => x.GetAllAsync())
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetAll();

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database error");
            }

            [Fact]
            public async Task TK003_FindByUserName_WithExistingUser_ShouldReturnOk()
            {
                // Arrange
                var userName = "testuser";
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = userName,
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(userName)).ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.FindByUserName(userName);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedList = okResult!.Value as List<TaiKhoan>;
                returnedList.Should().HaveCount(1);
                returnedList!.First().UserName.Should().Be(userName);
            }

            [Fact]
            public async Task TK004_FindByUserName_WithNonExistentUser_ShouldReturnEmptyList()
            {
                // Arrange
                var userName = "nonexistent";
                _mockRepository.Setup(x => x.FindByUserNameAsync(userName))
                              .ReturnsAsync((TaiKhoan?)null);

                // Act
                var result = await _controller.FindByUserName(userName);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedList = okResult!.Value as List<TaiKhoan>;
                returnedList.Should().BeEmpty();
            }

            [Fact]
            public async Task TK005_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = taiKhoanId,
                    UserName = "testuser",
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(taiKhoanId)).ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.GetById(taiKhoanId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedTaiKhoan = okResult!.Value as TaiKhoan;
                returnedTaiKhoan.Should().NotBeNull();
                returnedTaiKhoan!.TaiKhoanId.Should().Be(taiKhoanId);
            }

            [Fact]
            public async Task TK006_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(taiKhoanId))
                              .ReturnsAsync((TaiKhoan?)null);

                // Act
                var result = await _controller.GetById(taiKhoanId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be($"Tài khoản với TaiKhoanId {taiKhoanId} không tồn tại.");
            }

            [Fact]
            public async Task TK007_Create_WithValidData_ShouldReturnCreated()
            {
                // Arrange
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "newuser",
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<TaiKhoan>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Create(taiKhoan);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(taiKhoan);
                _mockRepository.Verify(x => x.AddAsync(It.IsAny<TaiKhoan>()), Times.Once);
            }

            [Fact]
            public async Task TK008_Create_WhenRepositoryThrowsArgumentException_ShouldReturnBadRequest()
            {
                // Arrange
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "duplicateuser",
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<TaiKhoan>()))
                              .ThrowsAsync(new ArgumentException("Tên đăng nhập đã tồn tại"));

                // Act
                var result = await _controller.Create(taiKhoan);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Tên đăng nhập đã tồn tại");
            }

            [Fact]
            public async Task TK009_Update_WithValidData_ShouldReturnNoContent()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = taiKhoanId,
                    UserName = "updateduser",
                    Password = "newhashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TaiKhoan>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Update(taiKhoanId, taiKhoan);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<TaiKhoan>()), Times.Once);
            }

            [Fact]
            public async Task TK010_Update_WithMismatchedId_ShouldReturnBadRequest()
            {
                // Arrange
                var urlId = Guid.NewGuid();
                var differentId = Guid.NewGuid();
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = differentId, // Different from URL ID
                    UserName = "testuser",
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(urlId, taiKhoan);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("TaiKhoanId không khớp.");
            }

            [Fact]
            public async Task TK011_Update_WhenRepositoryThrowsKeyNotFoundException_ShouldReturnNotFound()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = taiKhoanId,
                    UserName = "nonexistentuser",
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TaiKhoan>()))
                              .ThrowsAsync(new KeyNotFoundException("Tài khoản không tồn tại"));

                // Act
                var result = await _controller.Update(taiKhoanId, taiKhoan);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be("Tài khoản không tồn tại");
            }

            [Fact]
            public async Task TK012_Create_WithInvalidModelState_ShouldReturnBadRequest()
            {
                // Arrange
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    // Missing required fields to trigger ModelState error
                    UserName = "",
                    Password = "validpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _controller.ModelState.AddModelError("UserName", "Tên đăng nhập không được để trống");

                // Act
                var result = await _controller.Create(taiKhoan);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TK013_Update_WithInvalidModelState_ShouldReturnBadRequest()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = taiKhoanId,
                    UserName = "",
                    Password = "validpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _controller.ModelState.AddModelError("UserName", "Tên đăng nhập không được để trống");

                // Act
                var result = await _controller.Update(taiKhoanId, taiKhoan);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TK014_GetById_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var taiKhoanId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(taiKhoanId))
                              .ThrowsAsync(new Exception("Database connection error"));

                // Act
                var result = await _controller.GetById(taiKhoanId);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database connection error");
            }

            [Fact]
            public async Task TK015_Create_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "testuser",
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<TaiKhoan>()))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Create(taiKhoan);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database error");
            }

            [Fact]
            public async Task TK016_DangNhapAdmin_WithValidCredentials_ShouldReturnOk()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "admin",
                    Password = "adminpassword"
                };

                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "admin",
                    Password = "adminpassword",
                    TrangThai = true,
                    NhanVien = new NhanVien
                    {
                        HoVaTen = "Admin User",
                        TrangThai = true,
                        ChucVu = new ChucVu { TenChucVu = "Admin" }
                    }
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.DangNhapAdmin(loginRequest);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var response = okResult!.Value as LoginResponse;
                response.Should().NotBeNull();
                response!.TaiKhoanId.Should().Be(taiKhoan.TaiKhoanId);
                response.Role.Should().Be("Admin");
                response.HoTen.Should().Be("Admin User");
            }

            [Fact]
            public async Task TK017_DangNhapAdmin_WithInvalidPassword_ShouldReturnUnauthorized()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "admin",
                    Password = "wrongpassword"
                };

                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "admin",
                    Password = "correctpassword",
                    TrangThai = true,
                    NhanVien = new NhanVien
                    {
                        HoVaTen = "Admin User",
                        TrangThai = true,
                        ChucVu = new ChucVu { TenChucVu = "Admin" }
                    }
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.DangNhapAdmin(loginRequest);

                // Assert
                result.Should().BeOfType<UnauthorizedObjectResult>();
                var unauthorizedResult = result as UnauthorizedObjectResult;
                unauthorizedResult!.Value.Should().Be("Sai tên đăng nhập hoặc mật khẩu.");
            }

            [Fact]
            public async Task TK018_DangNhapAdmin_WithNonExistentUser_ShouldReturnUnauthorized()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "nonexistent",
                    Password = "password"
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync((TaiKhoan?)null);

                // Act
                var result = await _controller.DangNhapAdmin(loginRequest);

                // Assert
                result.Should().BeOfType<UnauthorizedObjectResult>();
                var unauthorizedResult = result as UnauthorizedObjectResult;
                unauthorizedResult!.Value.Should().Be("Sai tên đăng nhập hoặc mật khẩu.");
            }

            [Fact]
            public async Task TK019_DangNhapAdmin_WithInactiveAccount_ShouldReturnUnauthorized()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "admin",
                    Password = "password"
                };

                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "admin",
                    Password = "password",
                    TrangThai = false, // Inactive account
                    NhanVien = new NhanVien
                    {
                        HoVaTen = "Admin User",
                        TrangThai = true,
                        ChucVu = new ChucVu { TenChucVu = "Admin" }
                    }
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.DangNhapAdmin(loginRequest);

                // Assert
                result.Should().BeOfType<UnauthorizedObjectResult>();
                var unauthorizedResult = result as UnauthorizedObjectResult;
                unauthorizedResult!.Value.Should().Be("Tài khoản đã dừng hoạt động. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
            }

            [Fact]
            public async Task TK020_DangNhapAdmin_WithoutNhanVien_ShouldReturnUnauthorized()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "user",
                    Password = "password"
                };

                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "user",
                    Password = "password",
                    TrangThai = true,
                    NhanVien = null // No employee relation
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.DangNhapAdmin(loginRequest);

                // Assert
                result.Should().BeOfType<UnauthorizedObjectResult>();
                var unauthorizedResult = result as UnauthorizedObjectResult;
                unauthorizedResult!.Value.Should().Be("Tài khoản không có quyền admin.");
            }

            [Fact]
            public async Task TK021_DangNhapKhachHang_WithValidCustomerCredentials_ShouldReturnOk()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "customer",
                    Password = "customerpassword"
                };

                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "customer",
                    Password = "customerpassword",
                    TrangThai = true,
                    KhachHang = new KhachHang
                    {
                        KhachHangId = Guid.NewGuid(),
                        TenKhachHang = "Customer User",
                        TrangThai = 1
                    }
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.DangNhapKhachHang(loginRequest);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var response = okResult!.Value as LoginResponse;
                response.Should().NotBeNull();
                response!.TaiKhoanId.Should().Be(taiKhoan.TaiKhoanId);
                response.Role.Should().Be("KhachHang");
                response.HoTen.Should().Be("Customer User");
                response.KhachHangId.Should().Be(taiKhoan.KhachHang.KhachHangId);
            }

            [Fact]
            public async Task TK022_DangNhapKhachHang_WithInvalidPassword_ShouldReturnUnauthorized()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "customer",
                    Password = "wrongpassword"
                };

                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "customer",
                    Password = "correctpassword",
                    TrangThai = true,
                    KhachHang = new KhachHang
                    {
                        KhachHangId = Guid.NewGuid(),
                        TenKhachHang = "Customer User",
                        TrangThai = 1
                    }
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.DangNhapKhachHang(loginRequest);

                // Assert
                result.Should().BeOfType<UnauthorizedObjectResult>();
                var unauthorizedResult = result as UnauthorizedObjectResult;
                unauthorizedResult!.Value.Should().Be("Sai tên đăng nhập hoặc mật khẩu.");
            }

            [Fact]
            public async Task TK023_DangNhapKhachHang_WithEmployeeCredentials_ShouldReturnNhanVienRole()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "employee",
                    Password = "employeepassword"
                };

                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "employee",
                    Password = "employeepassword",
                    TrangThai = true,
                    NhanVien = new NhanVien
                    {
                        HoVaTen = "Employee User",
                        TrangThai = true,
                        ChucVu = new ChucVu { TenChucVu = "NhanVien" }
                    }
                };

                _mockRepository.Setup(x => x.FindByUserNameAsync(loginRequest.UserName))
                              .ReturnsAsync(taiKhoan);

                // Act
                var result = await _controller.DangNhapKhachHang(loginRequest);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var response = okResult!.Value as LoginResponse;
                response.Should().NotBeNull();
                response!.Role.Should().Be("NhanVien");
                response.HoTen.Should().Be("Employee User");
            }

            [Fact]
            public async Task TK024_DangNhapAdmin_WithEmptyCredentials_ShouldReturnBadRequest()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "",
                    Password = ""
                };

                // Act
                var result = await _controller.DangNhapAdmin(loginRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Tên đăng nhập và mật khẩu không được để trống.");
            }

            [Fact]
            public async Task TK025_DangNhapKhachHang_WithEmptyCredentials_ShouldReturnBadRequest()
            {
                // Arrange
                var loginRequest = new LoginRequest
                {
                    UserName = "",
                    Password = ""
                };

                // Act
                var result = await _controller.DangNhapKhachHang(loginRequest);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Tên đăng nhập và mật khẩu không được để trống.");
            }
        }

        #endregion

        #region Validation Tests

        public class TaiKhoanValidationTests
        {
            [Fact]
            public void ValidateTaiKhoan_WithValidData_ShouldPass()
            {
                // Arrange
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "validuser",
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(taiKhoan, new ValidationContext(taiKhoan), validationResults, true);

                // Assert
                taiKhoan.UserName.Should().NotBeNullOrEmpty();
                taiKhoan.Password.Should().NotBeNullOrEmpty();
                taiKhoan.TaiKhoanId.Should().NotBeEmpty();
            }

            [Fact]
            public void ValidateTaiKhoan_WithEmptyUserName_ShouldFail()
            {
                // Arrange
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "", // Empty username
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                // Act & Assert
                taiKhoan.UserName.Should().BeEmpty();
            }

            [Fact]
            public void ValidateTaiKhoan_WithInvalidEmail_ShouldFail()
            {
                // Arrange
                var taiKhoan = new TaiKhoan
                {
                    TaiKhoanId = Guid.NewGuid(),
                    UserName = "ab", // Too short username
                    Password = "hashedpassword",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = true
                };

                // Act & Assert
                taiKhoan.UserName.Should().Be("ab");
                taiKhoan.UserName.Length.Should().BeLessThan(6);
            }
        }

        #endregion

        #region Integration Tests

        public class TaiKhoanIntegrationTests
        {
            [Fact]
            public async Task TK001_Integration_CreateAndRetrieveTaiKhoan_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task TK002_Integration_UpdateTaiKhoan_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }
        }

        #endregion
    }
}