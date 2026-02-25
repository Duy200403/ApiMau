using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApiWebsite.Models;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Common;
using ApiWebsite.Helper.Middleware;

namespace ApiWebsite.Controllers
{
    public class LogController : BaseController
    {
        private readonly ILogger<LogController> _logger;
        private readonly ILogService _logService;

        public LogController(ILogger<LogController> logger, ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllByPaging([FromQuery] LogPagingFilter request)
        {
            var pagedResult = await _logService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _logService.DeleteAsync(id);
            return result ? Ok(id) : BadRequest(id);
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpPost("[action]")]
        public async Task<IActionResult> DeleteByDate([FromBody] LogRequest request)
        {
            var listLog = await _logService.GetAllByDate(request);
            _logService.LogDeleteMany(listLog);

            return Ok(request);
        }
        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpGet("[action]")]
        public async Task<IActionResult> CountLogsByDate([FromQuery] RequestFilterDateBase request)
        {
            var allCount = await _logService.CountLogsByDate(request);
            return Ok(allCount);
        }
    }
}