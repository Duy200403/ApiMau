using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models; // Chứa ApplicationDbContext
using ApiWebsite.Models.System.DonViDeNghi;
using ApiWebsite.Models.Response; // Chứa ErrorResponseModel nếu cần
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebsite.Core.Services
{
    // Cố gắng dùng IBaseService nhưng chúng ta sẽ phải lách các hàm dùng ID
    public interface IDonViDeNghiService : IBaseService<DonViDeNghi>
    {
        Task<PagedResult<DonViDeNghiResponse>> GetAllByPaging(DonViDeNghiPagingFilter request);

        // Cố tình định nghĩa lại các hàm dùng 'long' thay vì 'Guid' của Base
        Task<DonViDeNghi> GetByLongIdAsync(long id);
        Task<bool> DeleteByLongIdAsync(long id);
    }

    public class DonViDeNghiService : BaseService<DonViDeNghi>, IDonViDeNghiService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DonViDeNghiService(
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

        public async Task<PagedResult<DonViDeNghiResponse>> GetAllByPaging(DonViDeNghiPagingFilter request)
        {
            var predicateFilter = PredicateBuilder.True<DonViDeNghi>();
            predicateFilter = predicateFilter.And(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                string key = request.Keyword.ToLower().Trim();
                predicateFilter = predicateFilter.And(x =>
                    (x.Name != null && x.Name.ToLower().Contains(key)) ||
                    (x.Code != null && x.Code.ToLower().Contains(key))
                );
            }

            // Vì CountAsync và GetSortedPaginatedAsync của BaseService có thể không hỗ trợ long tốt
            // Ta dùng query LINQ trực tiếp từ _dbContext cho chắc ăn và an toàn
            var query = _dbContext.DonViDeNghi.Where(predicateFilter);

            long totalRow = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var dataMapped = _mapper.Map<IEnumerable<DonViDeNghiResponse>>(data);

            var pagedResult = new PagedResult<DonViDeNghiResponse>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Data = dataMapped
            };

            return pagedResult;
        }

        // =========================================================================
        // VIẾT CÁC HÀM RIÊNG ĐỂ XỬ LÝ KIỂU LONG (Vì BaseService mặc định Guid)
        // =========================================================================

        public async Task<DonViDeNghi> GetByLongIdAsync(long id)
        {
            return await _dbContext.DonViDeNghi.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<bool> DeleteByLongIdAsync(long id)
        {
            var entity = await _dbContext.DonViDeNghi.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true; // Xóa mềm
            // entity.UpdatedDate = DateTime.Now; // Cập nhật thời gian nếu cần
            _dbContext.DonViDeNghi.Update(entity);
            return (await _dbContext.SaveChangesAsync()) > 0;
        }

        // =========================================================================
        // TẬN DỤNG CÁC HÀM CỦA BASESERVICE KHÔNG DÙNG ID CỤ THỂ
        // =========================================================================

        public override async Task<DonViDeNghi> UpsertAsync(DonViDeNghi entity)
        {
            // Nếu _unitOfWork.DonViDeNghi.UpsertAsync không hoạt động do lỗi kiểu Guid/long
            // Hãy đổi thành dùng _dbContext như sau:
            if (entity.Id == 0) // Tạo mới
            {
                entity.CreatedDate = DateTime.Now;
                await _dbContext.DonViDeNghi.AddAsync(entity);
            }
            else // Cập nhật
            {
                // entity.UpdatedDate = DateTime.Now;
                _dbContext.DonViDeNghi.Update(entity);
            }

            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}