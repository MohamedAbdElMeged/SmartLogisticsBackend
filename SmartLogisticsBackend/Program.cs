using System.Threading.RateLimiting;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resend;
using Scalar.AspNetCore;
using SmartLogisticsBackend.Common.Abstractions;
using SmartLogisticsBackend.Common.Services;
using SmartLogisticsBackend.Features.Users.LoginUser;
using SmartLogisticsBackend.Features.Users.Profile;
using SmartLogisticsBackend.Features.Users.RegisterUser;
using SmartLogisticsBackend.Features.Users.ResendVerification;
using SmartLogisticsBackend.Features.Users.SwitchRole;
using SmartLogisticsBackend.Features.Users.VerifyUser;
using SmartLogisticsBackend.Infrastructure.Auth;
using SmartLogisticsBackend.Infrastructure.Email;
using SmartLogisticsBackend.Infrastructure.Middleware;
using SmartLogisticsBackend.Infrastructure.Persistence;
using SmartLogisticsBackend.Infrastructure.Persistence.Seeds;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.AddHangfire(x =>
    x.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[]
    {
        "critical",
        "emails",
        "default"
    };
});
builder.Services.AddOptions();

builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = builder.Configuration["Resend:ApiToken"]!;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit        = 10,
                Window             = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit         = 0
            }));
    
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Too many login attempts. Try again later." }, token);
    };
});
builder.Services.AddHttpClient();

builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<VerifyUserHandler>();
builder.Services.AddScoped<ResendVerificationHandler>();
builder.Services.AddScoped<LoginUserHandler>();
builder.Services.AddScoped<SwitchRoleHandler>();
builder.Services.AddScoped<GetProfileHandler>();

builder.Services.AddScoped<IVerificationLinkBuilder, VerificationLinkBuilder>();
var app = builder.Build();

// Configure the HTTP request pipeline.



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditLogMiddleware>(); 
app.UseHttpsRedirection();
app.UseRateLimiter(); 
app.MapRegisterEndpoint();
app.MapVerifyUserEndpoint();
app.MapResendEndpoint();
app.MapLoginEndpoint();
app.MapSwitchRoleEndpoint();
app.MapProfileEndpoint();

app.UseHangfireDashboard("/dashboard", new DashboardOptions
{
    Authorization = []
});


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        var seeder = new DbSeed(context);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seed failed");
    }
}

app.Run();