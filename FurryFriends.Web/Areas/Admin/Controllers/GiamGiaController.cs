using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services; // Nơi định nghĩa lớp ApiException
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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

        // GET: /Admin/GiamGia
        public async Task<IActionResult> Index()
        {
            try
            {
                var discounts = await _giamGiaService.GetAllAsync();
                return View(discounts);
            }
            catch (ApiException ex)
            {
                TempData["error"] = $"Không thể tải danh sách giảm giá. Lỗi từ API: {ex.Message}";
                return View(new List<GiamGiaDTO>());
            }
        }

        // GET: /Admin/GiamGia/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy các sản phẩm đang hoạt động để người dùng chọn
            var allProducts = await _sanPhamChiTietService.GetAllAsync();
            ViewBag.Products = allProducts.Where(p => p.TrangThai == 1).ToList();

            // Tạo một DTO mới với các giá trị mặc định
            return View(new GiamGiaDTO
            {
                NgayBatDau = DateTime.Now,
                NgayKetThuc = DateTime.Now.AddDays(7),
                TrangThai = true
            });
        }

        // POST: /Admin/GiamGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiamGiaDTO dto, List<Guid> selectedProducts)
        {
            dto.SanPhamChiTietIds = selectedProducts ?? new List<Guid>();

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _giamGiaService.CreateAsync(dto);
                    TempData["success"] = "Tạo chương trình giảm giá thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ApiException ex)
                {
                    // Bắt các lỗi cụ thể từ API và hiển thị cho người dùng
                    HandleApiException(ex);
                }
                catch (Exception ex)
                {
                    // Lỗi không mong muốn khác
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại. " + ex.Message);
                }
            }

            // Nếu có lỗi, tải lại danh sách sản phẩm và hiển thị lại form
            var allProducts = await _sanPhamChiTietService.GetAllAsync();
            ViewBag.Products = allProducts.Where(p => p.TrangThai == 1).ToList();
            return View(dto);
        }

        // GET: /Admin/GiamGia/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // 1. Lấy thông tin chương trình giảm giá cần sửa
            var discount = await _giamGiaService.GetByIdAsync(id);
            if (discount == null)
            {
                TempData["error"] = "Không tìm thấy chương trình giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Lấy TẤT CẢ các sản phẩm đang hoạt động để hiển thị
            var allProducts = await _sanPhamChiTietService.GetAllAsync();
            ViewBag.Products = allProducts.Where(p => p.TrangThai == 1).ToList();

            // 3. Truyền DTO của chương trình giảm giá vào View
            // DTO này đã chứa SanPhamChiTietIds, View sẽ dựa vào đó để biết sản phẩm nào đã được chọn
            return View(discount);
        }


        // POST: /Admin/GiamGia/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GiamGiaDTO dto, List<Guid> selectedProducts)
        {
            if (id != dto.GiamGiaId) return NotFound();

            // Gán danh sách ID sản phẩm mới được chọn từ View vào DTO
            dto.SanPhamChiTietIds = selectedProducts ?? new List<Guid>();

            if (ModelState.IsValid)
            {
                try
                {
                    await _giamGiaService.UpdateAsync(id, dto);
                    TempData["success"] = "Cập nhật chương trình giảm giá thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ApiException ex)
                {
                    HandleApiException(ex); // Dùng lại hàm xử lý lỗi của bạn
                }
            }

            // Nếu có lỗi, tải lại danh sách sản phẩm và hiển thị lại form
            var allProducts = await _sanPhamChiTietService.GetAllAsync();
            ViewBag.Products = allProducts.Where(p => p.TrangThai == 1).ToList();
            return View(dto);
        }

        // POST: /Admin/GiamGia/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var success = await _giamGiaService.DeleteAsync(id);
                if (success)
                {
                    TempData["success"] = "Xóa chương trình giảm giá thành công.";
                }
                else
                {
                    TempData["error"] = "Không tìm thấy chương trình giảm giá để xóa.";
                }
            }
            catch (ApiException ex)
            {
                TempData["error"] = $"Lỗi khi xóa: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        // Hàm hỗ trợ chung để xử lý lỗi từ API và thêm vào ModelState
        private void HandleApiException(ApiException ex)
        {
            // Lỗi nghiệp vụ có thông điệp rõ ràng (ví dụ: tên trùng, ngày sai)
            if (ex.StatusCode == HttpStatusCode.BadRequest || ex.StatusCode == HttpStatusCode.Conflict)
            {
                try
                {
                    // Cố gắng parse lỗi có cấu trúc { "message": "..." }
                    var errorObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(ex.Content);
                    if (errorObject != null && errorObject.ContainsKey("message"))
                    {
                        ModelState.AddModelError(string.Empty, errorObject["message"]);
                    }
                    else
                    {
                        // Nếu không parse được, hiển thị lỗi chung
                        ModelState.AddModelError(string.Empty, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.");
                    }
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.");
                }
            }
            else
            {
                // Các lỗi khác (500, 404...)
                ModelState.AddModelError(string.Empty, $"Đã xảy ra lỗi từ hệ thống. {ex.Message}");
            }
        }
    }
}