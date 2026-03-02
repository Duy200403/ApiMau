using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ApiWebsite.Models;
using ApiWebsite.Models.Bidding.ProcessBranch; // Dùng namespace tự sinh
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using AutoMapper;
using System.Collections.Generic;
using ApiWebsite.Core.Base;
using System.Linq;
using System.Net;
using ApiWebsite.Common;

namespace ApiWebsite.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProcessBranchController : BaseController // Kế thừa BaseController như Welcome
    {
        private readonly ILogger<ProcessBranchController> _logger;
        private readonly IProcessBranchService _processBranchService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public ProcessBranchController(
            ApplicationDbContext dbContext,
            ILogger<ProcessBranchController> logger,
            IMapper mapper,
            IProcessBranchService processBranchService,
            ILogService logService)
        {
            _logger = logger;
            _processBranchService = processBranchService;
            _logService = logService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(ProcessBranchRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Map ra đích danh Entity
            var processBranch = _mapper.Map<ApiWebsite.Models.ProcessBranch>(model);
            processBranch.Id = Guid.NewGuid();

            // Kiểm tra trùng lặp tại Controller
            bool isExist = await _processBranchService.AnyAsync(x => x.BranchCode == processBranch.BranchCode && !x.IsDeleted);
            if (isExist)
            {
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(processBranch);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "ProcessBranchController, Create, Đã có mã nhánh", paramError);
                return BadRequest("Mã nhánh quy trình này đã tồn tại trong cơ sở dữ liệu.");
            }

            var result = await _processBranchService.AddOneAsync(processBranch);
            // Lưu xuống DB (Nếu AddOneAsync chưa lưu)
            var response = _mapper.Map<ProcessBranchResponse>(processBranch);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "ProcessBranchController, Create " + (result ? "OK" : "not OK"), paramTrace);

            return Ok(response);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var processBranch = await _processBranchService.GetByIdAsync(id);
            return processBranch == null ? NotFound() : Ok(_mapper.Map<ProcessBranchResponse>(processBranch));
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<ProcessBranchResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] ProcessBranchPagingFilter request)
        {
            var pagedResult = await _processBranchService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _processBranchService.DeleteAsync(id);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"ProcessBranchController, DeleteItem " + (result ? "OK" : "not OK"), id.ToString());
            return result ? Ok(result) : BadRequest("Xóa không thành công.");
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, ProcessBranchRequest model)
        {
            var processBranch = _mapper.Map<ApiWebsite.Models.ProcessBranch>(model);
            processBranch.Id = id;

            var existingEntity = await _processBranchService.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return NotFound(new { message = $"ID {id} không tìm thấy." });
            }

            // Kiểm tra trùng lặp
            bool isExist = await _processBranchService.AnyAsync(x => x.BranchCode == processBranch.BranchCode && x.Id != id && !x.IsDeleted);
            if (isExist)
            {
                return BadRequest("Mã nhánh quy trình này đã tồn tại ở bản ghi khác.");
            }

            // Copy đè dữ liệu mới vào Entity cũ đang được Tracking
            existingEntity.BranchCode = processBranch.BranchCode;
            existingEntity.BranchName = processBranch.BranchName;
            existingEntity.Description = processBranch.Description;

            var updatedResult = await _processBranchService.UpsertAsync(existingEntity);
            var response = _mapper.Map<ProcessBranchResponse>(updatedResult);

            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "ProcessBranchController, UpdateItem, Ok", paramTrace);

            return Ok(response);
        }
    }
}