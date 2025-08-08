// Profiles/GiamGiaProfile.cs
using AutoMapper;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Models.DTO.BanHang;
using FurryFriends.API.Models.DTO.BanHang.Requests;
using FurryFriends.API.Repository;

namespace FurryFriends.API.Profiles
{
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
                .ForMember(dest => dest.NgayCapNhat, opt => opt.MapFrom(src => DateTime.UtcNow))

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
                .ForMember(dest => dest.DiemTichLuy, opt => opt.MapFrom(src => src.DiemKhachHang ?? 0));
            CreateMap<TaoKhachHangRequest, KhachHang>();


            // === V O U C H E R ===
            CreateMap<Voucher, VoucherDto>()
                 .ForMember(dest => dest.MaVoucher, opt => opt.MapFrom(src => src.TenVoucher)); // Giả sử TenVoucher là mã


            // === HÌNH THỨC THANH TOÁN ===
            CreateMap<HinhThucThanhToan, HinhThucThanhToanDto>();



            // Dùng cho API tìm kiếm sản phẩm
            CreateMap<SanPhamChiTiet, SanPhamBanHangDto>()
                .ForMember(dest => dest.SanPhamChiTietId, opt => opt.MapFrom(src => src.SanPhamChiTietId))
                .ForMember(dest => dest.TenSanPham, opt => opt.MapFrom(src => src.SanPham.TenSanPham))
                .ForMember(dest => dest.Gia, opt => opt.MapFrom(src => src.Gia))
                .ForMember(dest => dest.SoLuongTon, opt => opt.MapFrom(src => src.SoLuong))
                .ForMember(dest => dest.TenMauSac, opt => opt.MapFrom(src => src.MauSac.TenMau))
                .ForMember(dest => dest.TenKichCo, opt => opt.MapFrom(src => src.KichCo.TenKichCo));


            // === C H I   T I Ế T   H Ó A   Đ Ơ N ===
            // Sửa lỗi mapping sai, giờ sẽ map từ SanPhamChiTiet
            CreateMap<HoaDonChiTiet, ChiTietHoaDonDto>()
                .ForMember(dest => dest.SanPhamChiTietId, opt => opt.MapFrom(src => src.SanPhamChiTietId))
                .ForMember(dest => dest.TenSanPham, opt => opt.MapFrom(src => src.SanPhamChiTiet.SanPham.TenSanPham))
                .ForMember(dest => dest.MauSac, opt => opt.MapFrom(src => src.SanPhamChiTiet.MauSac.TenMau))
                .ForMember(dest => dest.KichCo, opt => opt.MapFrom(src => src.SanPhamChiTiet.KichCo.TenKichCo))
                .ForMember(dest => dest.Gia, opt => opt.MapFrom(src => src.Gia))
                .ForMember(dest => dest.SoLuong, opt => opt.MapFrom(src => src.SoLuongSanPham))
                .ForMember(dest => dest.ThanhTien, opt => opt.MapFrom(src => src.Gia * src.SoLuongSanPham));


            // === H Ó A   Đ Ơ N   (Chính) ===
            // Mapping quan trọng nhất
            CreateMap<HoaDon, HoaDonBanHangDto>()
                .ForMember(dest => dest.MaHoaDon, opt => opt.MapFrom(src => src.HoaDonId.ToString().Substring(0, 8).ToUpper()))
                .ForMember(dest => dest.TrangThai, opt => opt.MapFrom(src => GetTrangThaiHoaDon(src.TrangThai)))
                .ForMember(dest => dest.KhachHang, opt => opt.MapFrom(src => src.KhachHang))
                .ForMember(dest => dest.HinhThucThanhToan, opt => opt.MapFrom(src => src.HinhThucThanhToan))
                .ForMember(dest => dest.Voucher, opt => opt.MapFrom(src => src.Voucher)) // <--- ĐÂY LÀ PHẦN SỬA LỖI
                .ForMember(dest => dest.ChiTietHoaDon, opt => opt.MapFrom(src => src.HoaDonChiTiets));
            // Các trường TongTien, TienGiam, ThanhTien sẽ được tính toán trong Repository/Service nên không cần map ở đây

        }

        // Helper function để chuyển đổi trạng thái
        private string GetTrangThaiHoaDon(int trangThai)
        {
            return ((TrangThaiHoaDon)trangThai).ToString().Replace("_", " ");
        }
    }
}