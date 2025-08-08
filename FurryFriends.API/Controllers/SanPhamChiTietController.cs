using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq; // Added for LastOrDefault

namespace FurryFriends.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanPhamChiTietController : ControllerBase
    {
        private readonly ISanPhamChiTietService _service;

        public SanPhamChiTietController(ISanPhamChiTietService service)
        {
            _service = service;
        }

        // GET: api/SanPhamChiTiet
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/SanPhamChiTiet/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/SanPhamChiTiet
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SanPhamChiTietDTO dto)
        {
            // Manual validation
            if (dto.MauSacId == Guid.Empty)
            {
                ModelState.AddModelError("MauSacId", "Màu sắc không được để trống");
            }

            if (dto.KichCoId == Guid.Empty)
            {
                ModelState.AddModelError("KichCoId", "Kích cỡ không được để trống");
            }

            if (dto.Gia <= 0)
            {
                ModelState.AddModelError("Gia", "Giá bán phải là số dương");
            }

            if (dto.SoLuong < 0)
            {
                ModelState.AddModelError("SoLuong", "Số lượng phải là số dương hoặc bằng 0");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAndReturnAsync(dto);
                if (created == null)
                    return StatusCode(500, "Tạo sản phẩm chi tiết thất bại.");

                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo sản phẩm chi tiết: {ex.Message}");
            }
        }

        // PUT: api/SanPhamChiTiet/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SanPhamChiTietDTO dto)
        {
            // Manual validation
            if (dto.MauSacId == Guid.Empty)
            {
                ModelState.AddModelError("MauSacId", "Màu sắc không được để trống");
            }

            if (dto.KichCoId == Guid.Empty)
            {
                ModelState.AddModelError("KichCoId", "Kích cỡ không được để trống");
            }

            if (dto.Gia <= 0)
            {
                ModelState.AddModelError("Gia", "Giá bán phải là số dương");
            }

            if (dto.SoLuong < 0)
            {
                ModelState.AddModelError("SoLuong", "Số lượng phải là số dương hoặc bằng 0");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _service.UpdateAsync(id, dto);
                if (!success)
                    return NotFound();

                return Ok(new { message = "Cập nhật thành công", SanPhamChiTietId = id });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật sản phẩm chi tiết: {ex.Message}");
            }
        }

        // DELETE: api/SanPhamChiTiet/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success)
                    return NotFound();

                return Ok(new { message = "Xóa thành công", SanPhamChiTietId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa sản phẩm chi tiết: {ex.Message}");
            }
        }
    }
}
