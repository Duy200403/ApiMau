using System;

namespace ApiWebsite.Models.Bidding.ProjectStepAttachment
{
    public class ProjectStepAttachmentResponse
    {
        public Guid Id { get; set; }
        public Guid ProjectStepId { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}