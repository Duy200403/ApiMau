using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using ApiWebsite.Models.Response;
using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ApiWebsite.Core.Services
{
  public interface IAccountService : IBaseService<Account>
  {
    Task<PagedResult<AccountRespone>> GetAllPaging(AccountPagingFilter request);
    Task<dynamic> GetListOriginalAccount();
    Task<dynamic> Create(CreateAccountRequest model);
    Task<dynamic> CompleteServiceAsync();
    Task<dynamic> Update(Guid id, UpdateAccountRequest model);
    Task<dynamic> CountAllAccount();
  }

  public class AccountService : BaseService<Account>, IAccountService
  {
    private readonly ILogService _logService;
    private readonly IMapper _mapper;
    public AccountService(IUnitOfWork unitOfWork, ILogService logService, IMapper mapper) : base(unitOfWork)
    {
      _logService = logService;
      _mapper = mapper;
    }

    public async Task<dynamic> Create(CreateAccountRequest model)
    {
      var checkExitsUserName = await _unitOfWork.Account.AnyAsync(x => x.Username == model.Username);
      // check nếu tài khoản đã tồn tại thì đưa ra thông báo lỗi 
      if (checkExitsUserName)
      {
        ErrorResponseModel error = new ErrorResponseModel
        {
          Type = typeError.UserName.ToString(),
          Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.usernameEmailErrorMessage } } }
        };

        var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
        await _logService.AddLogWebInfo(LogLevelWebInfo.error, "AccountService, Create, Username", paramError);
        return error;
      }
      // var checkExitsEmail = await _unitOfWork.Account.AnyAsync(x => x.Email == model.Email);
      // // check nếu email đã tồn tại thì đưa ra thông báo lỗi 
      // if (checkExitsEmail)
      // {
      //   ErrorResponseModel error = new ErrorResponseModel
      //   {
      //     Type = typeError.Email.ToString(),
      //     Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.alreadyEmailErrorMessage } } }
      //   };
      //   var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
      //   await _logService.AddLogWebInfo(LogLevelWebInfo.error, "AccountService, Create, Email", paramError);
      //   return error;
      // }
      string salt = BC.GenerateSalt();
      Account account = new Account
      {
        Id = Guid.NewGuid(),
        FullName = model.FullName,
        Username = model.Username,
        Email = model.Email,
        Roles = string.Join(", ", model.Roles),
        PasswordHash = BC.HashPassword(model.Password), // = BC.HashPassword(model.Password, salt)
        // PasswordHash = BC.HashPassword("Admin$$1234"), // = BC.HashPassword(model.Password, salt)
        Salt = salt,
        PhoneNumber = model.PhoneNumber,
        Pseudonym = model.Pseudonym,
        IsActive = model.IsActive,
        CreatedDate = DateTime.UtcNow
      };
      await this.AddOneAsync(account);
      return null;
    }

    public async Task<dynamic> GetListOriginalAccount()
    {
      using (HttpClient client = new HttpClient())
      {
        // Specify the URI to get data from
        string uri = "http://108.108.108.52:10808/api/Users/SelectAll";

        try
        {
          HttpResponseMessage response = await client.GetAsync(uri);
          response.EnsureSuccessStatusCode();
          string responseBody = await response.Content.ReadAsStringAsync();
          var jsonResult = JsonConvert.DeserializeObject(responseBody).ToString();
          dynamic result = JsonConvert.DeserializeObject(jsonResult);
          var lstAccountOriginal = result._Data;

          // var test = lstAccountOriginal[0].UserName;

          return lstAccountOriginal;
        }
        catch (HttpRequestException e)
        {
          // Handle any errors that occurred during the request
          Console.WriteLine("\nException Caught!");
          Console.WriteLine("Message :{0} ", e.Message);

          return null;
        }
      }
      return null;
    }

    public async Task<dynamic> Update(Guid id, UpdateAccountRequest model)
    {
      var account = await _unitOfWork.Account.GetOneAsync(x => x.Id == id);
      if (account != null)
      {
        // var resultEmail = await _unitOfWork.Account.GetOneAsync(x => x.Email == model.Email);
        // // check nếu email update khác với email của tài khoản và email update chưa tồn tại
        // if (account.Email != model.Email && resultEmail != null)
        // {
        //   ErrorResponseModel error = new ErrorResponseModel
        //   {
        //     Type = typeError.Email.ToString(),
        //     Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.alreadyEmailErrorMessage } } }
        //   };

        //   var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
        //   await _logService.AddLogWebInfo(LogLevelWebInfo.error, "AccountService, Update, account.Email", paramError);
        //   return error;
        // }

        if (!string.IsNullOrEmpty(model.Password))
        {
          account.PasswordHash = BC.HashPassword(model.Password);
          // account.PasswordHash = BC.HashPassword("Admin$$1234");
          account.Salt = BC.GenerateSalt();
        }

        // account.PasswordHash = BC.HashPassword("Admin$$1234");
        // account.Salt = BC.GenerateSalt();
        account.Email = model.Email;
        account.FullName = model.FullName;
        account.UpdatedDate = DateTime.UtcNow;
        account.Roles = string.Join(", ", model.Roles);
        account.IsActive = model.IsActive;
        account.PhoneNumber = model.PhoneNumber;
        // account.Pseudonym = model.Pseudonym;
        await _unitOfWork.CompleteAsync();
      }
      return account;
    }

    public async Task<PagedResult<AccountRespone>> GetAllPaging(AccountPagingFilter request)
    {
      var predicateFilter = PredicateBuilder.True<Account>();
      predicateFilter = predicateFilter.And(x => true);

      if (!string.IsNullOrEmpty(request.Keyword))
      {
        string key = request.Keyword.ToLower().Trim();
        predicateFilter = predicateFilter.And(x => x.Email.ToLower().Contains(key) || x.Username.ToLower().Contains(key) || x.FullName.ToLower().Contains(key));
      }

      // if (request.LstAccountId.Count > 0) {
      //   predicateFilter = predicateFilter.And(x => request.LstAccountId.Contains(x.Id.ToString()));
      // }

      // if (request.Role != null)
      // {
      //   var roleString = request.Role.ToString();
      //   predicateFilter = predicateFilter.And(x => x.Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).Contains(roleString));
      // }

      // var lsstAccount = lay tu api chinh;

      // long totalRow = await this.CountAsync(predicateFilter);

      var resultData = await _unitOfWork.Account.ListPaging(predicateFilter, x => x.CreatedDate, null, (request.PageIndex - 1) * request.PageSize, request.PageSize);

      // var resultData = await _unitOfWork.Account.GetSortedPaginatedAsync(predicateFilter, nameof(Account.CreatedDate), SortDirection.DESC, request.PageIndex, request.PageSize);
      // var resultData = await _unitOfWork.Account.GetPaginatedAsync(predicateFilter, request.PageIndex, request.PageSize);

      IEnumerable<Account> filteredData = resultData;
      if (request.Role != null)
      {
        var roleString = request.Role.ToString();
        // Since we cannot use Split in SQL, apply role filter in-memory
        filteredData = resultData.Where(x => x.Roles != null && x.Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).Contains(roleString));
      }

      long totalRow = filteredData.Count();

      var pagedResult = new PagedResult<AccountRespone>()
      {
        TotalRecords = totalRow,
        PageSize = request.PageSize,
        PageIndex = request.PageIndex,
        Data = _mapper.Map<IEnumerable<AccountRespone>>(filteredData)
      };
      return pagedResult;
    }

    public async Task<dynamic> CompleteServiceAsync()
    {
      await _unitOfWork.CompleteAsync();
      return true;
    }

    public async Task<dynamic> CountAllAccount()
    {

      var predicateFilter = PredicateBuilder.True<Account>();
      predicateFilter = predicateFilter.And(x => true);
      int totalRow = await _unitOfWork.Account.CountRecordAsync(predicateFilter);
      return new
      {
        total = totalRow
      };
    }
  }
}