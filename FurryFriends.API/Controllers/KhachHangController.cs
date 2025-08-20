using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangRepository _repository;

        public KhachHangController(IKhachHangRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHang>>> GetAll()
        {
            var khachHangs = await _repository.GetAllAsync();
            return Ok(khachHangs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KhachHang>> GetById(Guid id)
        {
            var khachHang = await _repository.GetByIdAsync(id);
            if (khachHang == null) return NotFound();
            return Ok(khachHang);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            await _repository.AddAsync(khachHang);
            return CreatedAtAction(nameof(GetById), new { id = khachHang.KhachHangId }, khachHang);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, KhachHang khachHang)
        {
            if (id != khachHang.KhachHangId) return BadRequest();
            await _repository.UpdateAsync(khachHang);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<KhachHang>> GetByEmail(string email)
        {
            var khachHang = await _repository.FindByEmailAsync(email);
            if (khachHang == null) return NotFound();
            return Ok(khachHang);
        }

        [HttpGet("phone/{phone}")]
        public async Task<ActionResult<KhachHang>> GetByPhone(string phone)
        {
            var khachHang = await _repository.FindByPhoneAsync(phone);
            if (khachHang == null) return NotFound();
            return Ok(khachHang);
        }

        [HttpGet("all-including-deleted")]
        public async Task<ActionResult<IEnumerable<KhachHang>>> GetAllIncludingDeleted()
        {
            var khachHangs = await _repository.GetAllIncludingDeletedAsync();
            return Ok(khachHangs);
        }
    }
}