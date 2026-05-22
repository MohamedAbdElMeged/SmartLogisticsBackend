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
using SmartLogisticsBackend.Features.Users.RegisterUser;
using SmartLogisticsBackend.Features.Users.ResendVerification;
using SmartLogisticsBackend.Features.Users.VerifyUser;
using SmartLogisticsBackend.Infrastructure.Auth;
using SmartLogisticsBackend.Infrastructure.Email;
using SmartLogisticsBackend.Infrastructure.Persistence;

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

builder.Services.AddHttpClient();

builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<VerifyUserHandler>();
builder.Services.AddScoped<ResendVerificationHandler>();
builder.Services.AddScoped<LoginUserHandler>();


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
app.UseHttpsRedirection();

app.MapRegisterEndpoint();
app.MapVerifyUserEndpoint();
app.MapResendEndpoint();
app.MapLoginEndpoint();


app.UseHangfireDashboard("/dashboard", new DashboardOptions
{
    Authorization = []
});
app.Run();