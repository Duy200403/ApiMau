using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models.Bidding.ProjectStepAttachment;
using AutoMapper;
using System.Linq;

namespace ApiWebsite.Core.Services
{
    // INTERFACE
    public interface IProjectStepAttachmentService : IBaseService<ApiWebsite.Models.ProjectStepAttachment>
    {
        Task<PagedResult<ProjectStepAttachmentResponse>> GetAllPaging(ProjectStepAttachmentPagingFilter request);
    }

    // CLASS IMPLEMENTATION
    public class ProjectStepAttachmentService : BaseService<ApiWebsite.Models.ProjectStepAttachment>, IProjectStepAttachmentService
    {
        private readonly IMapper _mapper;

        public ProjectStepAttachmentService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult<ProjectStepAttachmentResponse>> GetAllPaging(ProjectStepAttachmentPagingFilter request)
        {
            var predicateFilter = PredicateBuilder.True<ApiWebsite.Models.ProjectStepAttachment>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            // Tìm kiếm theo tên file
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x => x.FileName.ToLower().Contains(key));
            }

            // Lọc theo ProjectStepId (Để hiển thị list file trong form của bước đó)
            if (request.ProjectStepId.HasValue && request.ProjectStepId != Guid.Empty)
            {
                predicateFilter = predicateFilter.And(x => x.ProjectStepId == request.ProjectStepId);
            }

            long totalRow = await this.CountAsync(predicateFilter);

            // Sắp xếp file mới nhất lên đầu
            var data = await _unitOfWork.ProjectStepAttachment.GetSortedPaginatedAsync(
                predicateFilter,
                nameof(ApiWebsite.Models.ProjectStepAttachment.CreatedDate),
                SortDirection.DESC,
                request.PageIndex,
                request.PageSize
            );

            var dataMapped = _mapper.Map<List<ProjectStepAttachmentResponse>>(data);

            return new PagedResult<ProjectStepAttachmentResponse>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = dataMapped
            };
        }

        public override async Task<ApiWebsite.Models.ProjectStepAttachment> UpsertAsync(ApiWebsite.Models.ProjectStepAttachment entity)
        {
            var result = await _unitOfWork.ProjectStepAttachment.UpsertAsync(entity);
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}