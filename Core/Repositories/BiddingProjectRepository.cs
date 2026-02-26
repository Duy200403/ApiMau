using ApiWebsite.Core.Base;
using ApiWebsite.Core.IRepositories;
using Microsoft.Extensions.Logging;

namespace ApiWebsite.Core.Repositories
{
    public class BiddingProjectRepository : GenericRepository<ApiWebsite.Models.BiddingProject>, IBiddingProjectRepository
    {
        public BiddingProjectRepository(ApplicationDbContext context, ILogger<ApiWebsite.Models.BiddingProject> logger) : base(context, logger)
        {
        }
    }
}