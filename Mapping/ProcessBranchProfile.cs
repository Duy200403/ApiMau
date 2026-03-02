using ApiWebsite.Models;
using ApiWebsite.Models.Bidding.ProcessBranch;
using AutoMapper;

namespace ApiWebsite.Mapping
{
    public class ProcessBranchProfile : Profile
    {
        public ProcessBranchProfile()
        {
            CreateMap<ProcessBranch, ProcessBranchResponse>();
            CreateMap<ProcessBranchRequest, ProcessBranch>();
        }
    }
}
