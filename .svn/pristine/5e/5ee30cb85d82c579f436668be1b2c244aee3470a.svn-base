using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using AutoMapper;

namespace ApiWebsite.Core.Services
{
  public interface IFileManagerService : IBaseService<FileManager>
  {
    Task<PagedResult<FileManagerRespone>> GetAllPaging(FileManagerPagingFilter request);
  }
  public class FileManagerService : BaseService<FileManager>, IFileManagerService
  {
    private readonly IMapper _mapper;
    public FileManagerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
    {
      _mapper = mapper;
    }
    public async Task<PagedResult<FileManagerRespone>> GetAllPaging(FileManagerPagingFilter request)
    {
      var predicateFilter = PredicateBuilder.True<FileManager>();
      predicateFilter = predicateFilter.And(x => true);

      if (!string.IsNullOrEmpty(request.Keyword))
      {
        string key = request.Keyword.ToLower().Trim();
        predicateFilter = predicateFilter.And(x => x.Name.ToLower().Contains(key));
      }

      long totalRow = await this.CountAsync(predicateFilter);

      var resultData = await this.GetSortedPaginatedAsync(predicateFilter, nameof(FileManager.Id), SortDirection.ASC, request.PageIndex, request.PageSize);

      var pagedResult = new PagedResult<FileManagerRespone>()
      {
        TotalRecords = totalRow,
        PageSize = request.PageSize,
        PageIndex = request.PageIndex,
        Data = _mapper.Map<IEnumerable<FileManagerRespone>>(resultData)
      };
      return pagedResult;
    }
  }
}