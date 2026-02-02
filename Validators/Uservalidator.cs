using FluentValidation;
using Application.Models;

namespace Application.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.FirstName).NotEmpty().WithMessage("First name is required.").MaximumLength(50);

            RuleFor(user => user.LastName).NotEmpty().WithMessage("Last name is required.").MaximumLength(50);

            RuleFor(user => user.Email).NotEmpty().EmailAddress();

            RuleFor(user => user.Age).InclusiveBetween(0, 120);
        }
    }
}