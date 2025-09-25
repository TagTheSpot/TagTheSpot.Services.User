using FluentValidation;
using TagTheSpot.Services.User.Application.DTO;

namespace TagTheSpot.Services.User.Application.Validators
{
    public sealed class SendPasswordResetRequestValidator
        : AbstractValidator<SendPasswordResetRequest>
    {
        public SendPasswordResetRequestValidator()
        {
            Include(new EmailValidator<SendPasswordResetRequest>(x => x.Email));
        }
    }
}
