using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApiWebsite.Models;
using ApiWebsite.Models.Bidding;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using AutoMapper;
using System.Collections.Generic;
using ApiWebsite.Core.Base;
using System.Linq;
using System.Net;
using ApiWebsite.Common;
using ApiWebsite.Models.Response;

namespace ApiWebsite.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BiddingProjectController : BaseController // Đã kế thừa BaseController
    {
        private readonly ILogger<BiddingProjectController> _logger;
        private readonly IBiddingProjectService _biddingProjectService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public BiddingProjectController(
            ILogger<BiddingProjectController> logger,
            IMapper mapper,
            IBiddingProjectService biddingProjectService,
            ILogService logService)
        {
            _logger = logger;
            _biddingProjectService = biddingProjectService;
            _logService = logService;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(BiddingProjectRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Gọi logic tạo mới phức tạp bên Service
            var result = await _biddingProjectService.CreateProjectAsync(model);

            if (result is ErrorResponseModel)
            {
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "BiddingProjectController, Create, Lỗi nghiệp vụ", paramError);
                return BadRequest(result);
            }

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "BiddingProjectController, Create OK", paramTrace);

            return Ok(result);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var biddingProject = await _biddingProjectService.GetByIdAsync(id);
            return biddingProject == null ? NotFound() : Ok(_mapper.Map<BiddingProjectResponse>(biddingProject));
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<BiddingProjectResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] BiddingProjectPagingFilter request)
        {
            var pagedResult = await _biddingProjectService.GetAllByPaging(request);
            return Ok(pagedResult);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _biddingProjectService.DeleteAsync(id);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"BiddingProjectController, DeleteItem " + (result ? "OK" : "not OK"), id.ToString());
            return result ? Ok(result) : BadRequest("Xóa không thành công.");
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, BiddingProjectRequest model)
        {
            var existingEntity = await _biddingProjectService.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return NotFound(new { message = $"ID {id} không tìm thấy." });
            }

            // Kiểm tra trùng lặp
            bool isExist = await _biddingProjectService.AnyAsync(x => x.ProjectCode == model.ProjectCode && x.Id != id && !x.IsDeleted);
            if (isExist)
            {
                return BadRequest("Mã gói thầu này đã tồn tại ở bản ghi khác.");
            }

            // Copy đè dữ liệu mới vào Entity cũ
            existingEntity.ProjectCode = model.ProjectCode;
            existingEntity.ProjectName = model.ProjectName;
            existingEntity.ProcessBranchId = model.ProcessBranchId;

            var updatedResult = await _biddingProjectService.UpsertAsync(existingEntity);
            var response = _mapper.Map<BiddingProjectResponse>(updatedResult);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "BiddingProjectController, UpdateItem, Ok", paramTrace);

            return Ok(response);
        }
    }
}