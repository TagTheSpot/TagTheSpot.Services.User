using FluentValidation;
using TagTheSpot.Services.User.Application.DTO;

namespace TagTheSpot.Services.User.Application.Validators
{
    public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            Include(new EmailValidator<RegisterRequest>(x => x.Email));
            Include(new PasswordValidator<RegisterRequest>(x => x.Password));
        }
    }
}
