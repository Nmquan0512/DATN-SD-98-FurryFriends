using FurryFriends.API.Controllers;
using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using FurryFriends.API.Repository;
using FurryFriends.API.Services;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;
using FluentAssertions;

namespace UnitTest.SanPhamTest
{
    public class SanPhamTests
    {
        #region Controller Tests - SanPham (Parent Product)

        public class SanPhamControllerTests
        {
            private readonly Mock<ISanPhamService> _mockService;
            private readonly SanPhamsController _controller;

            public SanPhamControllerTests()
            {
                _mockService = new Mock<ISanPhamService>();
                _controller = new SanPhamsController(_mockService.Object);
            }

            [Fact]
            public async Task SP001_ThemSanPhamChaHopLe_DoAn_ShouldReturnCreated()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true,
                    ThanhPhanIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
                };

                var createdDto = new SanPhamDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = dto.ThuongHieuId,
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<SanPhamDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task SP002_ThemSanPhamChaHopLe_DoDung_ShouldReturnCreated()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Vòng cổ da cho chó",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true,
                    ChatLieuIds = new List<Guid> { Guid.NewGuid() }
                };

                var createdDto = new SanPhamDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    TenSanPham = "Vòng cổ da cho chó",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = dto.ThuongHieuId,
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<SanPhamDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task SP003_ThemSanPhamChaThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "", // Tên rỗng
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP004_ThemSanPhamChaThieuThuongHieu_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.Empty, // Thương hiệu rỗng
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP007_ThemSanPhamChaTrungTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con", // Đã tồn tại
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<SanPhamDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Tên sản phẩm đã tồn tại"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP008_TenSanPhamQuaDai_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = new string('A', 256), // 256 ký tự
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP009_SuaSanPhamChaHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<SanPhamDTO>()))
                           .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task SP010_SuaSanPhamChaThanhTrung_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con", // Đã tồn tại
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<SanPhamDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Tên sản phẩm đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP011_XoaSanPhamCha_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();

                _mockService.Setup(x => x.DeleteAsync(id))
                           .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Delete(id);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task SP012_XoaSanPhamChaKhongTonTai_ShouldReturnNotFound()
            {
                // Arrange
                var id = Guid.NewGuid();

                _mockService.Setup(x => x.DeleteAsync(id))
                           .ThrowsAsync(new KeyNotFoundException("Không tìm thấy sản phẩm"));

                // Act
                var result = await _controller.Delete(id);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }
        }

        #endregion

        #region Controller Tests - SanPhamChiTiet (Child Product)

        public class SanPhamChiTietControllerTests
        {
            private readonly Mock<ISanPhamChiTietService> _mockService;
            private readonly SanPhamChiTietController _controller;

            public SanPhamChiTietControllerTests()
            {
                _mockService = new Mock<ISanPhamChiTietService>();
                _controller = new SanPhamChiTietController(_mockService.Object);
            }

            [Fact]
            public async Task SP013_ThemSanPhamConHopLe_ShouldReturnOk()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 50000,
                    SoLuong = 100
                };

                var createdDto = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = Guid.NewGuid(),
                    SanPhamId = dto.SanPhamId,
                    MauSacId = dto.MauSacId,
                    KichCoId = dto.KichCoId,
                    Gia = dto.Gia,
                    SoLuong = dto.SoLuong
                };

                _mockService.Setup(x => x.CreateAndReturnAsync(It.IsAny<SanPhamChiTietDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task SP014_ThemSanPhamConTrungLap_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 50000,
                    SoLuong = 100
                };

                _mockService.Setup(x => x.CreateAndReturnAsync(It.IsAny<SanPhamChiTietDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Biến thể đã tồn tại"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task SP015_ThemSanPhamConThieuMauSac_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.Empty, // Màu sắc rỗng
                    KichCoId = Guid.NewGuid(),
                    Gia = 60000,
                    SoLuong = 100
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP016_ThemSanPhamConThieuKichCo_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.Empty, // Kích cỡ rỗng
                    Gia = 55000,
                    SoLuong = 100
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP017_ThemSanPhamConGiaAm_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = -1000, // Giá âm
                    SoLuong = 100
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP018_ThemSanPhamConSoLuongAm_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 50000,
                    SoLuong = -50 // Số lượng âm
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP019_SuaSanPhamConHopLe_ShouldReturnOk()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 55000,
                    SoLuong = 100
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<SanPhamChiTietDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task SP020_SuaSanPhamConThanhTrung_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 50000,
                    SoLuong = 100
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<SanPhamChiTietDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Biến thể đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task SP021_XoaSanPhamCon_ShouldReturnOk()
            {
                // Arrange
                var id = Guid.NewGuid();

                _mockService.Setup(x => x.DeleteAsync(id))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Delete(id);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task SP022_XoaSanPhamConKhongTonTai_ShouldReturnNotFound()
            {
                // Arrange
                var id = Guid.NewGuid();

                _mockService.Setup(x => x.DeleteAsync(id))
                           .ReturnsAsync(false);

                // Act
                var result = await _controller.Delete(id);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
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
                _mockContext = new Mock<AppDbContext>();
                _service = new SanPhamService(_mockRepository.Object, _mockContext.Object);
            }

            [Fact]
            public async Task LayTatCaSanPham_ShouldReturnAllProducts()
            {
                // Arrange
                var expectedProducts = new List<SanPham>
                {
                    new SanPham { SanPhamId = Guid.NewGuid(), TenSanPham = "Sản phẩm 1" },
                    new SanPham { SanPhamId = Guid.NewGuid(), TenSanPham = "Sản phẩm 2" }
                };

                _mockRepository.Setup(x => x.GetAllAsync())
                              .ReturnsAsync(expectedProducts);

                // Act
                var result = await _service.GetAllAsync();

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
            }

            [Fact]
            public async Task LaySanPhamTheoIdHopLe_ShouldReturnProduct()
            {
                // Arrange
                var id = Guid.NewGuid();
                var expectedProduct = new SanPham
                {
                    SanPhamId = id,
                    TenSanPham = "Hạt Royal Canin cho mèo con"
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id))
                              .ReturnsAsync(expectedProduct);

                // Act
                var result = await _service.GetByIdAsync(id);

                // Assert
                result.Should().NotBeNull();
                result.TenSanPham.Should().Be("Hạt Royal Canin cho mèo con");
            }

            [Fact]
            public async Task LaySanPhamTheoIdKhongHopLe_ShouldReturnNull()
            {
                // Arrange
                var id = Guid.NewGuid();

                _mockRepository.Setup(x => x.GetByIdAsync(id))
                              .ReturnsAsync((SanPham?)null);

                // Act & Assert
                await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
            }

            [Fact]
            public async Task TaoSanPhamMoi_ShouldCreateNewProduct()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid()
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<SanPham>()))
                              .Returns(Task.CompletedTask);
                _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(1);

                // Act
                var result = await _service.CreateAsync(dto);

                // Assert
                result.Should().NotBeNull();
                result.TenSanPham.Should().Be("Hạt Royal Canin cho mèo con");
            }

            [Fact]
            public async Task CapNhatSanPhamVoiIdHopLe_ShouldUpdateProduct()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid()
                };

                var existingProduct = new SanPham { SanPhamId = id, TenSanPham = "Old Name" };

                _mockRepository.Setup(x => x.GetByIdAsync(id))
                              .ReturnsAsync(existingProduct);
                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<SanPham>()))
                              .Returns(Task.CompletedTask);
                _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(1);

                // Act
                await _service.UpdateAsync(id, dto);

                // Assert
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<SanPham>()), Times.Once);
            }

            [Fact]
            public async Task XoaSanPhamVoiIdHopLe_ShouldDeleteProduct()
            {
                // Arrange
                var id = Guid.NewGuid();

                _mockRepository.Setup(x => x.ExistsAsync(id))
                              .ReturnsAsync(true);
                _mockRepository.Setup(x => x.DeleteAsync(id))
                              .Returns(Task.CompletedTask);
                _mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(1);

                // Act
                await _service.DeleteAsync(id);

                // Assert
                _mockRepository.Verify(x => x.DeleteAsync(id), Times.Once);
            }
        }

        #endregion

        #region DTO Validation Tests

        public class SanPhamDTOValidationTests
        {
            [Fact]
            public void SP003_ValidateTenSanPhamRong_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid()
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().ContainSingle();
                validationResults[0].ErrorMessage.Should().Contain("Tên sản phẩm không được để trống");
            }

            [Fact]
            public void ValidateSanPhamHopLe_ShouldPass()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeTrue();
                validationResults.Should().BeEmpty();
            }

            [Fact]
            public void ValidateDoDaiTenSanPham_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = new string('A', 255), // Đúng 255 ký tự
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid()
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeTrue();
            }

            [Fact]
            public void ValidateDoDaiTenSanPham_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = new string('A', 256), // Vượt quá 255 ký tự
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid()
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().ContainSingle();
                validationResults[0].ErrorMessage.Should().Contain("vượt quá giới hạn ký tự cho phép");
            }

            [Fact]
            public void ValidateThuongHieuRong_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.Empty // Thương hiệu rỗng
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().ContainSingle();
                validationResults[0].ErrorMessage.Should().Contain("Thương hiệu không được để trống");
            }
        }

        public class SanPhamChiTietDTOValidationTests
        {
            [Fact]
            public void SP015_ValidateMauSacRong_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.Empty, // Màu sắc rỗng
                    KichCoId = Guid.NewGuid(),
                    Gia = 50000,
                    SoLuong = 100
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().ContainSingle();
                validationResults[0].ErrorMessage.Should().Contain("Màu sắc không được để trống");
            }

            [Fact]
            public void SP016_ValidateKichCoRong_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.Empty, // Kích cỡ rỗng
                    Gia = 50000,
                    SoLuong = 100
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().ContainSingle();
                validationResults[0].ErrorMessage.Should().Contain("Kích cỡ không được để trống");
            }

            [Fact]
            public void SP017_ValidateGiaAm_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = -1000, // Giá âm
                    SoLuong = 100
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().ContainSingle();
                validationResults[0].ErrorMessage.Should().Contain("Giá bán phải là số dương");
            }

            [Fact]
            public void SP018_ValidateSoLuongAm_ShouldFail()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 50000,
                    SoLuong = -50 // Số lượng âm
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().ContainSingle();
                validationResults[0].ErrorMessage.Should().Contain("Số lượng phải là số dương hoặc bằng 0");
            }

            [Fact]
            public void ValidateSanPhamChiTietHopLe_ShouldPass()
            {
                // Arrange
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = Guid.NewGuid(),
                    MauSacId = Guid.NewGuid(),
                    KichCoId = Guid.NewGuid(),
                    Gia = 50000,
                    SoLuong = 100
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(dto);
                var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Assert
                isValid.Should().BeTrue();
                validationResults.Should().BeEmpty();
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
            public async Task SP001_Integration_ThemSanPhamChaHopLe_DoAn_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task SP003_Integration_ThemSanPhamChaThieuTen_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                var dto = new SanPhamDTO
                {
                    TenSanPham = "", // Tên rỗng
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP004_Integration_ThemSanPhamChaThieuThuongHieu_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                var dto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.Empty, // Thương hiệu rỗng
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP008_Integration_TenSanPhamQuaDai_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                var dto = new SanPhamDTO
                {
                    TenSanPham = new string('A', 256), // 256 ký tự
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task SP009_Integration_SuaSanPhamChaHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                // Tạo sản phẩm trước
                var createDto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdProduct = (createResult as CreatedAtActionResult)!.Value as SanPhamDTO;

                // Sửa sản phẩm
                var updateDto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con - Cao cấp",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = createDto.ThuongHieuId,
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(createdProduct!.SanPhamId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task SP011_Integration_XoaSanPhamCha_ShouldReturnNoContent()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                // Tạo sản phẩm trước
                var createDto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdProduct = (createResult as CreatedAtActionResult)!.Value as SanPhamDTO;

                // Act
                var result = await controller.Delete(createdProduct!.SanPhamId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task SP012_Integration_XoaSanPhamChaKhongTonTai_ShouldReturnNotFound()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                var nonExistentId = Guid.NewGuid();

                // Act
                var result = await controller.Delete(nonExistentId);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task SP022_Integration_TimKiemSanPhamTheoTen_ShouldReturnProducts()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                // Tạo sản phẩm trước
                var createDto = new SanPhamDTO
                {
                    TenSanPham = "Hạt Royal Canin cho mèo con",
                    LoaiSanPham = "DoAn",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                await controller.Create(createDto);

                // Act
                var result = await controller.GetFiltered(loai: null, page: 1, pageSize: 10);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task SP023_Integration_TimKiemSanPhamTheoThuongHieu_ShouldReturnProducts()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                // Tạo sản phẩm trước
                var createDto = new SanPhamDTO
                {
                    TenSanPham = "Vòng cổ da cho chó",
                    LoaiSanPham = "DoDung",
                    ThuongHieuId = Guid.NewGuid(),
                    TrangThai = true
                };

                await controller.Create(createDto);

                // Act
                var result = await controller.GetFiltered(loai: null, page: 1, pageSize: 10);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task SP024_Integration_TimKiemSanPhamKhongTonTai_ShouldReturnEmpty()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                // Act
                var result = await controller.GetFiltered(loai: null, page: 1, pageSize: 10);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task SP025_Integration_SapXepSanPhamTheoTenAZ_ShouldWork()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                // Act
                var result = await controller.GetFiltered(loai: null, page: 1, pageSize: 10);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task SP026_Integration_PhanTrangSanPham_ShouldWork()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new SanPhamRepository(context);
                var service = new SanPhamService(repository, context);
                var controller = new SanPhamsController(service);

                // Act
                var result = await controller.GetFiltered(loai: null, page: 2, pageSize: 5);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }
        }

        #endregion
    }
} 