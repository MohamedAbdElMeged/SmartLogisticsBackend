using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartLogisticsBackend.Common;

namespace SmartLogisticsBackend.Features.Users.Profile;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapProfileEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/profile", Handle)
            .RequireAuthorization();
        return app;
    }

    private static async Task<IResult>  Handle([FromServices] GetProfileHandler handler, ClaimsPrincipal currentUser, CancellationToken ctx)
    {
        var result = await handler.HandleAsync(currentUser, ctx);
        return result.ToHttpResult();
    }
}