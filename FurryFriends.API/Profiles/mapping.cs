// Profiles/GiamGiaProfile.cs
using AutoMapper;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.API.Repository;

namespace FurryFriends.API.Profiles
{
    public class SanPhamProfile : Profile
    {
        public SanPhamProfile()
        {
            CreateMap<SanPhamChiTiet, SanPhamChiTietDTO>()
                .ForMember(dest => dest.TenSanPham,
                           opt => opt.MapFrom(src => src.SanPham.TenSanPham))
                .ForMember(dest => dest.TrangThaiSanPham,
                           opt => opt.MapFrom(src => src.SanPham != null ? (bool?)src.SanPham.TrangThai : null))
                .ForMember(dest => dest.TrangThai,
                           opt => opt.MapFrom(src => src.TrangThai));
        }
    }
    public class GiamGiaProfile : Profile
    {
        public GiamGiaProfile()
        {
            // ==========================================================
            // Mapping từ Entity (GiamGia) sang DTO (GiamGiaDTO)
            // ==========================================================
            CreateMap<GiamGia, GiamGiaDTO>()
                // Dạy Mapper cách lấy danh sách ID từ collection con
                .ForMember(dest => dest.SanPhamChiTietIds,
                           opt => opt.MapFrom(src => src.DotGiamGiaSanPhams.Select(d => d.SanPhamChiTietId).ToList()));


            // ==========================================================
            // Mapping từ DTO (GiamGiaDTO) sang Entity (GiamGia)
            // ==========================================================
            CreateMap<GiamGiaDTO, GiamGia>()
                // Bỏ qua mapping trực tiếp cho collection, vì chúng ta sẽ xử lý nó ở AfterMap
                .ForMember(dest => dest.DotGiamGiaSanPhams, opt => opt.Ignore())

                // Luôn cập nhật ngày tháng khi có thay đổi
                .ForMember(dest => dest.NgayCapNhat, opt => opt.MapFrom(src => DateTime.Now))

                // ==========================================================
                // ĐÂY LÀ PHẦN QUAN TRỌNG NHẤT
                // Thực hiện logic sau khi các thuộc tính chính đã được ánh xạ
                // ==========================================================
                .AfterMap((dto, entity) =>
                {
                    // === Đồng bộ danh sách sản phẩm ===

                    // 1. Xóa các sản phẩm không còn trong danh sách mới từ DTO
                    var removedProducts = entity.DotGiamGiaSanPhams
                        .Where(dggsp => !dto.SanPhamChiTietIds.Contains(dggsp.SanPhamChiTietId))
                        .ToList();

                    foreach (var removed in removedProducts)
                    {
                        entity.DotGiamGiaSanPhams.Remove(removed);
                    }

                    // 2. Thêm các sản phẩm mới có trong DTO nhưng chưa có trong Entity
                    var addedProductIds = dto.SanPhamChiTietIds
                        .Where(id => !entity.DotGiamGiaSanPhams.Any(dggsp => dggsp.SanPhamChiTietId == id))
                        .ToList();

                    foreach (var newId in addedProductIds)
                    {
                        entity.DotGiamGiaSanPhams.Add(new DotGiamGiaSanPham
                        {
                            SanPhamChiTietId = newId,
                            // Phần trăm giảm giá sẽ được lấy từ thuộc tính cha (entity)
                            // mà đã được map ở các bước trước.
                            PhanTramGiamGia = entity.PhanTramKhuyenMai
                        });
                    }

                    // 3. Cập nhật phần trăm giảm giá cho các sản phẩm còn lại (nếu có thay đổi)
                    foreach (var dggsp in entity.DotGiamGiaSanPhams)
                    {
                        dggsp.PhanTramGiamGia = entity.PhanTramKhuyenMai;
                    }
                });
        }
    }
    public class BanHangMappingProfile : Profile
    {
        public BanHangMappingProfile()
        {
            // === K H Á C H   H À N G ===
            CreateMap<KhachHang, KhachHangDto>()
                .ForMember(dest => dest.DiemTichLuy, opt => opt.MapFrom(src => src.DiemKhachHang ?? 0))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailCuaKhachHang))
                .ForMember(dest => dest.LaKhachLe, opt => opt.MapFrom(src => src.TenKhachHang == "Khách lẻ"));

            CreateMap<TaoKhachHangRequest, KhachHang>()
     .ForMember(dest => dest.EmailCuaKhachHang, opt => opt.MapFrom(src => src.Email));
            CreateMap<DiaChiMoiDto, DiaChiKhachHang>();

            // === V O U C H E R ===
            CreateMap<Voucher, VoucherDto>()
                 .ForMember(dest => dest.MaVoucher, opt => opt.MapFrom(src => src.TenVoucher))
                 .ForMember(dest => dest.NgayHetHan, opt => opt.MapFrom(src => src.NgayKetThuc))
                 .ForMember(dest => dest.GiaTriGiamToiDa, opt => opt.MapFrom(src => src.GiaTriGiamToiDa ?? 0));

            // === HÌNH THỨC THANH TOÁN ===
            CreateMap<HinhThucThanhToan, HinhThucThanhToanDto>();

            // ==========================================================
            // <<< SỬA LỖI 1: Mapping cho SanPhamBanHangDto >>>
            // ==========================================================
            CreateMap<SanPhamChiTiet, SanPhamBanHangDto>()
                .ForMember(dest => dest.TenSanPham, opt => opt.MapFrom(src => src.SanPham.TenSanPham))
                .ForMember(dest => dest.SoLuongTon, opt => opt.MapFrom(src => src.SoLuong))
                .ForMember(dest => dest.TenMauSac, opt => opt.MapFrom(src => src.MauSac.TenMau))
                .ForMember(dest => dest.TenKichCo, opt => opt.MapFrom(src => src.KichCo.TenKichCo))
              .ForMember(dest => dest.HinhAnh, opt => opt.MapFrom(src => src.Anh != null ? src.Anh.DuongDan : null));

            // <<< THÊM MỚI: Dạy Mapper cách lấy ảnh cho DTO tìm kiếm >>>


            // ==========================================================
            // <<< SỬA LỖI 2: Mapping cho ChiTietHoaDonDto >>>
            // ==========================================================
            CreateMap<HoaDonChiTiet, ChiTietHoaDonDto>()
                .ForMember(dest => dest.SanPhamChiTietId, opt => opt.MapFrom(src => src.SanPhamChiTietId))
                .ForMember(dest => dest.TenSanPham, opt => opt.MapFrom(src => src.SanPhamChiTiet.SanPham.TenSanPham))
                .ForMember(dest => dest.MauSac, opt => opt.MapFrom(src => src.SanPhamChiTiet.MauSac.TenMau))
                .ForMember(dest => dest.KichCo, opt => opt.MapFrom(src => src.SanPhamChiTiet.KichCo.TenKichCo))
                .ForMember(dest => dest.Gia, opt => opt.MapFrom(src => src.SanPhamChiTiet.Gia)) // Giá gốc từ sản phẩm
                .ForMember(dest => dest.GiaBan, opt => opt.MapFrom(src => src.GiaLucMua ?? src.Gia)) // Giá bán thực tế
                .ForMember(dest => dest.SoLuong, opt => opt.MapFrom(src => src.SoLuongSanPham))
                .ForMember(dest => dest.ThanhTien, opt => opt.MapFrom(src => (src.GiaLucMua ?? src.Gia) * src.SoLuongSanPham))
                .ForMember(dest => dest.SoLuongTon, opt => opt.MapFrom(src => src.SanPhamChiTiet.SoLuong))
                // <<< THÊM MỚI: Dạy Mapper cách lấy ảnh cho chi tiết hóa đơn >>>
                .ForMember(dest => dest.HinhAnh, opt => opt.MapFrom(src => src.SanPhamChiTiet.Anh != null ? src.SanPhamChiTiet.Anh.DuongDan : null));

            // === H Ó A   Đ Ơ N   (Chính) ===
            CreateMap<HoaDon, HoaDonBanHangDto>()
                .ForMember(dest => dest.MaHoaDon, opt => opt.MapFrom(src => src.HoaDonId.ToString().Substring(0, 8).ToUpper()))
                .ForMember(dest => dest.TrangThai, opt => opt.MapFrom(src => ((TrangThaiHoaDon)src.TrangThai).ToString().Replace("_", " ")))
                .ForMember(dest => dest.KhachHang, opt => opt.MapFrom(src => src.KhachHang))
                .ForMember(dest => dest.HinhThucThanhToan, opt => opt.MapFrom(src => src.HinhThucThanhToan))
                .ForMember(dest => dest.Voucher, opt => opt.MapFrom(src => src.Voucher))
                .ForMember(dest => dest.ChiTietHoaDon, opt => opt.MapFrom(src => src.HoaDonChiTiets))
                .ForMember(dest => dest.TongTien, opt => opt.MapFrom(src => src.TongTien))
                .ForMember(dest => dest.ThanhTien, opt => opt.MapFrom(src => src.TongTienSauKhiGiam))
                .ForMember(dest => dest.TienGiam, opt => opt.MapFrom(src => src.TongTien - src.TongTienSauKhiGiam))
                // ✅ Thêm mapping cho các trường snapshot khách hàng
                .ForMember(dest => dest.TenCuaKhachHang, opt => opt.MapFrom(src => src.TenCuaKhachHang))
                .ForMember(dest => dest.SdtCuaKhachHang, opt => opt.MapFrom(src => src.SdtCuaKhachHang))
                .ForMember(dest => dest.EmailCuaKhachHang, opt => opt.MapFrom(src => src.EmailCuaKhachHang))
                // ✅ Thêm mapping cho địa chỉ giao hàng lúc mua
                .ForMember(dest => dest.DiaChiGiaoHangLucMua, opt => opt.MapFrom(src => src.DiaChiGiaoHangLucMua));
        }
    }
}