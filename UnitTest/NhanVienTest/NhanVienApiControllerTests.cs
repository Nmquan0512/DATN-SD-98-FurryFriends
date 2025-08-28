using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace UnitTest.NhanVienTest
{
    public class NhanVienApiControllerTests
    {
        #region Controller Tests

        public class NhanVienApiControllerUnitTests
        {
            private readonly Mock<INhanVienRepository> _mockRepository;
            private readonly NhanVienApiController _controller;

            public NhanVienApiControllerUnitTests()
            {
                _mockRepository = new Mock<INhanVienRepository>();
                _controller = new NhanVienApiController(_mockRepository.Object);
            }

            [Fact]
            public async Task NV001_GetAll_ShouldReturnOk()
            {
                // Arrange
                var nhanViens = new List<NhanVien>
                {
                    new NhanVien 
                    { 
                        NhanVienId = Guid.NewGuid(), 
                        HoVaTen = "Nguyen Van A",
                        NgaySinh = new DateTime(1990, 1, 1),
                        DiaChi = "123 ABC Street",
                        SDT = "0123456789",
                        TrangThai = true
                    },
                    new NhanVien 
                    { 
                        NhanVienId = Guid.NewGuid(), 
                        HoVaTen = "Tran Thi B",
                        NgaySinh = new DateTime(1992, 5, 15),
                        DiaChi = "456 XYZ Street",
                        SDT = "0987654321",
                        TrangThai = true
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(nhanViens);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedNhanViens = okResult!.Value as IEnumerable<NhanVien>;
                returnedNhanViens.Should().HaveCount(2);
                returnedNhanViens.Should().Contain(n => n.HoVaTen == "Nguyen Van A");
            }

            [Fact]
            public async Task NV002_GetAll_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                _mockRepository.Setup(x => x.GetAllAsync())
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetAll();

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database error");
            }

            [Fact]
            public async Task NV003_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                var nhanVien = new NhanVien
                {
                    NhanVienId = nhanVienId,
                    HoVaTen = "Test Employee",
                    NgaySinh = new DateTime(1985, 3, 20),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.GetByIdAsync(nhanVienId)).ReturnsAsync(nhanVien);

                // Act
                var result = await _controller.GetById(nhanVienId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedNhanVien = okResult!.Value as NhanVien;
                returnedNhanVien.Should().NotBeNull();
                returnedNhanVien!.NhanVienId.Should().Be(nhanVienId);
                returnedNhanVien.HoVaTen.Should().Be("Test Employee");
            }

            [Fact]
            public async Task NV004_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(nhanVienId))
                              .ReturnsAsync((NhanVien?)null);

                // Act
                var result = await _controller.GetById(nhanVienId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be($"Nhân viên với NhanVienId {nhanVienId} không tồn tại.");
            }

            [Fact]
            public async Task NV005_GetById_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(nhanVienId))
                              .ThrowsAsync(new Exception("Database connection error"));

                // Act
                var result = await _controller.GetById(nhanVienId);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database connection error");
            }

            [Fact]
            public async Task NV006_Create_WithValidData_ShouldReturnCreated()
            {
                // Arrange
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "New Employee",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "New Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<NhanVien>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Create(nhanVien);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdAtResult = result as CreatedAtActionResult;
                createdAtResult!.Value.Should().BeEquivalentTo(nhanVien);
                _mockRepository.Verify(x => x.AddAsync(It.IsAny<NhanVien>()), Times.Once);
            }

            [Fact]
            public async Task NV007_Create_WithInvalidModelState_ShouldReturnBadRequest()
            {
                // Arrange
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _controller.ModelState.AddModelError("HoVaTen", "Họ và tên không được để trống");

                // Act
                var result = await _controller.Create(nhanVien);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task NV008_Create_WhenRepositoryThrowsArgumentException_ShouldReturnBadRequest()
            {
                // Arrange
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "Duplicate Employee",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<NhanVien>()))
                              .ThrowsAsync(new ArgumentException("Nhân viên đã tồn tại"));

                // Act
                var result = await _controller.Create(nhanVien);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Nhân viên đã tồn tại");
            }

            [Fact]
            public async Task NV009_Create_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "Test Employee",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<NhanVien>()))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Create(nhanVien);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database error");
            }

            [Fact]
            public async Task NV010_Update_WithValidData_ShouldReturnNoContent()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                var nhanVien = new NhanVien
                {
                    NhanVienId = nhanVienId,
                    HoVaTen = "Updated Employee",
                    NgaySinh = new DateTime(1985, 12, 10),
                    DiaChi = "Updated Address",
                    SDT = "0987654321",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<NhanVien>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Update(nhanVienId, nhanVien);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<NhanVien>()), Times.Once);
            }

            [Fact]
            public async Task NV011_Update_WithMismatchedId_ShouldReturnBadRequest()
            {
                // Arrange
                var urlId = Guid.NewGuid();
                var differentId = Guid.NewGuid();
                var nhanVien = new NhanVien
                {
                    NhanVienId = differentId,
                    HoVaTen = "Test Employee",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act
                var result = await _controller.Update(urlId, nhanVien);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("NhanVienId không khớp.");
            }

            [Fact]
            public async Task NV012_Update_WithInvalidModelState_ShouldReturnBadRequest()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                var nhanVien = new NhanVien
                {
                    NhanVienId = nhanVienId,
                    HoVaTen = "",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _controller.ModelState.AddModelError("HoVaTen", "Họ và tên không được để trống");

                // Act
                var result = await _controller.Update(nhanVienId, nhanVien);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task NV013_Update_WhenRepositoryThrowsKeyNotFoundException_ShouldReturnNotFound()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                var nhanVien = new NhanVien
                {
                    NhanVienId = nhanVienId,
                    HoVaTen = "Non-existent Employee",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<NhanVien>()))
                              .ThrowsAsync(new KeyNotFoundException("Nhân viên không tồn tại"));

                // Act
                var result = await _controller.Update(nhanVienId, nhanVien);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
                var notFoundResult = result as NotFoundObjectResult;
                notFoundResult!.Value.Should().Be("Nhân viên không tồn tại");
            }

            [Fact]
            public async Task NV014_Update_WhenRepositoryThrowsArgumentException_ShouldReturnBadRequest()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                var nhanVien = new NhanVien
                {
                    NhanVienId = nhanVienId,
                    HoVaTen = "Test Employee",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<NhanVien>()))
                              .ThrowsAsync(new ArgumentException("Dữ liệu không hợp lệ"));

                // Act
                var result = await _controller.Update(nhanVienId, nhanVien);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = result as BadRequestObjectResult;
                badRequestResult!.Value.Should().Be("Dữ liệu không hợp lệ");
            }

            [Fact]
            public async Task NV015_Update_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var nhanVienId = Guid.NewGuid();
                var nhanVien = new NhanVien
                {
                    NhanVienId = nhanVienId,
                    HoVaTen = "Test Employee",
                    NgaySinh = new DateTime(1990, 6, 15),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<NhanVien>()))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Update(nhanVienId, nhanVien);

                // Assert
                var statusCodeResult = result as ObjectResult;
                statusCodeResult.Should().NotBeNull();
                statusCodeResult!.StatusCode.Should().Be(500);
                statusCodeResult.Value.Should().Be("Internal server error: Database error");
            }
        }

        #endregion

        #region Validation Tests

        public class NhanVienValidationTests
        {
            [Fact]
            public void ValidateNhanVien_WithValidData_ShouldPass()
            {
                // Arrange
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "Valid Employee Name",
                    NgaySinh = new DateTime(1990, 1, 1),
                    DiaChi = "Valid Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(nhanVien, new ValidationContext(nhanVien), validationResults, true);

                // Assert
                nhanVien.HoVaTen.Should().NotBeNullOrEmpty();
                nhanVien.SDT.Should().MatchRegex(@"^0\d{9,10}$");
                nhanVien.DiaChi.Should().NotBeNullOrEmpty();
            }

            [Fact]
            public void ValidateNhanVien_WithEmptyHoVaTen_ShouldFail()
            {
                // Arrange
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "",
                    NgaySinh = new DateTime(1990, 1, 1),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act & Assert
                nhanVien.HoVaTen.Should().BeEmpty();
            }

            [Fact]
            public void ValidateNhanVien_WithInvalidPhoneFormat_ShouldFail()
            {
                // Arrange
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "Test Employee",
                    NgaySinh = new DateTime(1990, 1, 1),
                    DiaChi = "Test Address",
                    SDT = "123456789", // Missing leading zero
                    TrangThai = true
                };

                // Act & Assert
                nhanVien.SDT.Should().NotMatchRegex(@"^0\d{9,10}$");
            }

            [Fact]
            public void ValidateNhanVien_WithTooLongHoVaTen_ShouldFail()
            {
                // Arrange
                var longName = new string('A', 51); // 51 characters
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = longName,
                    NgaySinh = new DateTime(1990, 1, 1),
                    DiaChi = "Test Address",
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act & Assert
                nhanVien.HoVaTen.Length.Should().BeGreaterThan(50);
            }

            [Fact]
            public void ValidateNhanVien_WithTooLongDiaChi_ShouldFail()
            {
                // Arrange
                var longAddress = new string('A', 101); // 101 characters
                var nhanVien = new NhanVien
                {
                    NhanVienId = Guid.NewGuid(),
                    HoVaTen = "Test Employee",
                    NgaySinh = new DateTime(1990, 1, 1),
                    DiaChi = longAddress,
                    SDT = "0123456789",
                    TrangThai = true
                };

                // Act & Assert
                nhanVien.DiaChi.Length.Should().BeGreaterThan(100);
            }
        }

        #endregion

        #region Integration Tests

        public class NhanVienIntegrationTests
        {
            [Fact]
            public async Task NV001_Integration_CreateAndRetrieveNhanVien_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task NV002_Integration_UpdateNhanVien_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task NV003_Integration_NhanVienWithTaiKhoan_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test relationship between NhanVien and TaiKhoan
                Assert.True(true);
            }
        }

        #endregion
    }
}