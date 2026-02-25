using FluentValidation;

namespace ApiWebsite.Models.Auth
{
  public class AuthenticateRequest
  {
    public string Username { get; set; }

    public string Password { get; set; }
  }
  public class AuthenticateValidator : AbstractValidator<AuthenticateRequest>
  {
    public AuthenticateValidator()
    {
      RuleFor(x => x.Username).NotEmpty().WithMessage("Tên đăng nhập không được để trống");
      RuleFor(x => x.Password).MinimumLength(6).WithMessage("Mật khẩu không chính xác");
    }
  }
}