using System.Reflection.Metadata;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartLogisticsBackend.Common;

namespace SmartLogisticsBackend.Features.Users.LoginUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", Handle);

        return app;
    }

    private static async Task<IResult> Handle([FromServices] LoginUserHandler handler,
        LoginUserRequest request,
        IValidator<LoginUserRequest> _validator,
        CancellationToken ctx)
    {
        var validationResult = await _validator.ValidateAsync(request, ctx);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await handler.HandleAsync(request, ctx);
        return result.ToHttpResult();
    }
}