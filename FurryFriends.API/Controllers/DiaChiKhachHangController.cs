using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaChiKhachHangController : ControllerBase
    {
        private readonly IDiaChiKhachHangRepository _repository;

        public DiaChiKhachHangController(IDiaChiKhachHangRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var diaChi = await _repository.GetByIdAsync(id);
            if (diaChi == null) return NotFound();
            return Ok(diaChi);
        }

        [HttpGet("khachhang/{khachHangId}")]
        public async Task<IActionResult> GetByKhachHangId(Guid khachHangId)
        {
            var result = await _repository.GetByKhachHangIdAsync(khachHangId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiaChiKhachHang diaChi)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            diaChi.NgayTao = DateTime.UtcNow;
            diaChi.NgayCapNhat = DateTime.UtcNow;
            await _repository.AddAsync(diaChi);
            return CreatedAtAction(nameof(GetById), new { id = diaChi.DiaChiId }, diaChi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DiaChiKhachHang diaChi)
        {
            if (id != diaChi.DiaChiId) return BadRequest();
            diaChi.NgayCapNhat = DateTime.UtcNow;
            await _repository.UpdateAsync(diaChi);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
