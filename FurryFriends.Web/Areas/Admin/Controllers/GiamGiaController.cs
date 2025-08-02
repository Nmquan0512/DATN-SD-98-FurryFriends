using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiamGiaController : Controller
    {
        private readonly IGiamGiaService _giamGiaService;
        private readonly ISanPhamChiTietService _sanPhamChiTietService;
        private readonly ISanPhamService _sanPhamService;

        public GiamGiaController(IGiamGiaService giamGiaService, ISanPhamChiTietService sanPhamChiTietService, ISanPhamService sanPhamService)
        {
            _giamGiaService = giamGiaService;
            _sanPhamChiTietService = sanPhamChiTietService;
            _sanPhamService = sanPhamService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _giamGiaService.GetAllAsync();
            ViewBag.TotalCount = list.Count();
            ViewBag.ActiveCount = list.Count(x => x.TrangThai);
            ViewBag.InactiveCount = list.Count(x => !x.TrangThai);
            return View(list);
        }

        // ✅ GET: Admin/GiamGia/Create
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách sản phẩm chi tiết cho modal (truyền qua ViewBag)
            var sanPhamsChiTiet = await _sanPhamChiTietService.GetAllAsync();
            var sanPhams = await _sanPhamService.GetAllAsync();
            var list = sanPhamsChiTiet.Select(spct => new
            {
                spct.SanPhamChiTietId,
                TenSanPham = sanPhams.FirstOrDefault(sp => sp.SanPhamId == spct.SanPhamId)?.TenSanPham ?? "",
                spct.TenMau,
                spct.TenKichCo,
                spct.Gia,
                spct.DuongDan
            }).ToList();
            ViewBag.SanPhamChiTietList = list;
            return View(new GiamGiaCreateViewModel());
        }

        // ✅ POST: Admin/GiamGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiamGiaCreateViewModel model)
        {
            if (model.SanPhamChiTietIds == null || !model.SanPhamChiTietIds.Any())
            {
                ModelState.AddModelError("SanPhamChiTietIds", "Vui lòng chọn ít nhất một sản phẩm áp dụng giảm giá.");
                // Lấy lại danh sách cho modal
                var sanPhamsChiTiet = await _sanPhamChiTietService.GetAllAsync();
                var sanPhams = await _sanPhamService.GetAllAsync();
                var list = sanPhamsChiTiet.Select(spct => new
                {
                    spct.SanPhamChiTietId,
                    TenSanPham = sanPhams.FirstOrDefault(sp => sp.SanPhamId == spct.SanPhamId)?.TenSanPham ?? "",
                    spct.TenMau,
                    spct.TenKichCo,
                    spct.Gia,
                    spct.DuongDan
                }).ToList();
                ViewBag.SanPhamChiTietList = list;
                return View(model);
            }
            model.GiamGia.SanPhamChiTietIds = model.SanPhamChiTietIds;
            var result = await _giamGiaService.CreateAsync(model.GiamGia);
            if (result.Success)
            {
                TempData["success"] = "Tạo chương trình giảm giá thành công!";
                return RedirectToAction("Index");
            }
            if (result.Errors != null)
            {
                foreach (var err in result.Errors)
                {
                    foreach (var msg in err.Value)
                    {
                        ModelState.AddModelError(err.Key, msg);
                    }
                }
            }
            // Lấy lại danh sách cho modal nếu có lỗi
            var sanPhamsChiTiet2 = await _sanPhamChiTietService.GetAllAsync();
            var sanPhams2 = await _sanPhamService.GetAllAsync();
            var list2 = sanPhamsChiTiet2.Select(spct => new
            {
                spct.SanPhamChiTietId,
                TenSanPham = sanPhams2.FirstOrDefault(sp => sp.SanPhamId == spct.SanPhamId)?.TenSanPham ?? "",
                spct.TenMau,
                spct.TenKichCo,
                spct.Gia,
                spct.DuongDan
            }).ToList();
            ViewBag.SanPhamChiTietList = list2;
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _giamGiaService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GiamGiaDTO dto)
        {
            if (id != dto.GiamGiaId) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            var result = await _giamGiaService.UpdateAsync(id, dto);
            if (result.Data)
            {
                TempData["success"] = "Cập nhật chương trình giảm giá thành công!";
                return RedirectToAction("Index");
            }

            if (result.Errors != null)
                foreach (var err in result.Errors)
                    foreach (var msg in err.Value)
                        ModelState.AddModelError(err.Key, msg);
            else
                ModelState.AddModelError("", "Lỗi khi cập nhật!");

            return View(dto);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var item = await _giamGiaService.GetByIdAsync(id);
            if (item == null) return NotFound();

            // Lấy danh sách sản phẩm chi tiết từ service
            var allChiTiet = await _sanPhamChiTietService.GetAllAsync();
            var sanPhams = await _sanPhamService.GetAllAsync();
            var list = allChiTiet
                .Where(spct => item.SanPhamChiTietIds != null && item.SanPhamChiTietIds.Select(x => x.ToString()).Contains(spct.SanPhamChiTietId.ToString()))
                .Select(spct => new {
                    spct.SanPhamChiTietId,
                    TenSanPham = sanPhams.FirstOrDefault(sp => sp.SanPhamId == spct.SanPhamId)?.TenSanPham ?? "",
                    spct.TenMau,
                    spct.TenKichCo,
                    spct.Gia,
                    spct.DuongDan
                }).ToList();
            ViewBag.SanPhamChiTietList = list;
            return View(item);
        }

        // Modal gán sản phẩm chi tiết cho giảm giá
        [HttpGet]
        public async Task<IActionResult> LoadGanSanPhamModal(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null) return Content("Không tìm thấy chương trình!");
            var allChiTiet = await _sanPhamChiTietService.GetAllAsync();
            var sanPhams = await _sanPhamService.GetAllAsync();
            var list = allChiTiet.Select(spct => new
                {
                SanPhamChiTietId = spct.SanPhamChiTietId.ToString(), // ép về string GUID
                TenSanPham = sanPhams.FirstOrDefault(sp => sp.SanPhamId == spct.SanPhamId)?.TenSanPham ?? "",
                spct.TenMau,
                spct.TenKichCo,
                spct.Gia,
                spct.DuongDan,
                DuocChon = giamGia.SanPhamChiTietIds != null && giamGia.SanPhamChiTietIds.Contains(spct.SanPhamChiTietId)
            }).ToList();
            ViewBag.SanPhamChiTietList = list;
            ViewBag.GiamGiaId = id;
            return PartialView("_GanSanPhamModal");
        }

        // POST: Gán sản phẩm chi tiết vào giảm giá (AJAX)
        [HttpPost]
        public async Task<IActionResult> AssignSanPhamChiTiet(Guid id, [FromBody] List<string> sanPhamChiTietIds)
        {
            if (sanPhamChiTietIds == null || !sanPhamChiTietIds.Any())
                return BadRequest("Không có sản phẩm nào được chọn!");
            var guids = sanPhamChiTietIds.Select(x => Guid.Parse(x)).ToList();
            var result = await _giamGiaService.AddSanPhamChiTietToGiamGiaAsync(id, guids);
            if (result) return Ok();
            return BadRequest("Không thể gán sản phẩm!");
        }

        // POST: Xóa giảm giá (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            // IGiamGiaService không có DeleteAsync, nên trả về lỗi hoặc NotImplemented
            return BadRequest("Chức năng xóa chưa được triển khai đúng ở service!");
        }

        [HttpGet]
        public async Task<IActionResult> AddSanPham(Guid id)
        {
            // Lấy chương trình giảm giá
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null) return NotFound();
            // Lấy danh sách sản phẩm chi tiết
            var sanPhamChiTietList = await _sanPhamChiTietService.GetAllAsync();
            var sanPhams = await _sanPhamService.GetAllAsync();
            // Join tên sản phẩm cha
            var list = sanPhamChiTietList.Select(spct => new {
                spct.SanPhamChiTietId,
                TenSanPham = sanPhams.FirstOrDefault(sp => sp.SanPhamId == spct.SanPhamId)?.TenSanPham ?? "",
                spct.TenMau,
                spct.TenKichCo,
                spct.Gia,
                spct.DuongDan
            }).ToList();
            // Lấy các sản phẩm đã được gán cho chương trình này
            var daGan = giamGia.SanPhamChiTietIds ?? new List<Guid>();
            var model = new GiamGiaCreateViewModel
            {
                GiamGia = giamGia,
                SanPhamChiTietIds = daGan.ToList()
            };
            ViewBag.SanPhamChiTietList = list;
            return View("AddSanPham", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSanPham(GiamGiaCreateViewModel model)
        {
            if (model.GiamGia == null || model.GiamGia.GiamGiaId == Guid.Empty)
                return BadRequest();

            if (model.SanPhamChiTietIds != null && model.SanPhamChiTietIds.Any())
            {
                await _giamGiaService.AddSanPhamChiTietToGiamGiaAsync(model.GiamGia.GiamGiaId, model.SanPhamChiTietIds);
                TempData["success"] = "Gán sản phẩm thành công!";
                return RedirectToAction("Details", new { id = model.GiamGia.GiamGiaId });
            }
            else
            {
                TempData["error"] = "Vui lòng chọn ít nhất một sản phẩm!";
                // Lấy lại dữ liệu để render lại view
                var giamGia = await _giamGiaService.GetByIdAsync(model.GiamGia.GiamGiaId);
                var sanPhamChiTietList = await _sanPhamChiTietService.GetAllAsync();
                model.GiamGia = giamGia;
                ViewBag.SanPhamChiTietList = sanPhamChiTietList;
                return View("AddSanPham", model);
            }
        }
    }
}
