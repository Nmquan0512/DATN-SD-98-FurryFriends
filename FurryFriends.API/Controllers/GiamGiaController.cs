using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiamGiaController : ControllerBase
    {
        private readonly IGiamGiaService _giamGiaService;
        private readonly ILogger<GiamGiaController> _logger; // Thêm logger để ghi lại lỗi

        public GiamGiaController(IGiamGiaService giamGiaService, ILogger<GiamGiaController> logger)
        {
            _giamGiaService = giamGiaService;
            _logger = logger;
        }

        // GET: api/GiamGia
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GiamGiaDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var discounts = await _giamGiaService.GetAllAsync();
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi lấy danh sách chương trình giảm giá.");
                return StatusCode(500, "Lỗi hệ thống khi lấy danh sách giảm giá. Vui lòng thử lại sau.");
            }
        }

        // GET: api/GiamGia/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GiamGiaDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var discount = await _giamGiaService.GetByIdAsync(id);
                if (discount == null)
                {
                    return NotFound($"Không tìm thấy chương trình giảm giá với ID: {id}");
                }
                return Ok(discount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi lấy chi tiết giảm giá với ID: {DiscountId}", id);
                return StatusCode(500, "Lỗi hệ thống khi lấy thông tin giảm giá. Vui lòng thử lại sau.");
            }
        }

        // POST: api/GiamGia
        [HttpPost]
        [ProducesResponseType(typeof(GiamGiaDTO), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] GiamGiaDTO dto)
        {
            // ModelState.IsValid đã được xử lý tự động bởi [ApiController]

            try
            {
                var createdDiscount = await _giamGiaService.CreateAsync(dto);
                // Trả về đối tượng vừa tạo cùng với URL để truy cập nó
                return CreatedAtAction(nameof(GetById), new { id = createdDiscount.GiamGiaId }, createdDiscount);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // Thường dùng cho lỗi nghiệp vụ, ví dụ: tên bị trùng
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi tạo mới chương trình giảm giá.");
                return StatusCode(500, "Lỗi hệ thống khi tạo giảm giá. Vui lòng thử lại sau.");
            }
        }

        // PUT: api/GiamGia/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)] // Thành công không trả về nội dung
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiamGiaDTO dto)
        {
            if (id != dto.GiamGiaId)
            {
                return BadRequest("ID trong URL và ID trong body không khớp.");
            }

            // ModelState.IsValid đã được xử lý tự động bởi [ApiController]

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
                _logger.LogError(ex, "Lỗi nghiêm trọng khi cập nhật giảm giá với ID: {DiscountId}", id);
                return StatusCode(500, "Lỗi hệ thống khi cập nhật giảm giá. Vui lòng thử lại sau.");
            }
        }

        // DELETE: api/GiamGia/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _giamGiaService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound($"Không tìm thấy chương trình giảm giá với ID: {id} để xóa.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi xóa giảm giá với ID: {DiscountId}", id);
                return StatusCode(500, "Lỗi hệ thống khi xóa giảm giá. Vui lòng thử lại sau.");
            }
        }
    }
}