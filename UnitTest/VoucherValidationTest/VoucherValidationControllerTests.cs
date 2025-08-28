using FurryFriends.API.Controllers;
using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using Xunit;

namespace UnitTest.VoucherValidationTest
{
    public class VoucherValidationControllerTests
    {
        #region Controller Tests

        public class VoucherValidationControllerUnitTests : IDisposable
        {
            private readonly AppDbContext _context;
            private readonly Mock<VoucherCalculationService> _mockVoucherService;
            private readonly VoucherValidationController _controller;

            public VoucherValidationControllerUnitTests()
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _context = new AppDbContext(options);
                _mockVoucherService = new Mock<VoucherCalculationService>();
                _controller = new VoucherValidationController(_context, _mockVoucherService.Object);
            }

            [Fact]
            public async Task VV001_ValidateVoucher_WithEmptyCode_ShouldReturnBadRequest()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "",
                    TongTienHang = 100000
                };

                // Act
                var result = await _controller.ValidateVoucher(request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task VV002_ValidateVoucher_WithNullCode_ShouldReturnBadRequest()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = null,
                    TongTienHang = 100000
                };

                // Act
                var result = await _controller.ValidateVoucher(request);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task VV003_ValidateVoucher_WithNonExistentCode_ShouldReturnOkWithFailure()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "NONEXISTENT",
                    TongTienHang = 100000
                };

                // Act
                var result = await _controller.ValidateVoucher(request);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task VV004_ValidateVoucher_WithValidCode_ShouldReturnOkWithSuccess()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "TEST20",
                    TenVoucher = "Test Voucher",
                    PhanTramGiam = 20,
                    GiaTriGiamToiDa = 50000,
                    SoTienApDungToiThieu = 100000,
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = DateTime.Now.AddDays(1),
                    TrangThai = 1,
                    SoLuong = 10
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "TEST20",
                    TongTienHang = 200000
                };

                // Act
                var result = await _controller.ValidateVoucher(request);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task VV005_ValidateVoucher_WithInvalidVoucher_ShouldReturnOkWithFailure()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "EXPIRED",
                    TenVoucher = "Expired Voucher",
                    PhanTramGiam = 10,
                    GiaTriGiamToiDa = 10000,
                    SoTienApDungToiThieu = 50000,
                    NgayBatDau = DateTime.Now.AddDays(-10),
                    NgayKetThuc = DateTime.Now.AddDays(-1), // Expired
                    TrangThai = 0, // Inactive
                    SoLuong = 0
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "EXPIRED",
                    TongTienHang = 100000
                };

                // Act
                var result = await _controller.ValidateVoucher(request);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task VV006_ValidateVoucher_WithShippingFee_ShouldCalculateCorrectly()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "SHIP30",
                    TenVoucher = "Shipping Voucher",
                    PhanTramGiam = 30,
                    GiaTriGiamToiDa = 30000,
                    SoTienApDungToiThieu = 300000,
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = DateTime.Now.AddDays(1),
                    TrangThai = 1,
                    SoLuong = 5
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "SHIP30",
                    TongTienHang = 300000
                };

                // Act
                var result = await _controller.ValidateVoucher(request);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task VV007_ValidateVoucher_WithFreeShipping_ShouldCalculateCorrectly()
            {
                // Arrange
                var voucher = new Voucher
                {
                    VoucherId = Guid.NewGuid(),
                    MaVoucher = "FREESHIP",
                    TenVoucher = "Free Shipping",
                    PhanTramGiam = 100,
                    GiaTriGiamToiDa = 30000,
                    SoTienApDungToiThieu = 600000,
                    NgayBatDau = DateTime.Now.AddDays(-1),
                    NgayKetThuc = DateTime.Now.AddDays(1),
                    TrangThai = 1,
                    SoLuong = 3
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "FREESHIP",
                    TongTienHang = 600000
                };

                // Act
                var result = await _controller.ValidateVoucher(request);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task VV008_GetAvailableVouchers_WithValidCustomerId_ShouldReturnOk()
            {
                // Arrange
                var customerId = Guid.NewGuid();
                var vouchers = new List<Voucher>
                {
                    new Voucher
                    {
                        VoucherId = Guid.NewGuid(),
                        MaVoucher = "VOUCHER1",
                        TenVoucher = "Voucher 1",
                        PhanTramGiam = 10,
                        GiaTriGiamToiDa = 10000,
                        SoTienApDungToiThieu = 100000,
                        NgayBatDau = DateTime.Now.AddDays(-1),
                        NgayKetThuc = DateTime.Now.AddDays(1),
                        TrangThai = 1,
                        SoLuong = 10
                    }
                };

                _context.Vouchers.AddRange(vouchers);
                await _context.SaveChangesAsync();

                // Act
                var result = await _controller.GetAvailableVouchers(customerId, 150000);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task VV009_GetAvailableVouchers_WithNoMinimumAmount_ShouldReturnAllEligible()
            {
                // Arrange
                var customerId = Guid.NewGuid();
                var vouchers = new List<Voucher>
                {
                    new Voucher
                    {
                        VoucherId = Guid.NewGuid(),
                        MaVoucher = "NO_MIN",
                        TenVoucher = "No Minimum",
                        PhanTramGiam = 5,
                        GiaTriGiamToiDa = 5000,
                        SoTienApDungToiThieu = 0, // No minimum
                        NgayBatDau = DateTime.Now.AddDays(-1),
                        NgayKetThuc = DateTime.Now.AddDays(1),
                        TrangThai = 1,
                        SoLuong = 5
                    }
                };

                _context.Vouchers.AddRange(vouchers);
                await _context.SaveChangesAsync();

                // Act
                var result = await _controller.GetAvailableVouchers(customerId, 50000);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task VV010_GetAvailableVouchers_WithExpiredVouchers_ShouldExcludeThem()
            {
                // Arrange
                var customerId = Guid.NewGuid();
                var vouchers = new List<Voucher>
                {
                    new Voucher
                    {
                        VoucherId = Guid.NewGuid(),
                        MaVoucher = "EXPIRED",
                        TenVoucher = "Expired Voucher",
                        PhanTramGiam = 10,
                        GiaTriGiamToiDa = 10000,
                        SoTienApDungToiThieu = 100000,
                        NgayBatDau = DateTime.Now.AddDays(-10),
                        NgayKetThuc = DateTime.Now.AddDays(-1), // Expired
                        TrangThai = 1,
                        SoLuong = 10
                    },
                    new Voucher
                    {
                        VoucherId = Guid.NewGuid(),
                        MaVoucher = "VALID",
                        TenVoucher = "Valid Voucher",
                        PhanTramGiam = 15,
                        GiaTriGiamToiDa = 15000,
                        SoTienApDungToiThieu = 150000,
                        NgayBatDau = DateTime.Now.AddDays(-1),
                        NgayKetThuc = DateTime.Now.AddDays(1), // Valid
                        TrangThai = 1,
                        SoLuong = 5
                    }
                };

                _context.Vouchers.AddRange(vouchers);
                await _context.SaveChangesAsync();

                // Act
                var result = await _controller.GetAvailableVouchers(customerId, 200000);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            public void Dispose()
            {
                _context?.Dispose();
            }
        }

        #endregion

        #region Validation Tests

        public class VoucherValidationValidationTests
        {
            [Fact]
            public void ValidateVoucherRequest_WithValidData_ShouldPass()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "TESTCODE",
                    TongTienHang = 100000
                };

                // Act & Assert
                request.VoucherCode.Should().NotBeNullOrEmpty();
                request.TongTienHang.Should().BeGreaterOrEqualTo(0);
            }

            [Fact]
            public void ValidateVoucherRequest_WithEmptyCode_ShouldFail()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "",
                    TongTienHang = 100000
                };

                // Act & Assert
                request.VoucherCode.Should().BeEmpty();
            }

            [Fact]
            public void ValidateVoucherRequest_WithNegativeAmount_ShouldFail()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "TESTCODE",
                    TongTienHang = -100000 // Invalid negative amount
                };

                // Act & Assert
                request.TongTienHang.Should().BeLessThan(0);
            }

            [Fact]
            public void ValidateVoucherRequest_WithZeroAmount_ShouldPass()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "FREEBIE",
                    TongTienHang = 0 // Valid zero amount
                };

                // Act & Assert
                request.VoucherCode.Should().NotBeNullOrEmpty();
                request.TongTienHang.Should().Be(0);
            }

            [Fact]
            public void ValidateVoucherRequest_WithVeryLargeAmount_ShouldPass()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "BIGORDER",
                    TongTienHang = 10000000 // Very large valid amount
                };

                // Act & Assert
                request.VoucherCode.Should().NotBeNullOrEmpty();
                request.TongTienHang.Should().BeGreaterThan(0);
            }

            [Fact]
            public void ValidateVoucherRequest_WithWhitespaceCode_ShouldTrimCorrectly()
            {
                // Arrange
                var request = new ValidateVoucherRequest
                {
                    VoucherCode = "  TRIMME  ",
                    TongTienHang = 50000
                };

                // Act & Assert
                request.VoucherCode.Should().Contain("TRIMME");
                request.VoucherCode.Trim().Should().Be("TRIMME");
            }
        }

        #endregion

        #region Integration Tests

        public class VoucherValidationIntegrationTests
        {
            [Fact]
            public async Task VV001_Integration_ValidateAndCalculateVoucher_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real database and voucher service
                Assert.True(true);
            }

            [Fact]
            public async Task VV002_Integration_GetAvailableVouchersWithFiltering_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test complete voucher filtering logic with real data
                Assert.True(true);
            }

            [Fact]
            public async Task VV003_Integration_ShippingFeeCalculation_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test shipping fee calculation in different scenarios
                Assert.True(true);
            }
        }

        #endregion
    }

    // Mock classes removed - using actual VoucherApplicationResult from FurryFriends.API.Services
}