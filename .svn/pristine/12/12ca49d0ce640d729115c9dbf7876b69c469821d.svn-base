using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApiWebsite.Models;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using AutoMapper;
using ApiWebsite.Helper.Middleware;

namespace ApiWebsite.Controllers
{
    public class EmailConfigController : BaseController
    {
        private readonly ILogger<EmailConfigController> _logger;
        private readonly IEmailConfigService _emailConfigService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public EmailConfigController(ILogger<EmailConfigController> logger, IMapper mapper, IEmailConfigService emailConfigService, ILogService logService)
        {
            _logger = logger;
            _emailConfigService = emailConfigService;
            _logService = logService;
            _mapper = mapper;
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateEmailConfig(EmailConfigRequest model)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];

            var emailConfig = _mapper.Map<EmailConfig>(model);
            emailConfig.Id = Guid.NewGuid();
            emailConfig.CreatedBy = account.Username;
            var result = await _emailConfigService.AddOneAsync(emailConfig);

            var response = _mapper.Map<EmailConfigRespone>(emailConfig);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "EmailConfigsController, Create, " + (result ? "OK" : "not OK"), paramTrace);

            return Ok(response);
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var emailConfig = await _emailConfigService.GetByIdAsync(id);
            return emailConfig == null ? NotFound() : Ok(_mapper.Map<EmailConfigRespone>(emailConfig));
        }


        [Authorize(Role.admin, Role.manager)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllByPaging([FromQuery] EmailConfigPagingFilter request)
        {
            var pagedResult = await _emailConfigService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, EmailConfig model)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];

            var emailConfig = _mapper.Map<EmailConfig>(model);
            emailConfig.Id = id;
            emailConfig.UpdatedBy = account.Username;

            var emailConfigItem = await _emailConfigService.UpsertAsync(emailConfig);

            var response = _mapper.Map<EmailConfigRespone>(emailConfigItem);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "EmailConfigsController, UpdateItem, Ok", paramTrace);
            return Ok(response);
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _emailConfigService.DeleteAsync(id);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "EmailConfigsController, DeleteItem, " + (result ? "OK" : "not OK"), id.ToString());
            return result ? Ok(id) : BadRequest(id);
        }
    }
}