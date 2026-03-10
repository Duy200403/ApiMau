using AutoMapper;
using ApiWebsite.Models.Bidding.StepAttribute;

namespace ApiWebsite.Mapping
{
    public class StepAttributeProfile : Profile
    {
        public StepAttributeProfile()
        {
            CreateMap<StepAttributeRequest, ApiWebsite.Models.StepAttribute>();
            CreateMap<ApiWebsite.Models.StepAttribute, StepAttributeResponse>();
        }
    }
}