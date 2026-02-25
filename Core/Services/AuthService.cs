using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Model;
using ApiWebsite.Models;
using ApiWebsite.Models.Auth;
using ApiWebsite.Models.Response;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Caching.Distributed;

namespace ApiWebsite.Core.Services
{
    public interface IAuthService : IBaseService<Account>
    {
        Task<dynamic> ChangePassword(ChangePasswordRequest model, Account acc);
        Task<dynamic> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<dynamic> RefreshToken(AuthenticateRefreshTokenRequest model);
    }
    public class AuthService : BaseService<Account>, IAuthService
    {
        private readonly ILogService _logService;
        private readonly ILoginHistoryService _iLoginHistoryService;
        private readonly JwtIssuerOptions _jwtIssuerOptions;
        private readonly IDistributedCache _cache;
        public AuthService(IUnitOfWork unitOfWork, IDistributedCache cache, ILogService logService, IOptions<JwtIssuerOptions> appSettings, ILoginHistoryService iLoginHistoryService) : base(unitOfWork)
        {
            _logService = logService;
            _jwtIssuerOptions = appSettings.Value;
            _iLoginHistoryService = iLoginHistoryService;
            _cache = cache;
        }

        public async Task<dynamic> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var expirationTime = DateTime.Now.AddDays(15);
            var refreshTokenExpireTime = DateTime.Now.AddDays(20);

            // var expirationTime = DateTime.Now.AddMinutes(1);
            // var refreshTokenExpireTime = DateTime.Now.AddMinutes(2);

            var account = await _unitOfWork.Account.GetOneAsync(x => x.Username == model.Username);
            if (account == null)
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.loginErrorMessage } } }
                };
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Đăng nhập thất bại, tài khoản không tồn tại", paramError);
                return error;
            }
            else
            {
                if (string.IsNullOrEmpty(account.Roles))
                {
                    ErrorResponseModel error = new ErrorResponseModel
                    {
                        Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.loginErrorMessageNoRole } } }
                    };
                    var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
                    await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Đăng nhập thất bại, tài khoản không có quyền", paramError);
                    return error;
                }
            }

            var checker = await CheckAccountActive(account, model.Password);
            if (checker != null)
            {
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Đăng nhập thất bại, CheckAccountActive", paramError);
                return checker;
            }
            var token = generateJwtToken(account, expirationTime, refreshTokenExpireTime);

            var reFreshToken = GenerateRefreshToken();
            string keyTokenRedis = $"token-{token}";
            var optionsExpires = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(20));
            await _cache.SetStringAsync(keyTokenRedis, reFreshToken, optionsExpires);

            // khi đăng nhập thành công, ghi vào bảng lịch sử đăng nhập
            await _iLoginHistoryService.Create(model.Username, account.Id);
            await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "Đăng nhập thành công", "Sucsses");
            return new AuthenticateResponse(token, reFreshToken);
        }

        public async Task<dynamic> RefreshToken(AuthenticateRefreshTokenRequest model)
        {
            var expirationTime = DateTime.Now.AddDays(15);
            var refreshTokenExpireTime = DateTime.Now.AddDays(20);

            // var expirationTime = DateTime.Now.AddMinutes(1);
            // var refreshTokenExpireTime = DateTime.Now.AddMinutes(2);

            var resultTokenRedis = await _cache.GetStringAsync($"token-{model.Token}");
            if (resultTokenRedis == null)
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { "Token đã hết hạn" } } }
                };
                return error;
            }
            else
            {
                if (resultTokenRedis == model.RefreshToken)
                {
                    var principal = ValidateAndReadToken(model.Token);
                    var accountId = "";
                    if (principal != null)
                    {
                        var claims = principal.Claims;
                        foreach (var claim in claims)
                        {

                            if (claim.Type == "id")
                            {
                                accountId = claim.Value;
                            }
                        }
                        var account = await _unitOfWork.Account.GetOneAsync(x => x.Id == Guid.Parse(accountId));
                        if (account != null)
                        {
                            var token = generateJwtToken(account, expirationTime, refreshTokenExpireTime);
                            var reFreshToken = GenerateRefreshToken();
                            string keyTokenRedis = $"token-{token}";
                            // var optionsExpires = new DistributedCacheEntryOptions().SetAbsoluteExpiration(new DateTimeOffset(refreshTokenExpireTime));
                            var optionsExpires = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(20));
                            await _cache.SetStringAsync(keyTokenRedis, reFreshToken, optionsExpires);
                            return new AuthenticateResponse(token, reFreshToken);
                        }
                    }
                }
                else
                {
                    ErrorResponseModel error = new ErrorResponseModel
                    {
                        Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { "Giá trị không khớp với dữ liệu" } } }
                    };
                    return error;
                }
            }
            return 200;
        }

        public ClaimsPrincipal ValidateAndReadToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtIssuerOptions.Secret)),
                ValidateIssuer = true,
                // ValidIssuer = "1nh9917-issuer",
                ValidIssuer = "108hos-issuer",
                ValidateAudience = true,
                // ValidAudience = "1nh9917-audience",
                ValidAudience = "108hos-audience",
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenException e)
            {
                // Token không hợp lệ hoặc không thể xác minh
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32]; // Độ dài 32 byte cho mức độ an toàn cao
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string generateJwtToken(Account account, DateTime expireTime, DateTime refreshExpireTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtIssuerOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()), new Claim("fullname", account.FullName), new Claim("email", account.Email), new Claim("role", account.Roles) }),
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()), new Claim("username", account.Username), new Claim("role", account.Roles), new Claim(JwtRegisteredClaimNames.Iss, "108hos-issuer"),
                    new Claim(JwtRegisteredClaimNames.Aud, "108hos-audience"), new Claim("refreshexp", refreshExpireTime.ToString()) }),
                Expires = expireTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private async Task<dynamic> CheckAccountActive(Account account, string password)
        {
            if (!account.IsActive)
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.AccountNotActive } } }
                };
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Tài khoản chưa được kích hoạt, account", paramError);
                return error;
            }
            else
            {
                // check xem tài khoản đã bị khóa chưa => khóa rồi 2 trường hợp time khóa hết hiệu lực và còn hiệu lực
                if (account.IsLock == true)
                {
                    // thời gian khóa vẫn còn hiệu lực
                    if (account.TimeLock != null && DateTime.UtcNow < account.TimeLock)
                    {
                        ErrorResponseModel error = new ErrorResponseModel
                        {
                            Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.AccountInActive } } }
                        };
                        return error;
                    }
                    // thời gian khóa vẫn hết hiệu lực phải check thêm password để đăng nhập và mở khóa
                    if (account.TimeLock != null && DateTime.UtcNow > account.TimeLock && BC.Verify(password, account.PasswordHash))
                    {
                        return await updateAccountUnLock(account);
                    }

                }
                // check tài khoản chưa bị khóa
                if (!BC.Verify(password, account.PasswordHash))
                {
                    ErrorResponseModel error = new ErrorResponseModel
                    {
                        Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.loginErrorMessage } } }
                    };
                    // đăng nhập sai cập nhật các trường đủ điều kiện để khóa tài khoản
                    account.AccessFailedCount = account.AccessFailedCount + 1;
                    if (account.AccessFailedCount >= 5)
                    {
                        account.IsLock = true;
                        account.TimeLock = DateTime.UtcNow.AddMinutes(5);
                    }
                    await _unitOfWork.CompleteAsync();
                    return error;
                }
                // tài khoản không bị khóa và check password thỏa mãn
                return await updateAccountUnLock(account);
            }

        }
        private async Task<dynamic> updateAccountUnLock(Account account)
        {
            account.IsLock = false;
            account.AccessFailedCount = 0;
            account.TimeLock = null;
            await _unitOfWork.CompleteAsync();
            return null;
        }

        public async Task<dynamic> ChangePassword(ChangePasswordRequest model, Account account)
        {
            // check nếu như tài khoản không tồn tại hoặc xác nhận mật khẩu không đúng với mật khẩu hiện tại
            if (account == null || !BC.Verify(model.CurrentPassword, account.PasswordHash))
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.CurrentPasswordInvalid } } }
                };
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Thay đổi mật khẩu thất bại", paramError);
                return error;
            }
            // check mật khấu mới và xác nhận mật khẩu mới phải trùng khớp
            if (!BC.Verify(model.NewPassword, model.ConfirmPassword))
            {
                ErrorResponseModel error = new ErrorResponseModel
                {
                    Errors = new Dictionary<string, string[]> { { Enum.GetName(typeof(ErrorModelPropertyName), ErrorModelPropertyName.content), new string[] { ConstantsInternal.ConfirmPasswordInvalid } } }
                };
                var paramError = Newtonsoft.Json.JsonConvert.SerializeObject(error);
                await _logService.AddLogWebInfo(LogLevelWebInfo.error, "Thay đổi mật khẩu thất bại", paramError);
                return error;
            }
            account.PasswordHash = BC.HashPassword(model.NewPassword);
            account.Salt = BC.GenerateSalt();
            await _unitOfWork.CompleteAsync();
            return null;
        }
    }
}