using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurryFriends.Web.Controllers
{
	public class KhachHangHoanTraController : Controller
	{
		private readonly IPhieuHoanTraService _service;
		private readonly IHoaDonService _hoaDonService;

		public KhachHangHoanTraController(IPhieuHoanTraService service, IHoaDonService hoaDonService)
		{
			_service = service;
			_hoaDonService = hoaDonService;
		}

		// Lấy KhachHangId từ Session an toàn
		private bool TryGetKhachHangId(out Guid khId)
		{
			khId = Guid.Empty;
			var s = HttpContext.Session.GetString("KhachHangId"); // bạn đã set ở Login POST
			return !string.IsNullOrWhiteSpace(s) && Guid.TryParse(s, out khId);
		}

		// GET: /KhachHangHoanTra
		public async Task<IActionResult> Index()
		{
			if (!TryGetKhachHangId(out var khId))
				return RedirectToAction("DangNhap", "KhachHangLogin");

			var list = await _service.GetByKhachHangAsync(khId);
			return View(list);
		}

		// GET: /KhachHangHoanTra/Create?hoaDonId=...&hoaDonChiTietId=...
		public async Task<IActionResult> Create(Guid hoaDonId, Guid hoaDonChiTietId)
		{
			if (!TryGetKhachHangId(out _))
				return RedirectToAction("DangNhap", "KhachHangLogin");

			var chiTiets = await _hoaDonService.GetChiTietHoaDonAsync(hoaDonId);
			var ct = chiTiets.FirstOrDefault(x => x.HoaDonChiTietId == hoaDonChiTietId);
			if (ct == null) return NotFound();

			var model = new PhieuHoanTraCreateRequest { HoaDonChiTietId = ct.HoaDonChiTietId };
			ViewBag.HoaDonChiTiet = ct;
			ViewBag.HoaDonId = hoaDonId;
			return View(model);
		}

		// POST: /KhachHangHoanTra/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Guid hoaDonId, PhieuHoanTraCreateRequest request)
		{
			if (!TryGetKhachHangId(out _))
				return RedirectToAction("DangNhap", "KhachHangLogin");

			if (!ModelState.IsValid) return View(request);

			var ok = await _service.CreateAsync(request);
			if (ok) return RedirectToAction("Index", "KhachHangHoanTra");

			ModelState.AddModelError("", "Tạo phiếu hoàn thất bại");
			return View(request);
		}

		// GET: /KhachHangHoanTra/Details/{id}
		public async Task<IActionResult> Details(Guid id)
		{
			if (!TryGetKhachHangId(out _))
				return RedirectToAction("DangNhap", "KhachHangLogin");

			var phieu = await _service.GetByIdAsync(id);
			if (phieu == null) return NotFound();
			return View(phieu);
		}
	}
}
