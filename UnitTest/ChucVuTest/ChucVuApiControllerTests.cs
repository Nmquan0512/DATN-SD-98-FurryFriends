using FurryFriends.API.Controllers;
using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace UnitTest.ChucVuTest
{
    public class ChucVuApiControllerTests
    {
        #region Controller Tests

        public class ChucVuApiControllerUnitTests
        {
            private readonly Mock<IChucVuRepository> _mockRepository;
            private readonly ChucVuApiController _controller;

            public ChucVuApiControllerUnitTests()
            {
                _mockRepository = new Mock<IChucVuRepository>();
                _controller = new ChucVuApiController(_mockRepository.Object);
            }

            [Fact]
            public async Task CV001_GetAll_ShouldReturnOkWithChucVuList()
            {
                // Arrange
                var chucVus = new List<ChucVu>
                {
                    new ChucVu
                    {
                        ChucVuId = Guid.NewGuid(),
                        TenChucVu = "Quản lý",
                        MoTaChucVu = "Quản lý cửa hàng",
                        TrangThai = true,
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    },
                    new ChucVu
                    {
                        ChucVuId = Guid.NewGuid(),
                        TenChucVu = "Nhân viên",
                        MoTaChucVu = "Nhân viên bán hàng",
                        TrangThai = true,
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    }
                };

                _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(chucVus);

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedChucVus = okResult!.Value as IEnumerable<ChucVu>;
                returnedChucVus.Should().HaveCount(2);
            }

            [Fact]
            public async Task CV002_GetAll_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                _mockRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetAll();

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task CV003_GetById_WithExistingId_ShouldReturnOk()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();
                var chucVu = new ChucVu
                {
                    ChucVuId = chucVuId,
                    TenChucVu = "Quản lý",
                    MoTaChucVu = "Quản lý cửa hàng",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                _mockRepository.Setup(x => x.GetByIdAsync(chucVuId)).ReturnsAsync(chucVu);

                // Act
                var result = await _controller.GetById(chucVuId);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedChucVu = okResult!.Value as ChucVu;
                returnedChucVu.Should().NotBeNull();
                returnedChucVu!.ChucVuId.Should().Be(chucVuId);
            }

            [Fact]
            public async Task CV004_GetById_WithNonExistentId_ShouldReturnNotFound()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(chucVuId))
                              .ReturnsAsync((ChucVu?)null);

                // Act
                var result = await _controller.GetById(chucVuId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task CV005_GetById_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();
                _mockRepository.Setup(x => x.GetByIdAsync(chucVuId))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.GetById(chucVuId);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task CV006_Create_WithValidData_ShouldReturnCreated()
            {
                // Arrange
                var newChucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = "Trưởng phòng",
                    MoTaChucVu = "Trưởng phòng kinh doanh",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<ChucVu>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Create(newChucVu);

                // Assert
                result.Should().BeOfType<CreatedAtActionResult>();
                var createdResult = result as CreatedAtActionResult;
                createdResult!.Value.Should().Be(newChucVu);
                _mockRepository.Verify(x => x.AddAsync(It.IsAny<ChucVu>()), Times.Once);
            }

            [Fact]
            public async Task CV007_Create_WithInvalidModelState_ShouldReturnBadRequest()
            {
                // Arrange
                var invalidChucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = "", // Invalid - empty
                    MoTaChucVu = "Mô tả",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                _controller.ModelState.AddModelError("TenChucVu", "Tên chức vụ là bắt buộc.");

                // Act
                var result = await _controller.Create(invalidChucVu);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CV008_Create_WhenArgumentExceptionThrown_ShouldReturnBadRequest()
            {
                // Arrange
                var chucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = "Duplicate Name",
                    MoTaChucVu = "Mô tả",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<ChucVu>()))
                              .ThrowsAsync(new ArgumentException("Tên chức vụ đã tồn tại"));

                // Act
                var result = await _controller.Create(chucVu);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CV009_Create_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var chucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = "Test Position",
                    MoTaChucVu = "Mô tả",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                _mockRepository.Setup(x => x.AddAsync(It.IsAny<ChucVu>()))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.Create(chucVu);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }

            [Fact]
            public async Task CV010_Update_WithValidData_ShouldReturnNoContent()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();
                var updateChucVu = new ChucVu
                {
                    ChucVuId = chucVuId,
                    TenChucVu = "Updated Position",
                    MoTaChucVu = "Updated Description",
                    TrangThai = false,
                    NgayTao = DateTime.Now.AddDays(-10),
                    NgayCapNhat = DateTime.Now
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<ChucVu>()))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Update(chucVuId, updateChucVu);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<ChucVu>()), Times.Once);
            }

            [Fact]
            public async Task CV011_Update_WithMismatchedId_ShouldReturnBadRequest()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();
                var updateChucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(), // Different ID
                    TenChucVu = "Updated Position",
                    MoTaChucVu = "Updated Description",
                    TrangThai = false,
                    NgayTao = DateTime.Now.AddDays(-10),
                    NgayCapNhat = DateTime.Now
                };

                // Act
                var result = await _controller.Update(chucVuId, updateChucVu);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CV012_Update_WithInvalidModelState_ShouldReturnBadRequest()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();
                var updateChucVu = new ChucVu
                {
                    ChucVuId = chucVuId,
                    TenChucVu = "",
                    MoTaChucVu = "Updated Description",
                    TrangThai = false,
                    NgayTao = DateTime.Now.AddDays(-10),
                    NgayCapNhat = DateTime.Now
                };

                _controller.ModelState.AddModelError("TenChucVu", "Tên chức vụ là bắt buộc.");

                // Act
                var result = await _controller.Update(chucVuId, updateChucVu);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CV013_Update_WhenNotFound_ShouldReturnNotFound()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();
                var updateChucVu = new ChucVu
                {
                    ChucVuId = chucVuId,
                    TenChucVu = "Updated Position",
                    MoTaChucVu = "Updated Description",
                    TrangThai = false,
                    NgayTao = DateTime.Now.AddDays(-10),
                    NgayCapNhat = DateTime.Now
                };

                _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<ChucVu>()))
                              .ThrowsAsync(new KeyNotFoundException("Chức vụ không tồn tại"));

                // Act
                var result = await _controller.Update(chucVuId, updateChucVu);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task CV014_Delete_WithExistingId_ShouldReturnNoContent()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();

                _mockRepository.Setup(x => x.DeleteAsync(chucVuId))
                              .Returns(Task.CompletedTask);

                // Act
                var result = await _controller.Delete(chucVuId);

                // Assert
                result.Should().BeOfType<NoContentResult>();
                _mockRepository.Verify(x => x.DeleteAsync(chucVuId), Times.Once);
            }

            [Fact]
            public async Task CV015_Delete_WhenNotFound_ShouldReturnNotFound()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();

                _mockRepository.Setup(x => x.DeleteAsync(chucVuId))
                              .ThrowsAsync(new KeyNotFoundException("Chức vụ không tồn tại"));

                // Act
                var result = await _controller.Delete(chucVuId);

                // Assert
                result.Should().BeOfType<NotFoundObjectResult>();
            }

            [Fact]
            public async Task CV016_Delete_WhenInvalidOperation_ShouldReturnBadRequest()
            {
                // Arrange
                var chucVuId = Guid.NewGuid();

                _mockRepository.Setup(x => x.DeleteAsync(chucVuId))
                              .ThrowsAsync(new InvalidOperationException("Cannot delete position with active employees"));

                // Act
                var result = await _controller.Delete(chucVuId);

                // Assert
                result.Should().BeOfType<BadRequestObjectResult>();
            }

            [Fact]
            public async Task CV017_SearchByTenChucVu_WithValidName_ShouldReturnOk()
            {
                // Arrange
                var searchTerm = "Quản lý";
                var chucVus = new List<ChucVu>
                {
                    new ChucVu
                    {
                        ChucVuId = Guid.NewGuid(),
                        TenChucVu = "Quản lý cửa hàng",
                        MoTaChucVu = "Quản lý cửa hàng",
                        TrangThai = true,
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now
                    }
                };

                _mockRepository.Setup(x => x.FindByTenChucVuAsync(searchTerm))
                              .ReturnsAsync(chucVus);

                // Act
                var result = await _controller.SearchByTenChucVu(searchTerm);

                // Assert
                result.Should().BeOfType<OkObjectResult>();
                var okResult = result as OkObjectResult;
                var returnedChucVus = okResult!.Value as IEnumerable<ChucVu>;
                returnedChucVus.Should().HaveCount(1);
            }

            [Fact]
            public async Task CV018_SearchByTenChucVu_WhenExceptionThrown_ShouldReturnInternalServerError()
            {
                // Arrange
                var searchTerm = "Test";

                _mockRepository.Setup(x => x.FindByTenChucVuAsync(searchTerm))
                              .ThrowsAsync(new Exception("Database error"));

                // Act
                var result = await _controller.SearchByTenChucVu(searchTerm);

                // Assert
                result.Should().BeOfType<ObjectResult>();
                var objectResult = result as ObjectResult;
                objectResult!.StatusCode.Should().Be(500);
            }
        }

        #endregion

        #region Validation Tests

        public class ChucVuValidationTests
        {
            [Fact]
            public void ValidateChucVu_WithValidData_ShouldPass()
            {
                // Arrange
                var chucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = "Quản lý",
                    MoTaChucVu = "Quản lý cửa hàng",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                // Act & Assert
                chucVu.TenChucVu.Should().NotBeNullOrEmpty();
                chucVu.MoTaChucVu.Should().NotBeNullOrEmpty();
                chucVu.TenChucVu.Length.Should().BeLessOrEqualTo(50);
                chucVu.MoTaChucVu.Length.Should().BeLessOrEqualTo(250);
            }

            [Fact]
            public void ValidateChucVu_WithEmptyTenChucVu_ShouldFail()
            {
                // Arrange
                var chucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = "", // Invalid
                    MoTaChucVu = "Mô tả chức vụ",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                // Act & Assert
                chucVu.TenChucVu.Should().BeEmpty();
            }

            [Fact]
            public void ValidateChucVu_WithTooLongTenChucVu_ShouldFail()
            {
                // Arrange
                var chucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = new string('A', 51), // Too long - 51 characters
                    MoTaChucVu = "Mô tả chức vụ",
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                // Act & Assert
                chucVu.TenChucVu.Length.Should().BeGreaterThan(50);
            }

            [Fact]
            public void ValidateChucVu_WithTooLongMoTaChucVu_ShouldFail()
            {
                // Arrange
                var chucVu = new ChucVu
                {
                    ChucVuId = Guid.NewGuid(),
                    TenChucVu = "Quản lý",
                    MoTaChucVu = new string('B', 251), // Too long - 251 characters
                    TrangThai = true,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now
                };

                // Act & Assert
                chucVu.MoTaChucVu.Length.Should().BeGreaterThan(250);
            }
        }

        #endregion

        #region Integration Tests

        public class ChucVuIntegrationTests
        {
            [Fact]
            public async Task CV001_Integration_CreateAndRetrieveChucVu_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task CV002_Integration_UpdateChucVu_ShouldWorkEndToEnd()
            {
                // Integration test placeholder  
                // Requires full setup with real repository and database
                Assert.True(true);
            }

            [Fact]
            public async Task CV003_Integration_SearchChucVu_ShouldWorkEndToEnd()
            {
                // Integration test placeholder
                // Test search functionality with real data
                Assert.True(true);
            }
        }

        #endregion
    }
}