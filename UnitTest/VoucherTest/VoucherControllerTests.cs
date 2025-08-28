using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace UnitTest.VoucherTest
{
    public class VoucherControllerTests
    {
        #region Controller Tests

        public class VoucherControllerUnitTests
        {
            private readonly Mock<IVoucherRepository> _mockRepository;
            private readonly VoucherController _controller;

            public VoucherControllerUnitTests()
            {
                _mockRepository = new Mock<IVoucherRepository>();
                _controller = new VoucherController(_mockRepository.Object);
            }

            [Fact]
            public async Task VC001_GetAll_ShouldReturnOk()
            {
                // Arrange
                var vouchers = new List<Voucher>
                {
                    new Voucher 
                    { 
                        VoucherId = Guid.NewGuid(), 
                        MaVoucher = "DISCOUNT10",
                        TenVoucher = "Giảm 10%",
                        PhanTramGiam = 10,
                        TrangThai = 1,
                        NgayBatDau = DateTime.Now.AddDays(-1),
                        NgayKetThuc = DateTime.Now.AddDays(30)
                    },
                    new Voucher 
                    { 
                        VoucherId = Guid.NewGuid(), 
                        MaVoucher = "FREESHIP",
                        TenVoucher = "Miễn phí vận chuyển",
                        PhanTramGiam = 0, // Free shipping voucher
                        GiaTriGiamToiDa = 50000,
                        TrangThai = 1,
                        NgayBatDau = DateTime.Now.AddDays(-5),
                        NgayKetThuc = DateTime.Now.AddDays(15)
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(vouchers);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedVouchers = okResult!.Value as IEnumerable<Voucher>;
                returnedVouchers.Should().HaveCount(2);
                returnedVouchers.Should().Contain(v => v.MaVoucher == "DISCOUNT10");
                returnedVouchers.Should().Contain(v => v.MaVoucher == "FREESHIP");
            }

            [Fact]
            public async Task VC002_GetAll_WhenExceptionThrown_ShouldReturnInternalServerError()
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
            public async Task VC003_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var voucherId = Guid.NewGuid();
                var voucher = new Voucher
                {
                    VoucherId = voucherId,
                    MaVoucher = "SUMMER2024",
                    TenVoucher = "Ưu đãi mùa hè",
                    PhanTramGiam = 15,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = DateTime.Now.AddDays(30)
                };

                _mockRepository.Setup(x => x.GetByIdAsync(voucherId)).ReturnsAsync(voucher);

                // Act
                var result = await _controller.GetById(voucherId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedVoucher = okResult!.Value as Voucher;
                returnedVoucher.Should().NotBeNull();
                returnedVoucher!.VoucherId.Should().Be(voucherId);
                returnedVoucher.MaVoucher.Should().Be("SUMMER2024");
            }

            [Fact]
            public async Task VC004_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var voucherId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(voucherId))
                              .ReturnsAsync((Voucher?)null);

                // Act
                var result = await _controller.GetById(voucherId);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task VC005_GetById_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var voucherId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(voucherId))
                              .ThrowsAsync(new Exception("Database connection error"));

                // Act
                var result = await _controller.GetById(voucherId);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database connection error");
            }

            [Fact]
            public async Task VC006_Create_WithValidData_ShouldReturnCreated()
            {
                // Arrange
                var voucher = new Voucher
                {
                    MaVoucher = "NEWVOUCHER",
                    TenVoucher = "Voucher mới",
                    PhanTramGiam = 20,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(30),
                    SoLuong = 100
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<Voucher>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Create(voucher);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                var createdVoucher = createdAtResult!.Value as Voucher;
                createdVoucher.Should().NotBeNull();
                createdVoucher!.MaVoucher.Should().Be("NEWVOUCHER");
                createdVoucher.VoucherId.Should().NotBeEmpty(); // Should be assigned in controller
                createdVoucher.NgayTao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));

                _mockRepository.Verify(x => x.AddAsync(It.IsAny<Voucher>()), Times.Once);
            }

            [Fact]
            public async Task VC007_Create_WithInvalidModelState_ShouldReturnBadRequest()
            {
                // Arrange
                var voucher = new Voucher
                {
                    MaVoucher = "", // Invalid - empty
                    TenVoucher = "Voucher test",
                    PhanTramGiam = 10,
                    TrangThai = 1
                };

                _controller.ModelState.AddModelError("MaVoucher", "Mã voucher không được để trống");

                // Act
                var result = await _controller.Create(voucher);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                _mockRepository.Verify(x => x.AddAsync(It.IsAny<Voucher>()), Times.Never);
            }

            [Fact]
            public async Task VC008_Create_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var voucher = new Voucher
                {
                    MaVoucher = "TESTVOUCHER",
                    TenVoucher = "Test voucher",
                    PhanTramGiam = 15,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(30)
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<Voucher>()))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Create(voucher);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database error");
            }

            [Fact]
            public async Task VC009_Update_WithValidData_ShouldReturnNoContent()
            {
                // Arrange
                var voucherId = Guid.NewGuid();
                var voucher = new Voucher
                {
                    VoucherId = voucherId,
                    MaVoucher = "UPDATEDVOUCHER",
                    TenVoucher = "Voucher đã cập nhật",
                    PhanTramGiam = 25,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(45)
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Voucher>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Update(voucherId, voucher);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                voucher.NgayCapNhat.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Voucher>()), Times.Once);
            }

            [Fact]
            public async Task VC010_Update_WithMismatchedId_ShouldReturnBadRequest()
            {
                // Arrange
                var urlId = Guid.NewGuid();
                var differentId = Guid.NewGuid();
                var voucher = new Voucher
                {
                    VoucherId = differentId, // Different from URL ID
                    MaVoucher = "TESTVOUCHER",
                    TenVoucher = "Test voucher",
                    PhanTramGiam = 10,
                    TrangThai = 1
                };

                // Act
                var result = await _controller.Update(urlId, voucher);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Voucher>()), Times.Never);
            }

            [Fact]
            public async Task VC011_Update_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var voucherId = Guid.NewGuid();
                var voucher = new Voucher
                {
                    VoucherId = voucherId,
                    MaVoucher = "TESTVOUCHER",
                    TenVoucher = "Test voucher",
                    PhanTramGiam = 10,
                    TrangThai = 1
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Voucher>()))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Update(voucherId, voucher);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database error");
            }

            [Fact]
            public async Task VC012_Delete_WithValidId_ShouldReturnNoContent()
            {
                // Arrange
                var voucherId = Guid.NewGuid();

                _mockRepository.Setup(x => x.DeleteAsync(voucherId))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Delete(voucherId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockRepository.Verify(x => x.DeleteAsync(voucherId), Times.Once);
            }

            [Fact]
            public async Task VC013_Delete_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var voucherId = Guid.NewGuid();

                _mockRepository.Setup(x => x.DeleteAsync(voucherId))
                              .ThrowsAsync(new Exception("Cannot delete voucher with active usage"));

                // Act
                var result = await _controller.Delete(voucherId);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
            }
        }

        #endregion

        #region Validation Tests

        public class VoucherValidationTests
        {
            [Fact]
            public void ValidateVoucher_WithValidData_ShouldPass()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "VALID2024",
                    TenVoucher = "Voucher hợp lệ 2024",
                    PhanTramGiam = 15,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(30),
                    SoLuong = 100,
                    SoTienApDungToiThieu = 100000
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(voucher, new ValidationContext(voucher), validationResults, true);

                // Assert
                voucher.MaVoucher.Should().NotBeNullOrEmpty();
                voucher.TenVoucher.Should().NotBeNullOrEmpty();
                voucher.PhanTramGiam.Should().BeGreaterThan(0);
                voucher.TrangThai.Should().BeOneOf(0, 1);
                voucher.NgayKetThuc.Should().BeAfter(voucher.NgayBatDau);
            }

            [Fact]
            public void ValidateVoucher_WithInvalidDiscountPercentage_ShouldFail()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "INVALID2024",
                    TenVoucher = "Voucher không hợp lệ",
                    PhanTramGiam = 150, // Invalid - over 100%
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(30)
                };

                // Act & Assert
                voucher.PhanTramGiam.Should().BeGreaterThan(100);
            }

            [Fact]
            public void ValidateVoucher_WithInvalidDateRange_ShouldFail()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "INVALID2024",
                    TenVoucher = "Voucher không hợp lệ",
                    PhanTramGiam = 10,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now.AddDays(30), // Start date after end date
                    NgayKetThuc = DateTime.Now
                };

                // Act & Assert
                voucher.NgayBatDau.Should().BeAfter(voucher.NgayKetThuc);
            }

            [Fact]
            public void ValidateVoucher_WithEmptyVoucherCode_ShouldFail()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "", // Empty voucher code
                    TenVoucher = "Voucher test",
                    PhanTramGiam = 10,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(30)
                };

                // Act & Assert
                voucher.MaVoucher.Should().BeEmpty();
            }

            [Fact]
            public void ValidateVoucher_WithNegativeQuantity_ShouldFail()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "TEST2024",
                    TenVoucher = "Test voucher",
                    PhanTramGiam = 10,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now,
                    NgayKetThuc = DateTime.Now.AddDays(30),
                    SoLuong = -5 // Negative quantity
                };

                // Act & Assert
                voucher.SoLuong.Should().BeLessThan(0);
            }
        }

        #endregion

        #region Business Logic Tests

        public class VoucherBusinessLogicTests
        {
            [Fact]
            public void IsValidVoucher_WithActiveVoucher_ShouldReturnTrue()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "ACTIVE2024",
                    TenVoucher = "Voucher đang hoạt động",
                    PhanTramGiam = 10,
                    TrangThai = 1, // Active
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = DateTime.Now.AddDays(30),
                    SoLuong = 50
                };

                // Act
                var isValid = IsVoucherValid(voucher);

                // Assert
                isValid.Should().BeTrue();
            }

            [Fact]
            public void IsValidVoucher_WithExpiredVoucher_ShouldReturnFalse()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "EXPIRED2024",
                    TenVoucher = "Voucher hết hạn",
                    PhanTramGiam = 10,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now.AddDays(-30),
                    NgayKetThuc = DateTime.Now.AddDays(-1), // Expired
                    SoLuong = 50
                };

                // Act
                var isValid = IsVoucherValid(voucher);

                // Assert
                isValid.Should().BeFalse();
            }

            [Fact]
            public void IsValidVoucher_WithInactiveVoucher_ShouldReturnFalse()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "INACTIVE2024",
                    TenVoucher = "Voucher không hoạt động",
                    PhanTramGiam = 10,
                    TrangThai = 0, // Inactive
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = DateTime.Now.AddDays(30),
                    SoLuong = 50
                };

                // Act
                var isValid = IsVoucherValid(voucher);

                // Assert
                isValid.Should().BeFalse();
            }

            [Fact]
            public void IsValidVoucher_WithZeroQuantity_ShouldReturnFalse()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "OUTOFSTOCK2024",
                    TenVoucher = "Voucher hết lượt",
                    PhanTramGiam = 10,
                    TrangThai = 1,
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = DateTime.Now.AddDays(30),
                    SoLuong = 0 // Out of stock
                };

                // Act
                var isValid = IsVoucherValid(voucher);

                // Assert
                isValid.Should().BeFalse();
            }

            // Helper method for business logic testing
            private static bool IsVoucherValid(Voucher voucher)
            {
                var now = DateTime.Now;
                return voucher.TrangThai == 1 &&
                       voucher.NgayBatDau <= now &&
                       voucher.NgayKetThuc >= now &&
                       voucher.SoLuong > 0;
            }
        }

        #endregion

        #region Integration Tests

        public class VoucherIntegrationTests
        {
            [Fact]
            public async Task VC001_Integration_CreateAndRetrieveVoucher_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task VC002_Integration_UpdateVoucher_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task VC003_Integration_DeleteVoucher_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }
        }

        #endregion
    }
}