using ApiWebsite.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    // Tài liệu đính kèm
    [Table("ProjectStep")]
    public class ProjectStep : AuditEntity<Guid>
    {
        public Guid BiddingProjectId { get; set; }
        public virtual BiddingProject BiddingProject { get; set; }

        public Guid ProcessStepId { get; set; }
        public virtual ProcessStep ProcessStep { get; set; }

        [Column(TypeName = "NVARCHAR(50)")]
        public string Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [Column(TypeName = "NVARCHAR(250)")]
        public string CompletedByUserId { get; set; }

        public string FormDataJson { get; set; } // Chứa toàn bộ dữ liệu người dùng nhập
        public string Comments { get; set; }

        public virtual ICollection<ProjectStepAttachment> Attachments { get; set; }
    }
}