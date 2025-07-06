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
            try
            {
                var vouchers = await _repository.GetAllAsync();
                return Ok(vouchers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Voucher/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var voucher = await _repository.GetByIdAsync(id);
                if (voucher == null)
                    return NotFound();
                return Ok(voucher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Voucher
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Voucher voucher)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                voucher.VoucherId = Guid.NewGuid();
                voucher.NgayTao = DateTime.UtcNow;
                await _repository.AddAsync(voucher);
                return CreatedAtAction(nameof(GetById), new { id = voucher.VoucherId }, voucher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Voucher/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Voucher voucher)
        {
            try
            {
                if (id != voucher.VoucherId)
                    return BadRequest();

                voucher.NgayCapNhat = DateTime.UtcNow;
                await _repository.UpdateAsync(voucher);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Voucher/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}