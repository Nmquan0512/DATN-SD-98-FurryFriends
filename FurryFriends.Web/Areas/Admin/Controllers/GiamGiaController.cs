using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiamGiaController : Controller
    {
        private readonly IGiamGiaService _giamGiaService;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;

        public GiamGiaController(
            IGiamGiaService giamGiaService,
            ISanPhamChiTietService sanPhamChiTietService)
        {
            _giamGiaService = giamGiaService;
            _sanPhamChiTietService = sanPhamChiTietService;
        }

        public async Task<IActionResult> Index()
        {
            var discounts = await _giamGiaService.GetAllAsync();
            return View(discounts);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var discount = await _giamGiaService.GetByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _sanPhamChiTietService.GetAllAsync();
            return View(new GiamGiaDTO
            {
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddDays(7),
                TrangThai = true
            });
        }

       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(GiamGiaDTO dto, List<Guid> selectedProducts)
{
    try
    {
        if (ModelState.IsValid)
        {
            dto.SanPhamChiTietIds = selectedProducts;
            
            var result = await _giamGiaService.CreateAsync(dto);
            
            if (result != null)
            {
                TempData["success"] = "Tạo chương trình giảm giá thành công";
                return RedirectToAction("Index", "GiamGia", new { area = "Admin" });
            }
        }
    }
    catch (Exception ex)
    {
        TempData["error"] = "Lỗi khi tạo chương trình giảm giá: " + ex.Message;
    }

    // Nếu có lỗi, load lại view với dữ liệu đã nhập
    ViewBag.Products = await _sanPhamChiTietService.GetAllAsync();
    return View(dto);
}

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var discount = await _giamGiaService.GetByIdAsync(id);
            if (discount == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _sanPhamChiTietService.GetAllAsync();
            return View(discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GiamGiaDTO dto, List<Guid> selectedProducts)
        {
            if (id != dto.GiamGiaId)
            {
                return NotFound();
            }

            dto.SanPhamChiTietIds = selectedProducts ?? new List<Guid>();

            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _giamGiaService.UpdateAsync(id, dto);
                    TempData["success"] = "Cập nhật chương trình giảm giá thành công";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật chương trình giảm giá");
            }

            ViewBag.Products = await _sanPhamChiTietService.GetAllAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _giamGiaService.DeleteAsync(id);
                TempData["success"] = result ? "Xóa chương trình giảm giá thành công" : "Không tìm thấy chương trình giảm giá";
            }
            catch (Exception)
            {
                TempData["error"] = "Đã xảy ra lỗi khi xóa chương trình giảm giá";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignProducts(Guid id, List<Guid> productIds)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                {
                    TempData["error"] = "Vui lòng chọn ít nhất một sản phẩm";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var result = await _giamGiaService.AssignProductsAsync(id, productIds);
                TempData["success"] = result ? "Gán sản phẩm vào chương trình giảm giá thành công" : "Gán sản phẩm thất bại";
            }
            catch (Exception)
            {
                TempData["error"] = "Đã xảy ra lỗi khi gán sản phẩm";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}