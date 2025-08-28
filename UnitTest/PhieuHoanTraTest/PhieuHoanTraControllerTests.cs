using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTest.PhieuHoanTraTest
{
    public class PhieuHoanTraControllerTests
    {
        private readonly Mock<IPhieuHoanTraService> _mockService;
        private readonly PhieuHoanTraController _controller;

        public PhieuHoanTraControllerTests()
        {
            _mockService = new Mock<IPhieuHoanTraService>();
            _controller = new PhieuHoanTraController(_mockService.Object);
        }

        [Fact]
        public async Task PHT001_GetById_ShouldReturnOk()
        {
            // Arrange
            var phieuId = Guid.NewGuid();
            var phieu = new PhieuHoanTraDto
            {
                PhieuHoanTraId = phieuId,
                HoaDonChiTietId = Guid.NewGuid(),
                SoLuongHoan = 2,
                LyDoHoanTra = "Sản phẩm bị lỗi",
                TrangThai = 1,
                NgayHoanTra = DateTime.Now
            };

            _mockService.Setup(x => x.GetByIdAsync(phieuId)).ReturnsAsync(phieu);

            // Act
            var result = await _controller.GetById(phieuId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedPhieu = okResult!.Value as PhieuHoanTraDto;
            returnedPhieu.Should().NotBeNull();
            returnedPhieu!.PhieuHoanTraId.Should().Be(phieuId);
        }

        [Fact]
        public async Task PHT002_GetById_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            var phieuId = Guid.NewGuid();
            _mockService.Setup(x => x.GetByIdAsync(phieuId))
                       .ReturnsAsync((PhieuHoanTraDto?)null);

            // Act
            var result = await _controller.GetById(phieuId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PHT003_GetAll_ShouldReturnOk()
        {
            // Arrange
            var phieuHoanTras = new List<PhieuHoanTraDto>
            {
                new PhieuHoanTraDto
                {
                    PhieuHoanTraId = Guid.NewGuid(),
                    HoaDonChiTietId = Guid.NewGuid(),
                    SoLuongHoan = 1,
                    LyDoHoanTra = "Sản phẩm hư hỏng",
                    TrangThai = 1,
                    NgayHoanTra = DateTime.Now
                },
                new PhieuHoanTraDto
                {
                    PhieuHoanTraId = Guid.NewGuid(),
                    HoaDonChiTietId = Guid.NewGuid(),
                    SoLuongHoan = 2,
                    LyDoHoanTra = "Không vừa size",
                    TrangThai = 0,
                    NgayHoanTra = DateTime.Now
                }
            };

            _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(phieuHoanTras);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedPhieus = okResult!.Value as IEnumerable<PhieuHoanTraDto>;
            returnedPhieus.Should().HaveCount(2);
        }

        [Fact]
        public async Task PHT004_GetByKhachHang_ShouldReturnOk()
        {
            // Arrange
            var khachHangId = Guid.NewGuid();
            var phieuHoanTras = new List<PhieuHoanTraDto>
            {
                new PhieuHoanTraDto
                {
                    PhieuHoanTraId = Guid.NewGuid(),
                    HoaDonChiTietId = Guid.NewGuid(),
                    SoLuongHoan = 1,
                    LyDoHoanTra = "Sản phẩm hư hỏng",
                    TrangThai = 1,
                    NgayHoanTra = DateTime.Now
                }
            };

            _mockService.Setup(x => x.GetByKhachHangAsync(khachHangId)).ReturnsAsync(phieuHoanTras);

            // Act
            var result = await _controller.GetByKhachHang(khachHangId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedPhieus = okResult!.Value as IEnumerable<PhieuHoanTraDto>;
            returnedPhieus.Should().HaveCount(1);
        }

        [Fact]
        public async Task PHT005_Create_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new PhieuHoanTraCreateRequest
            {
                HoaDonChiTietId = Guid.NewGuid(),
                SoLuongHoan = 1,
                LyDoHoanTra = "Sản phẩm bị lỗi"
            };

            _mockService.Setup(x => x.CreateAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task PHT006_Create_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new PhieuHoanTraCreateRequest
            {
                HoaDonChiTietId = Guid.NewGuid(),
                SoLuongHoan = 1,
                LyDoHoanTra = "Sản phẩm bị lỗi"
            };

            _mockService.Setup(x => x.CreateAsync(request)).ReturnsAsync(false);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be("Tạo phiếu hoàn trả thất bại");
        }

        [Fact]
        public async Task PHT007_Update_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var phieuId = Guid.NewGuid();
            var request = new PhieuHoanTraUpdateRequest
            {
                SoLuongHoan = 2,
                LyDoHoanTra = "Sản phẩm bị lỗi",
                TrangThai = 1
            };

            _mockService.Setup(x => x.UpdateAsync(phieuId, request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(phieuId, request);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task PHT008_Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var phieuId = Guid.NewGuid();
            var request = new PhieuHoanTraUpdateRequest
            {
                SoLuongHoan = 2,
                LyDoHoanTra = "Sản phẩm bị lỗi",
                TrangThai = 1
            };

            _mockService.Setup(x => x.UpdateAsync(phieuId, request)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(phieuId, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PHT009_Delete_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var phieuId = Guid.NewGuid();
            _mockService.Setup(x => x.DeleteAsync(phieuId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(phieuId);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task PHT010_Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var phieuId = Guid.NewGuid();
            _mockService.Setup(x => x.DeleteAsync(phieuId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(phieuId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}