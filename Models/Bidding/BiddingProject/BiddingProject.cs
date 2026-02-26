using ApiWebsite.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    // Gói thầu
    [Table("BiddingProject")]
    public class BiddingProject : AuditEntity<Guid>
    {
        [Column(TypeName = "NVARCHAR(100)")]
        public string ProjectCode { get; set; }

        [Column(TypeName = "NVARCHAR(500)")]
        public string ProjectName { get; set; }

        public Guid ProcessBranchId { get; set; }
        public virtual ProcessBranch ProcessBranch { get; set; }

        public int CurrentStepOrder { get; set; }

        [Column(TypeName = "NVARCHAR(50)")]
        public string Status { get; set; } // Đang chờ, Đang chạy, Hoàn thành, Hủy

        public virtual ICollection<ProjectStep> ProjectSteps { get; set; }
    }
}