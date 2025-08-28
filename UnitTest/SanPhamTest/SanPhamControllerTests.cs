using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services;
using FurryFriends.API.Services.IServices;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Repository;
using FurryFriends.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace UnitTest.SanPhamTest
{
    public class SanPhamControllerTests
    {
        #region Controller Tests

        public class SanPhamsControllerTests
        {
            private readonly Mock<ISanPhamService> _mockService;
            private readonly SanPhamsController _controller;

            public SanPhamsControllerTests()
            {
                _mockService = new Mock<ISanPhamService>();
                _controller = new SanPhamsController(_mockService.Object);
            }

            [Fact]
            public async Task SP001_GetAllProducts_ShouldReturnOk()
            {
                // Arrange
                var products = new List<SanPhamDTO>
                {
                    new SanPhamDTO 
                    { 
                        SanPhamId = Guid.NewGuid(), 
                        TenSanPham = "Vòng cổ chó", 
                        LoaiSanPham = "DoDung",
                        ThuongHieuId = Guid.NewGuid(),
                        TrangThai = true 
                    },
                    new SanPhamDTO 
                    { 
                        SanPhamId = Guid.NewGuid(), 
                        TenSanPham = "Thức ăn mèo", 
                        LoaiSanPham = "DoAn",
                        ThuongHieuId = Guid.NewGuid(),
                        TrangThai = true 
                    }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedProducts = okResult!.Value as IEnumerable<SanPhamDTO>;
                returnedProducts.Should().HaveCount(2);
                returnedProducts.Should().Contain(p => p.TenSanPham == "Vòng cổ chó");
            }

            [Fact]
            public async Task SP002_GetAllProducts_WhenServiceThrows_ShouldReturnInternalServerError()
            {
                // Arrange
                _mockService.Setup(x => x.GetAllAsync())
                           .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetAll();

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task SP003_GetProductById_WithValidId_ShouldReturnOk()
            {
                // Arrange
                var productId = Guid.NewGuid();
                var product = new SanPhamDTO
                {
                    SanPhamId = productId,
                    TenSanPham = "Đồ chơi xương",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockService.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

                // Act
                var result = await _controller.GetById(productId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedProduct = okResult!.Value as SanPhamDTO;
                returnedProduct.Should().NotBeNull();
                returnedProduct!.TenSanPham.Should().Be("Đồ chơi xương");
            }

            [Fact]
            public async Task SP004_GetProductById_WithInvalidId_ShouldReturnNotFound()
            {
                // Arrange
                var productId = Guid.NewGuid();
                _mockService.Setup(x => x.GetByIdAsync(productId))
                           .ThrowsAsync(new KeyNotFoundException("Không tìm thấy sản phẩm"));

                // Act
                var result = await _controller.GetById(productId);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task SP005_CreateProduct_WithValidData_ShouldReturnCreated()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Chuồng mèo",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                var createdProduct = new SanPhamDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    TenSanPham = "Chuồng mèo",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = dto.ThuongHieuId,
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<SanPhamDTO>()))
                           .ReturnsAsync(createdProduct);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(createdProduct);
            }

            [Fact]
            public async Task SP006_CreateProduct_WithEmptyName_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "", // Empty name
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP007_CreateProduct_WithExceedingNameLength_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = new string('A', 256), // Exceeds 255 character limit
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP008_CreateProduct_WithEmptyBrandId_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Sản phẩm test",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.Empty, // Empty brand id
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP009_CreateProduct_WhenServiceThrowsInvalidOperation_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Sản phẩm trùng",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<SanPhamDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Sản phẩm đã tồn tại"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP010_UpdateProduct_WithValidData_ShouldReturnNoContent()
            {
                // Arrange
                var productId = Guid.NewGuid();
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Sản phẩm đã cập nhật",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(productId, It.IsAny<SanPhamDTO>()))
                           .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Update(productId, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }
        }

        #endregion

        #region Service Tests

        public class SanPhamServiceTests
        {
            private readonly Mock<ISanPhamRepository> _mockRepository;
            private readonly Mock<AppDbContext> _mockContext;
            private readonly SanPhamService _service;

            public SanPhamServiceTests()
            {
                _mockRepository = new Mock<ISanPhamRepository>();
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
                _mockContext = new Mock<AppDbContext>(options);
                _service = new SanPhamService(_mockRepository.Object, new AppDbContext(options));
            }

            [Fact]
            public async Task GetAllAsync_ShouldReturnAllProducts()
            {
                // Arrange
                var products = new List<SanPham>
                {
                    new SanPham 
                    { 
                        SanPhamId = Guid.NewGuid(), 
                        TenSanPham = "Vòng cổ chó", 
                        ThuongHieuId = Guid.NewGuid(),
                        TrangThai = true 
                    },
                    new SanPham 
                    { 
                        SanPhamId = Guid.NewGuid(), 
                        TenSanPham = "Thức ăn mèo", 
                        ThuongHieuId = Guid.NewGuid(),
                        TrangThai = true 
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

                // Act
                var result = await _service.GetAllAsync();

                // Assert
                result.Should().HaveCount(2);
                result.Should().Contain(x => x.TenSanPham == "Vòng cổ chó");
                result.Should().Contain(x => x.TenSanPham == "Thức ăn mèo");
            }

            [Fact]
            public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
            {
                // Arrange
                var productId = Guid.NewGuid();
                var product = new SanPham
                {
                    SanPhamId = productId,
                    TenSanPham = "Đồ chơi xương",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

                // Act
                var result = await _service.GetByIdAsync(productId);

                // Assert
                result.Should().NotBeNull();
                result.TenSanPham.Should().Be("Đồ chơi xương");
            }

            [Fact]
            public async Task GetByIdAsync_WithInvalidId_ShouldThrowException()
            {
                // Arrange
                var productId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(productId))
                              .ReturnsAsync((SanPham?)null);

                // Act & Assert
                await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(productId));
            }

            [Fact]
            public async Task CreateAsync_WithValidData_ShouldReturnCreatedProduct()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Chuồng mèo",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<SanPham>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _service.CreateAsync(dto);

                // Assert
                result.Should().NotBeNull();
                result.TenSanPham.Should().Be("Chuồng mèo");
                result.SanPhamId.Should().NotBeEmpty();

                _mockRepository.Verify(x => x.AddAsync(It.IsAny<SanPham>()), Times.Once);
            }
        }

        #endregion

        #region Validation Tests

        public class SanPhamDTOValidationTests
        {
            [Fact]
            public void ValidateEmptyProductName_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().Contain(x => x.MemberNames.Contains(nameof(SanPhamDTO.TenSanPham)));
            }

            [Fact]
            public void ValidateValidProduct_ShouldPass()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Vòng cổ chó cao cấp",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeTrue();
                validationResults.Should().BeEmpty();
            }

            [Fact]
            public void ValidateProductNameLength_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = new string('A', 255), // Exactly at max length
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeTrue();
            }

            [Fact]
            public void ValidateProductNameLength_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = new string('A', 256), // Exceeds max length
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().Contain(x => x.MemberNames.Contains(nameof(SanPhamDTO.TenSanPham)));
            }

            [Fact]
            public void ValidateEmptyBrandId_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Sản phẩm test",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.Empty,
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().Contain(x => x.MemberNames.Contains(nameof(SanPhamDTO.ThuongHieuId)));
            }

            [Fact]
            public void ValidateInvalidLoaiSanPham_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Sản phẩm test",
                    LoaiSanPham = "InvalidType", // Invalid type
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().Contain(x => x.MemberNames.Contains(nameof(SanPhamDTO.LoaiSanPham)));
            }
        }

        #endregion

        #region Integration Tests

        public class SanPhamIntegrationTests
        {
            private readonly DbContextOptions<AppDbContext> _options;

            public SanPhamIntegrationTests()
            {
                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }

            [Fact]
            public async Task SP001_Integration_CreateProduct_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                var dto = new SanPhamDTO
                {
                    TenSanPham = "Vòng cổ chó cao cấp",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                var createdDto = createdAtResult!.Value as SanPhamDTO;
                createdDto.Should().NotBeNull();
                createdDto!.TenSanPham.Should().Be("Vòng cổ chó cao cấp");
            }
        }

        #endregion
    }
}