using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Helper.Middleware;
using ApiWebsite.Models;
using ApiWebsite.Models.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiWebsite.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _iAccountService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        public AccountController(ILogger<AccountController> logger, IAccountService iAccountService, ILogService logService, IMapper mapper)
        {
            _logger = logger;
            _iAccountService = iAccountService;
            _logService = logService;
            _mapper = mapper;
        }

        [Authorize(Role.admin, Role.manager, Role.general)]
        [HttpGet("[action]")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(PagedResult<Account>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllPaging([FromQuery] AccountPagingFilter request)
        {
            var result = await _iAccountService.GetAllPaging(request);
            return Ok(result);
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateAccountRequest model)
        {
            var result = await _iAccountService.Create(model);
            if (result != null && result.GetType() == typeof(ErrorResponseModel))
            {
                return BadRequest(result);
            }
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "Tạo tài khoản mới thành công", paramTrace);
            return Ok();
        }

        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateAccountRequest model)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            // Nếu không phải admin và manager thì check có phải nó sửa cho chính nó ko, nếu sửa cho tài khoản khác thì ko được phép
            List<string> listRoles = new List<string>(account.Roles.Split(", "));
            // if (!listRoles.Contains(Role.admin.ToString()) && !listRoles.Contains(Role.manager.ToString()))
            if (!listRoles.Contains(Role.admin.ToString()))
            {
                if (account.Id != id)
                {
                    var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                    await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Sửa tài khoản không thành công", paramError);
                    return Unauthorized();
                }
            }
            var result = await _iAccountService.Update(id, model);
            if (result != null && result.GetType() == typeof(ErrorResponseModel))
            {
                return BadRequest(result);
            }
            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "Sửa tài khoản thành công", paramTrace);
            return Ok(result);
        }

        [Authorize(Role.admin, Role.manager)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var account = await _iAccountService.GetByIdAsync(id);
            var accountLogin = (Account)HttpContext.Items[ConstantsInternal.Account];
            // nếu tài khoản null thì trả về badrequest
            if (account == null)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Gặp lỗi khi xóa tài khoản, tài khoản null", id.ToString());
                return BadRequest();
            }
            // không được xóa tài khoản admin
            List<string> listRoles = new List<string>(account.Roles.Split(','));
            if (account != null && listRoles.Contains(Role.admin.ToString()))
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.NotpermissionMessage } } }
                };
                return BadRequest(error);
            }
            // thỏa mãn hết thì cho xóa tài khoản
            await _iAccountService.DeleteAsync(id);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "Xóa tài khoản thành công + ${}", id.ToString());
            return Ok(_mapper.Map<AccountRespone>(account));
        }

        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpGet("[action]/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var account = await _iAccountService.GetByIdAsync(id);
            return account == null ? BadRequest() : Ok(_mapper.Map<AccountRespone>(account));
        }

        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpGet("[action]")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CountAllAccount()
        {
            var total = await _iAccountService.CountAllAccount();
            return Ok(total);
        }
    }

}