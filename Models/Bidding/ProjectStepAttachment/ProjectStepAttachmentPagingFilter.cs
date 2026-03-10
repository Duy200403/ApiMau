using ApiWebsite.Common;
using System;

namespace ApiWebsite.Models.Bidding.ProjectStepAttachment
{
    public class ProjectStepAttachmentPagingFilter : PagingRequestBase
    {
        // Bắt buộc phải có ID của bước để query danh sách file của bước đó
        public Guid? ProjectStepId { get; set; }
        public string Keyword { get; set; }
        public string Status { get; set; }
    }
}
