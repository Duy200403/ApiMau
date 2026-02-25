using System;
using System.ComponentModel.DataAnnotations.Schema;
using ApiWebsite.Models;
using ApiWebsite.Models.Base;

namespace ApiWebsite.Models
{
    [Table("LoginHistory")]
    public class LoginHistory : EntityBase<Guid>
    {
        [Column(TypeName = "NVARCHAR(250)")]
        public string UserName { get; set; }
        [Column(TypeName = "NVARCHAR(100)")]
        public DateTime LoginDate { get; set; }
        // [Column(TypeName = "NVARCHAR(100)")]
        public Guid AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; }
    }
}