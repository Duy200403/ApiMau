using ApiWebsite.Common;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Helper.Middleware;
using ApiWebsite.Models;
using ApiWebsite.Models.Bidding.StepAttribute;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ApiWebsite.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class StepAttributeController : BaseController
    {
        private readonly ILogger<StepAttributeController> _logger;
        private readonly IStepAttributeService _stepAttributeService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public StepAttributeController(
            ILogger<StepAttributeController> logger,
            IStepAttributeService stepAttributeService,
            ILogService logService,
            IMapper mapper)
        {
            _logger = logger;
            _stepAttributeService = stepAttributeService;
            _logService = logService;
            _mapper = mapper;
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(StepAttributeRequest model)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            var entity = _mapper.Map<ApiWebsite.Models.StepAttribute>(model);
            entity.Id = Guid.NewGuid();
            // entity.CreatedBy = username;

            // Chặn trùng AttributeCode trong cùng 1 bước
            bool isExist = await _stepAttributeService.AnyAsync(x => x.AttributeCode == entity.AttributeCode && x.ProcessStepId == entity.ProcessStepId && !x.IsDeleted);

            if (isExist)
            {
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(entity);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "StepAttributeController, Create, Đã có mã", paramError);
                return BadRequest("Mã thuộc tính này đã tồn tại trong bước hiện tại.");
            }

            var result = await _stepAttributeService.AddOneAsync(entity);
            var response = _mapper.Map<StepAttributeResponse>(entity);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "StepAttributeController, Create " + (result ? "OK" : "not OK") + " UserName: " + username, paramTrace);

            return Ok(response);
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var entity = await _stepAttributeService.GetByIdAsync(id);
            return entity == null ? NotFound() : Ok(_mapper.Map<StepAttributeResponse>(entity));
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<StepAttributeResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] StepAttributePagingFilter request)
        {
            var pagedResult = await _stepAttributeService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            var result = await _stepAttributeService.DeleteAsync(id);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"StepAttributeController, DeleteItem, {username} " + (result ? "OK" : "not OK"), id.ToString());

            return result ? Ok(result) : BadRequest(result);
        }

        //[Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, StepAttributeRequest model)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            var existingEntity = await _stepAttributeService.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return NotFound(new { message = $"ID {id} không tìm thấy." });
            }

            // Chặn trùng mã nếu đổi mã
            if (existingEntity.AttributeCode != model.AttributeCode)
            {
                bool isExist = await _stepAttributeService.AnyAsync(x => x.AttributeCode == model.AttributeCode && x.ProcessStepId == model.ProcessStepId && x.Id != id && !x.IsDeleted);
                if (isExist) return BadRequest("Mã thuộc tính này đã tồn tại.");
            }

            _mapper.Map(model, existingEntity);
            // existingEntity.UpdatedBy = username;

            var updatedEntity = await _stepAttributeService.UpsertAsync(existingEntity);
            var response = _mapper.Map<StepAttributeResponse>(updatedEntity);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "StepAttributeController, UpdateItem, Ok " + username, paramTrace);

            return Ok(response);
        }
    }
}