using FluentValidation;

namespace ApiWebsite.Models.Auth
{
  public class ResetPasswordRequest
  {

    public string Email { get; set; }
    public string Code { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
  }
  public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
  {
    public ResetPasswordValidator()
    {
      RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được để trống").EmailAddress().WithMessage("Email không đúng");
      RuleFor(x => x.Code).NotEmpty().WithMessage("Code không được để trống");
      RuleFor(x => x.Password).MinimumLength(6).WithMessage("Mật khẩu quá ngắn");
      RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Mật khẩu nhập lại không khớp");
    }
  }
}