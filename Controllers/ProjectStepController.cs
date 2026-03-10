using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ApiWebsite.Models;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using AutoMapper;
using ApiWebsite.Common;
using System.Net;
using ApiWebsite.Models.Bidding.ProjectStep;
using ApiWebsite.Helper.Middleware;

namespace ApiWebsite.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProjectStepController : BaseController
    {
        private readonly ILogger<ProjectStepController> _logger;
        private readonly IProjectStepService _projectStepService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public ProjectStepController(
            ILogger<ProjectStepController> logger,
            IProjectStepService projectStepService,
            ILogService logService,
            IMapper mapper)
        {
            _logger = logger;
            _projectStepService = projectStepService;
            _logService = logService;
            _mapper = mapper;
        }

        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<ProjectStepResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] ProjectStepPagingFilter request)
        {
            var pagedResult = await _projectStepService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var entity = await _projectStepService.GetByIdAsync(id);
            return entity == null ? NotFound(new { message = $"ID {id} không tìm thấy." }) : Ok(_mapper.Map<ProjectStepResponse>(entity));
        }

        /// <summary>
        /// API này dùng khi nhân viên nhập form Động và bấm Lưu Nháp / Hoàn thành
        /// </summary>
        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> SubmitStepData(Guid id, [FromBody] SubmitFormRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                var result = await _projectStepService.SubmitStepDataAsync(id, username, request);

                // Lưu vết lại cục JSON nếu cần thiết
                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(new { id, request });
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "ProjectStepController, SubmitStepData, UserName: " + username, paramTrace);

                if (result.success == false) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "ProjectStepController_SubmitStepData_Exception", ex.Message);
                return BadRequest(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        /// <summary>
        /// API dùng để chuyển gói thầu sang một nhánh quy trình mới
        /// </summary>
        // [Authorize(Role.admin, Role.manager, Role.editor)]
        [HttpPost("[action]/{projectId}")]
        public async Task<IActionResult> SwitchBranch(Guid projectId, [FromBody] SwitchBranchRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                var result = await _projectStepService.SwitchBranchAsync(projectId, request.NewBranchId, username);

                // Lưu vết log hành động chuyển nhánh
                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(new { projectId, request.NewBranchId });
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "ProjectStepController, SwitchBranch, UserName: " + username, paramTrace);

                if (result.success == false) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "ProjectStepController_SwitchBranch_Exception", ex.Message);
                return BadRequest(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}