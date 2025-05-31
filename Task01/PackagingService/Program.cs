using Common;
using PackagingService.Core.Application;
using PackagingService.Core.Domain.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddGrpc();
builder.Services.AddSingleton<ICacheProvider, RedisCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGrpcService<PackagingServiceImpl>();

app.Run();