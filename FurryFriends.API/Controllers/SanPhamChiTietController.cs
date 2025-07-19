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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAndReturnAsync(dto);
            if (created == null)
                return StatusCode(500, "Tạo sản phẩm chi tiết thất bại.");

            return Ok(created);
        }

        // PUT: api/SanPhamChiTiet/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SanPhamChiTietDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound(new { message = "Không tìm thấy sản phẩm chi tiết." });

            return Ok(new { message = "Cập nhật thành công" , SanPhamChiTietId = success });
        }

        // DELETE: api/SanPhamChiTiet/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy sản phẩm chi tiết để xóa." });

            return Ok(new { message = "Xóa thành công" , SanPhamChiTietId =success });
        }
    }
}
