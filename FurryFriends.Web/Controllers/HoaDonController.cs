using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IHoaDonService _hoaDonService;

        public HoaDonController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var hoaDons = await _hoaDonService.GetHoaDonListAsync();
                return View(hoaDons);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(new List<HoaDon>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["error"] = "ID hóa đơn không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var hoaDon = await _hoaDonService.GetHoaDonByIdAsync(id);
                if (hoaDon == null)
                {
                    TempData["error"] = $"Không tìm thấy hóa đơn với ID: {id}";
                    return RedirectToAction(nameof(Index));
                }

                return View(hoaDon);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Search(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    TempData["error"] = "Từ khóa tìm kiếm không được để trống";
                    return RedirectToAction(nameof(Index));
                }

                var hoaDons = await _hoaDonService.SearchHoaDonAsync(keyword);
                return View("Index", hoaDons);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> ExportPdf(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["error"] = "ID hóa đơn không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var pdfBytes = await _hoaDonService.ExportHoaDonToPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"HoaDon_{id}.pdf");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 