using System.Net;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Helper.Middleware;
using ApiWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiWebsite.Controllers
{
  public class LoginHistoryController : BaseController
  {
    private readonly ILogger<LoginHistoryController> _logger;
    private readonly ILoginHistoryService _iLoginHistoryService;
    private readonly ILogService _logService;
    public LoginHistoryController(ILogger<LoginHistoryController> logger, ILoginHistoryService iLoginHistoryService, ILogService logService)
    {
      _logger = logger;
      _iLoginHistoryService = iLoginHistoryService;
      _logService = logService;
    }
    [Authorize(Role.admin, Role.manager)]
    [HttpGet("[action]")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(PagedResult<LoginHistoryRespose>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllPaging([FromQuery] LoginHistoryPagingFilter request)
    {
      var result = await _iLoginHistoryService.GetAllPaging(request);
      return Ok(result);
    }
    [Authorize(Role.admin, Role.manager)]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetStatisticsLoginByAccount([FromQuery] LoginHisReportRequest request)
    {
      var listCount = await _iLoginHistoryService.GetStatisticsLoginByAccount(request);
      return Ok(listCount);
    }
    [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
    [HttpGet("[action]")]
    public async Task<IActionResult> CountAllLogin()
    {
      var allCount = await _iLoginHistoryService.CountAll();
      return Ok(allCount);
    }
    [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
    [HttpGet("[action]")]
    public async Task<IActionResult> CountAllByDate([FromQuery] RequestFilterDateBase request)
    {
      var allCount = await _iLoginHistoryService.CountAllByDate(request);
      return Ok(allCount);
    }
  }
}