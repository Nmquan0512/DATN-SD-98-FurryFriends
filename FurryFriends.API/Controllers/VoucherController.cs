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
        private readonly IVoucherRepository _voucherRepository;

        public VoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vouchers = await _voucherRepository.GetAllAsync();
            return Ok(vouchers);
        }

        // GET: api/Voucher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id);
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

            var created = await _voucherRepository.CreateAsync(voucher);
            return CreatedAtAction(nameof(GetById), new { id = created.VoucherId }, created);
        }

        // PUT: api/Voucher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Voucher voucher)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _voucherRepository.UpdateAsync(id, voucher);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: api/Voucher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _voucherRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
