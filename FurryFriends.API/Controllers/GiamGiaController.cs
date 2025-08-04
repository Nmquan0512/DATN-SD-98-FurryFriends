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

        public GiamGiaController(IGiamGiaService giamGiaService)
        {
            _giamGiaService = giamGiaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiamGiaDTO>>> GetAll()
        {
            try
            {
                var discounts = await _giamGiaService.GetAllAsync();
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi lấy danh sách giảm giá");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GiamGiaDTO>> GetById(Guid id)
        {
            try
            {
                var discount = await _giamGiaService.GetByIdAsync(id);
                if (discount == null)
                {
                    return NotFound();
                }
                return Ok(discount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi lấy thông tin giảm giá");
            }
        }

        [HttpPost]
        public async Task<ActionResult<GiamGiaDTO>> Create([FromBody] GiamGiaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdDiscount = await _giamGiaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDiscount.GiamGiaId }, createdDiscount);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi tạo giảm giá");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiamGiaDTO dto)
        {
            try
            {
                if (id != dto.GiamGiaId)
                {
                    return BadRequest("ID không khớp");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _giamGiaService.UpdateAsync(dto);
                if (result == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi cập nhật giảm giá");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _giamGiaService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi xóa giảm giá");
            }
        }

        [HttpPost("{id}/assign-products")]
        public async Task<IActionResult> AssignProducts(Guid id, [FromBody] List<Guid> productIds)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                {
                    return BadRequest("Danh sách sản phẩm không được rỗng");
                }

                var result = await _giamGiaService.AssignProductsAsync(id, productIds);
                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi gán sản phẩm vào chương trình giảm giá");
            }
        }
    }
}