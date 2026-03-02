using ApiWebsite.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiWebsite.Models
{
    [Table("ProjectFormSchema")]
    public class ProjectFormSchema : AuditEntity<Guid>
    {
        public Guid BiddingProjectId { get; set; }
        public virtual BiddingProject BiddingProject { get; set; }

        // Chứa toàn bộ cấu trúc các Bước & cấu hình Form (UI Schema) dưới dạng JSON
        public string StepsSchemaJson { get; set; }
    }
}