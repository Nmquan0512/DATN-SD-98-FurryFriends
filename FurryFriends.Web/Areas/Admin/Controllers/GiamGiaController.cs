using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services; // Nơi định nghĩa lớp ApiException
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
    public class GiamGiaController : Controller
    {
        private readonly IGiamGiaService _giamGiaService;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;
        private readonly IThongBaoService _thongBaoService;

        public GiamGiaController(
            IGiamGiaService giamGiaService,
            ISanPhamChiTietService sanPhamChiTietService,
            IThongBaoService thongBaoService)
        {
            _giamGiaService = giamGiaService;
            _sanPhamChiTietService = sanPhamChiTietService;
            _thongBaoService = thongBaoService;
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
            ViewBag.Products = allProducts
    .Where(p => p.TrangThaiSanPham == true   // SP đang hoạt động
             && p.TrangThai == 1)            // SPCT đang hoạt động
    .ToList();


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
                    var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Chương trình giảm giá mới",
                        NoiDung = $"Đã tạo chương trình giảm giá \"{dto.TenGiamGia}\" từ {dto.NgayBatDau:dd/MM/yyyy} đến {dto.NgayKetThuc:dd/MM/yyyy}.",
                        Loai = "GiamGia",
                        UserName = tenNhanVien,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });
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
            ViewBag.Products = allProducts
    .Where(p => p.TrangThaiSanPham == true   // SP đang hoạt động
             && p.TrangThai == 1)            // SPCT đang hoạt động
    .ToList();

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
            ViewBag.Products = allProducts
    .Where(p => p.TrangThaiSanPham == true   // SP đang hoạt động
             && p.TrangThai == 1)            // SPCT đang hoạt động
    .ToList();


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
                    var tenNhanVien = HttpContext.Session.GetString("HoTen") ?? "Unknown";
                    await _thongBaoService.CreateAsync(new ThongBaoDTO
                    {
                        TieuDe = "Cập nhật chương trình giảm giá",
                        NoiDung = $"Đã cập nhật chương trình giảm giá \"{dto.TenGiamGia}\" (hiệu lực {dto.NgayBatDau:dd/MM/yyyy} - {dto.NgayKetThuc:dd/MM/yyyy}).",
                        Loai = "GiamGia",
                        UserName = tenNhanVien,
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });
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
            ViewBag.Products = allProducts
    .Where(p => p.TrangThaiSanPham == true   // SP đang hoạt động
             && p.TrangThai == 1)            // SPCT đang hoạt động
    .ToList();

            return View(dto);
        }

        // GET: /Admin/GiamGia/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null) return NotFound();
            return View(giamGia);
        }

        // POST: /Admin/GiamGia/Delete/{id} - XÓA MỀM (Vô hiệu hóa vĩnh viễn)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var giamGia = await _giamGiaService.GetByIdAsync(id);
                if (giamGia == null)
                {
                    TempData["Error"] = "Không tìm thấy chương trình giảm giá.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa mềm - đổi trạng thái thành không hoạt động
                giamGia.TrangThai = false;
                await _giamGiaService.UpdateAsync(id, giamGia);

                TempData["Success"] = "Chương trình giảm giá đã được vô hiệu hóa thành công.";

                // 🔔 Thêm thông báo
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = "Vô hiệu hóa chương trình giảm giá",
                    NoiDung = $"Chương trình giảm giá '{giamGia.TenGiamGia}' đã được vô hiệu hóa",
                    Loai = "GiamGia",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Admin/GiamGia/ToggleStatus/{id} - CHUYỂN ĐỔI TRẠNG THÁI (Hoạt động ↔ Không hoạt động)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var giamGia = await _giamGiaService.GetByIdAsync(id);
                if (giamGia == null)
                {
                    TempData["Error"] = "Không tìm thấy chương trình giảm giá.";
                    return RedirectToAction(nameof(Index));
                }

                // Chuyển đổi trạng thái
                giamGia.TrangThai = !giamGia.TrangThai;
                await _giamGiaService.UpdateAsync(id, giamGia);

                var statusText = giamGia.TrangThai ? "kích hoạt" : "tạm dừng";
                TempData["Success"] = $"Chương trình giảm giá đã được {statusText} thành công.";

                // 🔔 Thêm thông báo
                var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";
                await _thongBaoService.CreateAsync(new ThongBaoDTO
                {
                    TieuDe = $"Chuyển đổi trạng thái chương trình giảm giá",
                    NoiDung = $"Chương trình giảm giá '{giamGia.TenGiamGia}' đã được {statusText}",
                    Loai = "GiamGia",
                    UserName = userName,
                    NgayTao = DateTime.Now,
                    DaDoc = false
                });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
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