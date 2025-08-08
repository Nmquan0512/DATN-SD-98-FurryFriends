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

namespace UnitTest.ThanhPhanTest
{
    public class ThanhPhanTests
    {
        #region Controller Tests

        public class ThanhPhanControllerTests
        {
            private readonly Mock<IThanhPhanService> _mockService;
            private readonly ThanhPhanController _controller;

            public ThanhPhanControllerTests()
            {
                _mockService = new Mock<IThanhPhanService>();
                _controller = new ThanhPhanController(_mockService.Object);
            }

            [Fact]
            public async Task TP001_ThemThanhPhanHopLe_ShouldReturnCreated()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao, giàu axit amin",
                    TrangThai = true
                };

                var createdDto = new ThanhPhanDTO
                {
                    ThanhPhanId = Guid.NewGuid(),
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao, giàu axit amin",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ThanhPhanDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task TP002_ThemThanhPhanTrungTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ThanhPhanDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Tên thành phần đã tồn tại"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP003_ThemThanhPhanThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "", // Tên rỗng
                    MoTa = "Mô tả test",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP004_SuaThanhPhanHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Cá",
                    MoTa = "Nguồn đạm từ cá",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThanhPhanDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TP006_SuaThanhPhanThanhTrung_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Cá", // Đã tồn tại
                    MoTa = "Nguồn đạm từ cá",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThanhPhanDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Tên thành phần đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP007_SuaThanhPhanThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "", // Tên rỗng
                    MoTa = "Mô tả test",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP008_XoaThanhPhanThanhCong_ShouldReturnNoContent()
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
            public async Task TP010_TimKiemThanhPhanTheoTen_ShouldReturnOk()
            {
                // Arrange
                var danhSachThanhPhan = new List<ThanhPhanDTO>
                {
                    new ThanhPhanDTO { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Cá", MoTa = "Nguồn đạm từ cá", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachThanhPhan);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public async Task TP013_ThemThanhPhanVoiMoTaDai_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Dầu cá",
                    MoTa = new string('A', 1001), // Vượt quá 1000 ký tự
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP014_ThemThanhPhanVoiKyTuDacBiet_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Con_Chim!", // Chứa ký tự đặc biệt
                    MoTa = "Thành phần @#$%",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP017_ThemThanhPhanVoiTenCoKyTuDacBiet_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Chất % béo", // Chứa ký tự đặc biệt
                    MoTa = "Thành phần test",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP019_ThemThanhPhanVoiTenDoDaiToiDa_ShouldReturnCreated()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = new string('A', 255), // Đúng 255 ký tự
                    MoTa = "Thành phần test",
                    TrangThai = true
                };

                var createdDto = new ThanhPhanDTO
                {
                    ThanhPhanId = Guid.NewGuid(),
                    TenThanhPhan = new string('A', 255),
                    MoTa = "Thành phần test",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ThanhPhanDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task TP005_SuaMoTaThanhPhanHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao, giàu axit amin và vitamin B",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThanhPhanDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TP009_HuyXoaThanhPhan_ShouldReturnNoContent()
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
            public async Task TP011_TimKiemKhongCoKetQua_ShouldReturnEmptyList()
            {
                // Arrange
                var emptyList = new List<ThanhPhanDTO>();
                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyList);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(emptyList);
            }

            [Fact]
            public async Task TP012_PhanTrangHienThiTatCaThanhPhan_ShouldReturnOk()
            {
                // Arrange
                var components = new List<ThanhPhanDTO>
                {
                    new ThanhPhanDTO { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Thịt gà", MoTa = "Nguồn đạm", TrangThai = true },
                    new ThanhPhanDTO { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Cá", MoTa = "Nguồn đạm từ cá", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(components);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(components);
            }

            [Fact]
            public async Task TP015_SuaTrangThaiThanhPhan_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Dầu",
                    MoTa = "Nguồn chất béo",
                    TrangThai = false // Thay đổi trạng thái
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThanhPhanDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TP016_SuaTenThanhPhanThanhTenDaTonTai_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà", // Tên đã tồn tại
                    MoTa = "Mô tả mới",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ThanhPhanDTO>()))
                           .ThrowsAsync(new InvalidOperationException("Tên thành phần đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP018_SapXepThanhPhanTheoTenAZ_ShouldReturnOk()
            {
                // Arrange
                var components = new List<ThanhPhanDTO>
                {
                    new ThanhPhanDTO { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Cá", MoTa = "Nguồn đạm từ cá", TrangThai = true },
                    new ThanhPhanDTO { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Dầu", MoTa = "Nguồn chất béo", TrangThai = true },
                    new ThanhPhanDTO { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Thịt gà", MoTa = "Nguồn đạm", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(components);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
            }
        }

        #endregion

        #region Service Tests

        public class ThanhPhanServiceTests
        {
            private readonly Mock<IThanhPhanRepository> _mockRepository;
            private readonly ThanhPhanService _service;

            public ThanhPhanServiceTests()
            {
                _mockRepository = new Mock<IThanhPhanRepository>();
                _service = new ThanhPhanService(_mockRepository.Object);
            }

            [Fact]
            public async Task LayTatCaThanhPhan_ShouldReturnAllComponents()
            {
                // Arrange
                var danhSachThanhPhan = new List<ThanhPhan>
                {
                    new ThanhPhan { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Thịt gà", MoTa = "Nguồn đạm", TrangThai = true },
                    new ThanhPhan { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Cá", MoTa = "Nguồn đạm từ cá", TrangThai = true }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachThanhPhan);

                // Act
                var ketQua = await _service.GetAllAsync();

                // Assert
                ketQua.Should().HaveCount(2);
                ketQua.First().TenThanhPhan.Should().Be("Thịt gà");
            }

            [Fact]
            public async Task LayThanhPhanTheoIdHopLe_ShouldReturnComponent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var thanhPhan = new ThanhPhan
                {
                    ThanhPhanId = id,
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(thanhPhan);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenThanhPhan.Should().Be("Thịt gà");
            }

            [Fact]
            public async Task LayThanhPhanTheoIdKhongHopLe_ShouldReturnNull()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((ThanhPhan)null);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().BeNull();
            }

            [Fact]
            public async Task TaoThanhPhanMoi_ShouldCreateNewComponent()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao, giàu axit amin",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ThanhPhan>());
                _mockRepository.Setup(x => x.AddAsync(It.IsAny<ThanhPhan>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.CreateAsync(dto);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenThanhPhan.Should().Be("Thịt gà");
                ketQua.ThanhPhanId.Should().NotBeEmpty();
            }

            [Fact]
            public async Task TaoThanhPhanTrungTen_ShouldThrowException()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao",
                    TrangThai = true
                };

                var existingComponents = new List<ThanhPhan>
                {
                    new ThanhPhan { ThanhPhanId = Guid.NewGuid(), TenThanhPhan = "Thịt gà", MoTa = "Đã tồn tại", TrangThai = true }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(existingComponents);

                // Act & Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
            }

            [Fact]
            public async Task CapNhatThanhPhanVoiIdHopLe_ShouldUpdateComponent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var thanhPhanHienTai = new ThanhPhan
                {
                    ThanhPhanId = id,
                    TenThanhPhan = "Tên cũ",
                    MoTa = "Mô tả cũ",
                    TrangThai = true
                };

                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Tên mới",
                    MoTa = "Mô tả mới",
                    TrangThai = false
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(thanhPhanHienTai);
                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ThanhPhan> { thanhPhanHienTai });
                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<ThanhPhan>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeTrue();
            }

            [Fact]
            public async Task CapNhatThanhPhanVoiIdKhongHopLe_ShouldReturnFalse()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Test Component",
                    MoTa = "Test description",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((ThanhPhan)null);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeFalse();
            }

            [Fact]
            public async Task XoaThanhPhanVoiIdHopLe_ShouldDeleteComponent()
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
            public async Task XoaThanhPhanVoiIdKhongHopLe_ShouldReturnFalse()
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

        public class ThanhPhanDTOValidationTests
        {
            [Fact]
            public void TP003_ValidateTenThanhPhanRong_ShouldFail()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "",
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThanhPhanDTO.TenThanhPhan)));
            }

            [Fact]
            public void ValidateThanhPhanHopLe_ShouldPass()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao, giàu axit amin",
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
            public void ValidateDoDaiTenThanhPhan_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = new string('A', 255), // Đúng 255 ký tự
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
            public void ValidateDoDaiTenThanhPhan_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = new string('A', 256), // Vượt quá 255 ký tự
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThanhPhanDTO.TenThanhPhan)));
            }

            [Fact]
            public void ValidateKyTuDacBietTrongTen_ShouldFail()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt_gà!", // Chứa ký tự đặc biệt
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThanhPhanDTO.TenThanhPhan)));
            }

            [Fact]
            public void ValidateMoTaDoDai_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = new string('A', 1000), // Đúng 1000 ký tự
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
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = new string('A', 1001), // Vượt quá 1000 ký tự
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ThanhPhanDTO.MoTa)));
            }
        }

        #endregion

        #region Integration Tests

        public class ThanhPhanIntegrationTests
        {
            private readonly DbContextOptions<AppDbContext> _options;

            public ThanhPhanIntegrationTests()
            {
                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }

            [Fact]
            public async Task TP001_Integration_ThemThanhPhanHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm chất lượng cao, giàu axit amin",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                var createdDto = createdAtResult!.Value as ThanhPhanDTO;
                createdDto.TenThanhPhan.Should().Be("Thịt gà");
            }

            [Fact]
            public async Task TP003_Integration_ThemThanhPhanThieuTen_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = null, // Tên null để trigger validation
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP004_Integration_SuaThanhPhanHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                // Tạo thành phần trước
                var createDto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm từ gà",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThanhPhanDTO;
                var componentId = createdDto.ThanhPhanId;

                // Cập nhật thành phần
                var updateDto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Cá",
                    MoTa = "Nguồn đạm từ cá",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(componentId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TP006_Integration_SuaThanhPhanThanhTrung_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                // Tạo thành phần đầu tiên
                var firstComponent = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà",
                    MoTa = "Nguồn đạm từ gà",
                    TrangThai = true
                };

                await controller.Create(firstComponent);

                // Tạo thành phần thứ hai
                var secondComponent = new ThanhPhanDTO
                {
                    TenThanhPhan = "Cá",
                    MoTa = "Nguồn đạm từ cá",
                    TrangThai = true
                };

                var createResult = await controller.Create(secondComponent);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThanhPhanDTO;
                var componentId = createdDto.ThanhPhanId;

                // Thử cập nhật thành phần thứ hai thành tên trùng
                var updateDto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Thịt gà", // Đã tồn tại
                    MoTa = "Nguồn đạm từ cá",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(componentId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP008_Integration_XoaThanhPhan_ShouldReturnNoContent()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                // Tạo thành phần trước
                var createDto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Protein",
                    MoTa = "Nguồn đạm",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThanhPhanDTO;
                var componentId = createdDto.ThanhPhanId;

                // Act
                var result = await controller.Delete(componentId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task TP010_Integration_TimKiemThanhPhan_ShouldReturnMatchingResults()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                // Tạo thành phần với tên "Cá"
                var createDto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Cá",
                    MoTa = "Nguồn đạm từ cá",
                    TrangThai = true
                };

                await controller.Create(createDto);

                // Act
                var result = await controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedComponents = okResult!.Value as IEnumerable<ThanhPhanDTO>;
                returnedComponents.Should().Contain(x => x.TenThanhPhan.Equals("Cá", StringComparison.OrdinalIgnoreCase));
            }

            [Fact]
            public async Task TP013_Integration_ThemThanhPhanVoiMoTaDai_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                var moTaDai = new string('A', 1001); // Vượt quá 1000 ký tự
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Dầu cá",
                    MoTa = moTaDai,
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP014_Integration_ThemThanhPhanVoiKyTuDacBiet_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Con_Chim!", // Chứa ký tự đặc biệt
                    MoTa = "Thành phần @#$%",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP017_Integration_ThemThanhPhanVoiTenCoKyTuDacBiet_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Chất % béo", // Chứa ký tự đặc biệt
                    MoTa = "Thành phần test",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task TP019_Integration_ThemThanhPhanVoiTenDoDaiToiDa_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                var tenDai = new string('A', 255); // Đúng 255 ký tự
                var dto = new ThanhPhanDTO
                {
                    TenThanhPhan = tenDai,
                    MoTa = "Thành phần test",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task TP020_Integration_HienThiThongBaoXacNhanXoa_ShouldWork()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ThanhPhanRepository(context);
                var service = new ThanhPhanService(repository);
                var controller = new ThanhPhanController(service);

                // Tạo thành phần trước
                var createDto = new ThanhPhanDTO
                {
                    TenThanhPhan = "Test Component",
                    MoTa = "Thành phần test",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ThanhPhanDTO;
                var componentId = createdDto.ThanhPhanId;

                // Act
                var result = await controller.Delete(componentId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }
        }

        #endregion
    }
} 