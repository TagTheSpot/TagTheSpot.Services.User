using FluentValidation;
using TagTheSpot.Services.User.Application.DTO;

namespace TagTheSpot.Services.User.Application.Validators
{
    public sealed class ResetPasswordRequestValidator
        : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            Include(new EmailValidator<ResetPasswordRequest>(x => x.Email));

            Include(new PasswordValidator<ResetPasswordRequest>(x => x.NewPassword));

            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }
}
