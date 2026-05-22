using Microsoft.AspNetCore.Mvc;
using SmartLogisticsBackend.Common;

namespace SmartLogisticsBackend.Features.Users.VerifyUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapVerifyUserEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/verify-email", Handle);

        return app;
    }

    private static async Task<IResult> Handle(
        [FromServices] VerifyUserHandler handler,
        [AsParameters] Request request,
        CancellationToken ct
        )
    {
        var result = await handler.HandleAsync(request);
        return result.ToHttpResult();
    }
}