using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly ISanPhamChiTietService _chiTietService;
        private readonly IAnhService _anhService;
        private readonly IThuongHieuService _thuongHieuService;
        private readonly IThanhPhanService _thanhPhanService;
        private readonly IChatLieuService _chatLieuService;
        private readonly IMauSacService _mauSacService;
        private readonly IKichCoService _kichCoService;

        public SanPhamController(
            ISanPhamService sanPhamService,
            ISanPhamChiTietService chiTietService,
            IAnhService anhService,
            IThuongHieuService thuongHieuService,
            IThanhPhanService thanhPhanService,
            IChatLieuService chatLieuService,
            IMauSacService mauSacService,
            IKichCoService kichCoService)
        {
            _sanPhamService = sanPhamService;
            _chiTietService = chiTietService;
            _anhService = anhService;
            _thuongHieuService = thuongHieuService;
            _thanhPhanService = thanhPhanService;
            _chatLieuService = chatLieuService;
            _mauSacService = mauSacService;
            _kichCoService = kichCoService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _sanPhamService.GetAllAsync();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();

            var model = new SanPhamFullCreateViewModel
            {
                SanPham = new SanPhamDTO(),
                ChiTietList = new List<SanPhamChiTietCreateViewModel> { new() }
            };

            return View(model);
        }

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
            var sanPham = await _sanPhamService.CreateAsync(model.SanPham);
            if (sanPham == null)
            {
                ModelState.AddModelError("", "❌ Không thể tạo sản phẩm chính.");
                await LoadDropdownData();
                return View(model);
            }

            // 2. Tạo biến thể sản phẩm và upload ảnh
            foreach (var chiTiet in model.ChiTietList)
            {
                var chiTietDTO = new SanPhamChiTietDTO
                {
                    SanPhamId = sanPham.SanPhamId,
                    MauSacId = chiTiet.MauSacId,
                    KichCoId = chiTiet.KichCoId,
                    SoLuong = chiTiet.SoLuongTon,
                    Gia = chiTiet.GiaBan,
                    MoTa = chiTiet.MoTa
                };

                var chiTietResult = await _chiTietService.CreateAsync(chiTietDTO);
                if (!chiTietResult.Success || chiTietResult.Data == null)
                {
                    ModelState.AddModelError("", "❌ Không thể tạo biến thể sản phẩm.");
                    continue;
                }

                var chiTietId = chiTietResult.Data.SanPhamChiTietId;

                // 3. Upload ảnh cho biến thể
                if (chiTiet.Files != null && chiTiet.Files.Any())
                {
                    foreach (var file in chiTiet.Files)
                    {
                        try
                        {
                            var upload = await _anhService.UploadAsync(file, chiTietId);
                            if (upload == null)
                                ModelState.AddModelError("", $"❌ Upload thất bại: {file.FileName}");
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"❌ Lỗi khi upload ảnh: {file.FileName} - {ex.Message}");
                        }
                    }
                }
            }

            TempData["Success"] = "✅ Tạo sản phẩm và các biến thể thành công!";
            return RedirectToAction("Index");
        }

        private async Task LoadDropdownData()
        {
            ViewBag.ThuongHieuList = await _thuongHieuService.GetAllAsync();
            ViewBag.ThanhPhanList = await _thanhPhanService.GetAllAsync();
            ViewBag.ChatLieuList = await _chatLieuService.GetAllAsync();
            ViewBag.MauSacList = await _mauSacService.GetAllAsync();
            ViewBag.KichCoList = await _kichCoService.GetAllAsync();
        }
    }
}
