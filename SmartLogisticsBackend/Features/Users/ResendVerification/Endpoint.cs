using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using SmartLogisticsBackend.Common;

namespace SmartLogisticsBackend.Features.Users.ResendVerification;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapResendEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/resend-verification", Handle);

        return app;
    }

    private static async Task<IResult> Handle([FromServices] ResendVerificationHandler handler,
        ResendVerificationRequest request,
        CancellationToken ctx)
    {
        var result = await handler.HandleAsync(request, ctx);
        return result.ToHttpResult();
    }
}