using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models.Bidding.ProcessStep;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ApiWebsite.Core.Services
{
    public interface IProcessStepService : IBaseService<ApiWebsite.Models.ProcessStep>
    {
        Task<PagedResult<ProcessStepResponse>> GetAllPaging(ProcessStepPagingFilter request);
    }

    public class ProcessStepService : BaseService<ApiWebsite.Models.ProcessStep>, IProcessStepService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public ProcessStepService(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<PagedResult<ProcessStepResponse>> GetAllPaging(ProcessStepPagingFilter request)
        {
            var predicateFilter = PredicateBuilder.True<ApiWebsite.Models.ProcessStep>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.StepName.ToLower().Contains(key) || x.StepCode.ToLower().Contains(key));
            }

            if (request.ProcessBranchId.HasValue && request.ProcessBranchId != Guid.Empty)
            {
                predicateFilter = predicateFilter.And(x => x.ProcessBranchId == request.ProcessBranchId);
            }

            long totalRow = await this.CountAsync(predicateFilter);

            var data = await _unitOfWork.ProcessStep.GetSortedPaginatedAsync(
                predicateFilter,
                nameof(ApiWebsite.Models.ProcessStep.Order),
                SortDirection.ASC,
                request.PageIndex,
                request.PageSize
            );

            var dataMapped = _mapper.Map<List<ProcessStepResponse>>(data);

            // Bắt liên kết mềm: Query tên Nhánh và tên Đơn vị
            if (dataMapped.Any())
            {
                var branchIds = data.Select(x => x.ProcessBranchId).Distinct();
                var branches = await _dbContext.ProcessBranches.Where(x => branchIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.BranchName);

                var donViIds = data.Select(x => x.TargetDonViId).Distinct();
                var donVis = await _dbContext.DonViDeNghi.Where(x => donViIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.Name);

                foreach (var item in dataMapped)
                {
                    if (branches.ContainsKey(item.ProcessBranchId)) item.BranchName = branches[item.ProcessBranchId];
                    if (donVis.ContainsKey(item.TargetDonViId)) item.TargetDonViName = donVis[item.TargetDonViId];
                }
            }

            return new PagedResult<ProcessStepResponse>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = dataMapped
            };
        }

        // 1. Tận dụng UpsertAsync
        public override async Task<ApiWebsite.Models.ProcessStep> UpsertAsync(ApiWebsite.Models.ProcessStep entity)
        {
            var result = await _unitOfWork.ProcessStep.UpsertAsync(entity);
            await _unitOfWork.CompleteAsync();
            return result;
        }

        // 2. Ghi đè DeleteAsync để chặn hành vi xóa nếu Bước đã được sử dụng
        public override async Task<bool> DeleteAsync(Guid id)
        {
            bool isUsed = await _dbContext.ProjectSteps.AnyAsync(x => x.ProcessStepId == id && !x.IsDeleted);
            if (isUsed) throw new Exception("Bước này đã được sử dụng trong Gói thầu thực tế, không thể xóa.");

            return await base.DeleteAsync(id); // Gọi lại hàm xóa mềm của BaseService
        }
    }
}