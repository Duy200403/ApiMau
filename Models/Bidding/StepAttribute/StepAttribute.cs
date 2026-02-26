using ApiWebsite.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    // Cấu hình Form động
    [Table("StepAttribute")]
    public class StepAttribute : AuditEntity<Guid>
    {
        public Guid ProcessStepId { get; set; }
        public virtual ProcessStep ProcessStep { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string AttributeCode { get; set; } // VD: so_van_ban

        [Column(TypeName = "NVARCHAR(250)")]    
        public string AttributeName { get; set; } // VD: Số văn bản

        public int InputType { get; set; } // 1: Chữ, 2: Chọn nhiều, 3: Hộp kiểm, 4: Ngày tháng
        public string SelectOptionsJson { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }
}