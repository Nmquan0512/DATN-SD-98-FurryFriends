using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IThanhPhanService _thanhPhanService;
        private readonly IChatLieuService _chatLieuService;

        public SanPhamController(
            ISanPhamService sanPhamService,
            IThuongHieuService thuongHieuService,
            IThanhPhanService thanhPhanService,
            IChatLieuService chatLieuService)
        {
            _sanPhamService = sanPhamService;
            _thuongHieuService = thuongHieuService;
            _thanhPhanService = thanhPhanService;
            _chatLieuService = chatLieuService;
        }

        public async Task<IActionResult> Index(string? loai = null, int page = 1, int pageSize = 10)
        {
            await PopulateDropdownsAsync(); // Đảm bảo filter luôn có dữ liệu
            var (items, totalItems) = await _sanPhamService.GetFilteredAsync(loai, page, pageSize);

            ViewBag.Loai = loai;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            // Đảm bảo dropdown mặc định là rỗng
            var model = new SanPhamDTO { LoaiSanPham = "" };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(dto);
            }

            try
            {
                var result = await _sanPhamService.CreateAsync(dto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Thêm sản phẩm thất bại: {ex.Message}");
                await PopulateDropdownsAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _sanPhamService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            await PopulateDropdownsAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SanPhamDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(dto);
            }

            var updated = await _sanPhamService.UpdateAsync(id, dto);
            if (updated)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật thất bại!");
            await PopulateDropdownsAsync();
            return View(dto);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _sanPhamService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deleted = await _sanPhamService.DeleteAsync(id);
            if (deleted)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Xóa thất bại!");
            var dto = await _sanPhamService.GetByIdAsync(id);
            return View("Delete", dto);
        }

        private async Task PopulateDropdownsAsync()
        {
            var thuongHieus = await _thuongHieuService.GetAllAsync();
            var thanhPhans = await _thanhPhanService.GetAllAsync();
            var chatLieus = await _chatLieuService.GetAllAsync();

            ViewBag.ThuongHieus = thuongHieus.Select(th => new SelectListItem
            {
                Value = th.ThuongHieuId.ToString(),
                Text = th.TenThuongHieu
            }).ToList();

            ViewBag.ThanhPhans = thanhPhans.Select(tp => new SelectListItem
            {
                Value = tp.ThanhPhanId.ToString(),
                Text = tp.TenThanhPhan
            }).ToList();

            ViewBag.ChatLieus = chatLieus.Select(cl => new SelectListItem
            {
                Value = cl.ChatLieuId.ToString(),
                Text = cl.TenChatLieu
            }).ToList();

            ViewBag.LoaiSanPhamList = new List<SelectListItem>
            {
                new SelectListItem { Value = "DoAn", Text = "Đồ ăn" },
                new SelectListItem { Value = "DoDung", Text = "Đồ dùng" }
            };
        }
    }
}
