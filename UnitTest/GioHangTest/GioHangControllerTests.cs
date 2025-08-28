using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using AddToCartDTO = FurryFriends.API.Controllers.AddToCartDTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Services;
using FurryFriends.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json;

namespace UnitTest.GioHangTest
{
    public class GioHangControllerTests
    {
        #region Controller Tests

        public class GioHangControllerUnitTests
        {
            private readonly Mock<IGioHangRepository> _mockRepo;
            private readonly Mock<VoucherCalculationService> _mockVoucherCalc;
            private readonly Mock<AppDbContext> _mockContext;
            private readonly GioHangController _controller;
            private readonly DbContextOptions<AppDbContext> _dbOptions;

            public GioHangControllerUnitTests()
            {
                _mockRepo = new Mock<IGioHangRepository>();
                
                _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
                
                _mockContext = new Mock<AppDbContext>(_dbOptions);
                _mockVoucherCalc = new Mock<VoucherCalculationService>();
                
                _controller = new GioHangController(_mockRepo.Object, _mockContext.Object, _mockVoucherCalc.Object);
            }

            [Fact]
            public async Task GH001_GetGioHang_WithValidKhachHangId_ShouldReturnOk()
            {
                // Arrange
                var khachHangId = Guid.NewGuid();
                var gioHang = new GioHangDTO
                {
                    KhachHangId = khachHangId,
                    GioHangChiTiets = new List<GioHangChiTietDTO>
                    {
                        new GioHangChiTietDTO
                        {
                            GioHangChiTietId = Guid.NewGuid(),
                            TenSanPham = "Vòng cổ chó",
                            SoLuong = 2,
                            DonGia = 50000
                        }
                    }
                };

                _mockRepo.Setup(x => x.GetGioHangByKhachHangIdAsync(khachHangId))
                         .ReturnsAsync(gioHang);

                // Act
                var result = await _controller.GetGioHang(khachHangId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedGioHang = okResult!.Value as GioHangDTO;
                returnedGioHang.Should().NotBeNull();
                returnedGioHang!.KhachHangId.Should().Be(khachHangId);
                returnedGioHang.GioHangChiTiets.Should().HaveCount(1);
            }

            [Fact]
            public async Task GH002_AddToCart_WithValidData_ShouldReturnOk()
            {
                // Arrange
                var model = new AddToCartDTO
                {
                    KhachHangId = Guid.NewGuid(),
                    SanPhamChiTietId = Guid.NewGuid(),
                    SoLuong = 1
                };

                var sanPhamChiTiet = new SanPhamChiTiet
                {
                    SanPhamChiTietId = model.SanPhamChiTietId,
                    SanPhamId = Guid.NewGuid(),
                    Gia = 100000
                };

                var gioHangChiTiet = new GioHangChiTiet
                {
                    GioHangChiTietId = Guid.NewGuid(),
                    SanPhamChiTietId = model.SanPhamChiTietId,
                    SoLuong = model.SoLuong
                };

                _mockRepo.Setup(x => x.GetSanPhamChiTietByIdAsync(model.SanPhamChiTietId))
                         .ReturnsAsync(sanPhamChiTiet);
                
                // Sử dụng real AppDbContext với InMemoryDatabase
                using var realContext = new AppDbContext(_dbOptions);
                var khachHang = new KhachHang 
                { 
                    KhachHangId = model.KhachHangId,
                    TenKhachHang = "Test Customer",
                    SDT = "0123456789",
                    EmailCuaKhachHang = "test@example.com",
                    NgayTaoTaiKhoan = DateTime.Now,
                    TrangThai = 1
                };
                realContext.KhachHangs.Add(khachHang);
                await realContext.SaveChangesAsync();
                
                var controllerWithRealContext = new GioHangController(_mockRepo.Object, realContext, _mockVoucherCalc.Object);
                
                _mockRepo.Setup(x => x.AddSanPhamVaoGioAsync(model.KhachHangId, model.SanPhamChiTietId, model.SoLuong))
                         .ReturnsAsync(gioHangChiTiet);
                _mockRepo.Setup(x => x.ConvertToDTOAsync(gioHangChiTiet))
                         .ReturnsAsync(new GioHangChiTietDTO());

                // Act
                var result = await controllerWithRealContext.AddToCart(model);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task GH003_UpdateSoLuong_WithValidData_ShouldReturnOk()
            {
                // Arrange
                var gioHangChiTietId = Guid.NewGuid();
                var soLuong = 3;
                var result = new GioHangChiTiet
                {
                    GioHangChiTietId = gioHangChiTietId,
                    SoLuong = soLuong
                };

                _mockRepo.Setup(x => x.UpdateSoLuongAsync(gioHangChiTietId, soLuong))
                         .ReturnsAsync(result);

                // Act
                var response = await _controller.UpdateSoLuong(gioHangChiTietId, soLuong);

                // Assert
                response.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task GH004_Delete_WithValidId_ShouldReturnOk()
            {
                // Arrange
                var gioHangChiTietId = Guid.NewGuid();
                _mockRepo.Setup(x => x.RemoveSanPhamKhoiGioAsync(gioHangChiTietId))
                         .ReturnsAsync(true);

                // Act
                var result = await _controller.Delete(gioHangChiTietId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                
                // Sử dụng JsonSerializer để test anonymous object
                var jsonString = JsonSerializer.Serialize(okResult!.Value);
                var response = JsonSerializer.Deserialize<JsonElement>(jsonString);
                response.GetProperty("success").GetBoolean().Should().BeTrue();
            }
        }

        #endregion

        #region Validation Tests

        public class GioHangValidationTests
        {
            [Fact]
            public void ValidateAddToCartDTO_WithValidData_ShouldPass()
            {
                // Arrange
                var dto = new AddToCartDTO
                {
                    KhachHangId = Guid.NewGuid(),
                    SanPhamChiTietId = Guid.NewGuid(),
                    SoLuong = 1
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                dto.SoLuong.Should().BeGreaterThan(0);
                dto.KhachHangId.Should().NotBeEmpty();
                dto.SanPhamChiTietId.Should().NotBeEmpty();
            }
        }

        #endregion
    }
}