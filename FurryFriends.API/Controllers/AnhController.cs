//using FurryFriends.API.Models.DTO;
//using FurryFriends.API.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading.Tasks;

//namespace FurryFriends.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AnhController : ControllerBase
//    {
//        private readonly IAnhService _service;

//        public AnhController(IAnhService service)
//        {
//            _service = service;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var list = await _service.GetAllAsync();
//            return Ok(list);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var dto = await _service.GetByIdAsync(id);
//            if (dto == null)
//                return NotFound("Không tìm thấy ảnh!");
//            return Ok(dto);
//        }

//        [HttpPost("upload")]
//        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string tenAnh)
//        {
//            try
//            {
//                var dto = await _service.UploadAsync(file, tenAnh);
//                return CreatedAtAction(nameof(GetById), new { id = dto.AnhId }, dto);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(Guid id, [FromBody] AnhDTO dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var result = await _service.UpdateAsync(id, dto);
//            if (!result)
//                return NotFound("Không tìm thấy ảnh!");

//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var result = await _service.DeleteAsync(id);
//            if (!result)
//                return NotFound("Không tìm thấy ảnh!");

//            return NoContent();
//        }
//    }
//}