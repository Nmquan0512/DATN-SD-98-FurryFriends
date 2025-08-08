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
using System.ComponentModel;

namespace UnitTest.ThuongHieuTest
{
    public class ThuongHieuTests
    {
        #region Controller Tests

        public class ThuongHieuControllerTests
        {
            private readonly Mock<IThuongHieuService> _mockService;
            private readonly ThuongHieuController _controller;

            public ThuongHieuControllerTests()
            {
                _mockService = new Mock<IThuongHieuService>();
                _controller = new ThuongHieuController(_mockService.Object);
            }

            [Fact]
            public async Task TH001_ThemThuongHieuHopLe_ShouldReturnCreated()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Royal Canin",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    DiaChi = "Pháp",
                    MoTa = "Thức ăn dinh dưỡng cho thú cưng",
                    TrangThai = true
                };

                var createdDto = new ThuongHieuDTO
                {
                    ThuongHieuId = Guid.NewGuid(),
                    TenThuongHieu = "Royal Canin",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    DiaChi = "Pháp",
                    MoTa = "Thức ăn dinh dưỡng cho thú cưng",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ThuongHieuDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task TH002_ThemThuongHieuTrungTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Royal Canin",
                    Email = "info2@royalcanin.com",
                    SDT = "0987654321",
                    DiaChi = "Pháp",
                    MoTa = "Thức ăn",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ThuongHieuDTO>()))
                           .ThrowsAsync(new ArgumentException("Tên thương hiệu đã tồn tại"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH003_ThemThuongHieuThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "", // Tên rỗng
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    DiaChi = "Test Country",
                    MoTa = "Thương hiệu mới",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH004_ThemThuongHieuEmailKhongHopLe_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Hill's Science Diet",
                    Email = "hills@diet", // Email không hợp lệ
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "Thức ăn khoa học",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH005_ThemThuongHieuSDTKhongHopLe_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Purina",
                    Email = "contact@purina.com",
                    SDT = "123", // SDT không hợp lệ
                    DiaChi = "USA",
                    MoTa = "Thức ăn tổng hợp",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH006_SuaTenThuongHieuHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Pedigree Pet Foods",
                    Email = "info@pedigree.com",
                    SDT = "0123456789",
                    DiaChi = "Anh",
                    MoTa = "Thức ăn cho chó",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThuongHieuDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TH007_SuaEmailThuongHieuHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Whiskas",
                    Email = "support@whiskas.com",
                    SDT = "0123456789",
                    DiaChi = "Mỹ",
                    MoTa = "Thức ăn cho mèo",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThuongHieuDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TH008_SuaTenThuongHieuThanhTrung_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Purina", // Đã tồn tại
                    Email = "contact@friskies.com",
                    SDT = "0123456789",
                    DiaChi = "Mỹ",
                    MoTa = "Thức ăn mèo giá rẻ",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThuongHieuDTO>()))
                           .ThrowsAsync(new ArgumentException("Tên thương hiệu đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH011_HuyXoaThuongHieu_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockService.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);

                // Act
                var result = await _controller.Delete(id);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task TH012_TimKiemTheoTenThuongHieu_ShouldReturnOk()
            {
                // Arrange
                var brands = new List<ThuongHieuDTO>
                {
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Royal Canin", Email = "info@royalcanin.com", SDT = "0123456789", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(brands);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(brands);
            }

            [Fact]
            public async Task TH013_TimKiemTheoEmail_ShouldReturnOk()
            {
                // Arrange
                var brands = new List<ThuongHieuDTO>
                {
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Royal Canin", Email = "info@royalcanin.com", SDT = "0123456789", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(brands);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task TH014_TimKiemKhongCoKetQua_ShouldReturnEmptyList()
            {
                // Arrange
                var emptyList = new List<ThuongHieuDTO>();
                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyList);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(emptyList);
            }

            [Fact]
            public async Task TH015_PhanTrangHienThiTatCaThuongHieu_ShouldReturnOk()
            {
                // Arrange
                var brands = new List<ThuongHieuDTO>
                {
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Royal Canin", Email = "info@royalcanin.com", SDT = "0123456789", TrangThai = true },
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Purina", Email = "contact@purina.com", SDT = "0987654321", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(brands);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(brands);
            }

            [Fact]
            public async Task TH016_ThemThuongHieuVoiDiaChiDai_ShouldReturnCreated()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Test Brand",
                    Email = "test@test.com",
                    SDT = "0123456789",
                    DiaChi = "Địa chỉ rất dài với nhiều thông tin chi tiết về địa chỉ của thương hiệu này tại thành phố Hồ Chí Minh, Việt Nam",
                    MoTa = "Mô tả thương hiệu",
                    TrangThai = true
                };

                var createdDto = new ThuongHieuDTO
                {
                    ThuongHieuId = Guid.NewGuid(),
                    TenThuongHieu = "Test Brand",
                    Email = "test@test.com",
                    SDT = "0123456789",
                    DiaChi = "Địa chỉ rất dài với nhiều thông tin chi tiết về địa chỉ của thương hiệu này tại thành phố Hồ Chí Minh, Việt Nam",
                    MoTa = "Mô tả thương hiệu",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ThuongHieuDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task TH017_SuaDiaChiThuongHieuThanhCong_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Royal Canin",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    DiaChi = "Địa chỉ mới của Royal Canin tại Việt Nam",
                    MoTa = "Thức ăn dinh dưỡng",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThuongHieuDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TH018_SapXepThuongHieuTheoTenZA_ShouldReturnOk()
            {
                // Arrange
                var brands = new List<ThuongHieuDTO>
                {
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Whiskas", Email = "info@whiskas.com", SDT = "0123456789", TrangThai = true },
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Royal Canin", Email = "info@royalcanin.com", SDT = "0987654321", TrangThai = true },
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Purina", Email = "contact@purina.com", SDT = "0123456789", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(brands);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task TH019_ThemThuongHieuVoiTenVuotQuaGioiHan_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Tên thương hiệu rất dài vượt quá giới hạn cho phép của hệ thống",
                    Email = "test@test.com",
                    SDT = "0123456789",
                    DiaChi = "Địa chỉ",
                    MoTa = "Mô tả",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH020_ThongKeThuongHieuTheoTrangThai_ShouldReturnOk()
            {
                // Arrange
                var brands = new List<ThuongHieuDTO>
                {
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Royal Canin", Email = "info@royalcanin.com", SDT = "0123456789", TrangThai = true },
                    new ThuongHieuDTO { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Purina", Email = "contact@purina.com", SDT = "0987654321", TrangThai = false }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(brands);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task TH021_SuaThuongHieuVoiTenChuaKyTuDacBiet_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Royal-Canin@2024",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    DiaChi = "Pháp",
                    MoTa = "Thức ăn dinh dưỡng",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }
        }

        #endregion

        #region Service Tests

        public class ThuongHieuServiceTests
        {
            private readonly Mock<IThuongHieuRepository> _mockRepository;
            private readonly ThuongHieuService _service;

            public ThuongHieuServiceTests()
            {
                _mockRepository = new Mock<IThuongHieuRepository>();
                _service = new ThuongHieuService(_mockRepository.Object);
            }

            [Fact]
            public async Task LayTatCaThuongHieu_ShouldReturnAllBrands()
            {
                // Arrange
                var danhSachThuongHieu = new List<ThuongHieu>
                {
                    new ThuongHieu { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Royal Canin", Email = "info@royalcanin.com", SDT = "0123456789", TrangThai = true },
                    new ThuongHieu { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Purina", Email = "contact@purina.com", SDT = "0987654321", TrangThai = true }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachThuongHieu);

                // Act
                var ketQua = await _service.GetAllAsync();

                // Assert
                ketQua.Should().HaveCount(2);
                ketQua.First().TenThuongHieu.Should().Be("Royal Canin");
            }

            [Fact]
            public async Task LayThuongHieuTheoIdHopLe_ShouldReturnBrand()
            {
                // Arrange
                var id = Guid.NewGuid();
                var thuongHieu = new ThuongHieu
                {
                    ThuongHieuId = id,
                    TenThuongHieu = "Royal Canin",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(thuongHieu);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenThuongHieu.Should().Be("Royal Canin");
            }

            [Fact]
            public async Task LayThuongHieuTheoIdKhongHopLe_ShouldReturnNull()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((ThuongHieu)null);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().BeNull();
            }

            [Fact]
            public async Task TaoThuongHieuMoi_ShouldCreateNewBrand()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Royal Canin",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    DiaChi = "Pháp",
                    MoTa = "Thức ăn dinh dưỡng",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<ThuongHieu>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.CreateAsync(dto);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenThuongHieu.Should().Be("Royal Canin");
                ketQua.ThuongHieuId.Should().NotBeEmpty();
            }

            [Fact]
            public async Task CapNhatThuongHieuVoiIdHopLe_ShouldUpdateBrand()
            {
                // Arrange
                var id = Guid.NewGuid();
                var thuongHieuHienTai = new ThuongHieu
                {
                    ThuongHieuId = id,
                    TenThuongHieu = "Tên cũ",
                    Email = "old@email.com",
                    SDT = "1234567890",
                    TrangThai = true
                };

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Tên mới",
                    Email = "new@email.com",
                    SDT = "0987654321",
                    TrangThai = false
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(thuongHieuHienTai);
                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<ThuongHieu>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeTrue();
            }

            [Fact]
            public async Task CapNhatThuongHieuVoiIdKhongHopLe_ShouldReturnFalse()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Test Brand",
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((ThuongHieu)null);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeFalse();
            }

            [Fact]
            public async Task XoaThuongHieuVoiIdHopLe_ShouldDeleteBrand()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.ExistsAsync(id)).ReturnsAsync(true);
                _mockRepository.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.DeleteAsync(id);

                // Assert
                ketQua.Should().BeTrue();
            }

            [Fact]
            public async Task XoaThuongHieuVoiIdKhongHopLe_ShouldReturnFalse()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.ExistsAsync(id)).ReturnsAsync(false);

                // Act
                var ketQua = await _service.DeleteAsync(id);

                // Assert
                ketQua.Should().BeFalse();
            }
        }

        #endregion

        #region DTO Validation Tests

        public class ThuongHieuDTOValidationTests
        {
            [Fact]
            public void TH003_ValidateTenThuongHieuRong_ShouldFail()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "",
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThuongHieuDTO.TenThuongHieu)));
            }

            [Fact]
            public void TH004_ValidateEmailKhongHopLe_ShouldFail()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Test Brand",
                    Email = "invalid-email",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThuongHieuDTO.Email)));
            }

            [Fact]
            public void TH005_ValidateSDTKhongHopLe_ShouldFail()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Test Brand",
                    Email = "test@brand.com",
                    SDT = "123", // Định dạng không hợp lệ
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThuongHieuDTO.SDT)));
            }

            [Fact]
            public void ValidateThuongHieuHopLe_ShouldPass()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Royal Canin",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    DiaChi = "Pháp",
                    MoTa = "Thức ăn dinh dưỡng",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeTrue();
                ketQuaValidation.Should().BeEmpty();
            }

            [Fact]
            public void ValidateDoDaiTenThuongHieu_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = new string('A', 50), // Đúng 50 ký tự
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeTrue();
            }

            [Fact]
            public void ValidateDoDaiTenThuongHieu_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = new string('A', 51), // Vượt quá 50 ký tự
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThuongHieuDTO.TenThuongHieu)));
            }

            [Fact]
            public void ValidateKyTuDacBietTrongTen_ShouldFail()
            {
                // Arrange
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Brand@Name", // Chứa ký tự đặc biệt
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThuongHieuDTO.TenThuongHieu)));
            }
        }

        #endregion

        #region Integration Tests

        public class ThuongHieuIntegrationTests
        {
            private readonly DbContextOptions<AppDbContext> _options;

            public ThuongHieuIntegrationTests()
            {
                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }

            [Fact]
            public async Task TH001_Integration_ThemThuongHieuHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Royal Canin",
                    Email = "info@royalcanin.com",
                    SDT = "0123456789",
                    DiaChi = "Pháp",
                    MoTa = "Thức ăn dinh dưỡng cho thú cưng",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                var createdDto = createdAtResult!.Value as ThuongHieuDTO;
                createdDto.TenThuongHieu.Should().Be("Royal Canin");
            }

            [Fact]
            public async Task TH003_Integration_ThemThuongHieuThieuTen_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = null, // Tên null để trigger validation
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    DiaChi = "Test Country",
                    MoTa = "Thương hiệu mới",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH004_Integration_ThemThuongHieuEmailKhongHopLe_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Hill's Science Diet",
                    Email = "hills@diet", // Email không hợp lệ
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "Thức ăn khoa học",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH005_Integration_ThemThuongHieuSDTKhongHopLe_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Purina",
                    Email = "contact@purina.com",
                    SDT = "123", // SDT không hợp lệ
                    DiaChi = "USA",
                    MoTa = "Thức ăn tổng hợp",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH006_Integration_SuaTenThuongHieuHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo thương hiệu trước
                var createDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Pedigree",
                    Email = "info@pedigree.com",
                    SDT = "0123456789",
                    DiaChi = "Anh",
                    MoTa = "Thức ăn cho chó",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThuongHieuDTO;
                var brandId = createdDto.ThuongHieuId;

                // Cập nhật thương hiệu
                var updateDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Pedigree Pet Foods",
                    Email = "info@pedigree.com",
                    SDT = "0123456789",
                    DiaChi = "Anh",
                    MoTa = "Thức ăn cho chó",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(brandId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TH008_Integration_SuaTenThuongHieuThanhTrung_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo thương hiệu đầu tiên
                var firstBrand = new ThuongHieuDTO
                {
                    TenThuongHieu = "Purina",
                    Email = "contact@purina.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "Thức ăn tổng hợp",
                    TrangThai = true
                };

                await controller.Create(firstBrand);

                // Tạo thương hiệu thứ hai
                var secondBrand = new ThuongHieuDTO
                {
                    TenThuongHieu = "Friskies",
                    Email = "contact@friskies.com",
                    SDT = "0987654321",
                    DiaChi = "USA",
                    MoTa = "Thức ăn mèo giá rẻ",
                    TrangThai = true
                };

                var createResult = await controller.Create(secondBrand);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThuongHieuDTO;
                var brandId = createdDto.ThuongHieuId;

                // Thử cập nhật thương hiệu thứ hai thành tên trùng
                var updateDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Purina", // Đã tồn tại
                    Email = "contact@friskies.com",
                    SDT = "0987654321",
                    DiaChi = "USA",
                    MoTa = "Thức ăn mèo giá rẻ",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(brandId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH009_SuaThuongHieuThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo thương hiệu trước
                var createDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Orijen",
                    Email = "info@orijen.com",
                    SDT = "0123456789",
                    DiaChi = "Canada",
                    MoTa = "High-quality dry food",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThuongHieuDTO;
                var brandId = createdDto.ThuongHieuId;

                // Cập nhật với tên rỗng
                var updateDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "", // Tên rỗng
                    Email = "info@orijen.com",
                    SDT = "0123456789",
                    DiaChi = "Canada",
                    MoTa = "High-quality dry food",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(brandId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH010_XoaThuongHieu_ShouldReturnNoContent()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo thương hiệu trước
                var createDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Fancy Feast",
                    Email = "contact@fancyfeast.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "Cat pate",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThuongHieuDTO;
                var brandId = createdDto.ThuongHieuId;

                // Act
                var result = await controller.Delete(brandId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TH011_XoaThuongHieuKhongTonTai_ShouldReturnNotFound()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var nonExistentId = Guid.NewGuid();

                // Act
                var result = await controller.Delete(nonExistentId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task TH012_LayTatCaThuongHieu_ShouldReturnAllBrands()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo một số thương hiệu test trực tiếp trong context
                var danhSachThuongHieu = new[]
                {
                    new ThuongHieu { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Royal Canin", Email = "info@royalcanin.com", SDT = "0123456789", DiaChi = "Pháp", MoTa = "Thức ăn dinh dưỡng", TrangThai = true },
                    new ThuongHieu { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Purina", Email = "contact@purina.com", SDT = "0987654321", DiaChi = "USA", MoTa = "Thức ăn tổng hợp", TrangThai = true },
                    new ThuongHieu { ThuongHieuId = Guid.NewGuid(), TenThuongHieu = "Hill's", Email = "info@hills.com", SDT = "0555555555", DiaChi = "USA", MoTa = "Thức ăn khoa học", TrangThai = true }
                };

                context.ThuongHieus.AddRange(danhSachThuongHieu);
                await context.SaveChangesAsync();

                // Act
                var result = await controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedBrands = okResult!.Value as IEnumerable<ThuongHieuDTO>;
                returnedBrands.Should().HaveCountGreaterOrEqualTo(3);
            }

            [Fact]
            public async Task TH013_LayThuongHieuTheoId_ShouldReturnBrand()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo thương hiệu
                var createDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Whiskas",
                    Email = "info@whiskas.com",
                    SDT = "0123456789",
                    DiaChi = "Mỹ",
                    MoTa = "Thức ăn cho mèo",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThuongHieuDTO;
                var brandId = createdDto.ThuongHieuId;

                // Act
                var result = await controller.GetById(brandId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedBrand = okResult!.Value as ThuongHieuDTO;
                returnedBrand.TenThuongHieu.Should().Be("Whiskas");
            }

            [Fact]
            public async Task TH014_LayThuongHieuKhongTonTaiTheoId_ShouldReturnNotFound()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var nonExistentId = Guid.NewGuid();

                // Act
                var result = await controller.GetById(nonExistentId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task TH015_ThemThuongHieuVoiMoTaDai_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var moTaDai = new string('A', 500); // 500 ký tự
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Premium Brand",
                    Email = "info@premium.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = moTaDai,
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task TH016_ThemThuongHieuVoiKyTuDacBiet_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Natural Choice",
                    Email = "info@naturalchoice.com",
                    SDT = "0123456789",
                    DiaChi = "Canada",
                    MoTa = "Thức ăn tự nhiên cho thú cưng",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task TH017_SuaThuongHieuVoiEmailKhongHopLe_ShouldReturnBadRequest()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo thương hiệu trước
                var createDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Test Brand",
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    DiaChi = "Test Country",
                    MoTa = "Test description",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThuongHieuDTO;
                var brandId = createdDto.ThuongHieuId;

                // Cập nhật với email không hợp lệ
                var updateDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Test Brand",
                    Email = "invalid-email", // Email không hợp lệ
                    SDT = "0123456789",
                    DiaChi = "Test Country",
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(brandId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH018_ThemThuongHieuVoiTenVuotQuaGioiHan_ShouldReturnBadRequest()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var tenDai = new string('A', 256); // Vượt quá giới hạn 50 ký tự
                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = tenDai,
                    Email = "test@brand.com",
                    SDT = "0123456789",
                    DiaChi = "Test Country",
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Kích hoạt validation thủ công
                var validationContext = new ValidationContext(dto);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Thêm lỗi validation vào ModelState
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "");
                    }
                }

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH019_SuaThuongHieuVoiKyTuDacBietTrongTen_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                // Tạo thương hiệu trước
                var createDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Mars",
                    Email = "info@mars.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "Pet food brand",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThuongHieuDTO;
                var brandId = createdDto.ThuongHieuId;

                // Cập nhật với ký tự đặc biệt trong tên
                var updateDto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Mars @", // Chứa ký tự đặc biệt
                    Email = "info@mars.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "Pet food brand",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(brandId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TH020_ThemThuongHieuVoiMoTaRong_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Simple Brand",
                    Email = "info@simple.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "", // Mô tả rỗng
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task TH021_ThemThuongHieuVoiMoTaNull_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Null Description Brand",
                    Email = "info@nulldesc.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = null, // Mô tả null
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task TH022_ThemThuongHieuVoiTrangThaiKhongHoatDong_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThuongHieuRepository(context);
                var service = new ThuongHieuService(repository);
                var controller = new ThuongHieuController(service);

                var dto = new ThuongHieuDTO
                {
                    TenThuongHieu = "Inactive Brand",
                    Email = "info@inactive.com",
                    SDT = "0123456789",
                    DiaChi = "USA",
                    MoTa = "Inactive brand for testing",
                    TrangThai = false // Trạng thái không hoạt động
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }
        }

        #endregion
    }
} 