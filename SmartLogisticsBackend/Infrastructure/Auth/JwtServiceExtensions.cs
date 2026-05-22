using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartLogisticsBackend.Common.Abstractions;

namespace SmartLogisticsBackend.Infrastructure.Auth;

public static class JwtServiceExtensions
{
    public static IServiceCollection AddJwtAuth(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        var settings = config.GetSection("JwtSettings").Get<JwtSettings>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(settings.SecretKey)),
                    ValidateIssuer = true, ValidIssuer = settings.Issuer,
                    ValidateAudience = true, ValidAudience = settings.Audience,
                    ValidateLifetime = true, ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}