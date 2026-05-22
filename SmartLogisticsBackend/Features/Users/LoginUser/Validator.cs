using FluentValidation;

namespace SmartLogisticsBackend.Features.Users.LoginUser;

public class Validator : AbstractValidator<LoginUserRequest>
{
    public Validator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}