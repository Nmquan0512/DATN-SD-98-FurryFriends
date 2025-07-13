using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiamGiaController : Controller
    {
        private readonly IGiamGiaRepository _giamGiaRepo;

        public GiamGiaController(IGiamGiaRepository giamGiaRepo)
        {
            _giamGiaRepo = giamGiaRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _giamGiaRepo.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _giamGiaRepo.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiamGia giamGia)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            giamGia.GiamGiaId = Guid.NewGuid();
            giamGia.NgayTao = DateTime.UtcNow;
            giamGia.NgayCapNhat = DateTime.UtcNow;

            await _giamGiaRepo.AddAsync(giamGia);
            await _giamGiaRepo.SaveAsync();
            return CreatedAtAction(nameof(GetById), new { id = giamGia.GiamGiaId }, giamGia);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiamGia updated)
        {
            var existing = await _giamGiaRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.TenGiamGia = updated.TenGiamGia;
            existing.PhanTramKhuyenMai = updated.PhanTramKhuyenMai;
            existing.NgayBatDau = updated.NgayBatDau;
            existing.NgayKetThuc = updated.NgayKetThuc;
            existing.TrangThai = updated.TrangThai;
            existing.NgayCapNhat = DateTime.UtcNow;

            _giamGiaRepo.Update(existing);
            await _giamGiaRepo.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _giamGiaRepo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _giamGiaRepo.Delete(entity);
            await _giamGiaRepo.SaveAsync();
            return NoContent();
        }

        [HttpGet("{id}/phantram")]
        public async Task<IActionResult> GetPhanTramKhuyenMai(Guid id)
        {
            var giamGia = await _giamGiaRepo.GetByIdAsync(id);
            if (giamGia == null) return NotFound();
            return Ok(giamGia.PhanTramKhuyenMai);
        }
    }
}