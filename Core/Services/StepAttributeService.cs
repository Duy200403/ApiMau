using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using ApiWebsite.Models.Bidding.StepAttribute;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ApiWebsite.Core.Services
{
    // Interface siêu sạch, y hệt IWelcomeService
    public interface IStepAttributeService : IBaseService<ApiWebsite.Models.StepAttribute>
    {
        Task<PagedResult<StepAttributeResponse>> GetAllPaging(StepAttributePagingFilter request);
    }

    public class StepAttributeService : BaseService<ApiWebsite.Models.StepAttribute>, IStepAttributeService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public StepAttributeService(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<PagedResult<StepAttributeResponse>> GetAllPaging(StepAttributePagingFilter request)
        {
            var predicateFilter = PredicateBuilder.True<ApiWebsite.Models.StepAttribute>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.AttributeName.ToLower().Contains(key) || x.AttributeCode.ToLower().Contains(key));
            }

            // Lọc theo Bước quy trình
            if (request.ProcessStepId.HasValue && request.ProcessStepId != Guid.Empty)
            {
                predicateFilter = predicateFilter.And(x => x.ProcessStepId == request.ProcessStepId);
            }

            long totalRow = await this.CountAsync(predicateFilter);

            var data = await _unitOfWork.StepAttribute.GetSortedPaginatedAsync(
                predicateFilter,
                nameof(ApiWebsite.Models.StepAttribute.DisplayOrder), // Sắp xếp theo thứ tự hiển thị
                SortDirection.ASC,
                request.PageIndex,
                request.PageSize
            );

            var dataMapped = _mapper.Map<List<StepAttributeResponse>>(data);

            // Lấy tên Bước qua liên kết mềm
            if (dataMapped.Any())
            {
                var stepIds = data.Select(x => x.ProcessStepId).Distinct();
                var steps = await _dbContext.ProcessSteps.Where(x => stepIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.StepName);

                foreach (var item in dataMapped)
                {
                    if (steps.ContainsKey(item.ProcessStepId)) item.ProcessStepName = steps[item.ProcessStepId];
                }
            }

            return new PagedResult<StepAttributeResponse>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = dataMapped
            };
        }

        public override async Task<ApiWebsite.Models.StepAttribute> UpsertAsync(ApiWebsite.Models.StepAttribute entity)
        {
            var result = await _unitOfWork.StepAttribute.UpsertAsync(entity);
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}