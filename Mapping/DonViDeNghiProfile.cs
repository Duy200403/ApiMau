using ApiWebsite.Models.System.DonViDeNghi;
using AutoMapper;

namespace ApiWebsite.Mapping
{
    public class DonViDeNghiProfile : Profile
    {
        public DonViDeNghiProfile()
        {
            CreateMap<DonViDeNghiRequest, DonViDeNghi>();
            CreateMap<DonViDeNghi, DonViDeNghiResponse>();
        }
    }
}
