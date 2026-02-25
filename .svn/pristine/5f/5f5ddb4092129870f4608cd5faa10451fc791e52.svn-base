using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Helper.Middleware;
using ApiWebsite.Models;
using ApiWebsite.Models.Auth;
using ApiWebsite.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ApiWebsite.Controllers
{
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly ILogService _logService;
        private IEmailConfigService _iEmailConfigService;
        private string subjectEmail = "Gửi mã xác thực để lấy lại mật khẩu";
        private string contentEmail = "Mã xác thực của bạn là {0}";
        public AuthController(ILogger<AuthController> logger, IAuthService authService, ILogService logService, IEmailConfigService iEmailConfigService, IAccountService accountService)
        {
            _logger = logger;
            _authService = authService;
            _logService = logService;
            _iEmailConfigService = iEmailConfigService;
            _accountService = accountService;
        }

        private async Task<bool> SelectUserNameAndPhanMemID(string UserName, string PhanMemID)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(PhanMemID))
            {
                // Instantiate the HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Specify the URI to get data from
                    string uri = $"http://108.108.108.52:10808/api/DM_PhanMemLogin/SelectByUserNameAndPhanMemID?_UserName={UserName}&_PhanMemID={PhanMemID}";

                    try
                    {
                        // Send a GET request to the specified Uri
                        HttpResponseMessage response = await client.GetAsync(uri);

                        // Ensure we receive a successful response.
                        response.EnsureSuccessStatusCode();

                        // Read the response content as a string asynchronously
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // var settings = new JsonSerializerSettings
                        // {
                        //     NullValueHandling = NullValueHandling.Ignore,
                        //     MissingMemberHandling = MissingMemberHandling.Ignore
                        // };

                        // var parsedJsonModel = JsonConvert.DeserializeObject<JsonModelObject>(responseBody, settings);
                        var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody).ToString();
                        dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResult);
                        if (result != null && result._Data != null && result._Data.Count > 0)
                        {
                            var itemData = result._Data[0];

                            if (itemData.UserName.Value != null && itemData.UserName.Value != "" && itemData.PhanMemID.Value != null)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        // Handle any errors that occurred during the request
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);

                        return false;
                    }
                }
            }
            return false;
        }

        private async Task<bool> DeleteUserNameAndPhanMemID(string UserName, string PhanMemID)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(PhanMemID))
            {
                // Instantiate the HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Specify the URI to get data from
                    string uri = $"http://108.108.108.52:10808/api/DM_PhanMemLogin/DeleteByUserNameAndPhanMemID?_UserName={UserName}&_PhanMemID={PhanMemID}";

                    try
                    {
                        // Send a GET request to the specified Uri
                        HttpResponseMessage response = await client.GetAsync(uri);

                        // Ensure we receive a successful response.
                        response.EnsureSuccessStatusCode();

                        // Read the response content as a string asynchronously
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // var settings = new JsonSerializerSettings
                        // {
                        //     NullValueHandling = NullValueHandling.Ignore,
                        //     MissingMemberHandling = MissingMemberHandling.Ignore
                        // };

                        // var parsedJsonModel = JsonConvert.DeserializeObject<JsonModelObject>(responseBody, settings);
                        var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody).ToString();
                        dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResult);
                        var itemData = result._Data[0];

                        if (result != null && result._Data != null && result._Data.Count > 0)
                        {
                            if (itemData.UserName.Value != null && itemData.UserName.Value != "" && itemData.PhanMemID.Value != null)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        // Handle any errors that occurred during the request
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);

                        return false;
                    }
                }
            }
            return false;
        }

        private async Task<dynamic> SelectEmployeeByUserName(string UserName)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                // Instantiate the HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Specify the URI to get data from
                    string uri = $"http://108.108.108.52:10808/api/DM_NhanVien/SelectNhanVienByUser?_UserName={UserName}";

                    try
                    {
                        // Send a GET request to the specified Uri
                        HttpResponseMessage response = await client.GetAsync(uri);

                        // Ensure we receive a successful response.
                        response.EnsureSuccessStatusCode();

                        // Read the response content as a string asynchronously
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // var settings = new JsonSerializerSettings
                        // {
                        //     NullValueHandling = NullValueHandling.Ignore,
                        //     MissingMemberHandling = MissingMemberHandling.Ignore
                        // };

                        // var parsedJsonModel = JsonConvert.DeserializeObject<JsonModelObject>(responseBody, settings);
                        var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody).ToString();
                        dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResult);
                        if (result != null && result._Data != null && result._Data.Count > 0)
                        {
                            var itemData = result._Data[0];

                            return itemData;

                            // if (itemData.UserName.Value != null && itemData.UserName.Value != "" && itemData.PhanMemID.Value != null)
                            // {
                            //     return true;
                            // }
                            // else
                            // {
                            //     return false;
                            // }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception e)
                    {
                        // Handle any errors that occurred during the request
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);

                        return null;
                    }
                }
            }
            return null;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = await _authService.Authenticate(model, ipAddress());
            if (response == null)
            {
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Đăng nhập thất bại, Unauthorized", paramError);
                return Unauthorized();
            }
            if (response.GetType() == typeof(ErrorResponseModel))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] AuthenticateRefreshTokenRequest model)
        {
            var response = await _authService.RefreshToken(model);
            if (response.GetType() == typeof(ErrorResponseModel))
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize(Role.admin, Role.manager, Role.editor, Role.publisher)]
        [HttpPut("[action]")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var account = (Account)HttpContext.Items[ConstantsInternal.Account];
            var response = await _authService.ChangePassword(request, account);
            if (response.GetType() == typeof(ErrorResponseModel))
            {
                return BadRequest(response);
            }
            var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Thay đổi mật khẩu thành công", paramTrace);
            return Ok();
        }
        [HttpPost("[action]")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var account = await _accountService.GetOneAsync(x => x.Email == request.Email);
            // check email để gửi code có tồn tại hay không
            if (account is null)
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Type = typeError.Email.ToString(),
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.EmailNotFoundMessage } } }
                };

                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "AuthController, ForgotPassword, Email không tồn tại", paramError);
                return BadRequest(error);
            }
            // check VerifacationCode có dữ liệu, check xem mã xác thực còn hiệu lực không
            if (account.VerifacationCode != null)
            {
                var objVC = Newtonsoft.Json.JsonConvert.DeserializeObject<VerificationCode>(account.VerifacationCode);
                if (objVC.ExpiredAt > DateTime.UtcNow && objVC.IsUsed == false)
                {
                    ErrorResponseModel error = new ErrorResponseModel
                    {
                        Type = typeError.Code.ToString(),
                        Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.VerificationCodeStillValid } } }
                    };

                    var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    await _logService.AddLogWebInfo(LogLevelWebInfo.error, "AuthController, ForgotPassword, Mã xác thực còn hiệu lực", paramError);
                    return BadRequest(error);
                }
            }
            Random _random = new Random();
            VerificationCode VC = new VerificationCode()
            {
                IsUsed = false,
                Code = _random.Next(100000, 999999).ToString(),
                ExpiredAt = DateTime.UtcNow.AddMinutes(5)
            };
            account.VerifacationCode = Newtonsoft.Json.JsonConvert.SerializeObject(VC);
            await _accountService.CompleteServiceAsync();
            await Util.SendEmail(_iEmailConfigService, subjectEmail, request.Email, string.Format(contentEmail, VC.Code));
            return Ok();
        }
        [HttpPost("[action]")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var account = await _accountService.GetOneAsync(x => x.Email == request.Email);
            if (account is null)
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Type = typeError.Email.ToString(),
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.EmailNotFoundMessage } } }
                };
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "AuthController, ResetPassword, Email không tồn tại", paramError);
                return BadRequest(error);
            }
            if (account.VerifacationCode != null)
            {
                var objVC = Newtonsoft.Json.JsonConvert.DeserializeObject<VerificationCode>(account.VerifacationCode);
                if (objVC.Code == request.Code && objVC.ExpiredAt >= DateTime.UtcNow && objVC.IsUsed == false)
                {
                    objVC.IsUsed = true;
                    account.PasswordHash = BC.HashPassword(request.Password);
                    account.Salt = BC.GenerateSalt();
                    // account.VerifacationCode = Newtonsoft.Json.JsonConvert.SerializeObject(objVC);
                    account.VerifacationCode = null;
                    await _accountService.CompleteServiceAsync();
                    await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "AuthController, ResetPassword, Lấy lại mật khẩu thành công", "Succses");
                    return Ok();
                }
                if (objVC.Code != request.Code)
                {
                    ErrorResponseModel error = new ErrorResponseModel
                    {
                        Type = typeError.Code.ToString(),
                        Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.VerificationCodeExactly } } }
                    };
                    return BadRequest(error);
                }
                if (objVC.ExpiredAt <= DateTime.UtcNow && objVC.IsUsed == false)
                {
                    ErrorResponseModel error = new ErrorResponseModel
                    {
                        Type = typeError.Code.ToString(),
                        Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.VerificationCodeExp } } }
                    };
                    return BadRequest(error);
                }
            }
            return BadRequest();

        }
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}