using ApiWebsite.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ApiWebsite.Models
{

    [Table("Welcome")]
    public class Welcome : AuditEntity<Guid>
    {
        public Welcome()
        {
        }
        public string Ten { get; set; }
        public string Trangthai { get; set; }
        
    }
}