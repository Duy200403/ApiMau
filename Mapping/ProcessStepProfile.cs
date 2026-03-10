using AutoMapper;
using ApiWebsite.Models; // Chứa Entity ProcessStep
using ApiWebsite.Models.Bidding.ProcessStep;

namespace ApiWebsite.Mapping
{
    public class ProcessStepProfile : Profile
    {
        public ProcessStepProfile()
        {
            // Map từ Request vào Entity để Insert/Update
            CreateMap<ProcessStepRequest, ProcessStep>();

            // Map từ Entity ra Response để trả về FE
            CreateMap<ProcessStep, ProcessStepResponse>()
                // Bắt liên kết mềm: Nếu có Branch thì lấy tên Branch
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.ProcessBranch != null ? src.ProcessBranch.BranchName : null))
                // Nếu Entity của bạn có thuộc tính ảo DonViDeNghi thì map như sau:
                // .ForMember(dest => dest.TargetDonViName, opt => opt.MapFrom(src => src.DonViDeNghi != null ? src.DonViDeNghi.Name : null))
                ;
        }
    }
}