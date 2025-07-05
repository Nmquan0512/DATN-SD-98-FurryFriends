using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _repository;

        public VoucherController(IVoucherRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vouchers = await _repository.GetAllAsync();
            return Ok(vouchers);
        }

        // GET: api/Voucher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var voucher = await _repository.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();
            return Ok(voucher);
        }

        // POST: api/Voucher
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Voucher voucher)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            voucher.VoucherId = Guid.NewGuid();
            voucher.NgayTao = DateTime.UtcNow;
            await _repository.AddAsync(voucher);
            return CreatedAtAction(nameof(GetById), new { id = voucher.VoucherId }, voucher);
        }

        // PUT: api/Voucher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Voucher voucher)
        {
            if (id != voucher.VoucherId)
                return BadRequest();

            voucher.NgayCapNhat = DateTime.UtcNow;
            await _repository.UpdateAsync(voucher);
            return NoContent();
        }

        // DELETE: api/Voucher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}