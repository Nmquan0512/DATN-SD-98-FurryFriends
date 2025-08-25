using FurryFriends.API.Models.DTO;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FurryFriends.Web.Filter;
using FurryFriends.API.Models;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeEmployee]

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
        private readonly IThongBaoService _thongBaoService;

        public SanPhamController(
            ISanPhamService sanPhamService, ISanPhamChiTietService chiTietService, IAnhService anhService,
            IThuongHieuService thuongHieuService, IKichCoService kichCoService, IMauSacService mauSacService,
            IThanhPhanService thanhPhanService, IChatLieuService chatLieuService, IThongBaoService thongBaoService)
        {
            _sanPhamService = sanPhamService;
            _chiTietService = chiTietService;
            _anhService = anhService;
            _thuongHieuService = thuongHieuService;
            _kichCoService = kichCoService;
            _mauSacService = mauSacService;
            _thanhPhanService = thanhPhanService;
            _chatLieuService = chatLieuService;
            _thongBaoService = thongBaoService;
        }

        // ---------------- GET: Hiển thị danh sách sản phẩm ----------------
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _sanPhamService.GetAllAsync();
                await LoadDropdownData();
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
            await LoadDropdownData(isCreateMode: true);

            var viewModel = new SanPhamFullCreateViewModel
            {
                SanPham = new SanPhamDTO { TrangThai = true },
                ChiTietList = new List<SanPhamChiTietCreateViewModel>()
            };
            return View(viewModel);
        }

        // ---------------- POST: Tạo sản phẩm đầy đủ ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ReadOnly]
        public async Task<IActionResult> Create(SanPhamFullCreateViewModel model)
        {
            ValidateChiTietList(model.ChiTietList);

            if (!ModelState.IsValid)
            {
                await LoadDropdownData(isCreateMode: true);
                return View(model);
            }

            var createResult = await _sanPhamService.CreateAsync(model.SanPham);

            if (!createResult.Success)
            {
                AddApiErrorsToModelState(createResult);
                await LoadDropdownData(isCreateMode: true);
                return View(model);
            }

            var createdSanPham = createResult.Data;
            bool hasVariantError = false;

            foreach (var chiTietVM in model.ChiTietList)
            {
                var chiTietToCreate = new SanPhamChiTietDTO
                {
                    SanPhamId = createdSanPham.SanPhamId,
                    MauSacId = chiTietVM.MauSacId,
                    KichCoId = chiTietVM.KichCoId,
                    SoLuong = chiTietVM.SoLuongTon,
                    Gia = chiTietVM.GiaBan,
                    GiaNhap = chiTietVM.GiaNhap,
                    MoTa = chiTietVM.MoTa,
                    AnhId = chiTietVM.AnhId
                };
                var variantResult = await _chiTietService.CreateAsync(chiTietToCreate);
                if (!variantResult.Success)
                {
                    hasVariantError = true;
                    AddApiErrorsToModelState(variantResult);
                }
            }

            if (hasVariantError)
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi tạo một số biến thể. Vui lòng kiểm tra lại.");
                await LoadDropdownData(isCreateMode: true);
                return View(model);
            }

            TempData["Success"] = "Tạo sản phẩm và các biến thể thành công!";
            await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = $"Thêm sản phẩm: {createdSanPham.TenSanPham}",
                NoiDung = $"Sản phẩm '{createdSanPham.TenSanPham}' đã được thêm vào hệ thống.",
                Loai = "SanPham",
                UserName = "admin",
                NgayTao = DateTime.Now,
                DaDoc = false
            });
            return RedirectToAction("Index");
        }

        // ---------------- GET: Hiển thị form chỉnh sửa ----------------
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var sanPham = await _sanPhamService.GetByIdAsync(id);
            if (sanPham == null) return NotFound();

            var allChiTiet = await _chiTietService.GetAllAsync();
                            var chiTietList = allChiTiet?
                .Where(x => x.SanPhamId == id)
                .Select(x => new SanPhamChiTietCreateViewModel
                {
                    SanPhamChiTietId = x.SanPhamChiTietId,
                    MauSacId = x.MauSacId,
                    KichCoId = x.KichCoId,
                    SoLuongTon = x.SoLuong,
                    GiaBan = x.Gia,
                    GiaNhap = x.GiaNhap,
                    MoTa = x.MoTa,
                    AnhId = x.AnhId,
                    DuongDan = x.DuongDan
                }).ToList() ?? new List<SanPhamChiTietCreateViewModel>();

            await LoadDropdownData(sanPham);

            var viewModel = new SanPhamFullCreateViewModel
            {
                SanPham = sanPham,
                ChiTietList = chiTietList
            };

            return View(viewModel);
        }

        // ---------------- POST: Chỉnh sửa sản phẩm đầy đủ ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ReadOnly]
        public async Task<IActionResult> Edit(SanPhamFullCreateViewModel model)
        {
            var oldSanPham = await _sanPhamService.GetByIdAsync(model.SanPham.SanPhamId);
            if (oldSanPham == null) return NotFound();
            ValidateChiTietList(model.ChiTietList);

            if (!ModelState.IsValid)
            {
                await LoadDropdownData(model.SanPham);
                return View(model);
            }

            var updateResult = await _sanPhamService.UpdateAsync(model.SanPham.SanPhamId, model.SanPham);

            if (!updateResult.Success)
            {
                AddApiErrorsToModelState(updateResult);
                await LoadDropdownData(model.SanPham);
                return View(model);
            }

            await ProcessVariants(model.SanPham.SanPhamId, model.ChiTietList);

            TempData["Success"] = "Cập nhật sản phẩm thành công!";

            var changes = new List<string>();
            if (oldSanPham.TenSanPham != model.SanPham.TenSanPham)
                changes.Add($"Tên: '{oldSanPham.TenSanPham}' → '{model.SanPham.TenSanPham}'");
            if (oldSanPham.TrangThai != model.SanPham.TrangThai)
                changes.Add($"Trạng thái: {(oldSanPham.TrangThai ? "Hoạt động" : "Ngưng")} → {(model.SanPham.TrangThai ? "Hoạt động" : "Ngưng")}");

            // (Bạn có thể bổ sung so sánh biến thể tương tự)

            var userName = HttpContext.Session.GetString("HoTen") ?? "Hệ thống";

            await _thongBaoService.CreateAsync(new ThongBaoDTO
            {
                TieuDe = "Cập nhật sản phẩm",
                NoiDung = changes.Any()
                    ? $"Sản phẩm '{model.SanPham.TenSanPham}' đã được chỉnh sửa: {string.Join(", ", changes)}"
                    : $"Sản phẩm '{model.SanPham.TenSanPham}' đã được chỉnh sửa.",
                Loai = "SanPham",
                UserName = userName,
                NgayTao = DateTime.Now,
                DaDoc = false
            });

            return RedirectToAction("Index");
        }

        // ---------------- GET & POST: Xóa sản phẩm ----------------
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _sanPhamService.GetByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ReadOnly]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _sanPhamService.DeleteAsync(id);
                if (result.Success)
                {
                    TempData["Success"] = "Xóa sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorMessage = result.Errors?.FirstOrDefault().Value.FirstOrDefault() ?? "Xóa sản phẩm thất bại!";
                TempData["Error"] = errorMessage;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa sản phẩm: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ---------------- CÁC HÀM HỖ TRỢ (HELPER METHODS) ----------------

        private async Task ProcessVariants(Guid sanPhamId, List<SanPhamChiTietCreateViewModel> submittedVariants)
        {
            var existingVariants = (await _chiTietService.GetAllAsync() ?? new List<SanPhamChiTietDTO>())
                                     .Where(v => v.SanPhamId == sanPhamId).ToList();

            var submittedVariantIds = submittedVariants.Select(s => s.SanPhamChiTietId).Where(id => id.HasValue).ToList();

            var variantsToDelete = existingVariants.Where(e => !submittedVariantIds.Contains(e.SanPhamChiTietId));
            foreach (var variant in variantsToDelete)
            {
                await _chiTietService.DeleteAsync(variant.SanPhamChiTietId);
            }

            foreach (var submittedVariant in submittedVariants)
            {
                var dto = new SanPhamChiTietDTO
                {
                    SanPhamId = sanPhamId,
                    MauSacId = submittedVariant.MauSacId,
                    KichCoId = submittedVariant.KichCoId,
                    SoLuong = submittedVariant.SoLuongTon,
                    Gia = submittedVariant.GiaBan,
                    GiaNhap = submittedVariant.GiaNhap,
                    MoTa = submittedVariant.MoTa,
                    AnhId = submittedVariant.AnhId,
                    TrangThai = 1
                };

                if (submittedVariant.SanPhamChiTietId.HasValue && submittedVariant.SanPhamChiTietId != Guid.Empty)
                {
                    await _chiTietService.UpdateAsync(submittedVariant.SanPhamChiTietId.Value, dto);
                }
                else
                {
                    await _chiTietService.CreateAsync(dto);
                }
            }
        }

        private void ValidateChiTietList(List<SanPhamChiTietCreateViewModel> chiTietList)
        {
            if (chiTietList == null || !chiTietList.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một biến thể sản phẩm!");
                return;
            }

            for (int i = 0; i < chiTietList.Count; i++)
            {
                var chiTiet = chiTietList[i];
                if (chiTiet.MauSacId == Guid.Empty) ModelState.AddModelError($"ChiTietList[{i}].MauSacId", "Vui lòng chọn màu sắc.");
                if (chiTiet.KichCoId == Guid.Empty) ModelState.AddModelError($"ChiTietList[{i}].KichCoId", "Vui lòng chọn kích cỡ.");
                if (chiTiet.GiaBan <= 0) ModelState.AddModelError($"ChiTietList[{i}].GiaBan", "Giá bán phải lớn hơn 0.");
                if (chiTiet.SoLuongTon < 0) ModelState.AddModelError($"ChiTietList[{i}].SoLuongTon", "Số lượng không được âm.");
            }
        }

        private void AddApiErrorsToModelState<T>(ApiResult<T> result)
        {
            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Key, string.Join(", ", error.Value));
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi không xác định từ API.");
            }
        }

        private async Task LoadDropdownData(SanPhamDTO? sanPham = null, bool isCreateMode = false)
        {
            var allThuongHieu = (await _thuongHieuService.GetAllAsync()).ToList();
            var allChatLieu = (await _chatLieuService.GetAllAsync()).ToList();
            var allThanhPhan = (await _thanhPhanService.GetAllAsync()).ToList();
            var allKichCo = (await _kichCoService.GetAllAsync()).ToList();
            var allMauSac = (await _mauSacService.GetAllAsync()).ToList();

            ViewBag.ThuongHieuList = allThuongHieu
                .Where(th => isCreateMode ? th.TrangThai : (th.TrangThai || th.ThuongHieuId == sanPham?.ThuongHieuId))
                .Select(th => new SelectListItem { Value = th.ThuongHieuId.ToString(), Text = th.TrangThai ? th.TenThuongHieu : $"{th.TenThuongHieu} (Ngưng hoạt động)" });

            ViewBag.ChatLieuList = allChatLieu
                .Where(cl => cl.TrangThai == true)
                .Select(cl => new SelectListItem { Value = cl.ChatLieuId.ToString(), Text = cl.TenChatLieu })
                .ToList();

            ViewBag.ThanhPhanList = allThanhPhan
                  .Where(tp => isCreateMode ? tp.TrangThai : (tp.TrangThai || (sanPham?.ThanhPhanIds?.Contains(tp.ThanhPhanId) ?? false)))
                  .Select(tp => new SelectListItem { Value = tp.ThanhPhanId.ToString(), Text = tp.TrangThai ? tp.TenThanhPhan : $"{tp.TenThanhPhan} (Ngưng hoạt động)" }).ToList();

            ViewBag.DanhSachThuongHieu = allThuongHieu;
            ViewBag.DanhSachChatLieu = allChatLieu;
            ViewBag.DanhSachThanhPhan = allThanhPhan;

            ViewBag.KichCoList = new SelectList(allKichCo.Where(k => isCreateMode ? k.TrangThai : true), "KichCoId", "TenKichCo");
            ViewBag.MauSacList = new SelectList(allMauSac.Where(m => isCreateMode ? m.TrangThai : true), "MauSacId", "TenMau");
            ViewBag.AnhList = await _anhService.GetAllAsync();
        }
    }
}