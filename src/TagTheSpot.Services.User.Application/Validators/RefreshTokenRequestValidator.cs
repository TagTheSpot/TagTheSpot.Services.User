using FluentValidation;
using TagTheSpot.Services.User.Application.DTO;

namespace TagTheSpot.Services.User.Application.Validators
{
    internal sealed class RefreshTokenRequestValidator 
        : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }
}
