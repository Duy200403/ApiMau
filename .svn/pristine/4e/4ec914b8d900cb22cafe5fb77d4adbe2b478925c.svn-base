using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiWebsite.Models.Base;

namespace ApiWebsite.Models
{

    [Table("EmailConfig")]
    public class EmailConfig : AuditEntity<Guid>
    {
        public EmailConfig()
        {

        }
        [Required(ErrorMessage = "Email không được trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Column(TypeName = "NVARCHAR(250)")]
        public string Email { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "NVARCHAR(100)")]
        public string MailServer { get; set; }
        public int Port { get; set; }
        public bool EnableSSl { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string EmailTitle { get; set; }
    }
}