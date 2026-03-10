using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models; // Chứa Entity ProjectDataIndex, BiddingProject...
using ApiWebsite.Models.Bidding.ProjectStep;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiWebsite.Core.Services
{
    public interface IProjectStepService : IBaseService<ProjectStep>
    {
        Task<PagedResult<ProjectStepResponse>> GetAllPaging(ProjectStepPagingFilter request);
        Task<dynamic> SubmitStepDataAsync(Guid projectStepId, string username, SubmitFormRequest request);
        Task<dynamic> SwitchBranchAsync(Guid projectId, Guid newBranchId, string username);
    }

    public class ProjectStepService : BaseService<ProjectStep>, IProjectStepService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public ProjectStepService(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
        // =======================================================================
        // HÀM LẤY DANH SÁCH BƯỚC TIẾN ĐỘ THEO GÓI THẦU, CÓ LỌC VÀ PHÂN TRANG
        // =======================================================================
        public async Task<PagedResult<ProjectStepResponse>> GetAllPaging(ProjectStepPagingFilter request)
        {
            var predicateFilter = Helper.PredicateBuilder.True<ApiWebsite.Models.ProjectStep>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            // 1. Lọc theo ID Gói thầu
            if (request.BiddingProjectId.HasValue && request.BiddingProjectId != Guid.Empty)
            {
                predicateFilter = predicateFilter.And(x => x.BiddingProjectId == request.BiddingProjectId);
            }

            // 2. LỌC THEO TRẠNG THÁI (Bổ sung thêm đoạn này)
            if (!string.IsNullOrEmpty(request.Status))
            {
                string statusVal = request.Status.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.Status.ToLower() == statusVal);
            }

            // 3. LỌC THEO TỪ KHÓA (Bổ sung thêm đoạn này)
            // Tùy nhu cầu, bạn có thể cho tìm keyword trong Comments hoặc nội dung FormDataJson
            // 3. LỌC THEO TỪ KHÓA (Search TẤT CẢ mọi thứ)
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();

                // Bước 3.1: Đi tìm trước các ID của Nhánh/Bước có tên chứa từ khóa (Vì mình dùng liên kết mềm)
                var matchingStepIds = await _dbContext.ProcessSteps
                    .Where(s => !s.IsDeleted && (s.StepName.ToLower().Contains(key) || s.StepCode.ToLower().Contains(key)))
                    .Select(s => s.Id)
                    .ToListAsync();

                // Bước 3.2: Gom tất cả điều kiện vào 1 cục Predicate OR
                predicateFilter = predicateFilter.And(x =>
                    (x.Comments != null && x.Comments.ToLower().Contains(key)) ||                           // Tìm trong ghi chú
                    (x.FormDataJson != null && x.FormDataJson.ToLower().Contains(key)) ||                   // Tìm thẳng vào nội dung JSON form động
                    (x.CompletedByUserId != null && x.CompletedByUserId.ToLower().Contains(key)) ||         // Tìm theo username người duyệt
                    (x.Status != null && x.Status.ToLower().Contains(key)) ||                               // Tìm theo chữ Pending, Completed...
                    matchingStepIds.Contains(x.ProcessStepId)                                               // Tìm theo Tên bước (Sub-query)
                );
            }

            long totalRow = await this.CountAsync(predicateFilter);

            // Tạm thời lấy danh sách không sort vì ta cần lấy thông tin StepOrder từ ProcessStep để sort
            var data = await _dbContext.ProjectSteps.Where(predicateFilter).ToListAsync();

            var dataMapped = _mapper.Map<List<ProjectStepResponse>>(data);

            // Xử lý Liên kết mềm: Lấy thông tin Tên bước và Thứ tự từ ProcessStep
            if (dataMapped.Any())
            {
                var processStepIds = dataMapped.Select(x => x.ProcessStepId).Distinct();
                var processSteps = await _dbContext.ProcessSteps
                    .Where(x => processStepIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => new { x.StepName, x.Order });

                foreach (var item in dataMapped)
                {
                    if (processSteps.ContainsKey(item.ProcessStepId))
                    {
                        item.ProcessStepName = processSteps[item.ProcessStepId].StepName;
                        item.StepOrder = processSteps[item.ProcessStepId].Order;
                    }
                }
            }

            // Sắp xếp lại theo StepOrder sau khi đã map
            dataMapped = dataMapped.OrderBy(x => x.StepOrder).ToList();

            // Cắt trang thủ công
            var pagedData = dataMapped.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();

            return new PagedResult<ProjectStepResponse>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = pagedData
            };
        }

        // =======================================================================
        // HÀM LƯU FORM ĐỘNG, BÓC TÁCH JSON VÀ CHUYỂN BƯỚC TỰ ĐỘNG
        // =======================================================================
        public async Task<dynamic> SubmitStepDataAsync(Guid projectStepId, string username, SubmitFormRequest request)
        {
            var currentStep = await _dbContext.ProjectSteps.FindAsync(projectStepId);
            if (currentStep == null) return new { success = false, message = "Không tìm thấy bước tiến độ này." };

            if (currentStep.Status == "Completed") return new { success = false, message = "Bước này đã hoàn thành, không thể sửa đổi." };

            // 1. LƯU DỮ LIỆU GỐC VÀ CẬP NHẬT PROJECT_STEP
            if (request.FormData != null)
            {
                currentStep.FormDataJson = JsonSerializer.Serialize(request.FormData);

                // 2. BÓC TÁCH JSON VÀ LƯU VÀO BẢNG INDEX ĐỂ TÌM KIẾM
                // Xóa index cũ
                var oldIndexes = await _dbContext.ProjectDataIndexs.Where(x => x.ProjectStepId == currentStep.Id).ToListAsync();
                _dbContext.ProjectDataIndexs.RemoveRange(oldIndexes);

                var jsonDoc = JsonSerializer.SerializeToDocument(request.FormData);
                var newIndexes = new List<ApiWebsite.Models.ProjectDataIndex>();

                foreach (var prop in jsonDoc.RootElement.EnumerateObject())
                {
                    var indexRecord = new ApiWebsite.Models.ProjectDataIndex
                    {
                        Id = Guid.NewGuid(),
                        BiddingProjectId = currentStep.BiddingProjectId,
                        ProjectStepId = currentStep.Id,
                        FieldCode = prop.Name
                    };

                    if (prop.Value.ValueKind == JsonValueKind.Number)
                    {
                        indexRecord.NumberValue = prop.Value.GetDecimal();
                        indexRecord.TextValue = prop.Value.ToString();
                    }
                    else if (prop.Value.ValueKind == JsonValueKind.String)
                    {
                        var strVal = prop.Value.GetString();
                        indexRecord.TextValue = strVal;
                        if (DateTime.TryParse(strVal, out DateTime parsedDate)) indexRecord.DateValue = parsedDate;
                    }
                    else
                    {
                        indexRecord.TextValue = prop.Value.ToString();
                    }
                    newIndexes.Add(indexRecord);
                }
                await _dbContext.ProjectDataIndexs.AddRangeAsync(newIndexes);
            }

            currentStep.Comments = request.Comments;

            // 3. NẾU BẤM HOÀN THÀNH -> CHUYỂN TRẠNG THÁI VÀ KÍCH HOẠT BƯỚC TIẾP THEO
            if (request.IsCompleted)
            {
                currentStep.Status = "Completed";
                currentStep.CompletedDate = DateTime.Now;
                currentStep.CompletedByUserId = username;

                // Tìm thông tin gói thầu và các bước của gói
                var project = await _dbContext.BiddingProjects.FindAsync(currentStep.BiddingProjectId);
                var processStepInfo = await _dbContext.ProcessSteps.FindAsync(currentStep.ProcessStepId);

                // Tìm bước tiếp theo có Order lớn hơn Order hiện tại
                var nextStepProcess = await _dbContext.ProcessSteps
                    .Where(x => x.ProcessBranchId == project.ProcessBranchId && x.Order > processStepInfo.Order && !x.IsDeleted)
                    .OrderBy(x => x.Order)
                    .FirstOrDefaultAsync();

                if (nextStepProcess != null)
                {
                    // Kích hoạt bước tiếp theo trong ProjectStep
                    var nextProjectStep = await _dbContext.ProjectSteps
                        .FirstOrDefaultAsync(x => x.BiddingProjectId == project.Id && x.ProcessStepId == nextStepProcess.Id);

                    if (nextProjectStep != null)
                    {
                        nextProjectStep.Status = "Processing";
                        nextProjectStep.StartDate = DateTime.Now;
                        _dbContext.ProjectSteps.Update(nextProjectStep);
                    }

                    // Cập nhật lại Order hiện tại của Gói thầu
                    project.CurrentStepOrder = nextStepProcess.Order;
                }
                else
                {
                    // Nếu không còn bước nào tiếp theo -> Gói thầu hoàn thành 100%
                    project.Status = "Completed";
                }

                _dbContext.BiddingProjects.Update(project);
            }

            _dbContext.ProjectSteps.Update(currentStep);
            await _dbContext.SaveChangesAsync();

            return new { success = true, message = request.IsCompleted ? "Đã hoàn thành bước và chuyển tiếp!" : "Đã lưu nháp dữ liệu thành công!" };
        }
        // =======================================================================
        // HÀM CHUYỂN NHÁNH QUY TRÌNH (QUY TRÌNH MỚI KHÁC HOẶC CÙNG NHÁNH NHƯNG QUY TRÌNH ĐÃ THAY ĐỔI)
        // =======================================================================
        public async Task<dynamic> SwitchBranchAsync(Guid projectId, Guid newBranchId, string username)
        {
            // 1. Lấy thông tin gói thầu
            var project = await _dbContext.BiddingProjects.FindAsync(projectId);
            if (project == null) return new { success = false, message = "Không tìm thấy gói thầu." };

            // (Tùy chọn: Kiểm tra xem các bước của nhánh cũ đã Completed hết chưa mới cho chuyển nhánh)
            bool isOldStepsNotDone = await _dbContext.ProjectSteps
                .AnyAsync(x => x.BiddingProjectId == projectId && x.Status != "Completed" && !x.IsDeleted);
            if (isOldStepsNotDone)
                return new { success = false, message = "Vui lòng hoàn thành tất cả các bước hiện tại trước khi chuyển sang quy trình mới." };

            // 2. Cập nhật Gói thầu sang nhánh mới
            project.ProcessBranchId = newBranchId;

            // Tìm bước đầu tiên của Nhánh mới
            var firstStepOfNewBranch = await _dbContext.ProcessSteps
                .Where(x => x.ProcessBranchId == newBranchId && !x.IsDeleted)
                .OrderBy(x => x.Order)
                .FirstOrDefaultAsync();

            if (firstStepOfNewBranch == null)
                return new { success = false, message = "Nhánh quy trình mới chưa được cấu hình bước nào." };

            project.CurrentStepOrder = firstStepOfNewBranch.Order;
            project.Status = "Processing"; // Đảm bảo trạng thái đang chạy

            // 3. Sinh ra các ProjectStep (Tiến độ) mới cho nhánh mới
            var newProcessSteps = await _dbContext.ProcessSteps
                .Where(x => x.ProcessBranchId == newBranchId && !x.IsDeleted)
                .OrderBy(x => x.Order)
                .ToListAsync();

            var newProjectSteps = new List<ApiWebsite.Models.ProjectStep>();

            foreach (var step in newProcessSteps)
            {
                newProjectSteps.Add(new ApiWebsite.Models.ProjectStep
                {
                    Id = Guid.NewGuid(),
                    BiddingProjectId = project.Id,
                    ProcessStepId = step.Id,
                    Status = step.Id == firstStepOfNewBranch.Id ? "Processing" : "Pending", // Kích hoạt bước đầu tiên
                    StartDate = step.Id == firstStepOfNewBranch.Id ? DateTime.Now : null,
                    CreatedDate = DateTime.Now,
                    CreatedBy = username,
                    IsDeleted = false
                });
            }

            // 4. Lưu tất cả xuống Database
            _dbContext.BiddingProjects.Update(project);
            await _dbContext.ProjectSteps.AddRangeAsync(newProjectSteps);
            await _dbContext.SaveChangesAsync();

            return new { success = true, message = "Đã chuyển sang nhánh quy trình mới thành công và khởi tạo các bước tiếp theo!" };
        }
    }
}