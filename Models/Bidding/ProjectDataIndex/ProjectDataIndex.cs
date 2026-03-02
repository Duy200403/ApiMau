using ApiWebsite.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    [Table("ProjectDataIndex")]
    public class ProjectDataIndex : AuditEntity<Guid>
    {
        public Guid BiddingProjectId { get; set; }
        public virtual BiddingProject BiddingProject { get; set; }

        public Guid ProjectStepId { get; set; }
        public virtual ProjectStep ProjectStep { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string FieldCode { get; set; } // Mã trường cấu hình (VD: ten_goi, gia_tri_du_kien)

        // Tách riêng các kiểu dữ liệu để dễ Search (Lọc) và Sum (Tính toán số liệu)

        public string TextValue { get; set; } // Lưu chuỗi (Dùng để search text LIKE %%)

        [Column(TypeName = "decimal(18,4)")]
        public decimal? NumberValue { get; set; } // Lưu số (Dùng để tính Tổng tiền, Max, Min)

        public DateTime? DateValue { get; set; } // Lưu ngày tháng (Dùng để lọc khoảng thời gian Từ ngày - Đến ngày)
    }
}