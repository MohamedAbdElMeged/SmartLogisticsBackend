using FluentValidation;

namespace SmartLogisticsBackend.Features.Users.RegisterUser;

public class Validator : AbstractValidator<RegisterRequest>
{
    public Validator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name should be provided");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name should be provided");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email should be provided");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email is not valid");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is Required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain a number.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");

        RuleFor(x => x.PasswordConfirmation)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}