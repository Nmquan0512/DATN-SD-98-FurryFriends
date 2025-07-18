using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanPhamsController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamsController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var sanPhams = await _sanPhamService.GetAllAsync();
                return Ok(sanPhams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var sp = await _sanPhamService.GetByIdAsync(id);
                return Ok(sp);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SanPhamDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _sanPhamService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.SanPhamId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Không thể tạo sản phẩm: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SanPhamDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _sanPhamService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _sanPhamService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa sản phẩm: {ex.Message}");
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] string? loai,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page và pageSize phải lớn hơn 0");

            try
            {
                var (data, total) = await _sanPhamService.GetFilteredAsync(loai, page, pageSize);

                return Ok(new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    Items = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lọc sản phẩm: {ex.Message}");
            }
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalProducts()
        {
            try
            {
                var total = await _sanPhamService.GetTotalProductsAsync();
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy tổng số sản phẩm: {ex.Message}");
            }
        }

        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] int top = 5)
        {
            try
            {
                var result = await _sanPhamService.GetTopSellingProductsAsync(top);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy sản phẩm bán chạy: {ex.Message}");
            }
        }
    }
}