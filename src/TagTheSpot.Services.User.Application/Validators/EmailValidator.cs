using FluentValidation;
using System.Linq.Expressions;

namespace TagTheSpot.Services.User.Application.Validators
{
    internal sealed class EmailValidator<T> : AbstractValidator<T>
    {
        public EmailValidator(Expression<Func<T, string>> emailSelector)
        {
            RuleFor(emailSelector)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .MaximumLength(100).WithMessage("Email must be at most 100 characters.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email is not in the proper format.");
        }
    }
}
