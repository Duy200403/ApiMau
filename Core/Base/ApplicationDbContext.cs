using Microsoft.EntityFrameworkCore;
using ApiWebsite.Models;
using System;
using ApiWebsite.Helper;
using Microsoft.Data.Sqlite;

namespace ApiWebsite.Core.Base
{
    public class ApplicationDbContext : DbContext
    {
        // the dbset property will tell ef core that we have
        // a table that needs to be created if doesnt exist
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<FileManager> FileManager { get; set; }
        public virtual DbSet<EmailConfig> EmailConfig { get; set; }
        public virtual DbSet<LoginHistory> LoginHistory { get; set; }
        public virtual DbSet<Welcome> Welcome { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // concurrent write 
            modelBuilder.Entity<Account>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<Log>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<FileManager>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<LoginHistory>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<EmailConfig>().Property(c => c.Timestamp).IsRowVersion();
            modelBuilder.Entity<Welcome>().Property(c => c.Timestamp).IsRowVersion();

            modelBuilder
              .Entity<Log>()
              .Property(e => e.LogLevel)
              .HasConversion(
                  v => v.ToString(),
                  v => (LogLevelWebInfo)Enum.Parse(typeof(LogLevelWebInfo), v));

            // modelBuilder.Entity<BookDevice>()
            //     .HasOne(wa => wa.Account)
            //     .WithMany()
            //     .HasForeignKey(wa => wa.AccountId)
            //     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}