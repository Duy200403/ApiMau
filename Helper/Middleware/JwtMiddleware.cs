using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiWebsite.Core.Services;
using ApiWebsite.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiWebsite.Helper.Middleware
{
  public class JwtMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly JwtIssuerOptions _jwtIssuerOptions;

    public JwtMiddleware(RequestDelegate next, IOptions<JwtIssuerOptions> appSettings)
    {
      _next = next;
      _jwtIssuerOptions = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, IAccountService accountService)
    {
      var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

      if (token != null)
        await attachAccountToContext(context, accountService, token);

      await _next(context);
    }

    private async Task attachAccountToContext(HttpContext context, IAccountService accountService, string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtIssuerOptions.Secret);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var accountId = jwtToken.Claims.First(x => x.Type == "id").Value;
        // attach account to context on successful jwt validation
        var guidAcountId = Guid.Parse(accountId);
        var account = await accountService.GetByIdAsync(guidAcountId);
        context.Items[ConstantsInternal.Account] = account;
      }
      catch
      {
        // do nothing if jwt validation fails
        // account is not attached to context so request won't have access to secure routes
      }
    }
  }
}
