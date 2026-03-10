using ApiWebsite.Common;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Helper.Middleware;
using ApiWebsite.Model;
using ApiWebsite.Models;
using ApiWebsite.Models.Bidding.ProjectStepAttachment;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApiWebsite.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ProjectStepAttachmentController : BaseController
    {
        private readonly ILogger<ProjectStepAttachmentController> _logger;
        private readonly IProjectStepAttachmentService _attachmentService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly List<VirtualPathConfig> _virtualPaths;

        public ProjectStepAttachmentController(
            ILogger<ProjectStepAttachmentController> logger,
            IProjectStepAttachmentService attachmentService,
            ILogService logService,
            IMapper mapper,
            IOptions<List<VirtualPathConfig>> virtualPathsOptions)
        {
            _logger = logger;
            _attachmentService = attachmentService;
            _logService = logService;
            _mapper = mapper;
            _virtualPaths = virtualPathsOptions.Value;
        }

        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromForm] ProjectStepAttachmentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            // SỬA: Đổi request.File thành request.Files và dùng .Count
            if (request.Files == null || request.Files.Count == 0)
            {
                return BadRequest(new { success = false, message = "Vui lòng chọn ít nhất 1 file tải lên." });
            }

            try
            {
                var pathConfig = _virtualPaths.FirstOrDefault();
                if (pathConfig == null) return StatusCode(500, "Chưa cấu hình VirtualPathConfig trong appsettings.");

                string yearMonthFolder = Path.Combine("Bidding", DateTime.Now.ToString("yyyy-MM"));
                string physicalUploadPath = Path.Combine(Directory.GetCurrentDirectory(), pathConfig.RealPath, yearMonthFolder);

                if (!Directory.Exists(physicalUploadPath)) Directory.CreateDirectory(physicalUploadPath);

                // Khởi tạo list để chứa các entity lưu xuống DB và list để trả về Frontend
                var listEntities = new List<ApiWebsite.Models.ProjectStepAttachment>();
                var listResponses = new List<ProjectStepAttachmentResponse>();

                // SỬA: DÙNG VÒNG LẶP FOREACH ĐỂ DUYỆT QUA TỪNG FILE
                foreach (var file in request.Files)
                {
                    if (file.Length > 0)
                    {
                        string originalFileName = Path.GetFileName(file.FileName);
                        string fileExtension = Path.GetExtension(originalFileName);
                        string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        string fullPhysicalFilePath = Path.Combine(physicalUploadPath, uniqueFileName);

                        // Copy từng file thực tế vào server
                        using (var stream = new FileStream(fullPhysicalFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        string fileUrl = $"{pathConfig.RequestPath}/{yearMonthFolder}/{uniqueFileName}".Replace("\\", "/");

                        // Gom thông tin để lưu DB
                        var entity = new ApiWebsite.Models.ProjectStepAttachment
                        {
                            Id = Guid.NewGuid(),
                            ProjectStepId = request.ProjectStepId,
                            FileName = originalFileName,
                            FilePath = fileUrl,
                            FileExtension = fileExtension,
                            FileSizeInBytes = file.Length,
                            // CreatedBy = username
                        };

                        listEntities.Add(entity);
                        listResponses.Add(_mapper.Map<ProjectStepAttachmentResponse>(entity));
                    }
                }

                // SỬA: Gọi AddManyAsync để insert 1 loạt list file này xuống database thay vì AddOneAsync
                if (listEntities.Any())
                {
                    await _attachmentService.AddManyAsync(listEntities);
                }

                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(new { request.ProjectStepId, TotalFiles = listEntities.Count });
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"ProjectStepAttachmentController, Create {listEntities.Count} files. UserName: {username}", paramTrace);

                // Trả về danh sách file vừa đính kèm thành công
                return Ok(new { success = true, data = listResponses });
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "ProjectStepAttachmentController_Create_Exception", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var entity = await _attachmentService.GetByIdAsync(id);
            return entity == null ? NotFound(new { message = $"ID {id} không tìm thấy." }) : Ok(_mapper.Map<ProjectStepAttachmentResponse>(entity));
        }

        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher, Role.general)]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<ProjectStepAttachmentResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] ProjectStepAttachmentPagingFilter request)
        {
            var pagedResult = await _attachmentService.GetAllPaging(request);
            return Ok(pagedResult);
        }

        // [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                var result = await _attachmentService.DeleteAsync(id);

                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"ProjectStepAttachmentController, DeleteItem, {username} " + (result ? "OK" : "not OK"), id.ToString());
                return result ? Ok(result) : BadRequest("Xóa không thành công");
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, $"ProjectStepAttachmentController, DeleteItem_Exception, {username}", ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}