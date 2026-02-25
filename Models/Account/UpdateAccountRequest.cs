using System;
using System.Collections.Generic;
using System.Linq;
using ApiWebsite.Helper;
using FluentValidation;

namespace ApiWebsite.Models
{
  public class UpdateAccountRequest
  {
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Pseudonym { get; set; }
    public List<Role> Roles { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool IsActive { get; set; }

    public string CurrentUserName { get; set; }
  }
   public class UpdateAccountValidator : AbstractValidator<UpdateAccountRequest>
  {
    public UpdateAccountValidator()
    {
      RuleFor(x => x.FullName).NotEmpty().WithMessage("Tên không được để trống");
      // RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email không đúng");
      RuleFor(x => x.Password).MinimumLength(6).WithMessage("Mật khẩu quá ngắn");
      RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Mật khẩu nhập lại không khớp");
      RuleFor(x => x.Roles).NotEmpty().Custom((lstRole, context) =>
      {
              // chuyển Role enum thành list role
              var lstSourceRole = Enum.GetValues(typeof(Role))
                    .Cast<Role>()
                    .ToList();

        if (!lstSourceRole.Intersect(lstRole).Any())
        {
          context.AddFailure("Role không hợp lệ");
        }
      });
    }
  }
}