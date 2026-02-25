using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ApiWebsite.Core.IRepositories;
using ApiWebsite.Models;
using ApiWebsite.Core.Base;

namespace ApiWebsite.Core.Repositories
{
    public class WelcomeRepository : GenericRepository<Welcome>, IWelcomeRepository
    {
        public WelcomeRepository(ApplicationDbContext context, ILogger<Welcome> logger) : base(context, logger)
        {

        }

        public override async Task<Welcome> UpsertAsync(Welcome entity)
        {
            try
            {
                var existItem = await DbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();

                if (existItem == null)
                {
                    await AddOneAsync(entity);
                    return entity;
                }
                existItem.Ten = entity.Ten;
                existItem.Trangthai = entity.Trangthai;

                
                existItem.UpdatedBy = entity.UpdatedBy;
                existItem.UpdatedDate = DateTime.UtcNow;

                return existItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpsertAsync method error", typeof(WelcomeRepository));
                throw ex;
            }
        }
    }
}