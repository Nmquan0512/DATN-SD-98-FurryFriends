using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Models.VNPay;
using FurryFriends.Web.Service.IService;
using FurryFriends.Web.Services;
using FurryFriends.Web.Services.IService;
using FurryFriends.Web.Services.IServices;
using FurryFriends.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurryFriends.Web.Controllers
{
    public class GioHangsController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IGioHangService _gioHangService;
        private readonly IVoucherService _voucherService;
        private readonly IKhachHangService _khachHangService;
        private readonly IHinhThucThanhToanService _hinhThucThanhToanService;
        private readonly IDiaChiKhachHangService _diaChiKhachHangService;
        private Guid GetKhachHangId()
        {
            var sessionValue = HttpContext.Session.GetString("KhachHangId"); // ✅ Sửa tại đây
            if (string.IsNullOrEmpty(sessionValue) || !Guid.TryParse(sessionValue, out var khachHangId))
            {
                // Nếu chưa đăng nhập, chuyển về trang đăng nhập
                Response.Redirect("/KhachHangLogin/DangNhap"); // ✅ Dùng Response.Redirect vì return trong void không có tác dụng ở đây
                throw new Exception("Người dùng chưa đăng nhập.");
            }
            return khachHangId;
        }


        public GioHangsController(IGioHangService gioHangService, IVoucherService voucherService, IKhachHangService khachHangService, IHinhThucThanhToanService hinhThucThanhToanService, IDiaChiKhachHangService diaChiKhachHangService, IVnPayService vnPayService)
        {
            _gioHangService = gioHangService;
            _voucherService = voucherService;
            _khachHangService = khachHangService;
            _hinhThucThanhToanService = hinhThucThanhToanService;
            _diaChiKhachHangService = diaChiKhachHangService;
            _vnPayService = vnPayService;
        }

        public async Task<IActionResult> Index(Guid? voucherId = null)
        {
            var khachHangId = GetKhachHangId();
            var gioHang = await _gioHangService.GetGioHangAsync(khachHangId);

            ViewBag.Vouchers = await _voucherService.GetAllAsync(); // Trả về List<VoucherDTO>
            ViewBag.VoucherId = voucherId;

            if (voucherId.HasValue)
            {
                ViewBag.TienSauGiam = await _gioHangService.TinhTongTienSauVoucher(khachHangId, voucherId.Value);
            }
            ViewBag.HinhThucThanhToanList = await _hinhThucThanhToanService.GetAllAsync();
            var diaChiList = await _diaChiKhachHangService.GetByKhachHangIdAsync(khachHangId);
            var diaChiSelectList = diaChiList.Select(d => new SelectListItem
            {
                Value = d.DiaChiId.ToString(),
                Text = $"{d.ThanhPho} - {d.PhuongXa} - {d.MoTa} - {d.TenDiaChi}"
            }).ToList();

            ViewBag.DiaChiList = diaChiSelectList;


            return View(gioHang);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu muốn quay lại trang sản phẩm chi tiết kèm lỗi, xử lý thêm tại đây
                return RedirectToAction("Index", "GioHangs");
            }

            var khachHangId = GetKhachHangId();

            var dto = new AddToCartDTO
            {
                KhachHangId = khachHangId,
                SanPhamChiTietId = model.SanPhamChiTietId,
                SoLuong = model.SoLuong
            };

            await _gioHangService.AddToCartAsync(dto);

            return RedirectToAction("Index", new { voucherId = ViewBag.VoucherId });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid chiTietId, int soLuong, Guid? voucherId)
        {
            await _gioHangService.UpdateSoLuongAsync(chiTietId, soLuong);
            return RedirectToAction("Index", new { voucherId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid chiTietId, Guid? voucherId)
        {
            await _gioHangService.RemoveAsync(chiTietId);
            return RedirectToAction("Index", new { voucherId }); // Truyền lại voucherId
        }


        [HttpGet]
        public async Task<IActionResult> ThanhToan(Guid khachHangId, Guid? voucherId)
        {
            var khachHang = await _khachHangService.GetByIdAsync(khachHangId);
            if (khachHang == null)
                return NotFound("Không tìm thấy khách hàng.");

            // Lấy danh sách hình thức thanh toán
            var hinhThucThanhToans = await _hinhThucThanhToanService.GetAllAsync();
            ViewBag.HinhThucThanhToanList = new SelectList(hinhThucThanhToans, "HinhThucThanhToanId", "TenHinhThuc");

            var diaChiList = await _diaChiKhachHangService.GetByKhachHangIdAsync(khachHangId);
            var diaChiSelectList = diaChiList.Select(d => new SelectListItem
            {
                Value = d.DiaChiId.ToString(),
                Text = $"{d.ThanhPho} - {d.PhuongXa} - {d.MoTa} - {d.TenDiaChi}"
            }).ToList();

            ViewBag.DiaChiList = diaChiSelectList;


            // Gửi DTO rỗng ban đầu để bind vào form
            var model = new ThanhToanDTO
            {
                KhachHangId = khachHangId,
                VoucherId = voucherId,
                TaiKhoanId = khachHang.TaiKhoanId ?? Guid.Empty,
                TenCuaKhachHang = khachHang.TenKhachHang,
                SdtCuaKhachHang = khachHang.SDT,
                EmailCuaKhachHang = khachHang.EmailCuaKhachHang,
                LoaiHoaDon = "Online",
                GhiChu = "Hóa đơn Online",
                NhanVienId = null
            };

            return View(model); // Trả về View có form để user chọn
        }

        [HttpPost]
        public async Task<IActionResult> ThanhToan(ThanhToanDTO dto)
        {
            // Validate
            if (dto.HinhThucThanhToanId == Guid.Empty)
            {
                ModelState.AddModelError("HinhThucThanhToanId", "Vui lòng chọn hình thức thanh toán.");
            }

            // Validate địa chỉ
            if (!dto.DiaChiId.HasValue || dto.DiaChiId == Guid.Empty)
            {
                ModelState.AddModelError("DiaChiId", "Vui lòng chọn địa chỉ giao hàng.");
            }

            // Nếu có lỗi thì load lại ViewBag và return View
            if (!ModelState.IsValid)
            {
                ViewBag.HinhThucThanhToanList = new SelectList(await _hinhThucThanhToanService.GetAllAsync(), "HinhThucThanhToanId", "TenHinhThuc");
                var diaChiList = await _diaChiKhachHangService.GetByKhachHangIdAsync(dto.KhachHangId);
                var diaChiSelectList = diaChiList.Select(d => new SelectListItem
                {
                    Value = d.DiaChiId.ToString(),
                    Text = $"{d.ThanhPho} - {d.PhuongXa} - {d.MoTa} - {d.TenDiaChi}"
                }).ToList();

                ViewBag.DiaChiList = diaChiSelectList;

                return View(dto);
            }

            // 👉 Lấy thông tin khách hàng từ database
            var khachHang = await _khachHangService.GetByIdAsync(dto.KhachHangId);
            if (khachHang == null)
                return NotFound("Không tìm thấy khách hàng.");

            // 👉 Gán lại dữ liệu cho dto
            dto.TenCuaKhachHang = khachHang.TenKhachHang;
            dto.EmailCuaKhachHang = khachHang.EmailCuaKhachHang;
            dto.SdtCuaKhachHang = khachHang.SDT;
            var taiKhoanIdString = HttpContext.Session.GetString("TaiKhoanId");
            if (taiKhoanIdString == null || !Guid.TryParse(taiKhoanIdString, out Guid taiKhoanId))
            {
                TempData["Loi"] = "Bạn cần đăng nhập để thanh toán.";
                return RedirectToAction("Index", "DangNhap");
            }

            dto.TaiKhoanId = taiKhoanId;

            dto.LoaiHoaDon = "Online";
            dto.GhiChu = "Hóa đơn Online";
            dto.NhanVienId = null; // hoặc gán nhân viên nếu có logic khác

            var hinhThuc = await _hinhThucThanhToanService.GetByIdAsync(dto.HinhThucThanhToanId);
            if (hinhThuc != null && hinhThuc.TenHinhThuc.Equals("Chuyển khoản ngân hàng", StringComparison.OrdinalIgnoreCase))
            {
                // Lấy tổng tiền từ giỏ hàng
                var gioHang = await _gioHangService.GetGioHangAsync(dto.KhachHangId);
                decimal tongTien = 0;
                if (dto.VoucherId.HasValue && dto.VoucherId != Guid.Empty)
                {
                    tongTien = await _gioHangService.TinhTongTienSauVoucher(dto.KhachHangId, dto.VoucherId.Value);
                }
                else
                {
                    tongTien = gioHang.GioHangChiTiets.Sum(x => x.ThanhTien);
                }



                // Lưu tạm DTO vào Session để callback xử lý
                HttpContext.Session.SetString("ThanhToanDTO", System.Text.Json.JsonSerializer.Serialize(dto));

                var paymentModel = new PaymentInformationModel
                {
                    Amount = (double)tongTien,
                    OrderDescription = $"Thanh toán đơn hàng cho {dto.TenCuaKhachHang}",
                    Name = dto.TenCuaKhachHang
                };

                var url = _vnPayService.CreatePaymentUrl(paymentModel, HttpContext);
                return Redirect(url);
            }

            var result = await _gioHangService.ThanhToanAsync(dto);
            return View("KetQuaThanhToan", (ThanhToanResultViewModel)result);
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        //[HttpGet]
        //public IActionResult PaymentCallbackVnpay()
        //{
        //    var response = _vnPayService.PaymentExecute(Request.Query);

        //    return Json(response);
        //}

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success)
            {
                // Lấy lại DTO từ Session
                var dtoJson = HttpContext.Session.GetString("ThanhToanDTO");
                if (!string.IsNullOrEmpty(dtoJson))
                {
                    var dto = System.Text.Json.JsonSerializer.Deserialize<ThanhToanDTO>(dtoJson);
                    if (dto != null)
                    {
                        var result = await _gioHangService.ThanhToanAsync(dto);
                        HttpContext.Session.Remove("ThanhToanDTO"); // Xóa sau khi xử lý
                        return View("KetQuaThanhToan", (ThanhToanResultViewModel)result);
                    }
                }
            }

            return View("ThanhToanThatBai", response);
        }

    }

}
