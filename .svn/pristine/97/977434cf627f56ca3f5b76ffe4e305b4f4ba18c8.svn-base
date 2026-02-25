using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiWebsite.Core.Services
{
    public interface ILogService : IBaseService<Log>
    {
        Task<PagedResult<LogRespone>> GetAllPaging(LogPagingFilter request);
        Task<IEnumerable<Log>> GetAllByDate(LogRequest request);
        void LogDeleteMany(IEnumerable<Log> lstLog);
        Task<dynamic> CountLogsByDate(RequestFilterDateBase request);
        Task<Log> AddLogWebInfo(LogLevelWebInfo logLevelWebInfo, string message, string param);
    }

    public class LogService : BaseService<Log>, ILogService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public LogService(IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<Log> AddLogWebInfo(LogLevelWebInfo logLevelWebInfo, string message, string param)
        {
            var log = new Log()
            {
                LogLevel = logLevelWebInfo,
                Message = message,
                Parameter = param,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Log.AddOneAsync(log);
            await _unitOfWork.CompleteAsync();
            return null;
        }

        public async Task<IEnumerable<Log>> GetAllByDate(LogRequest request)
        {
            var starOfDay = Util.StartOfDay(request.StartDay);
            var endOfDay = Util.EndOfDay(request.EndDay);
            return await _dbContext.Log.Where(x => x.CreatedDate >= starOfDay && x.CreatedDate <= endOfDay).ToListAsync();
        }

        public async Task<PagedResult<LogRespone>> GetAllPaging(LogPagingFilter request)
        {
            var predicateFilter = PredicateBuilder.True<Log>(); // khởi tạo mệnh đề truy vấn linq
            predicateFilter = predicateFilter.And(x => true);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.Message.ToLower().Contains(key) || x.Parameter.ToLower().Contains(key)); // thêm điều kiện truy vấn
            }

            // Paging
            long totalRow = await _unitOfWork.Log.CountRecordAsync(predicateFilter);

            var data = await _unitOfWork.Log.GetSortedPaginatedAsync(predicateFilter, nameof(Log.CreatedDate), SortDirection.DESC, request.PageIndex, request.PageSize);

            var pagedResult = new PagedResult<LogRespone>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = _mapper.Map<IEnumerable<LogRespone>>(data)
            };
            return pagedResult;
        }

        public void LogDeleteMany(IEnumerable<Log> lstLog)
        {
            if (lstLog.Count() > 0)
            {
                _dbContext.Log.RemoveRange(lstLog);
                _dbContext.SaveChanges();
            }
        }
        // thống kê số lượng log(Thông báo) theo ngày
        public async Task<dynamic> CountLogsByDate(RequestFilterDateBase request)
        {
            var fromDate = Util.FloorDate(request.fromDate);
            var toDate = Util.CeilDate(request.toDate);

            var resultByDate = await (
                from log in _dbContext.Log
                where (request.fromDate != null ? log.CreatedDate.Value.Date > fromDate.Value.Date : 1 == 1) && (request.toDate != null ? log.CreatedDate.Value.Date < toDate.Value.Date : 1 == 1)
                select log).CountAsync();
            var data = new
            {
                Title = "Tổng số",
                Total = resultByDate
            };
            return data;
        }

        public override async Task<Log> UpsertAsync(Log entity)
        {
            var result = await _unitOfWork.Log.UpsertAsync(entity);
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}