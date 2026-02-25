using FluentValidation;

namespace ApiWebsite.Models.Auth
{
  public class AuthenticateRefreshTokenRequest
  {
    public string Token { get; set; }

    public string RefreshToken { get; set; }
  }
  public class AuthenticateRefreshTokenValidator : AbstractValidator<AuthenticateRefreshTokenRequest>
  {
    public AuthenticateRefreshTokenValidator()
    {
      RuleFor(x => x.Token).NotEmpty().WithMessage("Token không được để trống");
      RuleFor(x => x.RefreshToken).MinimumLength(6).WithMessage("RefreshToken không chính xác");
    }
  }
}