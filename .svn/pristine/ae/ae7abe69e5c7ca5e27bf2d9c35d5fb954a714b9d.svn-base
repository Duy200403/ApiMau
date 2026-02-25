using ApiWebsite.Models;
using AutoMapper;

namespace ApiWebsite.Mapping
{
  public class EmailConfigProfile : Profile
  {
    public EmailConfigProfile()
    {
      //     // map entities to model
      //     CreateMap<Contact, ContactModel>()
      //     .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id.ToString()));

      //     // map model request to entities
      CreateMap<EmailConfigRequest, EmailConfig>();
      CreateMap<EmailConfig, EmailConfigRespone>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
    }
  }
}