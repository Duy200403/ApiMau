using ApiWebsite.Models;
using AutoMapper;

namespace ApiWebsite.Mapping
{
  public class FileManagerProfile : Profile
  {
    public FileManagerProfile()
    {
      CreateMap<FileManager, FileManagerRespone>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
    }
  }
}