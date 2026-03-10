using AutoMapper;
using ApiWebsite.Models.Bidding.ProjectStepAttachment;

namespace ApiWebsite.Mapping
{
    public class ProjectStepAttachmentProfile : Profile
    {
        public ProjectStepAttachmentProfile()
        {
            CreateMap<ProjectStepAttachmentRequest, ApiWebsite.Models.ProjectStepAttachment>();
            CreateMap<ApiWebsite.Models.ProjectStepAttachment, ProjectStepAttachmentResponse>();
        }
    }
}