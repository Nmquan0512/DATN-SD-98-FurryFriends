﻿using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HinhThucThanhToanController : ControllerBase
    {
        private readonly IHinhThucThanhToanRepository _repository;

        public HinhThucThanhToanController(IHinhThucThanhToanRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repository.GetAllAsync();
            return Ok(list);
        }
    }
}
