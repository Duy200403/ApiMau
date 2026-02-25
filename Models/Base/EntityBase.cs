using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApiWebsite.Models.Base
{
  public interface IEntityBase<TKey>
  {
    TKey Id { get; set; }
    [Timestamp] byte[] Timestamp { get; set; }
  }

  public interface IDeleteEntity
  {
    bool IsDeleted { get; set; }
  }

  public interface IDeleteEntity<TKey> : IDeleteEntity, IEntityBase<TKey>
  {
  }

  public interface IAuditEntity
  {
    DateTime CreatedDate { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedDate { get; set; }
    string UpdatedBy { get; set; }
    bool IsDeleted { get; set; }
  }
  // code mới
  public interface IAuditEntity<TKey> : IAuditEntity
  {
  }

  public abstract class EntityBase<TKey> : IEntityBase<TKey>
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // [Column(TypeName = "NVARCHAR(100)")]
    public virtual TKey Id { get; set; }
    [Timestamp] public byte[] Timestamp { get; set; }
  }

  public abstract class DeleteEntity<TKey> : EntityBase<TKey>, IDeleteEntity<TKey>
  {
    public bool IsDeleted { get; set; }
  }

  public abstract class AuditEntity<TKey> : EntityBase<TKey>, IAuditEntity<TKey>
  {
    // [Column(TypeName = "NVARCHAR(100)")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    [Column(TypeName = "NVARCHAR(250)")]
    public string CreatedBy { get; set; }
    // [Column(TypeName = "NVARCHAR(100)")]
    public DateTime? UpdatedDate { get; set; }
    [Column(TypeName = "NVARCHAR(250)")]
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
  }
}
