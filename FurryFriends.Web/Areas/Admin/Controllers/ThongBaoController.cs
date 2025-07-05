using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FurryFriends.Web.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongBaoController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _apiBase;
        public ThongBaoController(IHttpClientFactory clientFactory, IWebHostEnvironment env)
        {
            _clientFactory = clientFactory;
            _apiBase = "https://localhost:7289/api/ThongBao"; // Đã sửa lại đúng cổng API
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            var res = await client.GetAsync(_apiBase);
            if (!res.IsSuccessStatusCode) return View(new List<ThongBaoViewModel>());
            var json = await res.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ThongBaoViewModel>>(json);
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ThongBaoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            model.Loai = "Admin";
            model.UserName = "admin";
            var client = _clientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(_apiBase, content);
            if (res.IsSuccessStatusCode)
                return RedirectToAction("Index");
            ModelState.AddModelError("", "Tạo thông báo thất bại");
            return View(model);
        }
    }
} 