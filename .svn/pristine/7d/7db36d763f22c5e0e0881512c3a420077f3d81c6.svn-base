using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiWebsite.Models.Base;

namespace ApiWebsite.Models
{
    [Table("FileManager")]
    public class FileManager : AuditEntity<Guid>
    {
        [Column(TypeName = "NVARCHAR(250)")]
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string PhysicalPath { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string PhysicalThumbPath { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string FilePath { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string ThumbPath { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string FileType { get; set; }
        public int FileSizeInKB { get; set; }
    }
}