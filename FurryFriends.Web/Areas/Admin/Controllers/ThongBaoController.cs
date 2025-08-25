using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class ThongBaoController : Controller
    {
        private readonly IThongBaoService _thongBaoService;

        public ThongBaoController(IThongBaoService thongBaoService)
        {
            _thongBaoService = thongBaoService;
        }

        public async Task<IActionResult> Index()
        {
            var listDto = await _thongBaoService.GetAllAsync();

            var listVm = listDto.Select(dto => new ThongBaoViewModel
            {
                ThongBaoId = dto.ThongBaoId,                // cần có trong ViewModel
                TieuDe = dto.TieuDe,
                NoiDung = dto.NoiDung,
                NgayTao = dto.NgayTao,
                DaDoc = dto.DaDoc,
                Loai = dto.Loai,
                UserName = dto.UserName
            }).ToList();

            return View(listVm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThongBaoDTO dto)
        {
            if (!ModelState.IsValid) return View(dto);

            dto.Loai = "Admin";
            dto.UserName = "admin";

            await _thongBaoService.CreateAsync(dto);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            var list = await _thongBaoService.GetAllAsync();
            return Json(list.Where(x => !x.DaDoc).Take(5));
        }
    }
} 