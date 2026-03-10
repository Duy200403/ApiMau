using System.Threading.Tasks;
using ApiWebsite.Models;
using ApiWebsite.Models.Logger.AuditLog;
using ApiWebsite.Models.System.DonViDeNghi;

namespace ApiWebsite.Core.Base
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
        IGenericRepository<T> GetRepository<T>() where T : class;
        IGenericRepository<FileManager> FileManager { get; }
        IGenericRepository<EmailConfig> EmailConfig { get; }
        IGenericRepository<Account> Account { get; }
        IGenericRepository<Log> Log { get; }
        IGenericRepository<LoginHistory> LoginHistory { get; }
        IGenericRepository<Welcome> Welcome { get; }
        IGenericRepository<BiddingProject> BiddingProject { get; }
        IGenericRepository<ProcessBranch> ProcessBranch { get; }
        IGenericRepository<ProcessStep> ProcessStep { get; }
        IGenericRepository<ProjectStep> ProjectStep { get; }
        IGenericRepository<ProjectStepAttachment> ProjectStepAttachment { get; }
        IGenericRepository<StepAttribute> StepAttribute { get; }
        IGenericRepository<AuditLog> AuditLog { get; }
        IGenericRepository<ProjectFormSchema> ProjectFormSchema { get; }
        IGenericRepository<ProjectDataIndex> ProjectDataIndex { get; }
        IGenericRepository<DonViDeNghi> DonViDeNghi { get; }
    }
}