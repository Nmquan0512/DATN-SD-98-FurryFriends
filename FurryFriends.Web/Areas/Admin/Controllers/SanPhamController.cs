using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ISanPhamChiTietService _chiTietService;
        private readonly IAnhService _anhService;
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IKichCoService _kichCoService;
        private readonly IMauSacService _mauSacService;
        private readonly IThanhPhanService _thanhPhanService;
        private readonly IChatLieuService _chatLieuService;

        public SanPhamController(
            ISanPhamService sanPhamService,
            ISanPhamChiTietService chiTietService,
            IAnhService anhService,
            IThuongHieuService thuongHieuService,
            IKichCoService kichCoService,
            IMauSacService mauSacService,
            IThanhPhanService thanhPhanService,
            IChatLieuService chatLieuService)
        {
            _sanPhamService = sanPhamService;
            _chiTietService = chiTietService;
            _anhService = anhService;
            _thuongHieuService = thuongHieuService;
            _kichCoService = kichCoService;
            _mauSacService = mauSacService;
            _thanhPhanService = thanhPhanService;
            _chatLieuService = chatLieuService;
        }

        // ---------------- GET: Hiển thị danh sách sản phẩm ----------------
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _sanPhamService.GetAllAsync();
                
                // Load dữ liệu cho bộ lọc nâng cao
                var thuongHieuList = await _thuongHieuService.GetAllAsync();
                var chatLieuList = await _chatLieuService.GetAllAsync();
                var thanhPhanList = await _thanhPhanService.GetAllAsync();
                
                ViewBag.ThuongHieus = new SelectList(thuongHieuList, "ThuongHieuId", "TenThuongHieu");
                ViewBag.ChatLieus = new SelectList(chatLieuList, "ChatLieuId", "TenChatLieu");
                ViewBag.ThanhPhans = new SelectList(thanhPhanList, "ThanhPhanId", "TenThanhPhan");
                
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải dữ liệu: {ex.Message}";
                return View(new List<SanPhamDTO>());
            }
        }

        // ---------------- GET: Hiển thị form tạo mới ----------------
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();
            var anhList = await _anhService.GetAllAsync();
            ViewBag.AnhList = anhList;
            var viewModel = new SanPhamFullCreateViewModel
            {
                SanPham = new SanPhamDTO(),
                ChiTietList = new List<SanPhamChiTietCreateViewModel>()
            };
            return View(viewModel);
        }

        // ---------------- POST: Tạo sản phẩm đầy đủ ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamFullCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return View(model);
            }

            // 1. Tạo sản phẩm chính
            var sanPhamToCreate = new SanPhamDTO
            {
                TenSanPham = model.SanPham.TenSanPham,
                LoaiSanPham = model.SanPham.LoaiSanPham,
                ThanhPhanIds = model.SanPham.ThanhPhanIds,
                ChatLieuIds = model.SanPham.ChatLieuIds,
                ThuongHieuId = model.SanPham.ThuongHieuId,
                TrangThai = model.SanPham.TrangThai
            };

            var createdSanPham = await _sanPhamService.CreateAsync(sanPhamToCreate);
            if (createdSanPham == null || createdSanPham.SanPhamId == Guid.Empty)
            {
                ModelState.AddModelError("", "Không thể tạo sản phẩm.");
                await LoadDropdownData();
                return View(model);
            }

            // 2. Tạo chi tiết sản phẩm
            foreach (var chiTiet in model.ChiTietList)
            {
                var chiTietToCreate = new SanPhamChiTietDTO
                {
                    SanPhamId = createdSanPham.SanPhamId,
                    MauSacId = chiTiet.MauSacId,
                    KichCoId = chiTiet.KichCoId,
                    SoLuong = chiTiet.SoLuongTon,
                    Gia = chiTiet.GiaBan,
                    NgayTao = DateTime.Now,
                    TrangThai = 1,
                    MoTa = chiTiet.MoTa,
                    AnhId = chiTiet.AnhId
                };

                await _chiTietService.CreateAsync(chiTietToCreate);
            }

            TempData["Success"] = "Tạo sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _sanPhamService.GetByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var success = await _sanPhamService.DeleteAsync(id);
            if (success)
                return RedirectToAction("Index");
            ModelState.AddModelError("", "Xóa sản phẩm thất bại!");
            var model = await _sanPhamService.GetByIdAsync(id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var sanPham = await _sanPhamService.GetByIdAsync(id);
            if (sanPham == null) return NotFound();
            await LoadDropdownData();
            ViewBag.LoaiSanPhamList = new List<SelectListItem> {
                new SelectListItem { Text = "Đồ Ăn", Value = "DoAn" },
                new SelectListItem { Text = "Đồ Dùng", Value = "DoDung" }
            };
            ViewBag.ChatLieus = await _chatLieuService.GetAllAsync();
            ViewBag.ThanhPhans = await _thanhPhanService.GetAllAsync();
            ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
            var kichCoList = await _kichCoService.GetAllAsync();
            ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
            var mauSacList = await _mauSacService.GetAllAsync();
            ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
            var anhList = await _anhService.GetAllAsync();
            ViewBag.AnhList = anhList;
            // Lấy danh sách biến thể từ API
            var allChiTiet = await _chiTietService.GetAllAsync();
            var chiTietList = allChiTiet
                .Where(x => x.SanPhamId == id)
                .Select(x => new SanPhamChiTietCreateViewModel
                {
                    SanPhamChiTietId = x.SanPhamChiTietId,
                    MauSacId = x.MauSacId,
                    KichCoId = x.KichCoId,
                    SoLuongTon = x.SoLuong,
                    GiaBan = x.Gia,
                    MoTa = x.MoTa,
                    AnhId = x.AnhId,
                    DuongDan = x.DuongDan
                })
                .ToList();
            // Map lại ChatLieuIds/ThanhPhanIds nếu có
            var sanPhamDto = sanPham;
            if (sanPhamDto.LoaiSanPham == "DoDung" && sanPhamDto.ChatLieuIds == null)
                sanPhamDto.ChatLieuIds = new List<Guid>();
            if (sanPhamDto.LoaiSanPham == "DoAn" && sanPhamDto.ThanhPhanIds == null)
                sanPhamDto.ThanhPhanIds = new List<Guid>();
            ViewBag.ChatLieuList = new SelectList(await _chatLieuService.GetAllAsync(), "ChatLieuId", "TenChatLieu", sanPhamDto.ChatLieuIds);
            ViewBag.ThanhPhanList = new SelectList(await _thanhPhanService.GetAllAsync(), "ThanhPhanId", "TenThanhPhan", sanPhamDto.ThanhPhanIds);
            var viewModel = new SanPhamFullCreateViewModel
            {
                SanPham = sanPhamDto,
                ChiTietList = chiTietList
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SanPhamFullCreateViewModel model)
        {
            // Log dữ liệu biến thể gửi lên
            Console.WriteLine("[SanPham/Edit] ChiTietList gửi lên:");
            foreach (var chiTiet in model.ChiTietList)
            {
                Console.WriteLine($"  - SanPhamChiTietId: {chiTiet.SanPhamChiTietId}, SanPhamId: {chiTiet.SanPhamId}, KichCoId: {chiTiet.KichCoId}, MauSacId: {chiTiet.MauSacId}, GiaBan: {chiTiet.GiaBan}, SoLuongTon: {chiTiet.SoLuongTon}, AnhId: {chiTiet.AnhId}");
            }
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                ViewBag.LoaiSanPhamList = new List<SelectListItem> {
                    new SelectListItem { Text = "Đồ Ăn", Value = "DoAn" },
                    new SelectListItem { Text = "Đồ Dùng", Value = "DoDung" }
                };
                ViewBag.ChatLieus = await _chatLieuService.GetAllAsync();
                ViewBag.ThanhPhans = await _thanhPhanService.GetAllAsync();
                ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
                var kichCoList = await _kichCoService.GetAllAsync();
                ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
                var mauSacList = await _mauSacService.GetAllAsync();
                ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
                var anhList = await _anhService.GetAllAsync();
                ViewBag.AnhList = anhList;
                return View(model);
            }
            // Cập nhật sản phẩm chính
            model.SanPham.NgaySua = DateTime.Now;
            await _sanPhamService.UpdateAsync(model.SanPham.SanPhamId, model.SanPham);
            // Xử lý biến thể: thêm mới, cập nhật, xoá
            var allChiTiet = await _chiTietService.GetAllAsync();
            var oldChiTiet = allChiTiet.Where(x => x.SanPhamId == model.SanPham.SanPhamId).ToList();
            var formIds = model.ChiTietList.Where(x => x.SanPhamChiTietId != null).Select(x => x.SanPhamChiTietId.Value).ToList();
            // Xoá biến thể không còn trong form
            foreach (var old in oldChiTiet)
            {
                if (!formIds.Contains(old.SanPhamChiTietId))
                {
                    await _chiTietService.DeleteAsync(old.SanPhamChiTietId);
                }
            }
            // Thêm mới/cập nhật biến thể
            bool hasError = false;
            foreach (var chiTiet in model.ChiTietList)
            {
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamChiTietId = chiTiet.SanPhamChiTietId ?? Guid.Empty,
                    SanPhamId = model.SanPham.SanPhamId,
                    MauSacId = chiTiet.MauSacId,
                    KichCoId = chiTiet.KichCoId,
                    SoLuong = chiTiet.SoLuongTon,
                    Gia = chiTiet.GiaBan,
                    MoTa = chiTiet.MoTa,
                    AnhId = chiTiet.AnhId,
                    TrangThai = 1
                };
                if (chiTiet.SanPhamChiTietId == null || chiTiet.SanPhamChiTietId == Guid.Empty)
                {
                    var result = await _chiTietService.CreateAsync(dto);
                    Console.WriteLine($"[SanPham/Edit] Kết quả CreateAsync: {(result.Data != null ? "OK" : "FAIL")} - Lỗi: {string.Join("; ", result.Errors?.SelectMany(e => e.Value) ?? new string[0])}");
                    if (result.Errors != null && result.Errors.Count > 0)
                    {
                        hasError = true;
                        foreach (var err in result.Errors)
                        {
                            foreach (var msg in err.Value)
                                ModelState.AddModelError($"ChiTietList", $"[Thêm biến thể] {msg}");
                        }
                    }
                }
                else
                {
                    var result = await _chiTietService.UpdateAsync(chiTiet.SanPhamChiTietId.Value, dto);
                    Console.WriteLine($"[SanPham/Edit] Kết quả UpdateAsync: {(result.Data ? "OK" : "FAIL")} - Lỗi: {string.Join("; ", result.Errors?.SelectMany(e => e.Value) ?? new string[0])}");
                }
            }
            if (hasError)
            {
                await LoadDropdownData();
                ViewBag.LoaiSanPhamList = new List<SelectListItem> {
                    new SelectListItem { Text = "Đồ Ăn", Value = "DoAn" },
                    new SelectListItem { Text = "Đồ Dùng", Value = "DoDung" }
                };
                ViewBag.ChatLieus = await _chatLieuService.GetAllAsync();
                ViewBag.ThanhPhans = await _thanhPhanService.GetAllAsync();
                ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
                var kichCoList = await _kichCoService.GetAllAsync();
                ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
                var mauSacList = await _mauSacService.GetAllAsync();
                ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
                var anhList = await _anhService.GetAllAsync();
                ViewBag.AnhList = anhList;
                return View(model);
            }
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Load thương hiệu
                var thuongHieus = await _thuongHieuService.GetAllAsync();
                ViewBag.ThuongHieuList = new SelectList(thuongHieus, "ThuongHieuId", "TenThuongHieu");

                // Load kích cỡ
                var kichCos = await _kichCoService.GetAllAsync();
                ViewBag.KichCoList = new SelectList(kichCos, "KichCoId", "TenKichCo");

                // Load màu sắc
                var mauSacs = await _mauSacService.GetAllAsync();
                ViewBag.MauSacList = new SelectList(mauSacs, "MauSacId", "TenMau");

                // Load thành phần
                var thanhPhans = await _thanhPhanService.GetAllAsync();
                ViewBag.ThanhPhanList = new SelectList(thanhPhans, "ThanhPhanId", "TenThanhPhan");

                // Load chất liệu
                var chatLieus = await _chatLieuService.GetAllAsync();
                ViewBag.ChatLieuList = new SelectList(chatLieus, "ChatLieuId", "TenChatLieu");
            }
            catch (Exception ex)
            {
                // Log lỗi nếu không load được dữ liệu dropdown
                ModelState.AddModelError("", $"Không thể load dữ liệu dropdown: {ex.Message}");
            }
        }
    }
}
