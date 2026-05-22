
using FluentValidation;

using SmartLogisticsBackend.Common;
using SmartLogisticsBackend.Common.Abstractions;


namespace SmartLogisticsBackend.Features.Users.RegisterUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register", Handle);

        return app;
    }

    private static async Task<IResult> Handle(RegisterRequest registerRequest,
        IValidator<RegisterRequest> _validator,
        RegisterUserHandler handler)
    {
        var validation = await _validator.ValidateAsync(registerRequest);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());
        var result = await handler.HandleAsync(registerRequest);
        return result.ToHttpResult();
    }
}