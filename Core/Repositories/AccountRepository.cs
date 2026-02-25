using ApiWebsite.Core.Base;
using ApiWebsite.Core.IRepositories;
using ApiWebsite.Models;
using Microsoft.Extensions.Logging;

namespace ApiWebsite.Core.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext context, ILogger<Account> logger) : base(context, logger)
        {

        }

    }
}