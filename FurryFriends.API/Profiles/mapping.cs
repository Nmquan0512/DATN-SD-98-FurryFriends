// Profiles/GiamGiaProfile.cs
using AutoMapper;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;

namespace FurryFriends.API.Profiles
{
    public class GiamGiaProfile : Profile
    {
        public GiamGiaProfile()
        {
            // Mapping từ Entity sang DTO
            CreateMap<GiamGia, GiamGiaDTO>()
                .ForMember(dest => dest.SanPhamChiTietIds,
                    opt => opt.MapFrom(src => src.DotGiamGiaSanPhams.Select(d => d.SanPhamChiTietId)));

            // Mapping từ DTO sang Entity
            CreateMap<GiamGiaDTO, GiamGia>()
                .ForMember(dest => dest.DotGiamGiaSanPhams, opt => opt.Ignore())
                .ForMember(dest => dest.NgayCapNhat, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}