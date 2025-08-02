using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DotGiamGiaController : Controller
    {
        private readonly IDotGiamGiaSanPhamRepository _repo;

        public DotGiamGiaController(IDotGiamGiaSanPhamRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repo.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DotGiamGiaSanPham dot)
        {
            dot.DotGiamGiaSanPhamId = Guid.NewGuid();
            dot.NgayTao = DateTime.UtcNow;
            dot.NgayCapNhat = DateTime.UtcNow;

            await _repo.AddAsync(dot);
            return CreatedAtAction(nameof(GetById), new { id = dot.DotGiamGiaSanPhamId }, dot);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DotGiamGiaSanPham updated)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.GiamGiaId = updated.GiamGiaId;
            existing.SanPhamId = updated.SanPhamId;
            existing.PhanTramGiamGia = updated.PhanTramGiamGia;
            existing.TrangThai = updated.TrangThai;
            existing.NgayCapNhat = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _repo.DeleteAsync(id);
            return NoContent();
        }

    }
}