using ApiWebsite.Models;
using AutoMapper;

namespace ApiWebsite.Mapping
{
  public class LogProfile : Profile
  {
    public LogProfile()
    {
      //     // map entities to model
      //     CreateMap<Contact, ContactModel>()
      //     .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id.ToString()));

      CreateMap<Log, LogRespone>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
    }
  }
}