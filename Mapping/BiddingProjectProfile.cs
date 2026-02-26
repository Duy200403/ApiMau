using ApiWebsite.Models; // Chứa Entity BiddingProject
using ApiWebsite.Models.Bidding; // Chứa Response
using AutoMapper;

namespace ApiWebsite.Mapping
{
    public class BiddingProjectProfile : Profile
    {
        public BiddingProjectProfile()
        {
            // Map từ Database Entity ra Response trả về API
            CreateMap<ApiWebsite.Models.BiddingProject, BiddingProjectResponse>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.ProcessBranch != null ? src.ProcessBranch.BranchName : ""));

            // Map từ Request truyền vào sang Entity để lưu DB
            CreateMap<BiddingProjectRequest, ApiWebsite.Models.BiddingProject>();
        }
    }
}