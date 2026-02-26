using ApiWebsite.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models.System.DonViDeNghi
{
    // Đơn vị đề nghị
    [Table("DonViDeNghi")]
    public class DonViDeNghi : AuditEntity<long>
    {
        [Column(TypeName = "NVARCHAR(100)")]
        public string Code { get; set; }

        [Column(TypeName = "NVARCHAR(250)")]
        public string Name { get; set; }

        public long? ParentId { get; set; }
        public virtual DonViDeNghi Parent { get; set; }
    }
}