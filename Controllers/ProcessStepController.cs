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
using ApiWebsite.Models.Bidding.ProcessStep;
using ApiWebsite.Helper.Middleware;

namespace ApiWebsite.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProcessStepController : BaseController
    {
        private readonly ILogger<ProcessStepController> _logger;
        private readonly IProcessStepService _processStepService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public ProcessStepController(
            ILogger<ProcessStepController> logger,
            IProcessStepService processStepService,
            ILogService logService,
            IMapper mapper)
        {
            _logger = logger;
            _processStepService = processStepService;
            _logService = logService;
            _mapper = mapper;
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(ProcessStepRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            var entity = _mapper.Map<ApiWebsite.Models.ProcessStep>(model);
            entity.Id = Guid.NewGuid();
            // entity.CreatedBy = username;

            // Chặn trùng StepCode trong cùng 1 nhánh
            bool isExist = await _processStepService.AnyAsync(x => x.StepCode == entity.StepCode && x.ProcessBranchId == entity.ProcessBranchId && !x.IsDeleted);

            if (isExist)
            {
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "ProcessStepController, Create, Đã có mã", paramError);
                return BadRequest("Mã bước này đã tồn tại trong nhánh quy trình hiện tại.");
            }

            try
            {
                var result = await _processStepService.AddOneAsync(entity);
                var response = _mapper.Map<ProcessStepResponse>(entity);

                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "ProcessStepController, Create " + (result ? "OK" : "not OK") + " UserName: " + username, paramTrace);

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "ProcessStepController_Create_Exception", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var entity = await _processStepService.GetByIdAsync(id);
            return entity == null ? NotFound(new { message = $"ID {id} không tìm thấy." }) : Ok(_mapper.Map<ProcessStepResponse>(entity));
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<ProcessStepResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] ProcessStepPagingFilter request)
        {
            var pagedResult = await _processStepService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                // Hàm DeleteAsync đã được override bên ProcessStepService để chặn xóa nếu vi phạm
                var result = await _processStepService.DeleteAsync(id);

                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"ProcessStepController, DeleteItem, {username} " + (result ? "OK" : "not OK"), id.ToString());
                return result ? Ok(result) : BadRequest("Xóa không thành công");
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, $"ProcessStepController, DeleteItem_Exception, {username}", ex.Message);
                return BadRequest(ex.Message); // Bắt lỗi "Bước này đã sử dụng trong Gói thầu..."
            }
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, ProcessStepRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                var existingEntity = await _processStepService.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return NotFound(new { message = $"ID {id} không tìm thấy." });
                }

                // Chặn trùng mã nếu đổi mã
                if (existingEntity.StepCode != model.StepCode)
                {
                    bool isExist = await _processStepService.AnyAsync(x => x.StepCode == model.StepCode && x.ProcessBranchId == model.ProcessBranchId && x.Id != id && !x.IsDeleted);
                    if (isExist) return BadRequest("Mã bước này đã tồn tại trong nhánh quy trình.");
                }

                _mapper.Map(model, existingEntity);
                // existingEntity.UpdatedBy = username;

                var updatedEntity = await _processStepService.UpsertAsync(existingEntity);
                var response = _mapper.Map<ProcessStepResponse>(updatedEntity);

                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "ProcessStepController, UpdateItem, Ok " + username, paramTrace);

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "ProcessStepController_UpdateItem_Exception", ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}