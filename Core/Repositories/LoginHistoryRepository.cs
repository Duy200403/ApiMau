using ApiWebsite.Core.Base;
using ApiWebsite.Core.IRepositories;
using ApiWebsite.Models;
using Microsoft.Extensions.Logging;

namespace ApiWebsite.Core.Repositories
{
    public class LoginHistoryRepository : GenericRepository<LoginHistory>, ILoginHistoryRepository
    {
        // ?ã s?a ILogger<Log> thành ILogger<LoginHistory>
        public LoginHistoryRepository(ApplicationDbContext context, ILogger<LoginHistory> logger) : base(context, logger)
        {

        }
    }
}