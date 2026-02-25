using ApiWebsite.Core.Base;
using ApiWebsite.Core.IRepositories;
using ApiWebsite.Models;
using Microsoft.Extensions.Logging;

namespace ApiWebsite.Core.Repositories
{
    public class LoginHistoryRepository : GenericRepository<LoginHistory>, ILoginHistoryRepository
    {
        public LoginHistoryRepository(ApplicationDbContext context, ILogger<Log> logger) : base(context, logger)
        {

        }
    }
}