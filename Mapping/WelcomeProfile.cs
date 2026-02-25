using System;
using ApiWebsite.Models;
using AutoMapper;

namespace ApiWebsite.Mapping
{
  public class WelcomeProfile : Profile
  {
    public WelcomeProfile()
    {
      //     // map entities to model
      //     CreateMap<Contact, ContactModel>()
      //     .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id.ToString()));

      //     // map model request to entities
       CreateMap<WelcomeRequest, Welcome>();
      CreateMap<Welcome, WelcomeResponse>();
    }
  }
}