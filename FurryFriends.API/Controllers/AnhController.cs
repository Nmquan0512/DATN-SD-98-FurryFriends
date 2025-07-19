using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnhController : ControllerBase
    {
        private readonly IAnhService _service;

        public AnhController(IAnhService service)
        {
            _service = service;
        }

        // GET: api/Anh
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Anh/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/Anh/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid? sanPhamChiTietId = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file ảnh.");

            var result = await _service.UploadAsync(file, sanPhamChiTietId);
            if (result == null)
                return BadRequest("Không thể upload ảnh.");

            return Ok(result);
        }

        // PUT: api/Anh/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AnhDTO dto)
        {
            if (id != dto.AnhId)
                return BadRequest("ID không khớp.");

            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound("Không tìm thấy ảnh.");

            return Ok(new { Message = "Cập nhật thành công" });
        }

        // DELETE: api/Anh/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound("Không tìm thấy ảnh.");

            return Ok(new { Message = "Xóa thành công" });
        }

    }
}
