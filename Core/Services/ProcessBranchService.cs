using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using ApiWebsite.Models.Bidding.ProcessBranch; // Dùng namespace tự sinh của bạn
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ApiWebsite.Core.Services
{
    // Gọi đích danh ApiWebsite.Models.ProcessBranch để C# không nhầm với namespace
    public interface IProcessBranchService : IBaseService<ApiWebsite.Models.ProcessBranch>
    {
        Task<PagedResult<ProcessBranchResponse>> GetAllPaging(ProcessBranchPagingFilter request);
    }

    public class ProcessBranchService : BaseService<ApiWebsite.Models.ProcessBranch>, IProcessBranchService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public ProcessBranchService(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<PagedResult<ProcessBranchResponse>> GetAllPaging(ProcessBranchPagingFilter request)
        {
            var predicateFilter = PredicateBuilder.True<ApiWebsite.Models.ProcessBranch>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.BranchName.ToLower().Contains(key) || x.BranchCode.ToLower().Contains(key));
            }

            long totalRow = await this.CountAsync(predicateFilter);
            IEnumerable<ApiWebsite.Models.ProcessBranch> data = null;

            data = await _unitOfWork.ProcessBranch.GetSortedPaginatedAsync(
                predicateFilter,
                nameof(ApiWebsite.Models.ProcessBranch.CreatedDate),
                SortDirection.DESC,
                request.PageIndex,
                request.PageSize
            );

            var dataMapped = _mapper.Map<IEnumerable<ProcessBranchResponse>>(data);
            var pagedResult = new PagedResult<ProcessBranchResponse>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = dataMapped
            };

            return pagedResult;
        }

        public override async Task<ApiWebsite.Models.ProcessBranch> UpsertAsync(ApiWebsite.Models.ProcessBranch entity)
        {
            var result = await _unitOfWork.ProcessBranch.UpsertAsync(entity);
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}