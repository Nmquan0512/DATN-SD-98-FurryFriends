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

namespace UnitTest.MauSacTest
{
    public class MauSacTests
    {
        #region Controller Tests

        public class MauSacControllerTests
        {
            private readonly Mock<IMauSacService> _mockService;
            private readonly MauSacController _controller;

            public MauSacControllerTests()
            {
                _mockService = new Mock<IMauSacService>();
                _controller = new MauSacController(_mockService.Object);
            }

                    [Fact]
        public async Task MS001_AddValidColor_ShouldReturnCreated()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Xanh Ngọc",
                    MaMau = "#40E0D0",
                    MoTa = "Màu cho vòng cổ mèo",
                    TrangThai = true
                };

                var createdDto = new MauSacDTO
                {
                    MauSacId = Guid.NewGuid(),
                    TenMau = "Xanh Ngọc",
                    MaMau = "#40E0D0",
                    MoTa = "Màu cho vòng cổ mèo",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult.Value.Should().BeEquivalentTo(createdDto);
            }

            [Fact]
            public async Task MS002_AddDuplicateColorName_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Xanh Ngọc",
                    MaMau = "#008080",
                    MoTa = "Màu cho đồ chơi chó",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS003_AddColorWithEmptyName_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "", // Empty name
                    MaMau = "#F0E68C",
                    MoTa = "Màu cho bát ăn",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS004_AddColorWithEmptyColorCode_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Hồng Phấn",
                    MaMau = "", // Empty color code
                    MoTa = "Màu cho dây dắt chó",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS005_AddColorWithInvalidColorCode_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Vàng Chanh",
                    MaMau = "GOLDEN", // Invalid format
                    MoTa = "Màu cho chuồng hamster",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS006_EditValidColorName_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new MauSacDTO
                {
                    TenMau = "Đỏ Cherry",
                    MaMau = "#800000",
                    MoTa = "Màu cho áo choàng thú cưng",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task MS007_EditValidColorCode_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new MauSacDTO
                {
                    TenMau = "Nâu Đất",
                    MaMau = "#5A401D",
                    MoTa = "Màu cho chuồng gỗ",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task MS008_EditColorNameToDuplicate_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new MauSacDTO
                {
                    TenMau = "Xanh Ngọc", // Duplicate name
                    MaMau = "#36454F",
                    MoTa = "Màu cho chăn đệm",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<MauSacDTO>()))
                           .ThrowsAsync(new ArgumentException("Tên màu sắc đã tồn tại"));

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS009_EditColorWithEmptyName_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new MauSacDTO
                {
                    TenMau = "", // Empty name
                    MaMau = "#FF0000",
                    MoTa = "Màu đỏ",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS010_DeleteColor_ShouldReturnNoContent()
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
            public async Task MS011_DeleteNonExistentColor_ShouldReturnNotFound()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockService.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);

                // Act
                var result = await _controller.Delete(id);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task MS012_GetAllColors_ShouldReturnAllColors()
            {
                // Arrange
                var colors = new List<MauSacDTO>
                {
                    new MauSacDTO { MauSacId = Guid.NewGuid(), TenMau = "Đỏ", MaMau = "#FF0000", TrangThai = true },
                    new MauSacDTO { MauSacId = Guid.NewGuid(), TenMau = "Xanh", MaMau = "#0000FF", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(colors);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedColors = okResult!.Value as IEnumerable<MauSacDTO>;
                returnedColors.Should().HaveCount(2);
            }

            [Fact]
            public async Task MS013_GetColorById_ShouldReturnColor()
            {
                // Arrange
                var id = Guid.NewGuid();
                var color = new MauSacDTO
                {
                    MauSacId = id,
                    TenMau = "Vàng",
                    MaMau = "#FFFF00",
                    TrangThai = true
                };

                _mockService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(color);

                // Act
                var result = await _controller.GetById(id);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedColor = okResult!.Value as MauSacDTO;
                returnedColor.TenMau.Should().Be("Vàng");
            }

            [Fact]
            public async Task MS014_GetNonExistentColorById_ShouldReturnNotFound()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((MauSacDTO)null);

                // Act
                var result = await _controller.GetById(id);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task MS015_PaginationDisplayAllColors_ShouldReturnOk()
            {
                // Arrange
                var colors = new List<MauSacDTO>
                {
                    new MauSacDTO { MauSacId = Guid.NewGuid(), TenMau = "Đỏ", MaMau = "#FF0000", TrangThai = true },
                    new MauSacDTO { MauSacId = Guid.NewGuid(), TenMau = "Xanh", MaMau = "#0000FF", TrangThai = true },
                    new MauSacDTO { MauSacId = Guid.NewGuid(), TenMau = "Vàng", MaMau = "#FFFF00", TrangThai = true },
                    new MauSacDTO { MauSacId = Guid.NewGuid(), TenMau = "Tím", MaMau = "#800080", TrangThai = true },
                    new MauSacDTO { MauSacId = Guid.NewGuid(), TenMau = "Cam", MaMau = "#FFA500", TrangThai = true }
                };

                _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(colors);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedColors = okResult!.Value as IEnumerable<MauSacDTO>;
                returnedColors.Should().HaveCount(5);
            }

            [Fact]
            public async Task MS016_AddColorWithLongDescription_ShouldReturnCreated()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Bạc Ánh Kim",
                    MaMau = "#C0C0C0",
                    MoTa = new string('A', 500), // Mô tả dài 500 ký tự
                    TrangThai = true
                };

                var createdDto = new MauSacDTO
                {
                    MauSacId = Guid.NewGuid(),
                    TenMau = "Bạc Ánh Kim",
                    MaMau = "#C0C0C0",
                    MoTa = new string('A', 500),
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task MS017_EditColorWithInvalidColorCode_ShouldReturnBadRequest()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new MauSacDTO
                {
                    TenMau = "Màu Test",
                    MaMau = "INVALID_CODE", // Mã màu không hợp lệ
                    MoTa = "Màu test",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS018_AddColorWithNameExceedingMaxLength_ShouldReturnBadRequest()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = new string('A', 101), // Vượt quá 100 ký tự
                    MaMau = "#FF0000",
                    MoTa = "Màu đỏ",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS019_EditColorWithSpecialCharactersInName_ShouldReturnNoContent()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new MauSacDTO
                {
                    TenMau = "Màu Đỏ-Cam",
                    MaMau = "#FF4500",
                    MoTa = "Màu đỏ cam",
                    TrangThai = true
                };

                _mockService.Setup(x => x.UpdateAsync(id, It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(true);

                // Act
                var result = await _controller.Update(id, dto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task MS020_AddColorWithEmptyDescription_ShouldReturnCreated()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Màu Xám",
                    MaMau = "#808080",
                    MoTa = "", // Mô tả rỗng
                    TrangThai = true
                };

                var createdDto = new MauSacDTO
                {
                    MauSacId = Guid.NewGuid(),
                    TenMau = "Màu Xám",
                    MaMau = "#808080",
                    MoTa = "",
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task MS021_AddColorWithNullDescription_ShouldReturnCreated()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Màu Đen",
                    MaMau = "#000000",
                    MoTa = null, // Mô tả null
                    TrangThai = true
                };

                var createdDto = new MauSacDTO
                {
                    MauSacId = Guid.NewGuid(),
                    TenMau = "Màu Đen",
                    MaMau = "#000000",
                    MoTa = null,
                    TrangThai = true
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task MS022_AddColorWithInactiveStatus_ShouldReturnCreated()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Màu Cũ",
                    MaMau = "#696969",
                    MoTa = "Màu không còn sử dụng",
                    TrangThai = false // Trạng thái không hoạt động
                };

                var createdDto = new MauSacDTO
                {
                    MauSacId = Guid.NewGuid(),
                    TenMau = "Màu Cũ",
                    MaMau = "#696969",
                    MoTa = "Màu không còn sử dụng",
                    TrangThai = false
                };

                _mockService.Setup(x => x.CreateAsync(It.IsAny<MauSacDTO>()))
                           .ReturnsAsync(createdDto);

                // Act
                var result = await _controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }
        }

        #endregion

        #region Service Tests

        public class MauSacServiceTests
        {
            private readonly Mock<IMauSacRepository> _mockRepository;
            private readonly MauSacService _service;

            public MauSacServiceTests()
            {
                _mockRepository = new Mock<IMauSacRepository>();
                _service = new MauSacService(_mockRepository.Object);
            }

            [Fact]
            public async Task GetAllAsync_ShouldReturnAllColors()
            {
                // Arrange
                var colors = new List<MauSac>
                {
                    new MauSac { MauSacId = Guid.NewGuid(), TenMau = "Đỏ", MaMau = "#FF0000", TrangThai = true },
                    new MauSac { MauSacId = Guid.NewGuid(), TenMau = "Xanh", MaMau = "#0000FF", TrangThai = true }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(colors);

                // Act
                var result = await _service.GetAllAsync();

                // Assert
                result.Should().HaveCount(2);
                result.Should().Contain(x => x.TenMau == "Đỏ");
                result.Should().Contain(x => x.TenMau == "Xanh");
            }

            [Fact]
            public async Task GetByIdAsync_WithValidId_ShouldReturnColor()
            {
                // Arrange
                var id = Guid.NewGuid();
                var color = new MauSac
                {
                    MauSacId = id,
                    TenMau = "Vàng",
                    MaMau = "#FFFF00",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(color);

                // Act
                var result = await _service.GetByIdAsync(id);

                // Assert
                result.Should().NotBeNull();
                result.TenMau.Should().Be("Vàng");
                result.MaMau.Should().Be("#FFFF00");
            }

            [Fact]
            public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((MauSac)null);

                // Act
                var result = await _service.GetByIdAsync(id);

                // Assert
                result.Should().BeNull();
            }

            [Fact]
            public async Task CreateAsync_ShouldCreateNewColor()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Tím",
                    MaMau = "#800080",
                    MoTa = "Màu tím",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<MauSac>()))
                             .Returns(Task.CompletedTask);

                // Act
                var result = await _service.CreateAsync(dto);

                // Assert
                result.Should().NotBeNull();
                result.TenMau.Should().Be("Tím");
                result.MaMau.Should().Be("#800080");
                result.MauSacId.Should().NotBeEmpty();

                _mockRepository.Verify(x => x.AddAsync(It.IsAny<MauSac>()), Times.Once);
            }

            [Fact]
            public async Task UpdateAsync_WithValidId_ShouldUpdateColor()
            {
                // Arrange
                var id = Guid.NewGuid();
                var existingColor = new MauSac
                {
                    MauSacId = id,
                    TenMau = "Cũ",
                    MaMau = "#000000",
                    TrangThai = true
                };

                var dto = new MauSacDTO
                {
                    TenMau = "Mới",
                    MaMau = "#FFFFFF",
                    MoTa = "Màu mới",
                    TrangThai = false
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(existingColor);
                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<MauSac>()))
                             .Returns(Task.CompletedTask);

                // Act
                var result = await _service.UpdateAsync(id, dto);

                // Assert
                result.Should().BeTrue();
                _mockRepository.Verify(x => x.UpdateAsync(It.Is<MauSac>(c => 
                    c.TenMau == "Mới" && c.MaMau == "#FFFFFF")), Times.Once);
            }

            [Fact]
            public async Task UpdateAsync_WithInvalidId_ShouldReturnFalse()
            {
                // Arrange
                var id = Guid.NewGuid();
                var dto = new MauSacDTO
                {
                    TenMau = "Mới",
                    MaMau = "#FFFFFF",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((MauSac)null);

                // Act
                var result = await _service.UpdateAsync(id, dto);

                // Assert
                result.Should().BeFalse();
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<MauSac>()), Times.Never);
            }

            [Fact]
            public async Task DeleteAsync_WithValidId_ShouldDeleteColor()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.ExistsAsync(id)).ReturnsAsync(true);
                _mockRepository.Setup(x => x.DeleteAsync(id)).Returns(Task.CompletedTask);

                // Act
                var result = await _service.DeleteAsync(id);

                // Assert
                result.Should().BeTrue();
                _mockRepository.Verify(x => x.DeleteAsync(id), Times.Once);
            }

            [Fact]
            public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
            {
                // Arrange
                var id = Guid.NewGuid();
                _mockRepository.Setup(x => x.ExistsAsync(id)).ReturnsAsync(false);

                // Act
                var result = await _service.DeleteAsync(id);

                // Assert
                result.Should().BeFalse();
                _mockRepository.Verify(x => x.DeleteAsync(id), Times.Never);
            }
        }

        #endregion

        #region Validation Tests

        public class MauSacDTOValidationTests
        {
            [Fact]
            public void MS003_ValidateEmptyColorName_ShouldFail()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "", // Empty name
                    MaMau = "#F0E68C",
                    MoTa = "Màu cho bát ăn",
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().Contain(x => x.ErrorMessage.Contains("bắt buộc"));
            }

            [Fact]
            public void MS004_ValidateEmptyColorCode_ShouldFail()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Hồng Phấn",
                    MaMau = "", // Empty color code
                    MoTa = "Màu cho dây dắt chó",
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().Contain(x => x.ErrorMessage.Contains("Mã màu không được để trống"));
            }

            [Fact]
            public void MS005_ValidateInvalidColorCode_ShouldFail()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Vàng Chanh",
                    MaMau = "GOLDEN", // Invalid format
                    MoTa = "Màu cho chuồng hamster",
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse();
                validationResults.Should().Contain(x => x.ErrorMessage.Contains("Mã màu không đúng định dạng"));
            }

            [Fact]
            public void ValidateValidColor_ShouldPass()
            {
                // Arrange
                var dto = new MauSacDTO
                {
                    TenMau = "Xanh Ngọc",
                    MaMau = "#40E0D0",
                    MoTa = "Màu cho vòng cổ mèo",
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
            public void ValidateValidColorCodeFormats_ShouldPass()
            {
                // Arrange
                var validColorCodes = new[]
                {
                    "#FF0000", // Red
                    "#00FF00", // Green
                    "#0000FF", // Blue
                    "#FFFF00", // Yellow
                    "#FF00FF", // Magenta
                    "#00FFFF", // Cyan
                    "#000000", // Black
                    "#FFFFFF"  // White
                };

                foreach (var colorCode in validColorCodes)
                {
                    var dto = new MauSacDTO
                    {
                        TenMau = "Test Color",
                        MaMau = colorCode,
                        TrangThai = true
                    };

                    // Act
                    var validationResults = new List<ValidationResult>();
                    var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                    // Assert
                    isValid.Should().BeTrue($"Color code {colorCode} should be valid");
                    validationResults.Should().BeEmpty($"Color code {colorCode} should not have validation errors");
                }
            }

            [Fact]
            public void ValidateInvalidColorCodeFormats_ShouldFail()
            {
                // Arrange
                            var invalidColorCodes = new[]
            {
                "FF0000",   // Missing #
                "#FF000",   // Too short
                "#FF00000", // Too long
                "GOLDEN",   // Not hex
                "#GG0000",  // Invalid hex characters
                "#FF0000FF" // Alpha channel not supported
            };

                foreach (var colorCode in invalidColorCodes)
                {
                    var dto = new MauSacDTO
                    {
                        TenMau = "Test Color",
                        MaMau = colorCode,
                        TrangThai = true
                    };

                    // Act
                    var validationResults = new List<ValidationResult>();
                    var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                    // Assert
                    isValid.Should().BeFalse($"Color code {colorCode} should be invalid");
                    validationResults.Should().Contain(x => x.ErrorMessage.Contains("Mã màu không đúng định dạng"));
                }
            }

            [Fact]
            public void ValidateColorNameLength_ShouldRespectMaxLength()
            {
                // Arrange - Test with name exactly at max length
                var dto = new MauSacDTO
                {
                    TenMau = new string('A', 50), // Exactly 50 characters
                    MaMau = "#FF0000",
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeTrue("Name at max length should be valid");
            }

            [Fact]
            public void ValidateColorNameLength_ShouldFailWhenExceedsMaxLength()
            {
                // Arrange - Test with name exceeding max length
                var dto = new MauSacDTO
                {
                    TenMau = new string('A', 51), // Exceeds 50 characters
                    MaMau = "#FF0000",
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

                // Assert
                isValid.Should().BeFalse("Name exceeding max length should be invalid");
            }
        }

        #endregion

        #region Integration Tests

        public class MauSacIntegrationTests
        {
            private readonly DbContextOptions<AppDbContext> _options;

            public MauSacIntegrationTests()
            {
                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }

            [Fact]
            public async Task MS001_Integration_AddValidColor_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "Xanh Ngọc",
                    MaMau = "#40E0D0",
                    MoTa = "Màu cho vòng cổ mèo",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                var createdDto = createdAtResult.Value as MauSacDTO;
                createdDto.Should().NotBeNull();
                createdDto.TenMau.Should().Be("Xanh Ngọc");
                createdDto.MaMau.Should().Be("#40E0D0");
            }

            [Fact]
            public async Task MS003_Integration_AddColorWithEmptyName_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "", // Empty name
                    MaMau = "#F0E68C",
                    MoTa = "Màu cho bát ăn",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS004_Integration_AddColorWithEmptyColorCode_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "Hồng Phấn",
                    MaMau = "", // Empty color code
                    MoTa = "Màu cho dây dắt chó",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS005_Integration_AddColorWithInvalidColorCode_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "Vàng Chanh",
                    MaMau = "GOLDEN", // Invalid format
                    MoTa = "Màu cho chuồng hamster",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS006_Integration_EditValidColorName_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // First create a color
                var createDto = new MauSacDTO
                {
                    TenMau = "Đỏ Đô",
                    MaMau = "#800000",
                    MoTa = "Màu cho áo choàng thú cưng",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult).Value as MauSacDTO;
                var colorId = createdDto.MauSacId;

                // Then update it
                var updateDto = new MauSacDTO
                {
                    TenMau = "Đỏ Cherry",
                    MaMau = "#800000",
                    MoTa = "Màu cho áo choàng thú cưng",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(colorId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task MS007_Integration_EditValidColorCode_ShouldWorkEndToEnd()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // First create a color
                var createDto = new MauSacDTO
                {
                    TenMau = "Nâu Đất",
                    MaMau = "#8B4513",
                    MoTa = "Màu cho chuồng gỗ",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult).Value as MauSacDTO;
                var colorId = createdDto.MauSacId;

                // Then update it
                var updateDto = new MauSacDTO
                {
                    TenMau = "Nâu Đất",
                    MaMau = "#5A401D",
                    MoTa = "Màu cho chuồng gỗ",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(colorId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task MS008_Integration_EditColorNameToDuplicate_ShouldFail()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // First create two colors
                var createDto1 = new MauSacDTO
                {
                    TenMau = "Xanh Ngọc",
                    MaMau = "#40E0D0",
                    MoTa = "Màu cho vòng cổ mèo",
                    TrangThai = true
                };

                var createDto2 = new MauSacDTO
                {
                    TenMau = "Tím Than",
                    MaMau = "#36454F",
                    MoTa = "Màu cho chăn đệm",
                    TrangThai = true
                };

                await controller.Create(createDto1);
                var createResult2 = await controller.Create(createDto2);
                var createdDto2 = (createResult2 as CreatedAtActionResult).Value as MauSacDTO;
                var colorId2 = createdDto2.MauSacId;

                // Try to update second color with duplicate name
                var updateDto = new MauSacDTO
                {
                    TenMau = "Xanh Ngọc", // Duplicate name
                    MaMau = "#36454F",
                    MoTa = "Màu cho chăn đệm",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(colorId2, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            // Additional test cases based on the provided test specification
            [Fact]
            public async Task MS009_EditColorWithEmptyName_ShouldReturnBadRequest()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // First create a color
                var createDto = new MauSacDTO
                {
                    TenMau = "Đỏ Đô",
                    MaMau = "#800000",
                    MoTa = "Màu cho đồ chơi xương",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult).Value as MauSacDTO;
                var colorId = createdDto.MauSacId;

                // Try to update with empty name
                var updateDto = new MauSacDTO
                {
                    TenMau = "", // Empty name
                    MaMau = "#FFFFFF",
                    MoTa = "Màu cho đồ chơi xương",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(colorId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS010_DeleteColor_ShouldReturnNoContent()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // First create a color
                var createDto = new MauSacDTO
                {
                    TenMau = "Cam Đất",
                    MaMau = "#FF8C00",
                    MoTa = "Màu cho đệm ngủ",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult).Value as MauSacDTO;
                var colorId = createdDto.MauSacId;

                // Act
                var result = await controller.Delete(colorId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task MS011_DeleteNonExistentColor_ShouldReturnNotFound()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var nonExistentId = Guid.NewGuid();

                // Act
                var result = await controller.Delete(nonExistentId);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task MS012_GetAllColors_ShouldReturnAllColors()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // Create some test colors directly in context
                var colors = new[]
                {
                    new MauSac { MauSacId = Guid.NewGuid(), TenMau = "Vàng Chanh", MaMau = "#FFFF00", TrangThai = true },
                    new MauSac { MauSacId = Guid.NewGuid(), TenMau = "Đỏ Tươi", MaMau = "#FF0000", TrangThai = true },
                    new MauSac { MauSacId = Guid.NewGuid(), TenMau = "Xanh Dương", MaMau = "#0000FF", TrangThai = true }
                };

                context.MauSacs.AddRange(colors);
                await context.SaveChangesAsync();

                // Act
                var result = await controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedColors = okResult.Value as IEnumerable<MauSacDTO>;
                returnedColors.Should().HaveCountGreaterOrEqualTo(3);
            }

            [Fact]
            public async Task MS013_GetColorById_ShouldReturnColor()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // Create a color
                var createDto = new MauSacDTO
                {
                    TenMau = "Tím Than",
                    MaMau = "#800080",
                    MoTa = "Màu cho chăn đệm",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult).Value as MauSacDTO;
                var colorId = createdDto.MauSacId;

                // Act
                var result = await controller.GetById(colorId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedColor = okResult.Value as MauSacDTO;
                returnedColor.TenMau.Should().Be("Tím Than");
                returnedColor.MaMau.Should().Be("#800080");
            }

            [Fact]
            public async Task MS014_GetNonExistentColorById_ShouldReturnNotFound()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var nonExistentId = Guid.NewGuid();

                // Act
                var result = await controller.GetById(nonExistentId);

                // Assert
                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async Task MS015_AddColorWithLongDescription_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var longDescription = new string('A', 500); // 500 characters
                var dto = new MauSacDTO
                {
                    TenMau = "Bạc Ánh Kim",
                    MaMau = "#C0C0C0",
                    MoTa = longDescription,
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task MS016_AddColorWithSpecialCharacters_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "Xanh@Lá#",
                    MaMau = "#00FF00",
                    MoTa = "Màu sắc !@#$%^&*() cho sản phẩm độc đáo",
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task MS017_EditColorWithInvalidColorCode_ShouldReturnBadRequest()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // First create a color
                var createDto = new MauSacDTO
                {
                    TenMau = "Đỏ",
                    MaMau = "#FF0000",
                    MoTa = "Màu đỏ",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult).Value as MauSacDTO;
                var colorId = createdDto.MauSacId;

                // Try to update with invalid color code
                var updateDto = new MauSacDTO
                {
                    TenMau = "Đỏ",
                    MaMau = "INVALIDCODE", // Invalid format
                    MoTa = "Màu đỏ",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(colorId, updateDto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS018_AddColorWithNameExceedingMaxLength_ShouldReturnBadRequest()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var longName = new string('A', 256); // Exceeds 50 character limit
                var dto = new MauSacDTO
                {
                    TenMau = longName,
                    MaMau = "#C0C0C0",
                    MoTa = "Màu bạc",
                    TrangThai = true
                };

                // Manually trigger validation
                var validationContext = new ValidationContext(dto);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(dto, validationContext, validationResults, true);

                // Add validation errors to ModelState
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task MS019_EditColorWithSpecialCharactersInName_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                // First create a color
                var createDto = new MauSacDTO
                {
                    TenMau = "Xanh Lá",
                    MaMau = "#00FF00",
                    MoTa = "Màu xanh lá",
                    TrangThai = true
                };

                var createResult = await controller.Create(createDto);
                var createdDto = (createResult as CreatedAtActionResult).Value as MauSacDTO;
                var colorId = createdDto.MauSacId;

                // Update with special characters
                var updateDto = new MauSacDTO
                {
                    TenMau = "Xanh Lá cây @",
                    MaMau = "#00FF00",
                    MoTa = "Màu xanh lá cây",
                    TrangThai = true
                };

                // Act
                var result = await controller.Update(colorId, updateDto);

                // Assert
                result.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public async Task MS020_AddColorWithEmptyDescription_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "Xám",
                    MaMau = "#808080",
                    MoTa = "", // Empty description
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task MS021_AddColorWithNullDescription_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "Nâu",
                    MaMau = "#8B4513",
                    MoTa = null, // Null description
                    TrangThai = true
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
            }

            [Fact]
            public async Task MS022_AddColorWithInactiveStatus_ShouldSucceed()
            {
                // Arrange
                using var context = new AppDbContext(_options);
                var repository = new MauSacRepository(context);
                var service = new MauSacService(repository);
                var controller = new MauSacController(service);

                var dto = new MauSacDTO
                {
                    TenMau = "Đen",
                    MaMau = "#000000",
                    MoTa = "Màu đen",
                    TrangThai = false // Inactive status
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