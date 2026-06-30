using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartLogisticsBackend.Common;

namespace SmartLogisticsBackend.Features.Users.SwitchRole;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapSwitchRoleEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/switch-role", Handle)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> Handle(
        [FromServices] SwitchRoleHandler handler,
        [FromBody] SwitchRoleRequest request,
        ClaimsPrincipal currentUser,
        CancellationToken ctx)
    {
        var result = await handler.HandleAsync(request,currentUser, ctx);
        return result.ToHttpResult();
    }
}

