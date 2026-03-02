using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using ApiWebsite.Models.Bidding;
using ApiWebsite.Models.Response;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ApiWebsite.Core.Services
{
    // Gọi đích danh ApiWebsite.Models.BiddingProject để C# không nhầm với namespace
    public interface IBiddingProjectService : IBaseService<ApiWebsite.Models.BiddingProject>
    {
        Task<PagedResult<BiddingProjectResponse>> GetAllByPaging(BiddingProjectPagingFilter request);
        Task<dynamic> CreateProjectAsync(BiddingProjectRequest request);
    }

    public class BiddingProjectService : BaseService<ApiWebsite.Models.BiddingProject>, IBiddingProjectService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BiddingProjectService(
            ApplicationDbContext dbContext,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogService logService,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<BiddingProjectResponse>> GetAllByPaging(BiddingProjectPagingFilter request)
        {
            var predicateFilter = PredicateBuilder.True<ApiWebsite.Models.BiddingProject>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.ProjectName.ToLower().Contains(key) || x.ProjectCode.ToLower().Contains(key));
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                predicateFilter = predicateFilter.And(x => x.Status == request.Status);
            }

            if (request.ProcessBranchId.HasValue)
            {
                predicateFilter = predicateFilter.And(x => x.ProcessBranchId == request.ProcessBranchId.Value);
            }

            long totalRow = await this.CountAsync(predicateFilter);

            IEnumerable<ApiWebsite.Models.BiddingProject> data = await _unitOfWork.BiddingProject.GetSortedPaginatedAsync(
                predicateFilter,
                nameof(ApiWebsite.Models.BiddingProject.CreatedDate),
                SortDirection.DESC,
                request.PageIndex,
                request.PageSize,
                x => x.ProcessBranch
            );

            var dataMapped = _mapper.Map<IEnumerable<BiddingProjectResponse>>(data);

            var pagedResult = new PagedResult<BiddingProjectResponse>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = dataMapped
            };

            return pagedResult;
        }

        public async Task<dynamic> CreateProjectAsync(BiddingProjectRequest request)
        {
            try
            {
                bool isExist = await this.AnyAsync(x => x.ProjectCode == request.ProjectCode && !x.IsDeleted);
                if (isExist)
                {
                    return new ErrorResponseModel { Type = "DuplicateCode", Title = "Lỗi dữ liệu", Errors = new Dictionary<string, string[]> { { "ProjectCode", new[] { "Mã gói thầu đã tồn tại" } } } };
                }

                var processSteps = await _dbContext.ProcessSteps
                    .Where(x => x.ProcessBranchId == request.ProcessBranchId && !x.IsDeleted)
                    .OrderBy(x => x.Order)
                    .ToListAsync();

                if (!processSteps.Any())
                {
                    return new ErrorResponseModel { Type = "ConfigError", Title = "Lỗi", Errors = new Dictionary<string, string[]> { { "ProcessBranchId", new[] { "Nhánh này chưa cấu hình các bước." } } } };
                }

                var newProject = _mapper.Map<ApiWebsite.Models.BiddingProject>(request);
                newProject.Id = Guid.NewGuid();
                newProject.CurrentStepOrder = processSteps.First().Order;
                newProject.Status = "InProgress";

                var projectSteps = new List<ProjectStep>();
                foreach (var step in processSteps)
                {
                    projectSteps.Add(new ProjectStep
                    {
                        Id = Guid.NewGuid(),
                        BiddingProjectId = newProject.Id,
                        ProcessStepId = step.Id,
                        Status = step.Order == newProject.CurrentStepOrder ? "Processing" : "Pending",
                        StartDate = step.Order == newProject.CurrentStepOrder ? DateTime.Now : (DateTime?)null,
                    });
                }

                await _unitOfWork.BiddingProject.AddOneAsync(newProject);
                await _unitOfWork.ProjectStep.AddManyAsync(projectSteps);

                await _unitOfWork.CompleteAsync();

                return new { success = true, data = newProject, message = "Tạo gói thầu thành công!" };
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "BiddingProjectService - Create", ex.Message);
                return new ErrorResponseModel { Type = "Exception", Title = "Lỗi hệ thống", Errors = new Dictionary<string, string[]> { { "System", new[] { "Đã có lỗi xảy ra." } } } };
            }
        }

        // Ghi đè Upsert theo đúng chuẩn cũ
        public override async Task<ApiWebsite.Models.BiddingProject> UpsertAsync(ApiWebsite.Models.BiddingProject entity)
        {
            var result = await _unitOfWork.BiddingProject.UpsertAsync(entity);
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}