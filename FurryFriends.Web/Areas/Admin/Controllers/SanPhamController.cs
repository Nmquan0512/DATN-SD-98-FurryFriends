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
                // Thêm danh sách thương hiệu đầy đủ để kiểm tra trạng thái
                ViewBag.DanhSachThuongHieu = thuongHieuList.ToList();
                ViewBag.DanhSachChatLieu = chatLieuList.ToList();
                ViewBag.DanhSachThanhPhan = thanhPhanList.ToList();
                
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
            // Xóa TempData cũ để tránh hiển thị lỗi từ lần trước
            TempData.Remove("Error");
            TempData.Remove("error");
            TempData.Remove("Success");
            // Chỉ lấy thuộc tính hoạt động
            var allThuongHieu = await _thuongHieuService.GetAllAsync();
            var thuongHieuList = allThuongHieu.Where(th => th.TrangThai)
                .Select(th => new SelectListItem { Value = th.ThuongHieuId.ToString(), Text = th.TenThuongHieu }).ToList();
            ViewBag.ThuongHieuList = thuongHieuList;
            var allChatLieu = await _chatLieuService.GetAllAsync();
            var chatLieuList = allChatLieu.Where(cl => cl.TrangThai)
                .Select(cl => new SelectListItem { Value = cl.ChatLieuId.ToString(), Text = cl.TenChatLieu }).ToList();
            ViewBag.ChatLieuList = chatLieuList;
            var allThanhPhan = await _thanhPhanService.GetAllAsync();
            var thanhPhanList = allThanhPhan.Where(tp => tp.TrangThai)
                .Select(tp => new SelectListItem { Value = tp.ThanhPhanId.ToString(), Text = tp.TenThanhPhan }).ToList();
            ViewBag.ThanhPhanList = thanhPhanList;
            var anhList = await _anhService.GetAllAsync();
            ViewBag.AnhList = anhList;
            var kichCoList = (await _kichCoService.GetAllAsync()).Where(x => x.TrangThai).ToList();
            var mauSacList = (await _mauSacService.GetAllAsync()).Where(x => x.TrangThai).ToList();
            ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
            ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
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
            // Debug: Log dữ liệu nhận được
            Console.WriteLine($"DEBUG: Model null? {model == null}");
            Console.WriteLine($"DEBUG: ChiTietList null? {model?.ChiTietList == null}");
            Console.WriteLine($"DEBUG: ChiTietList count: {model?.ChiTietList?.Count ?? 0}");
            
            if (model?.ChiTietList != null)
            {
                for (int i = 0; i < model.ChiTietList.Count; i++)
                {
                    var chiTiet = model.ChiTietList[i];
                    Console.WriteLine($"DEBUG: ChiTiet[{i}] - MauSacId: {chiTiet.MauSacId}, KichCoId: {chiTiet.KichCoId}, GiaBan: {chiTiet.GiaBan}, SoLuongTon: {chiTiet.SoLuongTon}");
                }
            }
            
            // Validate biến thể
            if (model?.ChiTietList == null || !model.ChiTietList.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một biến thể sản phẩm!");
            }
            else
            {
                // Validate từng biến thể
                for (int i = 0; i < model.ChiTietList.Count; i++)
                {
                    var chiTiet = model.ChiTietList[i];
                    if (chiTiet.MauSacId == Guid.Empty)
                    {
                        ModelState.AddModelError($"ChiTietList[{i}].MauSacId", "Vui lòng chọn màu sắc cho biến thể!");
                    }
                    if (chiTiet.KichCoId == Guid.Empty)
                    {
                        ModelState.AddModelError($"ChiTietList[{i}].KichCoId", "Vui lòng chọn kích cỡ cho biến thể!");
                    }
                    if (chiTiet.GiaBan <= 0)
                    {
                        ModelState.AddModelError($"ChiTietList[{i}].GiaBan", "Giá bán phải lớn hơn 0!");
                    }
                    if (chiTiet.SoLuongTon < 0)
                    {
                        ModelState.AddModelError($"ChiTietList[{i}].SoLuongTon", "Số lượng không được âm!");
                    }
                }
            }
            
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                var anhList = await _anhService.GetAllAsync();
                ViewBag.AnhList = anhList;
                var kichCoList = await _kichCoService.GetAllAsync();
                var mauSacList = await _mauSacService.GetAllAsync();
                ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
                ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
                return View(model);
            }

            try
            {
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
                    var anhList = await _anhService.GetAllAsync();
                    ViewBag.AnhList = anhList;
                    var kichCoList = await _kichCoService.GetAllAsync();
                    var mauSacList = await _mauSacService.GetAllAsync();
                    ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
                    ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
                return View(model);
            }

                                // 2. Tạo chi tiết sản phẩm nếu có
                if (model.ChiTietList != null && model.ChiTietList.Any())
                {
                    int successCount = 0;
            foreach (var chiTiet in model.ChiTietList)
                    {
                        if (chiTiet.MauSacId != Guid.Empty && chiTiet.KichCoId != Guid.Empty)
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
                                MoTa = chiTiet.MoTa ?? "",
                                AnhId = chiTiet.AnhId
                            };

                            var result = await _chiTietService.CreateAsync(chiTietToCreate);
                            
                            if (result.Data != null)
                            {
                                successCount++;
                            }
                            else
                            {
                                // Log lỗi nếu có
                                if (result.Errors != null)
                                {
                                    foreach (var error in result.Errors)
                                    {
                                        ModelState.AddModelError("", $"Lỗi tạo biến thể: {string.Join(", ", error.Value)}");
                                    }
                                }
                            }
                        }
                    }
                }

                TempData["Success"] = "Tạo sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo sản phẩm: {ex.Message}");
                await LoadDropdownData();
                var anhList = await _anhService.GetAllAsync();
                ViewBag.AnhList = anhList;
                var kichCoList = await _kichCoService.GetAllAsync();
                var mauSacList = await _mauSacService.GetAllAsync();
                ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
                ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
                return View(model);
            }
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
            // --- SỬA CHẤT LIỆU ---
            var allChatLieu = await _chatLieuService.GetAllAsync();
            var selectedChatLieuIds = sanPham.ChatLieuIds ?? new List<Guid>();
            var chatLieuList = new List<SelectListItem>();
            foreach (var cl in allChatLieu)
            {
                if (cl.TrangThai)
                {
                    chatLieuList.Add(new SelectListItem { Value = cl.ChatLieuId.ToString(), Text = cl.TenChatLieu });
                }
                else if (selectedChatLieuIds.Contains(cl.ChatLieuId))
                {
                    chatLieuList.Add(new SelectListItem { Value = cl.ChatLieuId.ToString(), Text = cl.TenChatLieu + " (Ngưng hoạt động)" });
                }
            }
            ViewBag.ChatLieuList = chatLieuList;
            ViewBag.DanhSachChatLieu = allChatLieu.ToList();
            // --- END SỬA CHẤT LIỆU ---
            // --- SỬA THÀNH PHẦN (Edit) ---
            var allThanhPhan = await _thanhPhanService.GetAllAsync();
            var selectedThanhPhanIds = sanPham.ThanhPhanIds ?? new List<Guid>();
            var thanhPhanList = new List<SelectListItem>();
            foreach (var tp in allThanhPhan)
            {
                if (tp.TrangThai)
                {
                    thanhPhanList.Add(new SelectListItem { Value = tp.ThanhPhanId.ToString(), Text = tp.TenThanhPhan });
                }
                else if (selectedThanhPhanIds.Contains(tp.ThanhPhanId))
                {
                    thanhPhanList.Add(new SelectListItem { Value = tp.ThanhPhanId.ToString(), Text = tp.TenThanhPhan + " (Ngưng hoạt động)" });
                }
            }
            ViewBag.ThanhPhanList = thanhPhanList;
            ViewBag.DanhSachThanhPhan = allThanhPhan.ToList();
            // --- END SỬA THÀNH PHẦN (Edit) ---
            // --- Giữ nguyên các phần khác ---
            ViewBag.ThanhPhans = await _thanhPhanService.GetAllAsync();
            var allThuongHieu = await _thuongHieuService.GetAllAsync();
            var selectedThuongHieuId = sanPham.ThuongHieuId;
            var thuongHieuList = new List<SelectListItem>();
            foreach (var th in allThuongHieu)
            {
                if (th.TrangThai)
                {
                    thuongHieuList.Add(new SelectListItem { Value = th.ThuongHieuId.ToString(), Text = th.TenThuongHieu });
                }
                else if (selectedThuongHieuId != null && th.ThuongHieuId == selectedThuongHieuId)
                {
                    thuongHieuList.Add(new SelectListItem { Value = th.ThuongHieuId.ToString(), Text = th.TenThuongHieu + " (Ngưng hoạt động)" });
                }
            }
            ViewBag.ThuongHieuList = thuongHieuList;
            // --- END SỬA THƯƠNG HIỆU ---
            var kichCoList = await _kichCoService.GetAllAsync();
            ViewBag.KichCoList = new SelectList(kichCoList, "KichCoId", "TenKichCo");
            var mauSacList = await _mauSacService.GetAllAsync();
            ViewBag.MauSacList = new SelectList(mauSacList, "MauSacId", "TenMau");
            var anhList = await _anhService.GetAllAsync();
            ViewBag.AnhList = anhList;
            // Lấy danh sách biến thể từ API
            var allChiTiet = await _chiTietService.GetAllAsync();
            var chiTietList = new List<SanPhamChiTietCreateViewModel>();
            if (allChiTiet != null)
            {
                foreach (var x in allChiTiet)
                {
                    if (x.SanPhamId == id)
                    {
                        chiTietList.Add(new SanPhamChiTietCreateViewModel
                        {
                            SanPhamChiTietId = x.SanPhamChiTietId,
                            MauSacId = x.MauSacId,
                            KichCoId = x.KichCoId,
                            SoLuongTon = x.SoLuong,
                            GiaBan = x.Gia,
                            MoTa = x.MoTa,
                            AnhId = x.AnhId,
                            DuongDan = x.DuongDan
                        });
                    }
                }
            }
            // Map lại ChatLieuIds/ThanhPhanIds nếu có
            var sanPhamDto = sanPham;
            if (sanPhamDto.LoaiSanPham == "DoDung" && sanPhamDto.ChatLieuIds == null)
                sanPhamDto.ChatLieuIds = new List<Guid>();
            if (sanPhamDto.LoaiSanPham == "DoAn" && sanPhamDto.ThanhPhanIds == null)
                sanPhamDto.ThanhPhanIds = new List<Guid>();
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
            var oldChiTiet = new List<SanPhamChiTietDTO>();
            var formIds = new List<Guid>();
            
            if (allChiTiet != null)
            {
                foreach (var x in allChiTiet)
                {
                    if (x.SanPhamId == model.SanPham.SanPhamId)
                    {
                        oldChiTiet.Add(x);
                    }
                }
            }
            
            if (model.ChiTietList != null)
            {
                foreach (var x in model.ChiTietList)
                {
                    if (x.SanPhamChiTietId != null)
                    {
                        formIds.Add(x.SanPhamChiTietId.Value);
                    }
                }
            }
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

