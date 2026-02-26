using ApiWebsite.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models.System.Permission
{
    // Phân quyền dựa trên Role và Resource
    [Table("Permission")]
    public class Permission : AuditEntity<Guid>
    {
        [Column(TypeName = "NVARCHAR(100)")]
        public string RoleName { get; set; } // Khớp với chuỗi Roles trong bảng Account

        [Column(TypeName = "NVARCHAR(100)")]
        public string Resource { get; set; } // Tên Module (VD: BiddingProject)

        [Column(TypeName = "NVARCHAR(50)")]
        public string Action { get; set; } // Hành động (VD: Read, Create, Update)
    }
}