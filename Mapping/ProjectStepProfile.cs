using ApiWebsite.Models;
using ApiWebsite.Models.Bidding.ProjectStep;
using AutoMapper;

namespace ApiWebsite.Mapping
{
    public class ProjectStepProfile :Profile
    {
        public ProjectStepProfile()
        {
            CreateMap<ProjectStep, ProjectStepResponse>();
            CreateMap<SubmitFormRequest, ProjectStep>();
        }
    }
}
