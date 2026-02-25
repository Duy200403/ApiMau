using ApiWebsite.Models;
using AutoMapper;

namespace ApiWebsite.Mapping
{
  public class AccountProfile : Profile
  {
    public AccountProfile()
    {
      CreateMap<Account, AccountRespone>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
    }
  }
}