using FluentValidation;
using TagTheSpot.Services.User.Application.DTO;

namespace TagTheSpot.Services.User.Application.Validators
{
    public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            Include(new EmailValidator<LoginRequest>(x => x.Email));
            Include(new PasswordValidator<LoginRequest>(x => x.Password));
        }
    }
}
