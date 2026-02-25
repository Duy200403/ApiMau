using FluentValidation;

namespace ApiWebsite.Models.Auth
{
  public class ChangePasswordRequest
  {
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
      public ChangePasswordValidator()
      {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống");
        RuleFor(x => x.NewPassword).MinimumLength(6).WithMessage("Mật khẩu mới quá ngắn");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Mật khẩu nhập lại không khớp");
      }
    }
  }
}