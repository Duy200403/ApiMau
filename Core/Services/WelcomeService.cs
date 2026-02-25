using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ApiWebsite.Core.Services
{
  public interface IWelcomeService : IBaseService<Welcome>
  {
    Task<PagedResult<WelcomeResponse>> GetAllPaging(WelcomePagingFilter request);
  }


  public class WelcomeService : BaseService<Welcome>, IWelcomeService
  {
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;
    public WelcomeService(ApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
    {
      _mapper = mapper;
      _dbContext = dbContext;
    }

    public async Task<PagedResult<WelcomeResponse>> GetAllPaging(WelcomePagingFilter request)
    {
      var predicateFilter = PredicateBuilder.True<Welcome>(); // khởi tạo mệnh đề truy vấn linq
      predicateFilter = predicateFilter.And(x => true);
      if (!string.IsNullOrEmpty(request.Keyword))
      {
        string key = request.Keyword.ToLower().Trim();
        predicateFilter = predicateFilter.And(x => x.Ten.ToLower().Contains(key));
      }
      long totalRow = await this.CountAsync(predicateFilter);
      IEnumerable<Welcome> data = null;
      
      data = await _unitOfWork.Welcome.GetSortedPaginatedAsync(predicateFilter, nameof(Welcome.CreatedDate), SortDirection.DESC, request.PageIndex, request.PageSize);
      var dataWelcomes = _mapper.Map<IEnumerable<WelcomeResponse>>(data);
      var pagedResult = new PagedResult<WelcomeResponse>()
      {
        TotalRecords = totalRow,
        PageSize = request.PageSize,
        PageIndex = request.PageIndex,
        Data = dataWelcomes
      };

      return pagedResult;
    }

    public override async Task<Welcome> UpsertAsync(Welcome entity)
    {
      var result = await _unitOfWork.Welcome.UpsertAsync(entity);
      await _unitOfWork.CompleteAsync();
      return result;
    }
  }
}