using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ApiWebsite.Core.IRepositories;
using ApiWebsite.Models;
using ApiWebsite.Core.Base;

namespace ApiWebsite.Core.Repositories
{
    public class LogRepository : GenericRepository<Log>, ILogRepository
    {
        public LogRepository(ApplicationDbContext context, ILogger<Log> logger) : base(context, logger)
        {
        }
    }
}