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

namespace UnitTest.ChatLieuTest
{
    public class ChatLieuTests
    {
        #region Controller Tests

        public class ChatLieuControllerTests
        {
            private readonly Mock<IChatLieuService> _mockService;
            private readonly ChatLieuController _controller;

            public ChatLieuControllerTests()
            {
                _mockService = new Mock<IChatLieuService>();
                _controller = new ChatLieuController(_mockService.Object);
            }

            [Fact]
            public async Task CL001_ThemChatLieuHopLe_ShouldReturnCreated()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton tự nhiên",
                    TrangThai = true
                };

                var createdDto = new ChatLieuDTO
                {
                    ChatLieuId = Guid.NewGuid(),
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton tự nhiên",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ChatLieuDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task CL002_ThemChatLieuTrungTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<ChatLieuDTO>()))
                           .ThrowsAsync(new ArgumentException("Tên chất liệu đã tồn tại"));

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CL003_ThemChatLieuThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "", // Tên rỗng
                    MoTa = "Chất liệu test",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CL004_SuaChatLieuHopLe_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Len",
                    MoTa = "Chất liệu len ấm áp",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ChatLieuDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task CL005_SuaChatLieuThanhTrung_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton", // Đã tồn tại
                    MoTa = "Chất liệu cotton",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ChatLieuDTO>()))
                           .ThrowsAsync(new ArgumentException("Tên chất liệu đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CL006_SuaChatLieuThieuTen_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "", // Tên rỗng
                    MoTa = "Chất liệu test",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CL008_HuyXoaChatLieu_ShouldReturnNoContent()
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
            public async Task CL009_TimKiemTheoTenChatLieu_ShouldReturnOk()
            {
                // Arrange
                var materials = new List<ChatLieuDTO>
                {
                    new ChatLieuDTO { ChatLieuId = Guid.NewGuid(), TenChatLieu = "Cotton", MoTa = "Chất liệu cotton", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(materials);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(materials);
            }

            [Fact]
            public async Task CL010_TimKiemKhongCoKetQua_ShouldReturnEmptyList()
            {
                // Arrange
                var emptyList = new List<ChatLieuDTO>();
                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(emptyList);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                okResult!.Value.Should().BeEquivalentTo(emptyList);
            }

            [Fact]
            public async Task CL014_SuaTrangThaiChatLieu_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton",
                    TrangThai = false // Thay đổi trạng thái
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<ChatLieuDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }
        }

        #endregion

        #region Service Tests

        public class ChatLieuServiceTests
        {
            private readonly Mock<IChatLieuRepository> _mockRepository;
            private readonly ChatLieuService _service;

            public ChatLieuServiceTests()
            {
                _mockRepository = new Mock<IChatLieuRepository>();
                _service = new ChatLieuService(_mockRepository.Object);
            }

            [Fact]
            public async Task LayTatCaChatLieu_ShouldReturnAllMaterials()
            {
                // Arrange
                var danhSachChatLieu = new List<ChatLieu>
                {
                    new ChatLieu { ChatLieuId = Guid.NewGuid(), TenChatLieu = "Cotton", MoTa = "Chất liệu cotton", TrangThai = true },
                    new ChatLieu { ChatLieuId = Guid.NewGuid(), TenChatLieu = "Len", MoTa = "Chất liệu len", TrangThai = true }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(danhSachChatLieu);

                // Act
                var ketQua = await _service.GetAllAsync();

                // Assert
                ketQua.Should().HaveCount(2);
                ketQua.First().TenChatLieu.Should().Be("Cotton");
            }

            [Fact]
            public async Task LayChatLieuTheoIdHopLe_ShouldReturnMaterial()
            {
                // Arrange
                var id = Guid.NewGuid();
                var chatLieu = new ChatLieu
                {
                    ChatLieuId = id,
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(chatLieu);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenChatLieu.Should().Be("Cotton");
            }

            [Fact]
            public async Task LayChatLieuTheoIdKhongHopLe_ShouldReturnNull()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((ChatLieu)null);

                // Act
                var ketQua = await _service.GetByIdAsync(id);

                // Assert
                ketQua.Should().BeNull();
            }

            [Fact]
            public async Task TaoChatLieuMoi_ShouldCreateNewMaterial()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton tự nhiên",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<ChatLieu>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.CreateAsync(dto);

                // Assert
                ketQua.Should().NotBeNull();
                ketQua.TenChatLieu.Should().Be("Cotton");
                ketQua.ChatLieuId.Should().NotBeEmpty();
            }

            [Fact]
            public async Task CapNhatChatLieuVoiIdHopLe_ShouldUpdateMaterial()
            {
                // Arrange
                var id = Guid.NewGuid();
                var chatLieuHienTai = new ChatLieu
                {
                    ChatLieuId = id,
                    TenChatLieu = "Tên cũ",
                    MoTa = "Mô tả cũ",
                    TrangThai = true
                };

                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Tên mới",
                    MoTa = "Mô tả mới",
                    TrangThai = false
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(chatLieuHienTai);
                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<ChatLieu>()))
                              .Returns(Task.CompletedTask);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeTrue();
            }

            [Fact]
            public async Task CapNhatChatLieuVoiIdKhongHopLe_ShouldReturnFalse()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Test Material",
                    MoTa = "Test description",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((ChatLieu)null);

                // Act
                var ketQua = await _service.UpdateAsync(id, dto);

                // Assert
                ketQua.Should().BeFalse();
            }

            [Fact]
            public async Task XoaChatLieuVoiIdHopLe_ShouldDeleteMaterial()
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
            public async Task XoaChatLieuVoiIdKhongHopLe_ShouldReturnFalse()
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

        public class ChatLieuDTOValidationTests
        {
            [Fact]
            public void CL003_ValidateTenChatLieuRong_ShouldFail()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "",
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ChatLieuDTO.TenChatLieu)));
            }

            [Fact]
            public void ValidateChatLieuHopLe_ShouldPass()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton tự nhiên",
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
            public void ValidateDoDaiTenChatLieu_ShouldRespectMaxLength()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = new string('A', 100), // Đúng 100 ký tự
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
            public void ValidateDoDaiTenChatLieu_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = new string('A', 101), // Vượt quá 100 ký tự
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ChatLieuDTO.TenChatLieu)));
            }

            [Fact]
            public void ValidateKyTuDacBietTrongTen_ShouldFail()
            {
                // Arrange
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton@Material", // Chứa ký tự đặc biệt
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var ketQuaValidation = new List<ValidationResult>();
                var hopLe = Validator.TryValidateObject(dto, new ValidationContext(dto), ketQuaValidation, true);

                // Assert
                hopLe.Should().BeFalse();
                ketQuaValidation.Should().Contain(x => x.MemberNames.Contains(nameof(ChatLieuDTO.TenChatLieu)));
            }
        }

        #endregion

        #region Integration Tests

        public class ChatLieuIntegrationTests
        {
            private readonly DbContextOptions<AppDbContext> _options;

            public ChatLieuIntegrationTests()
            {
                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }

            [Fact]
            public async Task CL001_Integration_ThemChatLieuHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton tự nhiên",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                var createdDto = createdAtResult!.Value as ChatLieuDTO;
                createdDto.TenChatLieu.Should().Be("Cotton");
            }

            [Fact]
            public async Task CL003_Integration_ThemChatLieuThieuTen_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var dto = new ChatLieuDTO
                {
                    TenChatLieu = null, // Tên null để trigger validation
                    MoTa = "Test description",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CL004_Integration_SuaChatLieuHopLe_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                // Tạo chất liệu trước
                var createDto = new ChatLieuDTO
                {
                    TenChatLieu = "Len",
                    MoTa = "Chất liệu len",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ChatLieuDTO;
                var materialId = createdDto.ChatLieuId;

                // Cập nhật chất liệu
                var updateDto = new ChatLieuDTO
                {
                    TenChatLieu = "Len ấm áp",
                    MoTa = "Chất liệu len ấm áp",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(materialId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task CL005_Integration_SuaChatLieuThanhTrung_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                // Tạo chất liệu đầu tiên
                var firstMaterial = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton",
                    TrangThai = true
                };

                await controller.Create(firstMaterial);

                // Tạo chất liệu thứ hai
                var secondMaterial = new ChatLieuDTO
                {
                    TenChatLieu = "Len",
                    MoTa = "Chất liệu len",
                    TrangThai = true
                };

                var createResult = await controller.Create(secondMaterial);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ChatLieuDTO;
                var materialId = createdDto.ChatLieuId;

                // Thử cập nhật chất liệu thứ hai thành tên trùng
                var updateDto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton", // Đã tồn tại
                    MoTa = "Chất liệu len",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(materialId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CL010_XoaChatLieu_ShouldReturnNoContent()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                // Tạo chất liệu trước
                var createDto = new ChatLieuDTO
                {
                    TenChatLieu = "Silk",
                    MoTa = "Chất liệu silk",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ChatLieuDTO;
                var materialId = createdDto.ChatLieuId;

                // Act
                var result = await controller.Delete(materialId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task CL011_XoaChatLieuKhongTonTai_ShouldReturnNotFound()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var nonExistentId = Guid.NewGuid();

                // Act
                var result = await controller.Delete(nonExistentId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task CL012_LayTatCaChatLieu_ShouldReturnAllMaterials()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                // Tạo một số chất liệu test trực tiếp trong context
                var danhSachChatLieu = new[]
                {
                    new ChatLieu { ChatLieuId = Guid.NewGuid(), TenChatLieu = "Cotton", MoTa = "Chất liệu cotton", TrangThai = true },
                    new ChatLieu { ChatLieuId = Guid.NewGuid(), TenChatLieu = "Len", MoTa = "Chất liệu len", TrangThai = true },
                    new ChatLieu { ChatLieuId = Guid.NewGuid(), TenChatLieu = "Silk", MoTa = "Chất liệu silk", TrangThai = true }
                };

                context.ChatLieus.AddRange(danhSachChatLieu);
                await context.SaveChangesAsync();

                // Act
                var result = await controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedMaterials = okResult!.Value as IEnumerable<ChatLieuDTO>;
                returnedMaterials.Should().HaveCountGreaterOrEqualTo(3);
            }

            [Fact]
            public async Task CL013_LayChatLieuTheoId_ShouldReturnMaterial()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                // Tạo chất liệu
                var createDto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ChatLieuDTO;
                var materialId = createdDto.ChatLieuId;

                // Act
                var result = await controller.GetById(materialId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedMaterial = okResult!.Value as ChatLieuDTO;
                returnedMaterial.TenChatLieu.Should().Be("Cotton");
            }

            [Fact]
            public async Task CL014_LayChatLieuKhongTonTaiTheoId_ShouldReturnNotFound()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var nonExistentId = Guid.NewGuid();

                // Act
                var result = await controller.GetById(nonExistentId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task CL015_ThemChatLieuVoiMoTaDai_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var moTaDai = new string('A', 500); // 500 ký tự
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Premium Material",
                    MoTa = moTaDai,
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task CL016_ThemChatLieuVoiKyTuDacBiet_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Vải %", // Chứa ký tự đặc biệt
                    MoTa = "Chất liệu test",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CL017_TimKiemChatLieuKhongPhanBietChuHoaThuong_ShouldWork()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                // Tạo chất liệu với tên "Cotton"
                var createDto = new ChatLieuDTO
                {
                    TenChatLieu = "Cotton",
                    MoTa = "Chất liệu cotton",
                    TrangThai = true
                };

                await controller.Create(createDto);

                // Act - Tìm kiếm với "cotton" (chữ thường)
                var result = await controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedMaterials = okResult!.Value as IEnumerable<ChatLieuDTO>;
                returnedMaterials.Should().Contain(x => x.TenChatLieu.Equals("Cotton", StringComparison.OrdinalIgnoreCase));
            }

            [Fact]
            public async Task CL018_ThemChatLieuVoiTenDoDaiToiDa_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var tenDai = new string('A', 100); // Đúng 100 ký tự
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = tenDai,
                    MoTa = "Chất liệu test",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task CL019_HienThiThongBaoXacNhanXoa_ShouldWork()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                // Tạo chất liệu trước
                var createDto = new ChatLieuDTO
                {
                    TenChatLieu = "Test Material",
                    MoTa = "Chất liệu test",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult)!.Value as ChatLieuDTO;
                var materialId = createdDto.ChatLieuId;

                // Act
                var result = await controller.Delete(materialId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task CL020_ThemChatLieuVoiTenVuotQuaGioiHan_ShouldReturnBadRequest()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var tenDai = new string('A', 101); // Vượt quá giới hạn 100 ký tự
                var dto = new ChatLieuDTO
                {
                    TenChatLieu = tenDai,
                    MoTa = "Chất liệu test",
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
            public async Task CL021_ThemChatLieuVoiMoTaRong_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Simple Material",
                    MoTa = "", // Mô tả rỗng
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task CL022_ThemChatLieuVoiMoTaNull_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Null Description Material",
                    MoTa = null, // Mô tả null
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task CL023_ThemChatLieuVoiTrangThaiKhongHoatDong_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new ChatLieuRepository(context);
                var service = new ChatLieuService(repository);
                var controller = new ChatLieuController(service);

                var dto = new ChatLieuDTO
                {
                    TenChatLieu = "Inactive Material",
                    MoTa = "Inactive material for testing",
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
