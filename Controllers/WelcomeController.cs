using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApiWebsite.Models;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using AutoMapper;
using ApiWebsite.Helper.Middleware;
using System.Collections.Generic;
using ApiWebsite.Core.Base;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ApiWebsite.Common;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace ApiWebsite.Controllers
{
    public class WelcomeController : BaseController
    {
        private readonly ILogger<WelcomeController> _logger;
        private readonly IWelcomeService _WelcomeService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly IAccountService _iAccountService;
        private readonly ApplicationDbContext _dbContext;

        public WelcomeController(IAccountService iAccountService, ApplicationDbContext dbContext, ILogger<WelcomeController> logger, IMapper mapper, IWelcomeService WelcomeService, ILogService logService)
        {
            _logger = logger;
            _WelcomeService = WelcomeService;
            _logService = logService;
            _mapper = mapper;
            _dbContext = dbContext;
            _iAccountService = iAccountService;
        }
        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(WelcomeRequest model)
        {
           
            
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];

            var Welcome = _mapper.Map<Welcome>(model);
            Welcome.Id = Guid.NewGuid();
            Welcome.CreatedBy = account.Username;
            // Welcome.Donvi = account.Donvi;
            bool isWelcomeExist = await _WelcomeService.AnyAsync(x => x.Id == Welcome.Id);

            if (isWelcomeExist)
            {
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(Welcome);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "WelcomeController, Create, Đã có mã", paramError);
                return BadRequest("A device code exist in database");
            }
            // isWelcomeExist = await _WelcomeService.AnyAsync(
            //     x => x.Ten.ToLower() == Welcome.Ten.ToLower().Trim()
            // );
            var result = await _WelcomeService.AddOneAsync(Welcome);
            var response = _mapper.Map<WelcomeResponse>(Welcome);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "WelcomeController, Create " + (result ? "OK" : "not OK") + " UserName: " + account.Username, paramTrace);
            return Ok(response);

        }
        [HttpGet("[action]/{id}")]
        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var Welcome = await _WelcomeService.GetByIdAsync(id);

            return Welcome == null ? NotFound() : Ok(_mapper.Map<WelcomeResponse>(Welcome));
        }
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<Welcome>), (int)HttpStatusCode.OK)]
        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] WelcomePagingFilter request)
        {
            var pagedResult = await _WelcomeService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var result = await _WelcomeService.DeleteAsync(id);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"WelcomeController, DeleteItem, {account.Username} " + (result ? "OK" : "not OK"), id.ToString());
            return result ? Ok(result) : BadRequest(result);
        }

        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, WelcomeRequest model)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];

            var Welcome = _mapper.Map<Welcome>(model);
            Welcome.Id = id;
            Welcome.UpdatedBy = account.Username;
            var existingWelcome = await _WelcomeService.GetByIdAsync(id);
            if (existingWelcome == null)
            {
                // If the device with the specified ID doesn't exist, return not found
                return NotFound(new { message = $"ID {id} không tìm thấy." });
            }
            var Welcomes = await _WelcomeService.UpsertAsync(Welcome);
            var response = _mapper.Map<WelcomeResponse>(Welcomes);
            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "WelcomeController, UpdateItem, Ok" + account.Username, paramTrace);
            return Ok(response);
        }
        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        // [HttpPut("[action]/{id}")]
        // public async Task<IActionResult> UpdateStatus(Guid id, WelcomeSTTRequest model)
        // {
        //     var account = (Account)HttpContext.Items[ConstantsInternal.Account];

        //     var Welcome = _mapper.Map<Welcome>(model);
        //     Welcome.Id = id;
        //     Welcome.UpdatedBy = account.Username;
        //     var existingWelcome = await _WelcomeService.GetByIdAsync(id);
        //     if (existingWelcome == null)
        //     {
        //         // If the device with the specified ID doesn't exist, return not found
        //         return NotFound(new { message = $"ID {id} không tìm thấy." });
        //     }
        //     existingWelcome.Trangthai = model.Trangthai;
        //     var Welcomes = await _WelcomeService.UpsertAsync(existingWelcome);
        //     var response = _mapper.Map<WelcomeResponse>(Welcomes);
        //     var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
        //     await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "WelcomeController, UpdateItem, Ok" + account.Username, paramTrace);
        //     return Ok(response);
        // }
    }
}