using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiWebsite.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
// using System.Linq.Dynamic.Core;

namespace ApiWebsite.Core.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private ApplicationDbContext _context;
        private DbSet<T> dbSet;
        protected readonly ILogger<T> _logger;

        public GenericRepository(ApplicationDbContext context, ILogger<T> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected DbSet<T> DbSet
        {
            get => dbSet ?? (dbSet = _context.Set<T>());
        }

        public DbSet<T> GetDbSet()
        {
            return DbSet;
        }

        public virtual async Task<IEnumerable<T>> AllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ListPaging(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderDes, Expression<Func<T, object>> orderAsc, int skipN, int page)
        {
            IEnumerable<T> data;
            if (orderDes != null)
            {
                var query = GetDbSet().Where(expression).OrderByDescending(orderDes).Skip(skipN).Take(page);
                data = await query.ToListAsync();
            }
            else if (orderAsc != null)
            {
                var query = GetDbSet().Where(expression).OrderBy(orderDes).Skip(skipN).Take(page);
                data = await query.ToListAsync();
            }
            else
            {
                data = await GetDbSet().Where(expression).Skip(skipN).Take(page).ToListAsync();
            }
            return data;
        }

        public virtual async Task<IEnumerable<T>> GetPaginatedAsync(Expression<Func<T, bool>> predicateFilter, int page, int pageSize)
        {
            return await GetDbSet().Where(predicateFilter).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetSortedPaginatedAsync(Expression<Func<T, bool>> predicateFilter, string propertySort, SortDirection direction, int page, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            var query = GetDbSet().Where(predicateFilter);

            // Apply includes

            if (includes.Length > 0)
            {
                foreach (var includeExpression in includes)
                {
                    query = query.Include(includeExpression);
                }
            }

            // var sortDirection = direction == SortDirection.ASC ? "ASC" : "DESC";

            // Assuming OrderByProperty is a method that can order by a string property name.
            // You might need to adjust this if it's not compatible with includes.
            // query = query.OrderByProperty(propertySort, direction);

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, propertySort);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            query = direction == SortDirection.ASC ? query.OrderBy(lambda) : query.OrderByDescending(lambda);

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // return await GetDbSet().Where(predicateFilter).OrderByProperty(propertySort, direction).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            var item = await DbSet.FindAsync(id);
            return item;
        }

        public virtual async Task<bool> AddOneAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<bool> AddManyAsync(List<T> lstEntity)
        {
            await DbSet.AddRangeAsync(lstEntity);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var item = await DbSet.FindAsync(id);
            if (item != null)
            {
                DbSet.Remove(item);
                return true;
            }
            return false;
        }

        public virtual Task<T> UpsertAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountRecordAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).CountAsync();
        }

        // hàm này để lấy 1 item truyền từ điều kiện vào
        public async Task<T> GetOneAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            var item = await DbSet.Where(expression).FirstOrDefaultAsync();
            if (item != null)
            {
                return true;
            }
            return false;
        }
    }
}