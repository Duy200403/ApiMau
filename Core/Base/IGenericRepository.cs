using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ApiWebsite.Helper;

namespace ApiWebsite.Core.Base
{
    public interface IGenericRepository { }
    public interface IGenericRepository<T> : IGenericRepository where T : class
    {
        Task<IEnumerable<T>> AllAsync();
        Task<IEnumerable<T>> ListPaging(Expression<Func<T, bool>> expression,Expression<Func<T, object>> orderDes, Expression<Func<T, object>> orderAsc,int skipN,int page);
        Task<IEnumerable<T>> GetPaginatedAsync(Expression<Func<T, bool>> predicateFilter, int page, int pageSize);
        Task<IEnumerable<T>> GetSortedPaginatedAsync(Expression<Func<T, bool>> predicateFilter, string propertySort, SortDirection direction, int page, int pageSize, params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetOneAsync(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<bool> AddOneAsync(T entity);
        Task<bool> AddManyAsync(List<T> lstEntity);
        Task<int> CountRecordAsync(Expression<Func<T, bool>> expression);
        Task<bool> DeleteAsync(Guid id);
        Task<T> UpsertAsync(T entity);
        DbSet<T> GetDbSet();
    }
}