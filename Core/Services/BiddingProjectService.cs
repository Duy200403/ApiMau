using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models; // Chứa Entity BiddingProject, ProcessStep...
using ApiWebsite.Models.Bidding; // Chứa Request, Response, PagingFilter
using ApiWebsite.Models.Response;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ApiWebsite.Core.Services
{
    public interface IBiddingProjectService : IBaseService<BiddingProject>
    {
        Task<PagedResult<BiddingProjectResponse>> GetAllPaging(BiddingProjectPagingFilter request);
        Task<dynamic> CreateProjectAsync(BiddingProjectRequest request);
    }

    public class BiddingProjectService : BaseService<BiddingProject>, IBiddingProjectService
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

        // HÀM 1: PHÂN TRANG CHUẨN THEO BASE SERVICE
        public async Task<PagedResult<BiddingProjectResponse>> GetAllPaging(BiddingProjectPagingFilter request)
        {
            // 1. Khởi tạo mệnh đề truy vấn LINQ
            var predicateFilter = PredicateBuilder.True<BiddingProject>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            // 2. Lọc theo từ khóa (Mã gói thầu hoặc Tên gói thầu)
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.ProjectName.ToLower().Contains(key) || x.ProjectCode.ToLower().Contains(key));
            }

            // 3. Lọc theo Trạng thái (Nếu có truyền lên)
            if (!string.IsNullOrEmpty(request.Status))
            {
                predicateFilter = predicateFilter.And(x => x.Status == request.Status);
            }

            // 4. Lọc theo Nhánh quy trình
            if (request.ProcessBranchId.HasValue)
            {
                predicateFilter = predicateFilter.And(x => x.ProcessBranchId == request.ProcessBranchId.Value);
            }

            // 5. Đếm tổng số bản ghi bằng hàm CountAsync của BaseService
            long totalRow = await this.CountAsync(predicateFilter);

            // 6. Lấy dữ liệu phân trang, Sort giảm dần theo ngày tạo
            // Đặc biệt: Dùng params includes (x => x.ProcessBranch) để tự động JOIN lấy tên nhánh
            IEnumerable<BiddingProject> data = await _unitOfWork.BiddingProject.GetSortedPaginatedAsync(
                predicateFilter,
                nameof(BiddingProject.CreatedDate),
                SortDirection.DESC,
                request.PageIndex,
                request.PageSize,
                x => x.ProcessBranch // Gọi Include ProcessBranch
            );

            // 7. Map sang Response
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

        // HÀM 2: KHỞI TẠO GÓI THẦU 
        public async Task<dynamic> CreateProjectAsync(BiddingProjectRequest request)
        {
            try
            {
                // Sử dụng AnyAsync của BaseService để kiểm tra trùng lặp
                bool isExist = await this.AnyAsync(x => x.ProjectCode == request.ProjectCode && !x.IsDeleted);
                if (isExist)
                {
                    return new ErrorResponseModel { Type = "DuplicateCode", Title = "Lỗi dữ liệu", Errors = new Dictionary<string, string[]> { { "ProjectCode", new[] { "Mã gói thầu đã tồn tại" } } } };
                }

                // Lấy ra danh sách các bước của nhánh được chọn
                var processSteps = await _dbContext.ProcessSteps
                    .Where(x => x.ProcessBranchId == request.ProcessBranchId && !x.IsDeleted)
                    .OrderBy(x => x.Order)
                    .ToListAsync();

                if (!processSteps.Any())
                {
                    return new ErrorResponseModel { Type = "ConfigError", Title = "Lỗi", Errors = new Dictionary<string, string[]> { { "ProcessBranchId", new[] { "Nhánh này chưa cấu hình các bước." } } } };
                }

                // Map dữ liệu từ Request sang Entity
                var newProject = _mapper.Map<BiddingProject>(request);
                newProject.Id = Guid.NewGuid();
                newProject.CurrentStepOrder = processSteps.First().Order;
                newProject.Status = "InProgress";

                // Tự động sinh danh sách công việc thực tế
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

                // Sử dụng AddOneAsync và AddManyAsync của BaseService
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
    }
}