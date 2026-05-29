using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SmartLogisticsBackend.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/docs");
}
app.UseHttpsRedirection();



app.Run();