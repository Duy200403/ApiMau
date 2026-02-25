using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiWebsite.Helper;

namespace ApiWebsite.Core.Base
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            return await this._unitOfWork.GetRepository<T>().AllAsync();
        }

        public async Task<IEnumerable<T>> ListPaging(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderDes, Expression<Func<T, object>> orderAsc, int skipN, int page)
        {
            return await this._unitOfWork.GetRepository<T>().ListPaging(expression, orderDes, orderAsc, skipN, page);
        }

        public virtual async Task<IEnumerable<T>> GetSortedPaginatedAsync(Expression<Func<T, bool>> predicateFilter, string propertySort, SortDirection direction, int page, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            return await _unitOfWork.GetRepository<T>().GetSortedPaginatedAsync(predicateFilter, propertySort, direction, page, pageSize, includes);
        }

        public virtual async Task<IEnumerable<T>> GetPaginatedAsync(Expression<Func<T, bool>> predicateFilter, int page, int pageSize)
        {
            return await _unitOfWork.GetRepository<T>().GetPaginatedAsync(predicateFilter, page, pageSize);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await this._unitOfWork.GetRepository<T>().GetByIdAsync(id);
        }
        public async Task<T> GetOneAsync(Expression<Func<T, bool>> expression)
        {
            return await this._unitOfWork.GetRepository<T>().GetOneAsync(expression);
        }

        public async Task<bool> AddOneAsync(T entity)
        {
            var result = await this._unitOfWork.GetRepository<T>().AddOneAsync(entity);
            await this._unitOfWork.CompleteAsync();
            return result;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var result = await this._unitOfWork.GetRepository<T>().DeleteAsync(id);
            await this._unitOfWork.CompleteAsync();
            return result;
        }

        public virtual Task<T> UpsertAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            return await this._unitOfWork.GetRepository<T>().CountRecordAsync(expression);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await this._unitOfWork.GetRepository<T>().AnyAsync(expression);
        }

        public async Task<bool> AddManyAsync(List<T> lstEntity)
        {
            var result = await this._unitOfWork.GetRepository<T>().AddManyAsync(lstEntity);
            await this._unitOfWork.CompleteAsync();
            return result;
        }
    }
}