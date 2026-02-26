using ApiWebsite.Core.Services;
using ApiWebsite.Models.Bidding;
using ApiWebsite.Models.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiddingProjectController : ControllerBase
    {
        private readonly IBiddingProjectService _biddingProjectService;

        public BiddingProjectController(IBiddingProjectService biddingProjectService)
        {
            _biddingProjectService = biddingProjectService;
        }

        // Đã đổi thành GetAllPaging để khớp với Interface mới
        [HttpGet("get-all-paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] BiddingProjectPagingFilter filter)
        {
            var result = await _biddingProjectService.GetAllPaging(filter);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BiddingProjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Đã đổi thành CreateProjectAsync để khớp với Interface mới
            var result = await _biddingProjectService.CreateProjectAsync(request);

            // Bắt lỗi nếu Service trả ra ErrorResponseModel (Ví dụ: Trùng mã gói thầu, chưa cấu hình bước)
            if (result is ErrorResponseModel)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}