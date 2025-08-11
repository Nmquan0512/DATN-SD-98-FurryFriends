using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiamGiaController : ControllerBase
    {
        private readonly IGiamGiaService _giamGiaService;

        public GiamGiaController(IGiamGiaService giamGiaService)
        {
            _giamGiaService = giamGiaService;
        }

        // GET: api/GiamGia
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var giamGias = await _giamGiaService.GetAllAsync();
            return Ok(giamGias);
        }

        // GET: api/GiamGia/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null)
                return NotFound("Không tìm thấy mã giảm giá");

            return Ok(giamGia);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiamGiaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _giamGiaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.GiamGiaId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiamGiaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.GiamGiaId)
                return BadRequest("ID không khớp với DTO");

            var updated = await _giamGiaService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound("Không tìm thấy mã giảm giá để cập nhật");

<<<<<<< Updated upstream
            return Ok(updated);
=======
            try
            {
                await _giamGiaService.UpdateAsync(dto);
                return NoContent(); // HTTP 204: Yêu cầu đã được thực hiện thành công nhưng không có nội dung để trả về.
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // Tên bị trùng
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi cập nhật giảm giá với ID: {DiscountId}. Lỗi chi tiết: {ErrorMessage}", id, ex.Message);
                return StatusCode(500, $"Lỗi hệ thống khi cập nhật giảm giá: {ex.Message}");
            }
>>>>>>> Stashed changes
        }


        // POST: api/GiamGia/{id}/assign-sanphamchitiet
        [HttpPost("{id}/assign-sanphamchitiet")]
        public async Task<IActionResult> AssignSanPhamChiTiet(Guid id, [FromBody] List<Guid> sanPhamChiTietIds)
        {
            if (sanPhamChiTietIds == null || !sanPhamChiTietIds.Any())
                return BadRequest("Danh sách sản phẩm chi tiết không được rỗng.");

            var result = await _giamGiaService.AddSanPhamChiTietToGiamGiaAsync(id, sanPhamChiTietIds);
            if (!result)
                return BadRequest("Không thể gán sản phẩm chi tiết vào đợt giảm giá");

            return Ok("Gán sản phẩm chi tiết thành công");
        }
    }
}
