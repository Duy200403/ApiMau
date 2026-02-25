using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using AutoMapper;

namespace ApiWebsite.Core.Services
{
  public interface IEmailConfigService : IBaseService<EmailConfig>
  {
    Task<PagedResult<EmailConfigRespone>> GetAllPaging(EmailConfigPagingFilter request);
  }

  public class EmailConfigService : BaseService<EmailConfig>, IEmailConfigService
  {

    private readonly IMapper _mapper;
    public EmailConfigService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
    {
      _mapper = mapper;
    }

    public async Task<PagedResult<EmailConfigRespone>> GetAllPaging(EmailConfigPagingFilter request)
    {
      var predicateFilter = PredicateBuilder.True<EmailConfig>(); // khởi tạo mệnh đề truy vấn linq
      predicateFilter = predicateFilter.And(x => true);

      if (!string.IsNullOrEmpty(request.Keyword))
      {
        string key = request.Keyword.ToLower().Trim();
        predicateFilter = predicateFilter.And(x => x.Email.ToLower().Contains(key) || x.EmailTitle.ToLower().Contains(key)); // thêm điều kiện truy vấn
      }
      // Paging
      long totalRow = await _unitOfWork.EmailConfig.CountRecordAsync(predicateFilter);

      var data = await _unitOfWork.EmailConfig.ListPaging(predicateFilter, null, null, (request.PageIndex - 1) * request.PageSize, request.PageSize);

      var pagedResult = new PagedResult<EmailConfigRespone>()
      {
        TotalRecords = totalRow,
        PageSize = request.PageSize,
        PageIndex = request.PageIndex,
        Data = _mapper.Map<IEnumerable<EmailConfigRespone>>(data)
      };

      return pagedResult;
    }

    public override async Task<EmailConfig> UpsertAsync(EmailConfig entity)
    {
      var result = await _unitOfWork.EmailConfig.UpsertAsync(entity);
      await _unitOfWork.CompleteAsync();
      return result;
    }
  }
}