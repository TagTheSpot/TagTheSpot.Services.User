using FluentValidation;
using TagTheSpot.Services.User.Application.DTO;

namespace TagTheSpot.Services.User.Application.Validators
{
    public sealed class LogInWithGoogleRequestValidator
        : AbstractValidator<LogInWithGoogleRequest>
    {
        public LogInWithGoogleRequestValidator()
        {
            RuleFor(x => x.GoogleIdToken)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(300);
        }
    }
}
