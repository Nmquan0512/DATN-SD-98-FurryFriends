using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamsController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        // GET: api/SanPham
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

        // GET: api/SanPham/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var sp = await _sanPhamService.GetByIdAsync(id);
            if (sp == null)
                return NotFound("Không tìm thấy sản phẩm!");

            return Ok(sp);
        }

        // POST: api/SanPham
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

        // PUT: api/SanPham/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SanPhamDTO dto)
        {
            try
            {
                var updated = await _sanPhamService.UpdateAsync(id, dto);
                if (!updated)
                    return NotFound("Không tìm thấy sản phẩm!");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            }
        }

        // DELETE: api/SanPham/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _sanPhamService.DeleteAsync(id);
                if (!deleted)
                    return NotFound("Không tìm thấy sản phẩm!");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa sản phẩm: {ex.Message}");
            }
        }

        // GET: api/SanPham/filter?loai=DoAn&page=1&pageSize=10
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

                var response = new
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    Items = data
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lọc/phân trang sản phẩm: {ex.Message}");
            }
        }

        // GET: api/SanPham/total
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalProducts()
        {
            var total = await _sanPhamService.GetTotalProductsAsync();
            return Ok(total);
        }

        // GET: api/SanPham/top-selling?top=5
        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] int top = 5)
        {
            var result = await _sanPhamService.GetTopSellingProductsAsync(top);
            return Ok(result);
        }
    }
}
