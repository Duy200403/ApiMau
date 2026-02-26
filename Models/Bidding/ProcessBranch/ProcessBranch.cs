using ApiWebsite.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    // Nhánh quy trình
    [Table("ProcessBranch")]
    public class ProcessBranch : AuditEntity<Guid>
    {
        [Column(TypeName = "NVARCHAR(100)")]
        public string BranchCode { get; set; } // VD: NHANH_1

        [Column(TypeName = "NVARCHAR(250)")]
        public string BranchName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ProcessStep> Steps { get; set; }
    }
}