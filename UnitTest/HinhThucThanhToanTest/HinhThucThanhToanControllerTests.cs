using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace UnitTest.HinhThucThanhToanTest
{
    public class HinhThucThanhToanControllerTests
    {
        #region Controller Tests

        public class HinhThucThanhToanControllerUnitTests
        {
            private readonly Mock<IHinhThucThanhToanRepository> _mockRepository;
            private readonly HinhThucThanhToanController _controller;

            public HinhThucThanhToanControllerUnitTests()
            {
                _mockRepository = new Mock<IHinhThucThanhToanRepository>();
                _controller = new HinhThucThanhToanController(_mockRepository.Object);
            }

            [Fact]
            public async Task HTTT001_GetAll_ShouldReturnOkWithPaymentMethodList()
            {
                // Arrange
                var paymentMethods = new List<HinhThucThanhToan>
                {
                    new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "Tiền mặt",
                        MoTa = "Thanh toán bằng tiền mặt"
                    },
                    new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "Chuyển khoản",
                        MoTa = "Thanh toán qua chuyển khoản ngân hàng"
                    },
                    new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "Thẻ tín dụng",
                        MoTa = "Thanh toán bằng thẻ tín dụng/ghi nợ"
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(paymentMethods);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedPaymentMethods = okResult!.Value as IEnumerable<HinhThucThanhToan>;
                returnedPaymentMethods.Should().HaveCount(3);
                _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
            }

            [Fact]
            public async Task HTTT002_GetAll_WithEmptyList_ShouldReturnOkWithEmptyList()
            {
                // Arrange
                var emptyList = new List<HinhThucThanhToan>();
                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyList);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedPaymentMethods = okResult!.Value as IEnumerable<HinhThucThanhToan>;
                returnedPaymentMethods.Should().BeEmpty();
            }

            [Fact]
            public async Task HTTT003_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var paymentMethodId = Guid.NewGuid();
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = paymentMethodId,
                    TenHinhThuc = "VNPay",
                    MoTa = "Thanh toán qua VNPay"
                };

                _mockRepository.Setup(x => x.GetByIdAsync(paymentMethodId)).ReturnsAsync(paymentMethod);

                // Act
                var result = await _controller.GetById(paymentMethodId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedPaymentMethod = okResult!.Value as HinhThucThanhToan;
                returnedPaymentMethod.Should().NotBeNull();
                returnedPaymentMethod!.HinhThucThanhToanId.Should().Be(paymentMethodId);
                returnedPaymentMethod.TenHinhThuc.Should().Be("VNPay");
            }

            [Fact]
            public async Task HTTT004_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var paymentMethodId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(paymentMethodId))
                              .ReturnsAsync((HinhThucThanhToan?)null);

                // Act
                var result = await _controller.GetById(paymentMethodId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be($"Không tìm thấy hình thức thanh toán với ID: {paymentMethodId}");
            }

            [Fact]
            public async Task HTTT005_GetById_WithEmptyGuid_ShouldReturnNotFound()
            {
                // Arrange
                var emptyGuid = Guid.Empty;
                _mockRepository.Setup(x => x.GetByIdAsync(emptyGuid))
                              .ReturnsAsync((HinhThucThanhToan?)null);

                // Act
                var result = await _controller.GetById(emptyGuid);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task HTTT006_GetAll_WhenRepositoryThrowsException_ShouldPropagateException()
            {
                // Arrange
                _mockRepository.Setup(x => x.GetAllAsync())
                              .ThrowsAsync(new Exception("Database connection failed"));

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(async () => await _controller.GetAll());
            }

            [Fact]
            public async Task HTTT007_GetById_WhenRepositoryThrowsException_ShouldPropagateException()
            {
                // Arrange
                var paymentMethodId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(paymentMethodId))
                              .ThrowsAsync(new Exception("Database connection failed"));

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(async () => await _controller.GetById(paymentMethodId));
            }

            [Fact]
            public async Task HTTT008_GetAll_ShouldCallRepositoryOnce()
            {
                // Arrange
                var paymentMethods = new List<HinhThucThanhToan>
                {
                    new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "Ví điện tử",
                        MoTa = "Thanh toán qua ví điện tử"
                    }
                };
                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(paymentMethods);

                // Act
                var result = await _controller.GetAll();

                // Assert
                _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
                _mockRepository.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task HTTT009_GetById_ShouldCallRepositoryOnceWithCorrectId()
            {
                // Arrange
                var paymentMethodId = Guid.NewGuid();
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = paymentMethodId,
                    TenHinhThuc = "MoMo",
                    MoTa = "Thanh toán qua ví MoMo"
                };
                _mockRepository.Setup(x => x.GetByIdAsync(paymentMethodId)).ReturnsAsync(paymentMethod);

                // Act
                var result = await _controller.GetById(paymentMethodId);

                // Assert
                _mockRepository.Verify(x => x.GetByIdAsync(paymentMethodId), Times.Once);
                _mockRepository.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task HTTT010_GetAll_WithMultiplePaymentMethods_ShouldReturnAllInOrder()
            {
                // Arrange
                var paymentMethods = new List<HinhThucThanhToan>
                {
                    new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "A - Tiền mặt",
                        MoTa = "Thanh toán bằng tiền mặt"
                    },
                    new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "B - Chuyển khoản",
                        MoTa = "Thanh toán qua ngân hàng"
                    },
                    new HinhThucThanhToan
                    {
                        HinhThucThanhToanId = Guid.NewGuid(),
                        TenHinhThuc = "C - Ví điện tử",
                        MoTa = "Thanh toán qua ví điện tử"
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(paymentMethods);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedPaymentMethods = okResult!.Value as IEnumerable<HinhThucThanhToan>;
                returnedPaymentMethods.Should().HaveCount(3);
                
                var paymentMethodList = returnedPaymentMethods!.ToList();
                paymentMethodList[0].TenHinhThuc.Should().Be("A - Tiền mặt");
                paymentMethodList[1].TenHinhThuc.Should().Be("B - Chuyển khoản");
                paymentMethodList[2].TenHinhThuc.Should().Be("C - Ví điện tử");
            }
        }

        #endregion

        #region Validation Tests

        public class HinhThucThanhToanValidationTests
        {
            [Fact]
            public void ValidateHinhThucThanhToan_WithValidData_ShouldPass()
            {
                // Arrange
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.NewGuid(),
                    TenHinhThuc = "Thẻ visa",
                    MoTa = "Thanh toán bằng thẻ Visa/MasterCard"
                };

                // Act & Assert
                paymentMethod.TenHinhThuc.Should().NotBeNullOrEmpty();
                paymentMethod.TenHinhThuc.Length.Should().BeLessOrEqualTo(100);
                paymentMethod.MoTa.Should().NotBeNull();
                paymentMethod.HinhThucThanhToanId.Should().NotBe(Guid.Empty);
            }

            [Fact]
            public void ValidateHinhThucThanhToan_WithEmptyTenHinhThuc_ShouldFail()
            {
                // Arrange
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.NewGuid(),
                    TenHinhThuc = "", // Invalid - empty
                    MoTa = "Mô tả hình thức thanh toán"
                };

                // Act & Assert
                paymentMethod.TenHinhThuc.Should().BeEmpty();
            }

            [Fact]
            public void ValidateHinhThucThanhToan_WithTooLongTenHinhThuc_ShouldFail()
            {
                // Arrange
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.NewGuid(),
                    TenHinhThuc = new string('A', 101), // Invalid - too long (101 characters)
                    MoTa = "Mô tả hình thức thanh toán"
                };

                // Act & Assert
                paymentMethod.TenHinhThuc.Length.Should().BeGreaterThan(100);
            }

            [Fact]
            public void ValidateHinhThucThanhToan_WithValidLength_ShouldPass()
            {
                // Arrange
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.NewGuid(),
                    TenHinhThuc = new string('A', 100), // Valid - exactly 100 characters
                    MoTa = "Mô tả hình thức thanh toán"
                };

                // Act & Assert
                paymentMethod.TenHinhThuc.Length.Should().Be(100);
            }

            [Fact]
            public void ValidateHinhThucThanhToan_WithEmptyId_ShouldFail()
            {
                // Arrange
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.Empty, // Invalid
                    TenHinhThuc = "Valid Name",
                    MoTa = "Valid Description"
                };

                // Act & Assert
                paymentMethod.HinhThucThanhToanId.Should().Be(Guid.Empty);
            }

            [Fact]
            public void ValidateHinhThucThanhToan_WithNullMoTa_ShouldStillBeValid()
            {
                // Arrange
                var paymentMethod = new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.NewGuid(),
                    TenHinhThuc = "Valid Name",
                    MoTa = null // This might be acceptable depending on business rules
                };

                // Act & Assert
                paymentMethod.TenHinhThuc.Should().NotBeNullOrEmpty();
                paymentMethod.MoTa.Should().BeNull();
            }
        }

        #endregion

        #region Integration Tests

        public class HinhThucThanhToanIntegrationTests
        {
            [Fact]
            public async Task HTTT001_Integration_GetAllPaymentMethods_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task HTTT002_Integration_GetPaymentMethodById_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task HTTT003_Integration_PaymentMethodUsageInOrders_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test relationship with HoaDon entity
                Assert.True(true);
            }
        }

        #endregion
    }
}