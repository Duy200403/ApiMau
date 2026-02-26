using ApiWebsite.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    [Table("ProjectStepAttachment")]
    public class ProjectStepAttachment : AuditEntity<Guid>
    {
        public Guid ProjectStepId { get; set; }
        public virtual ProjectStep ProjectStep { get; set; }

        [Column(TypeName = "NVARCHAR(500)")]
        public string FileName { get; set; }
        public string FilePath { get; set; }

        [Column(TypeName = "NVARCHAR(50)")]
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
    }
}