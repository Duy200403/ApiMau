using System.Threading.Tasks;
using ApiWebsite.Models;

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
    }
}