using ApiWebsite.Common;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Helper.Middleware;
using ApiWebsite.Models;
using ApiWebsite.Models.System.DonViDeNghi;
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
    public class DonViDeNghiController : BaseController
    {
        private readonly IDonViDeNghiService _donViService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public DonViDeNghiController(IDonViDeNghiService donViService, ILogService logService, IMapper mapper)
        {
            _donViService = donViService;
            _logService = logService;
            _mapper = mapper;
        }

        // [Authorize(Role.admin, Role.manager)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(DonViDeNghiRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                // Hàm AnyAsync truyền predicate nên không bị vướng lỗi ID kiểu Guid/long
                if (await _donViService.AnyAsync(x => x.Code == model.Code && !x.IsDeleted))
                {
                    return BadRequest(new { message = "Mã Đơn vị/Phòng ban này đã tồn tại." });
                }

                var entity = _mapper.Map<ApiWebsite.Models.System.DonViDeNghi.DonViDeNghi>(model);
                entity.CreatedBy = username;

                // Gọi UpsertAsync (nếu Id = 0 thì nó sẽ hiểu là thêm mới)
                var result = await _donViService.UpsertAsync(entity);
                var response = _mapper.Map<DonViDeNghiResponse>(result);

                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "DonViDeNghiController, Create OK UserName: " + username, paramTrace);

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "DonViDeNghiController_Create_Exception", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // CHÚ Ý: Đổi từ Guid id sang long id, gọi hàm GetByLongIdAsync
        // [Authorize(Role.admin, Role.manager, Role.general)]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetItem(long id)
        {
            var entity = await _donViService.GetByLongIdAsync(id);
            if (entity == null) return NotFound(new { message = $"ID {id} không tìm thấy." });

            return Ok(_mapper.Map<DonViDeNghiResponse>(entity));
        }

        // [Authorize(Role.admin, Role.manager, Role.general)]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedResult<DonViDeNghiResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByPaging([FromQuery] DonViDeNghiPagingFilter request)
        {
            var pagedResult = await _donViService.GetAllByPaging(request);
            return Ok(pagedResult);
        }

        // CHÚ Ý: Đổi từ Guid id sang long id, gọi hàm GetByLongIdAsync
        // [Authorize(Role.admin, Role.manager)]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateItem(long id, DonViDeNghiRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                // Tìm kiếm bằng hàm dành riêng cho ID kiểu long
                var existingEntity = await _donViService.GetByLongIdAsync(id);
                if (existingEntity == null) return NotFound(new { message = $"ID {id} không tìm thấy." });

                // Kiểm tra trùng mã code khi cập nhật
                if (existingEntity.Code != model.Code)
                {
                    if (await _donViService.AnyAsync(x => x.Code == model.Code && x.Id != id && !x.IsDeleted))
                    {
                        return BadRequest(new { message = "Mã Đơn vị này đã tồn tại ở một bản ghi khác." });
                    }
                }

                _mapper.Map(model, existingEntity);
                existingEntity.UpdatedBy = username;

                var updatedEntity = await _donViService.UpsertAsync(existingEntity);
                var response = _mapper.Map<DonViDeNghiResponse>(updatedEntity);

                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "DonViDeNghiController, UpdateItem, Ok UserName: " + username, paramTrace);

                return Ok(response);
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "DonViDeNghiController_UpdateItem_Exception", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // CHÚ Ý: Đổi từ Guid id sang long id, gọi hàm DeleteByLongIdAsync
        // [Authorize(Role.admin, Role.manager)]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteItem(long id)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var username = account != null ? account.Username : "System";

            try
            {
                var result = await _donViService.DeleteByLongIdAsync(id);

                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, $"DonViDeNghiController, DeleteItem, {username} " + (result ? "OK" : "not OK"), id.ToString());
                return result ? Ok(result) : BadRequest(new { message = "Xóa không thành công hoặc không tìm thấy dữ liệu." });
            }
            catch (Exception ex)
            {
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, $"DonViDeNghiController, DeleteItem_Exception, {username}", ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}