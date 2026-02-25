using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ApiWebsite.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    // [EnableCors("AllowAnyCorsPolicy")]
    [Route("api/v{version:apiVersion}/[controller]")]
    
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class BaseController : ControllerBase
    {
    }
}
