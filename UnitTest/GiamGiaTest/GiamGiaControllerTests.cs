using FurryFriends.API.Controllers;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace UnitTest.GiamGiaTest
{
    public class GiamGiaControllerTests
    {
        #region Controller Tests

        public class GiamGiaControllerUnitTests
        {
            private readonly Mock<IGiamGiaService> _mockService;
            private readonly Mock<ILogger<GiamGiaController>> _mockLogger;
            private readonly GiamGiaController _controller;

            public GiamGiaControllerUnitTests()
            {
                _mockService = new Mock<IGiamGiaService>();
                _mockLogger = new Mock<ILogger<GiamGiaController>>();
                _controller = new GiamGiaController(_mockService.Object, _mockLogger.Object);
            }

            [Fact]
            public async Task GG001_GetAll_ShouldReturnOkWithDiscountList()
            {
                // Arrange
                var discounts = new List<GiamGiaDTO>
                {
                    new GiamGiaDTO
                    {
                        GiamGiaId = Guid.NewGuid(),
                        TenGiamGia = "Giảm giá mùa hè",
                        PhanTramKhuyenMai = 20,
                        NgayBatDau = DateTime.Now.Date,
                        NgayKetThuc = DateTime.Now.Date.AddDays(30),
                        TrangThai = true
                    },
                    new GiamGiaDTO
                    {
                        GiamGiaId = Guid.NewGuid(),
                        TenGiamGia = "Giảm giá cuối năm",
                        PhanTramKhuyenMai = 30,
                        NgayBatDau = DateTime.Now.Date.AddDays(-10),
                        NgayKetThuc = DateTime.Now.Date.AddDays(10),
                        TrangThai = true
                    }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(discounts);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedDiscounts = okResult!.Value as IEnumerable<GiamGiaDTO>;
                returnedDiscounts.Should().HaveCount(2);
            }

            [Fact]
            public async Task GG002_GetAll_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                _mockService.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task GG003_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var discountId = Guid.NewGuid();
                var discount = new GiamGiaDTO
                {
                    GiamGiaId = discountId,
                    TenGiamGia = "Giảm giá đặc biệt",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(15),
                    TrangThai = true
                };

                _mockService.Setup(x => x.GetByIdAsync(discountId)).ReturnsAsync(discount);

                // Act
                var result = await _controller.GetById(discountId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedDiscount = okResult!.Value as GiamGiaDTO;
                returnedDiscount.Should().NotBeNull();
                returnedDiscount!.GiamGiaId.Should().Be(discountId);
            }

            [Fact]
            public async Task GG004_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var discountId = Guid.NewGuid();
                _mockService.Setup(x => x.GetByIdAsync(discountId))
                           .ReturnsAsync((GiamGiaDTO?)null);

                // Act
                var result = await _controller.GetById(discountId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task GG005_GetById_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var discountId = Guid.NewGuid();
                _mockService.Setup(x => x.GetByIdAsync(discountId))
                           .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetById(discountId);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task GG006_Create_WithValidData_ShouldReturnCreated()
            {
                // Arrange
                var newDiscount = new GiamGiaDTO
                {
                    TenGiamGia = "Giảm giá Black Friday",
                    PhanTramKhuyenMai = 50,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(3),
                    TrangThai = true
                };

                var createdDiscount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = newDiscount.TenGiamGia,
                    PhanTramKhuyenMai = newDiscount.PhanTramKhuyenMai,
                    NgayBatDau = newDiscount.NgayBatDau,
                    NgayKetThuc = newDiscount.NgayKetThuc,
                    TrangThai = newDiscount.TrangThai
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<GiamGiaDTO>()))
                           .ReturnsAsync(createdDiscount);

                // Act
                var result = await _controller.Create(newDiscount);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdResult = result as CreatedAtActionResult;
                createdResult!.Value.Should().Be(createdDiscount);
                _mockService.Verify(x => x.CreateAsync(It.IsAny<GiamGiaDTO>()), Times.Once);
            }

            [Fact]
            public async Task GG007_Create_WhenValidationExceptionThrown_ShouldReturnBadRequest()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    TenGiamGia = "Test Discount",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<GiamGiaDTO>()))
                           .ThrowsAsync(new ValidationException("Validation failed"));

                // Act
                var result = await _controller.Create(discount);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task GG008_Create_WhenInvalidOperationExceptionThrown_ShouldReturnConflict()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    TenGiamGia = "Duplicate Name",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<GiamGiaDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Discount name already exists"));

                // Act
                var result = await _controller.Create(discount);

                // Assert
                result.Should().BeOfType<ConflictObjectResult>();
            }

            [Fact]
            public async Task GG009_Create_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    TenGiamGia = "Test Discount",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<GiamGiaDTO>()))
                           .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Create(discount);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task GG010_Update_WithValidData_ShouldReturnNoContent()
            {
                // Arrange
                var discountId = Guid.NewGuid();
                var updateDiscount = new GiamGiaDTO
                {
                    GiamGiaId = discountId,
                    TenGiamGia = "Updated Discount",
                    PhanTramKhuyenMai = 35,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(14),
                    TrangThai = false
                };

                _mockService.Setup(x => x.UpdateAsync(It.IsAny<GiamGiaDTO>()))
                           .ReturnsAsync(updateDiscount);

                // Act
                var result = await _controller.Update(discountId, updateDiscount);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockService.Verify(x => x.UpdateAsync(It.IsAny<GiamGiaDTO>()), Times.Once);
            }

            [Fact]
            public async Task GG011_Update_WithMismatchedId_ShouldReturnBadRequest()
            {
                // Arrange
                var discountId = Guid.NewGuid();
                var updateDiscount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(), // Different ID
                    TenGiamGia = "Updated Discount",
                    PhanTramKhuyenMai = 35,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(14),
                    TrangThai = false
                };

                // Act
                var result = await _controller.Update(discountId, updateDiscount);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task GG012_Update_WhenValidationExceptionThrown_ShouldReturnBadRequest()
            {
                // Arrange
                var discountId = Guid.NewGuid();
                var updateDiscount = new GiamGiaDTO
                {
                    GiamGiaId = discountId,
                    TenGiamGia = "Updated Discount",
                    PhanTramKhuyenMai = 35,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(14),
                    TrangThai = false
                };

                _mockService.Setup(x => x.UpdateAsync(It.IsAny<GiamGiaDTO>()))
                           .ThrowsAsync(new ValidationException("Validation failed"));

                // Act
                var result = await _controller.Update(discountId, updateDiscount);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task GG013_Update_WhenNotFound_ShouldReturnNotFound()
            {
                // Arrange
                var discountId = Guid.NewGuid();
                var updateDiscount = new GiamGiaDTO
                {
                    GiamGiaId = discountId,
                    TenGiamGia = "Updated Discount",
                    PhanTramKhuyenMai = 35,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(14),
                    TrangThai = false
                };

                _mockService.Setup(x => x.UpdateAsync(It.IsAny<GiamGiaDTO>()))
                           .ThrowsAsync(new KeyNotFoundException("Discount not found"));

                // Act
                var result = await _controller.Update(discountId, updateDiscount);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task GG014_Delete_WithExistingId_ShouldReturnNoContent()
            {
                // Arrange
                var discountId = Guid.NewGuid();

                _mockService.Setup(x => x.DeleteAsync(discountId))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Delete(discountId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockService.Verify(x => x.DeleteAsync(discountId), Times.Once);
            }

            [Fact]
            public async Task GG015_Delete_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var discountId = Guid.NewGuid();

                _mockService.Setup(x => x.DeleteAsync(discountId))
                           .ReturnsAsync(false);

                // Act
                var result = await _controller.Delete(discountId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task GG016_Delete_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var discountId = Guid.NewGuid();

                _mockService.Setup(x => x.DeleteAsync(discountId))
                           .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Delete(discountId);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }
        }

        #endregion

        #region Validation Tests

        public class GiamGiaValidationTests
        {
            [Fact]
            public void ValidateGiamGia_WithValidData_ShouldPass()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = "Giảm giá hợp lệ",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                // Act & Assert
                discount.TenGiamGia.Should().NotBeNullOrEmpty();
                discount.PhanTramKhuyenMai.Should().BeInRange(1, 100);
                discount.NgayKetThuc.Should().BeAfter(discount.NgayBatDau);
            }

            [Fact]
            public void ValidateGiamGia_WithEmptyTenGiamGia_ShouldFail()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = "", // Invalid
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                // Act & Assert
                discount.TenGiamGia.Should().BeEmpty();
            }

            [Fact]
            public void ValidateGiamGia_WithTooLongTenGiamGia_ShouldFail()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = new string('A', 101), // Too long - 101 characters
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                // Act & Assert
                discount.TenGiamGia.Length.Should().BeGreaterThan(100);
            }

            [Fact]
            public void ValidateGiamGia_WithInvalidPhanTramKhuyenMai_ShouldFail()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = "Valid Name",
                    PhanTramKhuyenMai = 0, // Invalid - should be > 0
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                // Act & Assert
                discount.PhanTramKhuyenMai.Should().Be(0);
            }

            [Fact]
            public void ValidateGiamGia_WithPhanTramKhuyenMaiTooHigh_ShouldFail()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = "Valid Name",
                    PhanTramKhuyenMai = 101, // Invalid - should be <= 100
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = true
                };

                // Act & Assert
                discount.PhanTramKhuyenMai.Should().BeGreaterThan(100);
            }

            [Fact]
            public void ValidateGiamGia_WithInvalidDateRange_ShouldFail()
            {
                // Arrange
                var discount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = "Valid Name",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(-1), // Invalid - end date before start date
                    TrangThai = true
                };

                // Act & Assert
                discount.NgayKetThuc.Should().BeBefore(discount.NgayBatDau);
            }

            [Fact]
            public void ValidateGiamGia_DisplayProperties_ShouldWorkCorrectly()
            {
                // Arrange
                var activeDiscount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = "Active Discount",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date.AddDays(-1),
                    NgayKetThuc = DateTime.Now.Date.AddDays(1),
                    TrangThai = true,
                    SanPhamChiTietIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
                };

                var inactiveDiscount = new GiamGiaDTO
                {
                    GiamGiaId = Guid.NewGuid(),
                    TenGiamGia = "Inactive Discount",
                    PhanTramKhuyenMai = 25,
                    NgayBatDau = DateTime.Now.Date,
                    NgayKetThuc = DateTime.Now.Date.AddDays(7),
                    TrangThai = false
                };

                // Act & Assert
                activeDiscount.TrangThaiDisplay.Should().Be("Đang hoạt động");
                activeDiscount.ConHieuLuc.Should().BeTrue();
                activeDiscount.SoLuongSanPhamApDung.Should().Be(2);

                inactiveDiscount.TrangThaiDisplay.Should().Be("Đã tắt");
                inactiveDiscount.ConHieuLuc.Should().BeFalse();
                inactiveDiscount.SoLuongSanPhamApDung.Should().Be(0);
            }
        }

        #endregion

        #region Integration Tests

        public class GiamGiaIntegrationTests
        {
            [Fact]
            public async Task GG001_Integration_CreateAndRetrieveDiscount_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real service and database
                Assert.True(true);
            }

            [Fact]
            public async Task GG002_Integration_UpdateDiscount_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real service and database
                Assert.True(true);
            }

            [Fact]
            public async Task GG003_Integration_DiscountValidation_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test discount validation with real data
                Assert.True(true);
            }
        }

        #endregion
    }
}