using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Services.IServices
{
    /// <summary>
    /// Service quản lý các nghiệp vụ của chức năng bán hàng tại quầy.
    /// Chịu trách nhiệm xác thực đầu vào và điều phối logic nghiệp vụ.
    /// </summary>
    public interface IBanHangService
    {
        #region Hóa Đơn
        /// <summary>
        /// Lấy danh sách tất cả hóa đơn (sử dụng cho trang lịch sử).
        /// </summary>
        Task<IEnumerable<HoaDonBanHangDto>> GetAllHoaDonsAsync();

        /// <summary>
        /// Lấy thông tin chi tiết của một hóa đơn bằng ID.
        /// </summary>
        Task<HoaDonBanHangDto> GetHoaDonByIdAsync(Guid id);

        /// <summary>
        /// Tạo một hóa đơn mới (chưa có sản phẩm).
        /// </summary>
        Task<HoaDonBanHangDto> TaoHoaDonAsync(TaoHoaDonRequest request);

        /// <summary>
        /// Hủy một hóa đơn đang ở trạng thái "Chưa thanh toán".
        /// Hành động này sẽ hoàn trả lại số lượng sản phẩm và voucher.
        /// </summary>
        Task<HoaDonBanHangDto> HuyHoaDonAsync(Guid hoaDonId);
        #endregion

        #region Quản lý Sản phẩm trong Hóa đơn
        /// <summary>
        /// Thêm một sản phẩm vào hóa đơn hoặc tăng số lượng nếu đã tồn tại.
        /// </summary>
        Task<HoaDonBanHangDto> ThemSanPhamVaoHoaDonAsync(ThemSanPhamVaoHoaDonRequest request);

        /// <summary>
        /// Cập nhật số lượng của một sản phẩm trong hóa đơn. Nếu số lượng <= 0, sản phẩm sẽ bị xóa.
        /// </summary>
        Task<HoaDonBanHangDto> CapNhatSoLuongSanPhamAsync(Guid hoaDonId, Guid sanPhamChiTietId, int soLuongMoi);

        /// <summary>
        /// Xóa hoàn toàn một sản phẩm khỏi hóa đơn.
        /// </summary>
        Task<HoaDonBanHangDto> XoaSanPhamKhoiHoaDonAsync(Guid hoaDonId, Guid sanPhamChiTietId);
        #endregion

        #region Voucher & Khách hàng
        /// <summary>
        /// Áp dụng một mã voucher cho hóa đơn.
        /// </summary>
        Task<HoaDonBanHangDto> ApDungVoucherAsync(Guid hoaDonId, string maVoucher);

        /// <summary>
        /// Gỡ bỏ voucher hiện tại khỏi hóa đơn.
        /// </summary>
        Task<HoaDonBanHangDto> GoBoVoucherAsync(Guid hoaDonId);

        /// <summary>
        /// Gán một khách hàng đã tồn tại vào hóa đơn.
        /// </summary>
        Task<HoaDonBanHangDto> GanKhachHangAsync(Guid hoaDonId, Guid khachHangId);
        #endregion

        #region Thanh toán
        /// <summary>
        /// Thực hiện thanh toán cho hóa đơn.
        /// </summary>
        Task<HoaDonBanHangDto> ThanhToanHoaDonAsync(ThanhToanRequest request);
        #endregion

        #region Tìm kiếm
        /// <summary>
        /// Tìm kiếm sản phẩm theo tên hoặc mã.
        /// </summary>
        Task<IEnumerable<SanPhamBanHangDto>> TimKiemSanPhamAsync(string keyword);

        /// <summary>
        /// Tìm kiếm khách hàng theo tên hoặc SĐT.
        /// </summary>
        Task<IEnumerable<KhachHangDto>> TimKiemKhachHangAsync(string keyword);

        /// <summary>
        /// Lấy danh sách các voucher hợp lệ có thể áp dụng cho hóa đơn.
        /// </summary>
        Task<IEnumerable<VoucherDto>> TimKiemVoucherHopLeAsync(Guid hoaDonId);
        #endregion

        #region Khách hàng
        /// <summary>
        /// Tạo nhanh một khách hàng mới.
        /// </summary>
        Task<KhachHangDto> TaoKhachHangMoiAsync(TaoKhachHangRequest request);
        #endregion
    }
}