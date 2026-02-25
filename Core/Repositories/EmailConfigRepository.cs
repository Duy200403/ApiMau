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
    public class EmailConfigRepository : GenericRepository<EmailConfig>, IEmailConfigRepository
    {
        public EmailConfigRepository(ApplicationDbContext context, ILogger<EmailConfig> logger) : base(context, logger)
        {

        }

        public override async Task<EmailConfig> UpsertAsync(EmailConfig entity)
        {
            try
            {
                var existItem = await DbSet.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();

                if (existItem == null)
                {
                    await AddOneAsync(entity);
                    return entity;
                }

                existItem.Email = entity.Email;
                existItem.Password = entity.Password;
                existItem.IsActive = entity.IsActive;
                existItem.MailServer = entity.MailServer;
                existItem.Port = entity.Port;
                existItem.EnableSSl = entity.EnableSSl;
                existItem.EmailTitle = entity.EmailTitle;

                existItem.UpdatedBy = entity.UpdatedBy;
                existItem.UpdatedDate = DateTime.UtcNow;

                return existItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpsertAsync method error", typeof(EmailConfigRepository));
                throw ex;
            }
        }
    }
}