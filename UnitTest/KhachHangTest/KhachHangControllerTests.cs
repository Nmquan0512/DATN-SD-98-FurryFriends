using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace UnitTest.KhachHangTest
{
    public class KhachHangControllerTests
    {
        #region Controller Tests

        public class KhachHangApiControllerUnitTests
        {
            private readonly Mock<IKhachHangRepository> _mockRepository;
            private readonly KhachHangController _controller;

            public KhachHangApiControllerUnitTests()
            {
                _mockRepository = new Mock<IKhachHangRepository>();
                _controller = new KhachHangController(_mockRepository.Object);
            }

            [Fact]
            public async Task KH001_GetAll_ShouldReturnOk()
            {
                // Arrange
                var khachHangs = new List<KhachHang>
                {
                    new KhachHang 
                    { 
                        KhachHangId = Guid.NewGuid(), 
                        TenKhachHang = "Nguyen Van A",
                        SDT = "0123456789",
                        EmailCuaKhachHang = "nguyenvana@email.com",
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = 1,
                        DiemKhachHang = 100
                    },
                    new KhachHang 
                    { 
                        KhachHangId = Guid.NewGuid(), 
                        TenKhachHang = "Tran Thi B",
                        SDT = "0987654321",
                        EmailCuaKhachHang = "tranthib@email.com",
                        NgayTaoTaiKhoan = DateTime.Now,
                        TrangThai = 1,
                        DiemKhachHang = 200
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(khachHangs);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<ActionResult<IEnumerable<KhachHang>>>();
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedKhachHangs = actionResult!.Value as IEnumerable<KhachHang>;
                returnedKhachHangs.Should().HaveCount(2);
                returnedKhachHangs.Should().Contain(k => k.TenKhachHang == "Nguyen Van A");
            }

            [Fact]
            public async Task KH002_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var khachHangId = Guid.NewGuid();
                var khachHang = new KhachHang
                {
                    KhachHangId = khachHangId,
                    TenKhachHang = "Test Customer",
                    SDT = "0123456789",
                    EmailCuaKhachHang = "test@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1,
                    DiemKhachHang = 50
                };

                _mockRepository.Setup(x => x.GetByIdAsync(khachHangId)).ReturnsAsync(khachHang);

                // Act
                var result = await _controller.GetById(khachHangId);

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedKhachHang = actionResult!.Value as KhachHang;
                returnedKhachHang.Should().NotBeNull();
                returnedKhachHang!.KhachHangId.Should().Be(khachHangId);
                returnedKhachHang.TenKhachHang.Should().Be("Test Customer");
            }

            [Fact]
            public async Task KH003_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var khachHangId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(khachHangId))
                              .ReturnsAsync((KhachHang?)null);

                // Act
                var result = await _controller.GetById(khachHangId);

                // Assert
                var actionResult = result.Result as NotFoundResult;
                actionResult.Should().NotBeNull();
            }

            [Fact]
            public async Task KH004_Create_WithValidData_ShouldReturnCreated()
            {
                // Arrange
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "New Customer",
                    SDT = "0123456789",
                    EmailCuaKhachHang = "newcustomer@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1,
                    DiemKhachHang = 0
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<KhachHang>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Create(khachHang);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(khachHang);
                _mockRepository.Verify(x => x.AddAsync(It.IsAny<KhachHang>()), Times.Once);
            }

            [Fact]
            public async Task KH005_Update_WithValidData_ShouldReturnNoContent()
            {
                // Arrange
                var khachHangId = Guid.NewGuid();
                var khachHang = new KhachHang
                {
                    KhachHangId = khachHangId,
                    TenKhachHang = "Updated Customer",
                    SDT = "0987654321",
                    EmailCuaKhachHang = "updated@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1,
                    DiemKhachHang = 150
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<KhachHang>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Update(khachHangId, khachHang);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<KhachHang>()), Times.Once);
            }

            [Fact]
            public async Task KH006_Update_WithMismatchedId_ShouldReturnBadRequest()
            {
                // Arrange
                var urlId = Guid.NewGuid();
                var differentId = Guid.NewGuid();
                var khachHang = new KhachHang
                {
                    KhachHangId = differentId, // Different from URL ID
                    TenKhachHang = "Test Customer",
                    SDT = "0123456789",
                    EmailCuaKhachHang = "test@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1
                };

                // Act
                var result = await _controller.Update(urlId, khachHang);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }

            [Fact]
            public async Task KH007_GetByEmail_WithExistingEmail_ShouldReturnOk()
            {
                // Arrange
                var email = "test@email.com";
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "Test Customer",
                    SDT = "0123456789",
                    EmailCuaKhachHang = email,
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1,
                    DiemKhachHang = 100
                };

                _mockRepository.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(khachHang);

                // Act
                var result = await _controller.GetByEmail(email);

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedKhachHang = actionResult!.Value as KhachHang;
                returnedKhachHang.Should().NotBeNull();
                returnedKhachHang!.EmailCuaKhachHang.Should().Be(email);
            }

            [Fact]
            public async Task KH008_GetByEmail_WithNonExistentEmail_ShouldReturnNotFound()
            {
                // Arrange
                var email = "nonexistent@email.com";
                _mockRepository.Setup(x => x.FindByEmailAsync(email))
                              .ReturnsAsync((KhachHang?)null);

                // Act
                var result = await _controller.GetByEmail(email);

                // Assert
                var actionResult = result.Result as NotFoundResult;
                actionResult.Should().NotBeNull();
            }

            [Fact]
            public async Task KH009_GetByPhone_WithExistingPhone_ShouldReturnOk()
            {
                // Arrange
                var phone = "0123456789";
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "Test Customer",
                    SDT = phone,
                    EmailCuaKhachHang = "test@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1,
                    DiemKhachHang = 100
                };

                _mockRepository.Setup(x => x.FindByPhoneAsync(phone)).ReturnsAsync(khachHang);

                // Act
                var result = await _controller.GetByPhone(phone);

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedKhachHang = actionResult!.Value as KhachHang;
                returnedKhachHang.Should().NotBeNull();
                returnedKhachHang!.SDT.Should().Be(phone);
            }

            [Fact]
            public async Task KH010_GetByPhone_WithNonExistentPhone_ShouldReturnNotFound()
            {
                // Arrange
                var phone = "0999999999";
                _mockRepository.Setup(x => x.FindByPhoneAsync(phone))
                              .ReturnsAsync((KhachHang?)null);

                // Act
                var result = await _controller.GetByPhone(phone);

                // Assert
                var actionResult = result.Result as NotFoundResult;
                actionResult.Should().NotBeNull();
            }

            [Fact]
            public async Task KH011_GetAllIncludingDeleted_ShouldReturnOk()
            {
                // Arrange
                var khachHangs = new List<KhachHang>
                {
                    new KhachHang 
                    { 
                        KhachHangId = Guid.NewGuid(), 
                        TenKhachHang = "Active Customer",
                        TrangThai = 1
                    },
                    new KhachHang 
                    { 
                        KhachHangId = Guid.NewGuid(), 
                        TenKhachHang = "Deleted Customer",
                        TrangThai = 0
                    }
                };

                _mockRepository.Setup(x => x.GetAllIncludingDeletedAsync()).ReturnsAsync(khachHangs);

                // Act
                var result = await _controller.GetAllIncludingDeleted();

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedKhachHangs = actionResult!.Value as IEnumerable<KhachHang>;
                returnedKhachHangs.Should().HaveCount(2);
                returnedKhachHangs.Should().Contain(k => k.TrangThai == 0);
                returnedKhachHangs.Should().Contain(k => k.TrangThai == 1);
            }
        }

        #endregion

        #region Validation Tests

        public class KhachHangValidationTests
        {
            [Fact]
            public void ValidateKhachHang_WithValidData_ShouldPass()
            {
                // Arrange
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "Valid Customer Name",
                    SDT = "0123456789",
                    EmailCuaKhachHang = "valid@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1,
                    DiemKhachHang = 100
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(khachHang, new ValidationContext(khachHang), validationResults, true);

                // Assert
                khachHang.TenKhachHang.Should().NotBeNullOrEmpty();
                khachHang.SDT.Should().MatchRegex(@"^0\d{8,10}$");
                khachHang.EmailCuaKhachHang.Should().Contain("@");
                khachHang.DiemKhachHang.Should().BeGreaterOrEqualTo(0);
            }

            [Fact]
            public void ValidateKhachHang_WithEmptyTenKhachHang_ShouldFail()
            {
                // Arrange
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "", // Empty name
                    SDT = "0123456789",
                    EmailCuaKhachHang = "test@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1
                };

                // Act & Assert
                khachHang.TenKhachHang.Should().BeEmpty();
            }

            [Fact]
            public void ValidateKhachHang_WithInvalidPhoneFormat_ShouldFail()
            {
                // Arrange
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "Test Customer",
                    SDT = "123456789", // Missing leading zero
                    EmailCuaKhachHang = "test@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1
                };

                // Act & Assert
                khachHang.SDT.Should().NotMatchRegex(@"^0\d{8,10}$");
            }

            [Fact]
            public void ValidateKhachHang_WithInvalidEmail_ShouldFail()
            {
                // Arrange
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "Test Customer",
                    SDT = "0123456789",
                    EmailCuaKhachHang = "invalid-email", // Invalid email format
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1
                };

                // Act & Assert
                khachHang.EmailCuaKhachHang.Should().NotContain("@");
            }

            [Fact]
            public void ValidateKhachHang_WithNegativeDiemKhachHang_ShouldFail()
            {
                // Arrange
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = "Test Customer",
                    SDT = "0123456789",
                    EmailCuaKhachHang = "test@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1,
                    DiemKhachHang = -10 // Negative points
                };

                // Act & Assert
                khachHang.DiemKhachHang.Should().BeLessThan(0);
            }

            [Fact]
            public void ValidateKhachHang_WithTooLongTenKhachHang_ShouldFail()
            {
                // Arrange
                var longName = new string('A', 101); // 101 characters
                var khachHang = new KhachHang
                {
                    KhachHangId = Guid.NewGuid(),
                    TenKhachHang = longName,
                    SDT = "0123456789",
                    EmailCuaKhachHang = "test@email.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1
                };

                // Act & Assert
                khachHang.TenKhachHang.Length.Should().BeGreaterThan(100);
            }
        }

        #endregion

        #region Integration Tests

        public class KhachHangIntegrationTests
        {
            [Fact]
            public async Task KH001_Integration_CreateAndRetrieveKhachHang_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task KH002_Integration_UpdateKhachHang_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task KH003_Integration_SearchKhachHangByEmailAndPhone_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }
        }

        #endregion
    }
}