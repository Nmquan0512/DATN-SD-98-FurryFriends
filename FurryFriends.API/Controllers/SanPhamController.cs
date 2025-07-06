using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly ISanPhamRepository _repo;

        public SanPhamsController(ISanPhamRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            var result = new List<SanPhamDTO>();
            foreach (var item in list)
            {
                result.Add(new SanPhamDTO
                {
                    SanPhamId = item.SanPhamId,
                    TenSanPham = item.TenSanPham,
                    TaiKhoanId = item.TaiKhoanId ?? Guid.Empty,
                    ThuongHieuId = item.ThuongHieuId ?? Guid.Empty,
                    TrangThai = item.TrangThai
                });
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var sp = await _repo.GetByIdAsync(id);
            if (sp == null) return NotFound();
            var dto = new SanPhamDTO
            {
                SanPhamId = sp.SanPhamId,
                TenSanPham = sp.TenSanPham,
                TaiKhoanId = sp.TaiKhoanId ?? Guid.Empty,
                ThuongHieuId = sp.ThuongHieuId ?? Guid.Empty,
                TrangThai = sp.TrangThai
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPhamDTO dto)
        {
            var entity = new SanPham
            {
                SanPhamId = Guid.NewGuid(),
                TenSanPham = dto.TenSanPham,
                TaiKhoanId = dto.TaiKhoanId,
                ThuongHieuId = dto.ThuongHieuId,
                TrangThai = dto.TrangThai
            };
            await _repo.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.SanPhamId }, entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SanPhamDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.TenSanPham = dto.TenSanPham;
            existing.TaiKhoanId = dto.TaiKhoanId;
            existing.ThuongHieuId = dto.ThuongHieuId;
            existing.TrangThai = dto.TrangThai;

            await _repo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}