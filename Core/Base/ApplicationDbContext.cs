using ApiWebsite.Helper;
using ApiWebsite.Models;
using ApiWebsite.Models.Base;
using ApiWebsite.Models.Logger.AuditLog;
using ApiWebsite.Models.System.DonViDeNghi;
using ApiWebsite.Models.System.Permission;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ApiWebsite.Core.Base
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        // the dbset property will tell ef core that we have
        // a table that needs to be created if doesnt exist
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<FileManager> FileManager { get; set; }
        public virtual DbSet<EmailConfig> EmailConfig { get; set; }
        public virtual DbSet<LoginHistory> LoginHistory { get; set; }
        public virtual DbSet<Welcome> Welcome { get; set; }
        public DbSet<DonViDeNghi> DonViDeNghi { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ProcessBranch> ProcessBranches { get; set; }
        public DbSet<ProcessStep> ProcessSteps { get; set; }
        public DbSet<StepAttribute> StepAttributes { get; set; }
        public DbSet<BiddingProject> BiddingProjects { get; set; }
        public DbSet<ProjectStep> ProjectSteps { get; set; }
        public DbSet<ProjectStepAttachment> ProjectStepAttachments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // concurrent write 
            modelBuilder.Entity<Account>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<Log>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<FileManager>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<LoginHistory>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<EmailConfig>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<Welcome>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<DonViDeNghi>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<ProcessBranch>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<ProcessStep>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<StepAttribute>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<BiddingProject>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<ProjectStep>().Property(c => c.Timestamp).IsRowVersion();
        
            modelBuilder
              .Entity<Log>()
              .Property(e => e.LogLevel)
              .HasConversion(
                  v => v.ToString(),
                  v => (LogLevelWebInfo)Enum.Parse(typeof(LogLevelWebInfo), v));
            // Tắt tính năng Cascade Delete cho ProjectStep -> ProcessStep để tránh lỗi vòng lặp SQL
            modelBuilder.Entity<ProjectStep>()
                .HasOne(x => x.ProcessStep)
                .WithMany()
                .HasForeignKey(x => x.ProcessStepId)
                .OnDelete(DeleteBehavior.Restrict);

            // (Tùy chọn thêm để an toàn tuyệt đối) Tắt Cascade cho BiddingProject -> ProcessBranch
            modelBuilder.Entity<BiddingProject>()
                .HasOne(x => x.ProcessBranch)
                .WithMany()
                .HasForeignKey(x => x.ProcessBranchId)
                .OnDelete(DeleteBehavior.Restrict);
            // modelBuilder.Entity<BookDevice>()
            //     .HasOne(wa => wa.Account)
            //     .WithMany()
            //     .HasForeignKey(wa => wa.AccountId)
            //     .OnDelete(DeleteBehavior.Cascade);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IDeleteEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(ConvertFilterExpression<IDeleteEntity>(e => !e.IsDeleted, entityType.ClrType));
                }
            }
        }
        // HÀM HỖ TRỢ BỘ LỌC
            private static LambdaExpression ConvertFilterExpression<TInterface>(Expression<Func<TInterface, bool>> filterExpression, Type entityType)
                    {
                        var newParam = Expression.Parameter(entityType);
                        var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);
                        return Expression.Lambda(newBody, newParam);
                    }

            // GHI ĐÈ HÀM SaveChangesAsync ĐỂ TỰ ĐỘNG GHI NHẬT KÝ
            public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Hệ thống";

                ChangeTracker.DetectChanges();
                var auditLogs = new List<AuditLog>();

                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                        continue;

                    // Xử lý tự động gán ngày tháng và người tạo/sửa
                    if (entry.Entity is IAuditEntity auditableEntity)
                    {
                        switch (entry.State)
                        {
                            case EntityState.Added:
                                auditableEntity.CreatedDate = DateTime.Now;
                                auditableEntity.CreatedBy = currentUserId;
                                auditableEntity.IsDeleted = false;
                                break;
                            case EntityState.Modified:
                                auditableEntity.UpdatedDate = DateTime.Now;
                                auditableEntity.UpdatedBy = currentUserId;
                                break;
                            case EntityState.Deleted:
                                // Đổi Xóa Cứng thành Xóa Mềm
                                entry.State = EntityState.Modified;
                                auditableEntity.IsDeleted = true;
                                auditableEntity.UpdatedDate = DateTime.Now;
                                auditableEntity.UpdatedBy = currentUserId;
                                break;
                        }
                    }

                    // Ghi chi tiết thay đổi dữ liệu vào bảng AuditLog (không ghi log chính bảng AuditLog và Log hệ thống)
                    if (!(entry.Entity is AuditLog) && !(entry.Entity is Log))
                    {
                        var auditLog = new AuditLog
                        {
                            TableName = entry.Metadata.GetTableName(),
                            UserId = currentUserId,
                            Action = entry.State == EntityState.Added ? "Create" :
                                     (entry.State == EntityState.Modified && entry.Entity is IDeleteEntity del && del.IsDeleted ? "Delete" : "Update"),
                        };

                        var oldValues = new Dictionary<string, object>();
                        var newValues = new Dictionary<string, object>();
                        var changedCols = new List<string>();

                        foreach (var prop in entry.Properties)
                        {
                            string propName = prop.Metadata.Name;
                            if (prop.Metadata.IsPrimaryKey())
                            {
                                auditLog.PrimaryKey = prop.CurrentValue?.ToString();
                                continue;
                            }

                            switch (entry.State)
                            {
                                case EntityState.Added:
                                    newValues[propName] = prop.CurrentValue;
                                    break;
                                case EntityState.Deleted:
                                    oldValues[propName] = prop.OriginalValue;
                                    break;
                                case EntityState.Modified:
                                    if (prop.IsModified && prop.OriginalValue?.ToString() != prop.CurrentValue?.ToString())
                                    {
                                        changedCols.Add(propName);
                                        oldValues[propName] = prop.OriginalValue;
                                        newValues[propName] = prop.CurrentValue;
                                    }
                                    break;
                            }
                        }

                        auditLog.OldValues = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues);
                        auditLog.NewValues = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues);
                        auditLog.AffectedColumns = changedCols.Count == 0 ? null : string.Join(",", changedCols);

                        if (auditLog.OldValues != null || auditLog.NewValues != null)
                            auditLogs.Add(auditLog);
                    }
                }

                if (auditLogs.Any())
                {
                    Set<AuditLog>().AddRange(auditLogs);
                }

                return await base.SaveChangesAsync(cancellationToken);
            }
        }
}