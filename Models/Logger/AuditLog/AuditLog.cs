using ApiWebsite.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models.Logger.AuditLog
{
    // Bảng lưu trữ lịch sử thay đổi dữ liệu
    [Table("AuditLog")]
    public class AuditLog : EntityBase<Guid>
    {
        [Column(TypeName = "NVARCHAR(250)")]
        public string UserId { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string TableName { get; set; }

        [Column(TypeName = "NVARCHAR(50)")]
        public string Action { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string PrimaryKey { get; set; }

        public string OldValues { get; set; } // Chuỗi JSON
        public string NewValues { get; set; } // Chuỗi JSON
        public string AffectedColumns { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}