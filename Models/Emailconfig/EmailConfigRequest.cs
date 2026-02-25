using FluentValidation;

namespace ApiWebsite.Models
{
    public class EmailConfigRequest
    {
        public EmailConfigRequest()
        {

        }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string MailServer { get; set; }
        public int Port { get; set; }
        public bool EnableSSl { get; set; }
        public string EmailTitle { get; set; }
    }
    public class EmailConfigValidator : AbstractValidator<EmailConfigRequest>
    {
        public EmailConfigValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Tên không được để trống").EmailAddress().WithMessage("Email không đúng định dạng");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu không được để trống");
            RuleFor(x => x.MailServer).NotEmpty().WithMessage("MailServer không được để trống");
            RuleFor(x => x.Port).NotNull().WithMessage("Port không được để trống");
        }
    }
}