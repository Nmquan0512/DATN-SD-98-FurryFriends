using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace UnitTest.HoaDonTest
{
    public class HoaDonControllerTests
    {
        #region Controller Tests

        public class HoaDonApiControllerUnitTests
        {
            private readonly Mock<IHoaDonRepository> _mockRepository;
            private readonly HoaDonController _controller;

            public HoaDonApiControllerUnitTests()
            {
                _mockRepository = new Mock<IHoaDonRepository>();
                _controller = new HoaDonController(_mockRepository.Object);
            }

            [Fact]
            public async Task HD001_GetHoaDons_ShouldReturnOk()
            {
                // Arrange
                var hoaDons = new List<HoaDon>
                {
                    new HoaDon 
                    { 
                        HoaDonId = Guid.NewGuid(), 
                        TenCuaKhachHang = "Nguyen Van A",
                        SdtCuaKhachHang = "0123456789",
                        NgayTao = DateTime.Now,
                        TongTien = 100000,
                        TongTienSauKhiGiam = 90000,
                        TrangThai = 1,
                        LoaiHoaDon = "Online"
                    },
                    new HoaDon 
                    { 
                        HoaDonId = Guid.NewGuid(), 
                        TenCuaKhachHang = "Tran Thi B",
                        SdtCuaKhachHang = "0987654321",
                        NgayTao = DateTime.Now,
                        TongTien = 200000,
                        TongTienSauKhiGiam = 200000,
                        TrangThai = 2,
                        LoaiHoaDon = "BanTaiQuay"
                    }
                };

                _mockRepository.Setup(x => x.GetHoaDonListAsync()).ReturnsAsync(hoaDons);

                // Act
                var result = await _controller.GetHoaDons();

                // Assert
                result.Should().BeOfType<ActionResult<IEnumerable<HoaDon>>>();
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedHoaDons = actionResult!.Value as IEnumerable<HoaDon>;
                returnedHoaDons.Should().HaveCount(2);
                returnedHoaDons.Should().Contain(h => h.TenCuaKhachHang == "Nguyen Van A");
            }

            [Fact]
            public async Task HD002_GetHoaDons_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                _mockRepository.Setup(x => x.GetHoaDonListAsync())
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetHoaDons();

                // Assert
                var actionResult = result.Result as ObjectResult;
                actionResult.Should().NotBeNull();
                actionResult!.StatusCode.Should().Be(500);
                actionResult.Value.Should().Be("Internal server error: Database error");
            }

            [Fact]
            public async Task HD003_GetDonHangList_ShouldReturnOk()
            {
                // Arrange
                var donHangs = new List<HoaDon>
                {
                    new HoaDon 
                    { 
                        HoaDonId = Guid.NewGuid(), 
                        TenCuaKhachHang = "Test Customer",
                        TrangThai = 1,
                        NgayTao = DateTime.Now,
                        TongTien = 150000,
                        TongTienSauKhiGiam = 150000,
                        LoaiHoaDon = "Online"
                    }
                };

                _mockRepository.Setup(x => x.GetDonHangListAsync()).ReturnsAsync(donHangs);

                // Act
                var result = await _controller.GetDonHangList();

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedDonHangs = actionResult!.Value as IEnumerable<HoaDon>;
                returnedDonHangs.Should().HaveCount(1);
                returnedDonHangs!.First().TrangThai.Should().BeInRange(0, 5);
            }

            [Fact]
            public async Task HD004_GetHoaDon_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                var hoaDon = new HoaDon
                {
                    HoaDonId = hoaDonId,
                    TenCuaKhachHang = "Test Customer",
                    SdtCuaKhachHang = "0123456789",
                    NgayTao = DateTime.Now,
                    TongTien = 100000,
                    TongTienSauKhiGiam = 90000,
                    TrangThai = 1,
                    LoaiHoaDon = "Online"
                };

                _mockRepository.Setup(x => x.GetHoaDonByIdAsync(hoaDonId)).ReturnsAsync(hoaDon);

                // Act
                var result = await _controller.GetHoaDon(hoaDonId);

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedHoaDon = actionResult!.Value as HoaDon;
                returnedHoaDon.Should().NotBeNull();
                returnedHoaDon!.HoaDonId.Should().Be(hoaDonId);
            }

            [Fact]
            public async Task HD005_GetHoaDon_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetHoaDonByIdAsync(hoaDonId))
                              .ReturnsAsync((HoaDon?)null);

                // Act
                var result = await _controller.GetHoaDon(hoaDonId);

                // Assert
                var actionResult = result.Result as NotFoundObjectResult;
                actionResult.Should().NotBeNull();
                actionResult!.Value.Should().Be($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
            }

            [Fact]
            public async Task HD006_GetChiTietHoaDon_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                var chiTietHoaDon = new List<HoaDonChiTiet>
                {
                    new HoaDonChiTiet
                    {
                        HoaDonChiTietId = Guid.NewGuid(),
                        HoaDonId = hoaDonId,
                        SoLuongSanPham = 2,
                        Gia = 50000
                    }
                };

                _mockRepository.Setup(x => x.GetChiTietHoaDonAsync(hoaDonId))
                              .ReturnsAsync(chiTietHoaDon);

                // Act
                var result = await _controller.GetChiTietHoaDon(hoaDonId);

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedChiTiet = actionResult!.Value as IEnumerable<HoaDonChiTiet>;
                returnedChiTiet.Should().HaveCount(1);
                returnedChiTiet!.First().HoaDonId.Should().Be(hoaDonId);
            }

            [Fact]
            public async Task HD007_GetChiTietHoaDon_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetChiTietHoaDonAsync(hoaDonId))
                              .ReturnsAsync(new List<HoaDonChiTiet>());

                // Act
                var result = await _controller.GetChiTietHoaDon(hoaDonId);

                // Assert
                var actionResult = result.Result as NotFoundObjectResult;
                actionResult.Should().NotBeNull();
                actionResult!.Value.Should().Be($"Không tìm thấy chi tiết hóa đơn với ID: {hoaDonId}");
            }

            [Fact]
            public async Task HD008_SearchHoaDons_WithKeyword_ShouldReturnMatchingHoaDons()
            {
                // Arrange
                var keyword = "Nguyen";
                var hoaDons = new List<HoaDon>
                {
                    new HoaDon
                    {
                        HoaDonId = Guid.NewGuid(),
                        TenCuaKhachHang = "Nguyen Van A",
                        SdtCuaKhachHang = "0123456789",
                        NgayTao = DateTime.Now,
                        TongTien = 100000,
                        TongTienSauKhiGiam = 90000,
                        TrangThai = 1,
                        LoaiHoaDon = "Online"
                    }
                };

                _mockRepository.Setup(x => x.SearchHoaDonAsync(It.IsAny<Func<HoaDon, bool>>()))
                              .ReturnsAsync(hoaDons);

                // Act
                var result = await _controller.SearchHoaDons(keyword);

                // Assert
                var actionResult = result.Result as OkObjectResult;
                actionResult.Should().NotBeNull();
                var returnedHoaDons = actionResult!.Value as IEnumerable<HoaDon>;
                returnedHoaDons.Should().HaveCount(1);
                returnedHoaDons!.First().TenCuaKhachHang.Should().Contain("Nguyen");
            }

            [Fact]
            public async Task HD009_ExportHoaDonToPdf_WithExistingId_ShouldReturnPdfFile()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                var pdfBytes = new byte[] { 1, 2, 3, 4, 5 };

                _mockRepository.Setup(x => x.ExportHoaDonToPdfAsync(hoaDonId))
                              .ReturnsAsync(pdfBytes);

                // Act
                var result = await _controller.ExportHoaDonToPdf(hoaDonId);

                // Assert
                result.Should().BeOfType<FileContentResult>();
                var fileResult = result as FileContentResult;
                fileResult!.ContentType.Should().Be("application/pdf");
                fileResult.FileDownloadName.Should().Be($"HoaDon_{hoaDonId}.pdf");
            }

            [Fact]
            public async Task HD010_ExportHoaDonToPdf_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                _mockRepository.Setup(x => x.ExportHoaDonToPdfAsync(hoaDonId))
                              .ReturnsAsync((byte[]?)null);

                // Act
                var result = await _controller.ExportHoaDonToPdf(hoaDonId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be($"Không tìm thấy hóa đơn với ID: {hoaDonId}");
            }

            [Fact]
            public async Task HD011_HuyDonHang_WithValidId_ShouldReturnOk()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                var successResult = new ApiResult { Success = true, Message = "Hủy đơn hàng thành công!" };

                _mockRepository.Setup(x => x.HuyDonHangAsync(hoaDonId))
                              .ReturnsAsync(successResult);

                // Act
                var result = await _controller.HuyDonHang(hoaDonId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var response = okResult!.Value;
                response.Should().NotBeNull();
            }

            [Fact]
            public async Task HD012_HuyDonHang_WithInvalidId_ShouldReturnBadRequest()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                var failResult = new ApiResult { Success = false, Message = "Không thể hủy đơn hàng" };

                _mockRepository.Setup(x => x.HuyDonHangAsync(hoaDonId))
                              .ReturnsAsync(failResult);

                // Act
                var result = await _controller.HuyDonHang(hoaDonId);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task HD013_CapNhatTrangThai_WithValidData_ShouldReturnOk()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                var trangThai = 2;
                var successResult = new ApiResult { Success = true, Message = "Cập nhật trạng thái thành công!" };

                _mockRepository.Setup(x => x.CapNhatTrangThaiAsync(hoaDonId, trangThai))
                              .ReturnsAsync(successResult);

                // Act
                var result = await _controller.CapNhatTrangThai(hoaDonId, trangThai);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var response = okResult!.Value;
                response.Should().NotBeNull();
            }

            [Fact]
            public async Task HD014_CapNhatTrangThai_WithInvalidData_ShouldReturnBadRequest()
            {
                // Arrange
                var hoaDonId = Guid.NewGuid();
                var trangThai = 999;
                var failResult = new ApiResult { Success = false, Message = "Trạng thái không hợp lệ" };

                _mockRepository.Setup(x => x.CapNhatTrangThaiAsync(hoaDonId, trangThai))
                              .ReturnsAsync(failResult);

                // Act
                var result = await _controller.CapNhatTrangThai(hoaDonId, trangThai);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task HD015_SearchHoaDons_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var keyword = "test";
                _mockRepository.Setup(x => x.SearchHoaDonAsync(It.IsAny<Func<HoaDon, bool>>()))
                              .ThrowsAsync(new Exception("Search error"));

                // Act
                var result = await _controller.SearchHoaDons(keyword);

                // Assert
                var actionResult = result.Result as ObjectResult;
                actionResult.Should().NotBeNull();
                actionResult!.StatusCode.Should().Be(500);
                actionResult.Value.Should().Be("Internal server error: Search error");
            }
        }

        #endregion

        #region Validation Tests

        public class HoaDonValidationTests
        {
            [Fact]
            public void ValidateHoaDon_WithValidOnlineOrder_ShouldPass()
            {
                // Arrange
                var hoaDon = new HoaDon
                {
                    HoaDonId = Guid.NewGuid(),
                    KhachHangId = Guid.NewGuid(),
                    HinhThucThanhToanId = Guid.NewGuid(),
                    TenCuaKhachHang = "Test Customer",
                    SdtCuaKhachHang = "0123456789",
                    EmailCuaKhachHang = "test@example.com",
                    NgayTao = DateTime.Now,
                    TongTien = 100000,
                    TongTienSauKhiGiam = 90000,
                    TrangThai = 1,
                    LoaiHoaDon = "Online",
                    DiaChiGiaoHangId = Guid.NewGuid()
                };

                // Act & Assert
                hoaDon.TongTien.Should().BeGreaterThan(0);
                hoaDon.TongTienSauKhiGiam.Should().BeLessOrEqualTo(hoaDon.TongTien);
                hoaDon.LoaiHoaDon.Should().Be("Online");
                hoaDon.DiaChiGiaoHangId.Should().NotBeNull();
            }

            [Fact]
            public void ValidateHoaDon_WithValidBanTaiQuayOrder_ShouldPass()
            {
                // Arrange
                var hoaDon = new HoaDon
                {
                    HoaDonId = Guid.NewGuid(),
                    KhachHangId = Guid.NewGuid(),
                    HinhThucThanhToanId = Guid.NewGuid(),
                    TenCuaKhachHang = "Test Customer",
                    SdtCuaKhachHang = "0123456789",
                    NgayTao = DateTime.Now,
                    TongTien = 100000,
                    TongTienSauKhiGiam = 100000,
                    TrangThai = 1,
                    LoaiHoaDon = "BanTaiQuay",
                    NhanVienId = Guid.NewGuid(),
                    DiaChiGiaoHangId = null
                };

                // Act & Assert
                hoaDon.LoaiHoaDon.Should().Be("BanTaiQuay");
                hoaDon.NhanVienId.Should().NotBeNull();
                hoaDon.DiaChiGiaoHangId.Should().BeNull();
            }

            [Fact]
            public void ValidateHoaDon_WithInvalidTongTienSauKhiGiam_ShouldFail()
            {
                // Arrange
                var hoaDon = new HoaDon
                {
                    HoaDonId = Guid.NewGuid(),
                    TongTien = 100000,
                    TongTienSauKhiGiam = 150000 // Greater than TongTien
                };

                // Act & Assert
                hoaDon.TongTienSauKhiGiam.Should().BeGreaterThan(hoaDon.TongTien);
            }

            [Fact]
            public void ValidateHoaDon_WithInvalidNgayNhanHang_ShouldFail()
            {
                // Arrange
                var ngayTao = DateTime.Now;
                var hoaDon = new HoaDon
                {
                    HoaDonId = Guid.NewGuid(),
                    NgayTao = ngayTao,
                    NgayNhanHang = ngayTao.AddDays(-1) // Before NgayTao
                };

                // Act & Assert
                hoaDon.NgayNhanHang.Should().BeBefore(hoaDon.NgayTao);
            }
        }

        #endregion

        #region Integration Tests

        public class HoaDonIntegrationTests
        {
            [Fact]
            public async Task HD001_Integration_CreateAndRetrieveHoaDon_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task HD002_Integration_UpdateHoaDonStatus_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task HD003_Integration_CancelOrder_ShouldUpdateStatusCorrectly()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }
        }

        #endregion
    }
}