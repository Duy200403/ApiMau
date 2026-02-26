using ApiWebsite.Models.Base;
using ApiWebsite.Models.System.DonViDeNghi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    // Các bước trong quy trình
    //Tiến độ từng bước và dữ liệu Form
    [Table("ProcessStep")]
    public class ProcessStep : AuditEntity<Guid>
    {
        public Guid ProcessBranchId { get; set; }
        public virtual ProcessBranch ProcessBranch { get; set; }

        [Column(TypeName = "NVARCHAR(50)")]
        public string StepCode { get; set; } // VD: N1-B1

        [Column(TypeName = "NVARCHAR(250)")]
        public string StepName { get; set; }
        public int Order { get; set; } // Thứ tự thực hiện

        public long TargetDonViId { get; set; } // Phòng ban thực hiện bước này
        public virtual DonViDeNghi TargetDonVi { get; set; }

        public int SlaDays { get; set; }
        public bool RequiresAttachment { get; set; }

        public virtual ICollection<StepAttribute> Attributes { get; set; }
    }
}