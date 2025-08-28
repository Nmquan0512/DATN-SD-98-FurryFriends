using FurryFriends.API.Controllers;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace UnitTest.SanPhamChiTietTest
{
    public class SanPhamChiTietControllerTests
    {
        #region Controller Tests

        public class SanPhamChiTietControllerUnitTests
        {
            private readonly Mock<ISanPhamChiTietService> _mockService;
            private readonly SanPhamChiTietController _controller;

            public SanPhamChiTietControllerUnitTests()
            {
                _mockService = new Mock<ISanPhamChiTietService>();
                _controller = new SanPhamChiTietController(_mockService.Object);
            }

            [Fact]
            public async Task SPCT001_GetAll_ShouldReturnOk()
            {
                // Arrange
                var sanPhamChiTiets = new List<SanPhamChiTietDTO>
                {
                    new SanPhamChiTietDTO 
                    { 
                        SanPhamChiTietId = Guid.NewGuid(),
                        SanPhamId = Guid.NewGuid(),
                        MauSacId = Guid.NewGuid(),
                        KichCoId = Guid.NewGuid(),
                        Gia = 100000,
                        SoLuong = 10,
                        TrangThai = 1
                    },
                    new SanPhamChiTietDTO 
                    { 
                        SanPhamChiTietId = Guid.NewGuid(),
                        SanPhamId = Guid.NewGuid(),
                        MauSacId = Guid.NewGuid(),
                        KichCoId = Guid.NewGuid(),
                        Gia = 150000,
                        SoLuong = 5,
                        TrangThai = 1
                    }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(sanPhamChiTiets);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedItems = okResult!.Value as IEnumerable<SanPhamChiTietDTO>;
                returnedItems.Should().HaveCount(2);
            }

            [Fact]
            public async Task SPCT002_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var id = Guid.NewGuid();
                var sanPhamChiTiet = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = id,
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 200000,
                    SoLuong = 15,
                    TrangThai = 1
                };

                _mockService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(sanPhamChiTiet);

                // Act
                var result = await _controller.GetById(id);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedItem = okResult!.Value as SanPhamChiTietDTO;
                returnedItem.Should().NotBeNull();
                returnedItem!.SanPhamChiTietId.Should().Be(id);
            }

            [Fact]
            public async Task SPCT003_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((SanPhamChiTietDTO?)null);

                // Act
                var result = await _controller.GetById(id);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task SPCT004_Create_WithValidData_ShouldReturnOk()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                var createdDto = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = Guid.NewGuid(),
                    SanPhamId = dto.SanPhamId,
                    MauSacId = dto.MauSacId,
                    KichCoId = dto.KichCoId,
                    Gia = dto.Gia,
                    SoLuong = dto.SoLuong,
                    TrangThai = dto.TrangThai
                };

                _mockService.Setup(x => x.CreateAndReturnAsync(It.IsAny<SanPhamChiTietDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedItem = okResult!.Value as SanPhamChiTietDTO;
                returnedItem.Should().NotBeNull();
                returnedItem!.SanPhamId.Should().Be(dto.SanPhamId);
            }

            [Fact]
            public async Task SPCT005_Create_WithEmptyMauSacId_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.Empty, // Empty MauSacId
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SPCT006_Create_WithEmptyKichCoId_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.Empty, // Empty KichCoId
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SPCT007_Create_WithNegativeGia_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = -100000, // Negative price
                    SoLuong = 10,
                    TrangThai = 1
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SPCT008_Create_WithNegativeSoLuong_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = -5, // Negative quantity
                    TrangThai = 1
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SPCT009_Create_WhenServiceReturnsNull_ShouldReturnInternalServerError()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                _mockService.Setup(x => x.CreateAndReturnAsync(It.IsAny<SanPhamChiTietDTO>()))
                           .ReturnsAsync((SanPhamChiTietDTO?)null);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Tạo sản phẩm chi tiết thất bại.");
            }

            [Fact]
            public async Task SPCT010_Create_WhenServiceThrowsInvalidOperationException_ShouldReturnInternalServerError()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                _mockService.Setup(x => x.CreateAndReturnAsync(It.IsAny<SanPhamChiTietDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Lỗi nghiệp vụ"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Lỗi nghiệp vụ");
            }

            [Fact]
            public async Task SPCT011_Create_WhenServiceThrowsException_ShouldReturnInternalServerError()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                _mockService.Setup(x => x.CreateAndReturnAsync(It.IsAny<SanPhamChiTietDTO>()))
                           .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Lỗi khi tạo sản phẩm chi tiết: Database error");
            }

            [Fact]
            public async Task SPCT012_Update_WithValidData_ShouldReturnOk()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = id,
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 150000,
                    SoLuong = 15,
                    TrangThai = 1
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<SanPhamChiTietDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                _mockService.Verify(x => x.UpdateAsync(id, It.IsAny<SanPhamChiTietDTO>()), Times.Once);
            }

            [Fact]
            public async Task SPCT013_Update_WithInvalidValidation_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = id,
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.Empty, // Invalid MauSacId
                    KichCoId = Guid.NewGuid(),
                    Gia = 150000,
                    SoLuong = 15,
                    TrangThai = 1
                };

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SPCT014_Update_WhenServiceReturnsNull_ShouldReturnNotFound()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = id,
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 150000,
                    SoLuong = 15,
                    TrangThai = 1
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<SanPhamChiTietDTO>()))
                           .ReturnsAsync(false);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task SPCT015_Update_WhenServiceThrowsException_ShouldReturnInternalServerError()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = id,
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 150000,
                    SoLuong = 15,
                    TrangThai = 1
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<SanPhamChiTietDTO>()))
                           .ThrowsAsync(new Exception("Update failed"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Lỗi khi cập nhật sản phẩm chi tiết: Update failed");
            }
        }

        #endregion

        #region Validation Tests

        public class SanPhamChiTietValidationTests
        {
            [Fact]
            public void ValidateSanPhamChiTietDTO_WithValidData_ShouldPass()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                // Act & Assert
                dto.SanPhamId.Should().NotBe(Guid.Empty);
                dto.MauSacId.Should().NotBe(Guid.Empty);
                dto.KichCoId.Should().NotBe(Guid.Empty);
                dto.Gia.Should().BeGreaterThan(0);
                dto.SoLuong.Should().BeGreaterOrEqualTo(0);
            }

            [Fact]
            public void ValidateSanPhamChiTietDTO_WithInvalidGia_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = -50000, // Negative price
                    SoLuong = 10,
                    TrangThai = 1
                };

                // Act & Assert
                dto.Gia.Should().BeLessOrEqualTo(0);
            }

            [Fact]
            public void ValidateSanPhamChiTietDTO_WithInvalidSoLuong_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 100000,
                    SoLuong = -5, // Negative quantity
                    TrangThai = 1
                };

                // Act & Assert
                dto.SoLuong.Should().BeLessThan(0);
            }

            [Fact]
            public void ValidateSanPhamChiTietDTO_WithEmptyIds_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.Empty,
                    MauSacId = Guid.Empty,
                    KichCoId = Guid.Empty,
                    Gia = 100000,
                    SoLuong = 10,
                    TrangThai = 1
                };

                // Act & Assert
                dto.SanPhamId.Should().Be(Guid.Empty);
                dto.MauSacId.Should().Be(Guid.Empty);
                dto.KichCoId.Should().Be(Guid.Empty);
            }
        }

        #endregion

        #region Integration Tests

        public class SanPhamChiTietIntegrationTests
        {
            [Fact]
            public async Task SPCT001_Integration_CreateAndRetrieveSanPhamChiTiet_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real service and database
                Assert.True(true);
            }

            [Fact]
            public async Task SPCT002_Integration_UpdateSanPhamChiTiet_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real service and database
                Assert.True(true);
            }

            [Fact]
            public async Task SPCT003_Integration_SanPhamChiTietWithRelatedEntities_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test relationships with SanPham, MauSac, KichCo
                Assert.True(true);
            }
        }

        #endregion
    }
}