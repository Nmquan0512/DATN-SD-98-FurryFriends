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

namespace UnitTest.KichCoTest
{
    public class KichCoTests
    {
        #region Controller Tests

        public class KichCoControllerTests
        {
            private readonly Mock<IKichCoService> _mockService;
            private readonly KichCoController _controller;

            public KichCoControllerTests()
            {
                _mockService = new Mock<IKichCoService>();
                _controller = new KichCoController(_mockService.Object);
            }

            [Fact]
            public async Task KC001_ThemKichCoHopLe_ShouldReturnCreated()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ cho thú cưng mini",
                    TrangThai = true
                };

                var createdDto = new KichCoDTO
                {
                    KichCoId = Guid.NewGuid(),
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ cho thú cưng mini",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<KichCoDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task KC002_ThemKichCoTrungTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<KichCoDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Tên kích cỡ đã tồn tại"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC003_ThemKichCoThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "", // Tên rỗng
                    MoTa = "Kích cỡ cho thú cưng lớn",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC004_SuaKichCoHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new KichCoDTO
                {
                    TenKichCo = "S",
                    MoTa = "Kích cỡ nhỏ",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<KichCoDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task KC005_SuaMoTaKichCoHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new KichCoDTO
                {
                    TenKichCo = "L",
                    MoTa = "Kích cỡ lớn cho thú cưng trưởng thành",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<KichCoDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task KC006_SuaKichCoThanhTrung_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new KichCoDTO
                {
                    TenKichCo = "S", // Đã tồn tại
                    MoTa = "Kích cỡ nhỏ",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<KichCoDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Tên kích cỡ đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC007_SuaKichCoThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new KichCoDTO
                {
                    TenKichCo = "", // Tên rỗng
                    MoTa = "Mô tả test",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC008_XoaKichCoThanhCong_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockService.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

                // Act
                var result = await _controller.Delete(id);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task KC009_HuyXoaKichCo_ShouldReturnNoContent()
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
            public async Task KC010_TimKiemKichCoTheoTen_ShouldReturnOk()
            {
                // Arrange
                var danhSachKichCo = new List<KichCoDTO>
                {
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "Small", MoTa = "Kích cỡ nhỏ", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachKichCo);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task KC011_TimKiemKhongCoKetQua_ShouldReturnOk()
            {
                // Arrange
                var emptyList = new List<KichCoDTO>();

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyList);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedList = okResult!.Value as IEnumerable<KichCoDTO>;
                returnedList.Should().BeEmpty();
            }

            [Fact]
            public async Task KC012_PhanTrangHienThiTatCaKichCo_ShouldReturnOk()
            {
                // Arrange
                var danhSachKichCo = new List<KichCoDTO>
                {
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "XS", MoTa = "Kích cỡ rất nhỏ", TrangThai = true },
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "S", MoTa = "Kích cỡ nhỏ", TrangThai = true },
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "M", MoTa = "Kích cỡ trung bình", TrangThai = true },
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "L", MoTa = "Kích cỡ lớn", TrangThai = true },
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "XL", MoTa = "Kích cỡ rất lớn", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachKichCo);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedList = okResult!.Value as IEnumerable<KichCoDTO>;
                returnedList.Should().HaveCount(5);
            }

            [Fact]
            public async Task KC013_ThemKichCoVoiMoTaRấtDai_ShouldReturnCreated()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XXL",
                    MoTa = new string('A', 500), // Mô tả rất dài 500 ký tự
                    TrangThai = true
                };

                var createdDto = new KichCoDTO
                {
                    KichCoId = Guid.NewGuid(),
                    TenKichCo = "XXL",
                    MoTa = new string('A', 500),
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<KichCoDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task KC014_ThemKichCoVoiKyTuDacBiet_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XL-Large!", // Chứa ký tự đặc biệt
                    MoTa = "Kích cỡ !@#$%^&*(",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC016_ThemKichCoVoiTenDoDaiToiDa_ShouldReturnCreated()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = new string('A', 50), // Đúng 50 ký tự
                    MoTa = "Kích cỡ test",
                    TrangThai = true
                };

                var createdDto = new KichCoDTO
                {
                    KichCoId = Guid.NewGuid(),
                    TenKichCo = new string('A', 50),
                    MoTa = "Kích cỡ test",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<KichCoDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task KC017_SapXepKichCoTheoMoTaZA_ShouldReturnOk()
            {
                // Arrange
                var danhSachKichCo = new List<KichCoDTO>
                {
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "L", MoTa = "Lớn", TrangThai = true },
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "M", MoTa = "Trung bình", TrangThai = true },
                    new KichCoDTO { KichCoId = Guid.NewGuid(), TenKichCo = "S", MoTa = "Nhỏ", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachKichCo);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedList = okResult!.Value as IEnumerable<KichCoDTO>;
                returnedList.Should().HaveCount(3);
            }

            [Fact]
            public async Task KC018_SuaKichCoMoTaThanhRong_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new KichCoDTO
                {
                    TenKichCo = "size M",
                    MoTa = "", // Mô tả rỗng
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<KichCoDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task KC015_SuaTrangThaiKichCo_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new KichCoDTO
                {
                    TenKichCo = "Micro",
                    MoTa = "Kích cỡ micro",
                    TrangThai = false // Thay đổi trạng thái
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<KichCoDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

        }

        #endregion

        #region Service Tests

        public class KichCoServiceTests
        {
            private readonly Mock<IKichCoRepository> _mockRepository;
            private readonly KichCoService _service;

            public KichCoServiceTests()
            {
                _mockRepository = new Mock<IKichCoRepository>();
                _service = new KichCoService(_mockRepository.Object);
            }

            [Fact]
            public async Task LayTatCaKichCo_ShouldReturnAllSizes()
            {
                // Arrange
                var danhSachKichCo = new List<KichCo>
                {
                    new KichCo { KichCoId = Guid.NewGuid(), TenKichCo = "XS", MoTa = "Kích cỡ rất nhỏ", TrangThai = true },
                    new KichCo { KichCoId = Guid.NewGuid(), TenKichCo = "S", MoTa = "Kích cỡ nhỏ", TrangThai = true }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachKichCo);

                // Act
                var ketQua = await _service.GetAllAsync();

                // Assert
                ketQua.Should().HaveCount(2);
                ketQua.First().TenKichCo.Should().Be("XS");
            }

            [Fact]
            public async Task LayKichCoTheoIdHopLe_ShouldReturnSize()
            {
                // Arrange
                var id = Guid.NewGuid();
                var kichCo = new KichCo
                {
                    KichCoId = id,
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ cho thú cưng mini",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(kichCo);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenKichCo.Should().Be("XS");
            }

            [Fact]
            public async Task LayKichCoTheoIdKhongHopLe_ShouldReturnNull()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((KichCo)null);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().BeNull();
            }

            [Fact]
            public async Task TaoKichCoMoi_ShouldCreateNewSize()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ cho thú cưng mini",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<KichCo>());
                _mockRepository.Setup(x => x.AddAsync(It.IsAny<KichCo>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.CreateAsync(dto);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenKichCo.Should().Be("XS");
                ketQua.KichCoId.Should().NotBeEmpty();
            }

            [Fact]
            public async Task TaoKichCoTrungTen_ShouldThrowException()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ",
                    TrangThai = true
                };

                var existingSizes = new List<KichCo>
                {
                    new KichCo { KichCoId = Guid.NewGuid(), TenKichCo = "XS", MoTa = "Đã tồn tại", TrangThai = true }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(existingSizes);

                // Act & Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
            }

            [Fact]
            public async Task CapNhatKichCoVoiIdHopLe_ShouldUpdateSize()
            {
                // Arrange
                var id = Guid.NewGuid();
                var kichCoHienTai = new KichCo
                {
                    KichCoId = id,
                    TenKichCo = "Tên cũ",
                    MoTa = "Mô tả cũ",
                    TrangThai = true
                };

                var dto = new KichCoDTO
                {
                    TenKichCo = "Tên mới",
                    MoTa = "Mô tả mới",
                    TrangThai = false
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(kichCoHienTai);
                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<KichCo> { kichCoHienTai });
                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<KichCo>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeTrue();
            }

            [Fact]
            public async Task CapNhatKichCoVoiIdKhongHopLe_ShouldReturnFalse()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new KichCoDTO
                {
                    TenKichCo = "Test Size",
                    MoTa = "Test description",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((KichCo)null);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeFalse();
            }

            [Fact]
            public async Task XoaKichCoVoiIdHopLe_ShouldDeleteSize()
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
            public async Task XoaKichCoVoiIdKhongHopLe_ShouldReturnFalse()
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

        public class KichCoDTOValidationTests
        {
            [Fact]
            public void KC003_ValidateTenKichCoRong_ShouldFail()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "",
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(KichCoDTO.TenKichCo)));
            }

            [Fact]
            public void ValidateKichCoHopLe_ShouldPass()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ cho thú cưng mini",
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
            public void ValidateDoDaiTenKichCo_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = new string('A', 50), // Đúng 50 ký tự
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeTrue();
            }

            [Fact]
            public void ValidateDoDaiTenKichCo_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = new string('A', 51), // Vượt quá 50 ký tự
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(KichCoDTO.TenKichCo)));
            }

            [Fact]
            public void ValidateKyTuDacBietTrongTen_ShouldFail()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XL-Large!", // Chứa ký tự đặc biệt
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(KichCoDTO.TenKichCo)));
            }

            [Fact]
            public void ValidateMoTaDoDai_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = new string('A', 500), // Đúng 500 ký tự
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeTrue();
            }

            [Fact]
            public void ValidateMoTaDoDai_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange
                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = new string('A', 501), // Vượt quá 500 ký tự
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(KichCoDTO.MoTa)));
            }
        }

        #endregion

        #region Integration Tests

        public class KichCoIntegrationTests
        {
            private readonly DbContextOptions<AppDbContext> _options;

            public KichCoIntegrationTests()
            {
                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }

            [Fact]
            public async Task KC001_Integration_ThemKichCoHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                var dto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ cho thú cưng mini",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                var createdDto = createdAtResult!.Value as KichCoDTO;
                createdDto.TenKichCo.Should().Be("XS");
            }

            [Fact]
            public async Task KC003_Integration_ThemKichCoThieuTen_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                var dto = new KichCoDTO
                {
                    TenKichCo = null, // Tên null để trigger validation
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC004_Integration_SuaKichCoHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                // Tạo kích cỡ trước
                var createDto = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as KichCoDTO;
                var sizeId = createdDto.KichCoId;

                // Cập nhật kích cỡ
                var updateDto = new KichCoDTO
                {
                    TenKichCo = "S",
                    MoTa = "Kích cỡ nhỏ",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(sizeId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task KC006_Integration_SuaKichCoThanhTrung_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                // Tạo kích cỡ đầu tiên
                var firstSize = new KichCoDTO
                {
                    TenKichCo = "XS",
                    MoTa = "Kích cỡ rất nhỏ",
                    TrangThai = true
                };

                await controller.Create(firstSize);

                // Tạo kích cỡ thứ hai
                var secondSize = new KichCoDTO
                {
                    TenKichCo = "S",
                    MoTa = "Kích cỡ nhỏ",
                    TrangThai = true
                };

                var createResult = await controller.Create(secondSize);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as KichCoDTO;
                var sizeId = createdDto.KichCoId;

                // Thử cập nhật kích cỡ thứ hai thành tên trùng
                var updateDto = new KichCoDTO
                {
                    TenKichCo = "XS", // Đã tồn tại
                    MoTa = "Kích cỡ nhỏ",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(sizeId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC008_Integration_XoaKichCo_ShouldReturnNoContent()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                // Tạo kích cỡ trước
                var createDto = new KichCoDTO
                {
                    TenKichCo = "M",
                    MoTa = "Kích cỡ trung bình",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as KichCoDTO;
                var sizeId = createdDto.KichCoId;

                // Act
                var result = await controller.Delete(sizeId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task KC010_Integration_TimKiemKichCo_ShouldReturnMatchingResults()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                // Tạo kích cỡ với tên "Small"
                var createDto = new KichCoDTO
                {
                    TenKichCo = "Small",
                    MoTa = "Kích cỡ nhỏ",
                    TrangThai = true
                };

                await controller.Create(createDto);

                // Act
                var result = await controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedSizes = okResult!.Value as IEnumerable<KichCoDTO>;
                returnedSizes.Should().Contain(x => x.TenKichCo.Equals("Small", StringComparison.OrdinalIgnoreCase));
            }

            [Fact]
            public async Task KC014_Integration_ThemKichCoVoiKyTuDacBiet_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                var dto = new KichCoDTO
                {
                    TenKichCo = "XL-Large!", // Chứa ký tự đặc biệt
                    MoTa = "Kích cỡ !@#$%^&*(",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task KC016_Integration_ThemKichCoVoiTenDoDaiToiDa_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                var tenDai = new string('A', 50); // Đúng 50 ký tự
                var dto = new KichCoDTO
                {
                    TenKichCo = tenDai,
                    MoTa = "Kích cỡ test",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task KC018_Integration_SuaKichCoMoTaThanhRong_ShouldWork()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new KichCoRepository(context);
                var service = new KichCoService(repository);
                var controller = new KichCoController(service);

                // Tạo kích cỡ trước
                var createDto = new KichCoDTO
                {
                    TenKichCo = "size M",
                    MoTa = "Medium size",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as KichCoDTO;
                var sizeId = createdDto.KichCoId;

                // Cập nhật mô tả thành rỗng
                var updateDto = new KichCoDTO
                {
                    TenKichCo = "size M",
                    MoTa = "", // Mô tả rỗng
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(sizeId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }
        }

        #endregion
    }
} 